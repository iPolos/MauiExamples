namespace MauiExamples.API.Models;

/// <summary>
/// Represents a product in the catalog
/// </summary>
public class Product
{
    /// <summary>
    /// Unique identifier for the product
    /// </summary>
    /// <example>101</example>
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the product
    /// </summary>
    /// <example>Pro Camera</example>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed description of the product
    /// </summary>
    /// <example>Professional-grade camera with advanced lens and features.</example>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Price of the product in the default currency
    /// </summary>
    /// <example>1299.99</example>
    public decimal Price { get; set; }
    
    /// <summary>
    /// URL or path to the product image
    /// </summary>
    /// <example>dotnet_bot.png</example>
    public string ImageUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Indicates whether the product is currently in stock
    /// </summary>
    /// <example>true</example>
    public bool InStock { get; set; }
} 