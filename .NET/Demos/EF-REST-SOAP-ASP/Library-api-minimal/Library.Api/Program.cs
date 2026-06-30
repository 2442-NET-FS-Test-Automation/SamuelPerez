using Microsoft.EntityFrameworkCore;
using Library.Data;

var builder = WebApplication.CreateBuilder(args);

var conn_string = "Server=localhost,1433;Database=LibraryMinimalDb;User ID=sa;Password=LibraryPass1!;TrustServerCertificate=True";

builder.Services.AddDbContext<LibraryDbContext>(options => options.UseSqlServer(conn_string));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Hello World!");


app.MapGet("/inventory", async (LibraryDbContext db) =>
{
    return await db.Inventory.ToListAsync();
});

app.MapGet("/inventory/by-value", (LibraryDbContext db) =>
{
    return db.Inventory.Include(i => i.Product)
        .GroupBy(i => i.CurrentStock >= 5 ? "well-stocked" : "low")
        .Select(g => new { tier = g.Key, count = g.Count(), units = g.Sum(i => i.CurrentStock)})
        .ToList();
});

app.Run();
