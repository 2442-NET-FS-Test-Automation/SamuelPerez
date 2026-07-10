using MedicLab.Api.Fulfillment;
using MedicLab.Data;
using MedicLab.Data.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

string conn_string = "Server=localhost,1433;Database=MedicLabDb;User ID=sa;Password=LibraryPass1!;TrustServerCertificate=True"; //gitignoreline

var columnOptions = new ColumnOptions
{
    AdditionalColumns = new Collection<SqlColumn>
{
        new SqlColumn
        {
            ColumnName = "AppointmentOrderId",
            DataType = SqlDbType.Int,
            AllowNull = true
        }
}
};
columnOptions.Id.ColumnName = "EventLogId";

var sinkOptions = new MSSqlServerSinkOptions
{
    TableName = "EventLogs",
    AutoCreateSqlTable = false
};

Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine($"SELFLOG: {msg}"));
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File("logs/fulfillment-log.log", rollingInterval: RollingInterval.Day)
    .WriteTo.MSSqlServer(
        connectionString: conn_string,
        sinkOptions: sinkOptions,
        columnOptions: columnOptions
    )
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContextFactory<MedicLabDbContext>(options => options.UseSqlServer(conn_string));


builder.Services.AddScoped<IFulfilmentService, FulfilmentService>();
builder.Services.AddScoped<ISeeder, Seeder>();
builder.Services.AddScoped<BurstPlanner>();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Welcome to a Medical Laboratory!");

app.MapGet("/inventory", async (MedicLabDbContext db) =>
{
    return await db.Availability.Where(a => a.Day >= DateTime.Today)
                                .Join(db.ClinicalStudies, a => a.ClinicalStudyId, cs => cs.ClinicalStudyId, (a, cs) => new { a, cs })
                                .GroupBy(x => new { x.cs.LOINC, x.cs.StudyName },
                                        x => x.a.Slots)
                                .Select(b => new
                                {
                                    b.Key.LOINC,
                                    b.Key.StudyName,
                                    TotalSlots = b.Sum()
                                }).ToListAsync();
});

app.MapPost("/appointmentOrders/burst", (int burstAmount, bool expedited, ISeeder seeder, 
    IServiceScopeFactory scopes, IHostApplicationLifetime lifetime) =>
{
    IReadOnlyList<int>? ids = seeder.SeedAppointmentOrders(burstAmount, expedited);
    CancellationToken appStopping = lifetime.ApplicationStopping;

    _ = Task.Run(async () =>
    {
        try
        {
            using IServiceScope scope = scopes.CreateScope();
            IFulfilmentService service = scope.ServiceProvider.GetRequiredService<IFulfilmentService>();
            await service.FulfillmentBurstAsync(ids, appStopping);
        }
        catch (Exception ex)
        {
            Log.Warning("Something failed during a 'appointmentOrders/burst' with {Exception}", ex.Message);
        }
    }, appStopping);

    return Results.Accepted("Request received");
});

app.MapGet("reports/fulfillment-rate", async (MedicLabDbContext db) =>
{
    var report = await db.AppointmentOrders
                        .Where(ao => ao.Status == Status.Completed || ao.Status == Status.Backordered)
                        .GroupBy(_ => 1)
                        .Select(g => new 
                        {
                            TotalProcessOrders = g.Count(),
                            FulfilledOrders = g.Sum(ao => ao.Status == Status.Completed ? 1 : 0),
                            BackorderedOrders = g.Sum(ao => ao.Status == Status.Backordered ? 1 : 0)
                        })
                        .FirstOrDefaultAsync();
    
    return report ?? new { TotalProcessOrders = 0, FulfilledOrders = 0, BackorderedOrders = 0 };
});

// app.MapPost("/benchmark", async (int burstAmount, bool expedited, ISeeder seeder, IFulfilmentService service, MedicLabDbContext db) =>
// {
//    var ids1 = seeder. 
// });

app.Run();
Log.CloseAndFlush();