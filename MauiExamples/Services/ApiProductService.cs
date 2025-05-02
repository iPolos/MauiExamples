using MauiExamples.Models;

namespace MauiExamples.Services;

/// <summary>
/// Simulated API implementation of IProductService
/// In a real application, this would make HTTP requests to a backend API
/// </summary>
public class ApiProductService : IProductService
{
    // For demo purposes, we'll use a similar in-memory list
    // but with different products to show it's a different implementation
    private readonly List<Product> _products = new()
    {
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
    };

    /// <inheritdoc />
    public List<Product> GetAllProducts()
    {
        // In a real implementation, this would make an HTTP request
        // For demo, we'll just return our mock data with a simulated delay
        Thread.Sleep(300); // Simulate network delay
        return _products;
    }

    /// <inheritdoc />
    public Product? GetProductById(int id)
    {
        // In a real implementation, this would make an HTTP request
        // For demo, we'll just return from our mock data with a simulated delay
        Thread.Sleep(150); // Simulate network delay
        return _products.FirstOrDefault(p => p.Id == id);
    }
} 