using Library.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Data;

public class InventoryReposiory : IInventoryReposiory
{
    private readonly IDbContextFactory<LibraryDbContext> _factory;

    public InventoryReposiory(IDbContextFactory<LibraryDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<IReadOnlyList<InventoryItem>> GetAllAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Inventory.Include(i => i.Product).ToListAsync();
    }

    public async Task<InventoryItem?> GetInventoryItemBySkuAsync(string sku)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Inventory.Include(i => i.Product).FirstOrDefaultAsync(i => i.Product.Sku == sku);
    }

    public async Task<InventoryItem> AddInventoryItemAsync(string sku, string name, decimal price, int quantity)
    {
        await using var db = await _factory.CreateDbContextAsync();

        InventoryItem newItem = new InventoryItem
        {
            Product = new Product {Sku = sku, Name = name, Price = price},
            CurrentStock = quantity
        };

        db.Inventory.Add(newItem);
        await db.SaveChangesAsync();

        return newItem;
    }

    public async Task<bool> RemoveBySkuAsync(string sku)
    {
        await using var db = await _factory.CreateDbContextAsync();

        InventoryItem? itemToRemove = await db.Inventory.Include(i => i.Product)
                                            .FirstOrDefaultAsync(i => i.Product.Sku == sku);

        if (itemToRemove is null)
        {
            return false;
        }

        db.Products.Remove(itemToRemove.Product);

        await db.SaveChangesAsync();
        return true;
    }
}