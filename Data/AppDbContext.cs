using Assignment1.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Description).IsRequired().HasMaxLength(2000);
            entity.Property(p => p.Price).HasColumnType("numeric(18,2)");
            entity.Property(p => p.ImageUrl).HasMaxLength(500);
        });

        // Seed some sample data
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Laptop Dell XPS 15",
                Description = "High-performance laptop with Intel Core i7, 16GB RAM, 512GB SSD",
                Price = 1299.99m,
                ImageUrl = null
            },
            new Product
            {
                Id = 2,
                Name = "Wireless Mouse",
                Description = "Ergonomic wireless mouse with adjustable DPI",
                Price = 29.99m,
                ImageUrl = null
            },
            new Product
            {
                Id = 3,
                Name = "Mechanical Keyboard",
                Description = "RGB mechanical keyboard with Cherry MX switches",
                Price = 149.99m,
                ImageUrl = null
            }
        );
    }
}
