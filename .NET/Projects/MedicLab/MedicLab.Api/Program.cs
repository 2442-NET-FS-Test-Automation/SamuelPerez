using MedicLab.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.MSSqlServer;
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
            AllowNull = false
        }
}
};

var sinkOptions = new MSSqlServerSinkOptions
{
    TableName = "EventLogs",
    AutoCreateSqlTable = false
};


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Hello World!");

app.Run();
Log.CloseAndFlush();