using Microsoft.EntityFrameworkCore;
using MauiExamples.API.Models;
using System.Security.Cryptography;
using System.Text;

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
    /// Users in the database
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// Configure the model that was discovered by convention from the entity types
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed initial product data
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
        
        // Seed initial user data
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = ComputeSha256Hash("Admin123!"),
                Email = "admin@example.com",
                Role = "Admin"
            },
            new User
            {
                Id = 2,
                Username = "user",
                PasswordHash = ComputeSha256Hash("User123!"),
                Email = "user@example.com",
                Role = "User"
            }
        );
    }
    
    // Simple password hashing for demo purposes
    // In a production app, you would use a more secure method like BCrypt or Argon2
    private static string ComputeSha256Hash(string rawData)
    {
        using var sha256Hash = SHA256.Create();
        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        
        var builder = new StringBuilder();
        foreach (var t in bytes)
        {
            builder.Append(t.ToString("x2"));
        }
        return builder.ToString();
    }
} 