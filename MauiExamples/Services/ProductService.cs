using MauiExamples.Models;

namespace MauiExamples.Services;

/// <summary>
/// Mock implementation of IProductService that provides sample product data
/// </summary>
public class ProductService : IProductService
{
    private readonly List<Product> _products = new()
    {
        new Product
        {
            Id = 1,
            Name = "Smartphone X",
            Description = "Latest smartphone with advanced features and a stunning display.",
            Price = 999.99m,
            ImageUrl = "dotnet_bot.png",
            InStock = true
        },
        new Product
        {
            Id = 2,
            Name = "Laptop Pro",
            Description = "High-performance laptop with cutting-edge technology for professionals.",
            Price = 1499.99m,
            ImageUrl = "dotnet_bot.png",
            InStock = true
        },
        new Product
        {
            Id = 3,
            Name = "Wireless Headphones",
            Description = "Premium noise-canceling headphones with crystal clear sound.",
            Price = 249.99m,
            ImageUrl = "dotnet_bot.png",
            InStock = false
        },
        new Product
        {
            Id = 4,
            Name = "Smartwatch",
            Description = "Track your fitness and stay connected with this elegant smartwatch.",
            Price = 299.99m,
            ImageUrl = "dotnet_bot.png",
            InStock = true
        },
        new Product
        {
            Id = 5,
            Name = "Tablet Ultra",
            Description = "Portable yet powerful tablet for work and entertainment.",
            Price = 699.99m,
            ImageUrl = "dotnet_bot.png",
            InStock = true
        }
    };

    /// <inheritdoc />
    public List<Product> GetAllProducts()
    {
        return _products;
    }

    /// <inheritdoc />
    public Product? GetProductById(int id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }
    
    /// <inheritdoc />
    public Product AddProduct(Product product)
    {
        // Generate a new ID (in a real app, this would be handled by the database)
        int newId = _products.Count > 0 ? _products.Max(p => p.Id) + 1 : 1;
        
        // Set the new ID
        product.Id = newId;
        
        // Add to our collection
        _products.Add(product);
        
        return product;
    }
} 