using Microsoft.EntityFrameworkCore;
using Library.Data.Entities;
using Library.Data;
using System.Data.Common;

namespace Library.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options){}

    public DbSet<Product> Products => Set<Product>();
    public DbSet<InventoryItem> Inventory => Set<InventoryItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Product>(e =>
        {
           e.HasIndex(p => p.Sku).IsUnique();

           e.Property(p => p.Price).HasColumnType("decimal(10,2)");

           e.HasOne(p => p.Inventory)
                .WithOne(i => i.Product)
                .HasForeignKey<InventoryItem>(i => i.ProductId); 
        });


        b.Entity<Product>().HasData(
            new Product { Id = 1, Sku = "BK-001", Name = "Clean Code", Price = 32.00m},
            new Product { Id = 2, Sku = "BK-002", Name = "The Pragmatic Programmer", Price = 38.00m},
            new Product { Id = 3, Sku = "BK-003", Name = "Refactoring", Price = 45.00m}

        );

        b.Entity<InventoryItem>().HasData(
            new InventoryItem { Id = 1, ProductId = 1, CurrentStock = 5},
            new InventoryItem { Id = 2, ProductId = 2, CurrentStock = 3},
            new InventoryItem { Id = 3, ProductId = 3, CurrentStock = 8}

        );

    }

}