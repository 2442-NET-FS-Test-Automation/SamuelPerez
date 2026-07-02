using Microsoft.EntityFrameworkCore;
using Library.Data;
using Library.Data.Entities;

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


app.MapGet("/peek/tracking", (LibraryDbContext db) => {
    var unchanged = db.Products.First();
    var modified = db.Products.Skip(1).First();

    modified.Price += 1;

    db.Products.Add(new Product {Sku = "BK-TMP", Name = "Tmp", Price = 1m});

    var states = db.ChangeTracker.Entries()
        .Select(e => new {entity = e.Entity.GetType().Name, state = e.State.ToString()})
        .ToList();
    db.ChangeTracker.Clear();

    return states;
});

app.MapGet("/peek/conflict", (IServiceScopeFactory scopes) =>
{
    using var scopeA = scopes.CreateScope();
    using var scopeB = scopes.CreateScope();

    var firstDb = scopeA.ServiceProvider.GetRequiredService<LibraryDbContext>();
    var secondDb = scopeB.ServiceProvider.GetRequiredService<LibraryDbContext>();

    var firstInventory = firstDb.Inventory.First(i => i.Id == 1);
    var secondInventory = secondDb.Inventory.First(i => i.Id == 1);

    firstInventory.CurrentStock --;
    firstDb.SaveChanges();

    secondInventory.CurrentStock --;

    try
    {
        secondDb.SaveChanges();
    } catch (DbUpdateConcurrencyException ex)
    {
        var entry = ex.Entries.Single();

        var current = entry.GetDatabaseValues();

        entry.OriginalValues.SetValues(current!);

        ((InventoryItem) entry.Entity).CurrentStock = current!.GetValue<int>(nameof(InventoryItem.CurrentStock)) - 1;

        secondDb.SaveChanges();
    }

    return Results.Ok("Conflic caught, reloaded and retried.");

});

app.Run();
