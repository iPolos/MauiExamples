using Microsoft.EntityFrameworkCore;
using MauiExamples.API.Models;

namespace MauiExamples.API.Data;

/// <summary>
/// Database context for the application
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Constructor for the database context
    /// </summary>
    /// <param name="options">Database context options</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Products in the database
    /// </summary>
    public DbSet<Product> Products { get; set; } = null!;

    /// <summary>
    /// Configure the model that was discovered by convention from the entity types
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed initial data
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 101,
                Name = "Pro Camera",
                Description = "Professional-grade camera with advanced lens and features.",
                Price = 1299.99m,
                ImageUrl = "dotnet_bot.png",
                InStock = true
            },
            new Product
            {
                Id = 102,
                Name = "Gaming Console",
                Description = "Next-generation gaming console with immersive experience.",
                Price = 499.99m,
                ImageUrl = "dotnet_bot.png",
                InStock = true
            },
            new Product
            {
                Id = 103,
                Name = "Smart Speaker",
                Description = "Voice-controlled smart speaker with premium sound quality.",
                Price = 179.99m,
                ImageUrl = "dotnet_bot.png",
                InStock = true
            },
            new Product
            {
                Id = 104,
                Name = "Drone",
                Description = "Advanced aerial drone with 4K camera and long flight time.",
                Price = 799.99m,
                ImageUrl = "dotnet_bot.png",
                InStock = false
            },
            new Product
            {
                Id = 105,
                Name = "Electric Scooter",
                Description = "Foldable electric scooter for convenient urban commuting.",
                Price = 399.99m,
                ImageUrl = "dotnet_bot.png",
                InStock = true
            }
        );
    }
} 