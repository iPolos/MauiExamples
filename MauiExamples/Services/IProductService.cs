using MauiExamples.Models;

namespace MauiExamples.Services;

/// <summary>
/// Interface defining the contract for product services
/// This allows for different implementations (real API, mock data, etc.)
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Get all available products
    /// </summary>
    /// <returns>A list of products</returns>
    List<Product> GetAllProducts();
    
    /// <summary>
    /// Get a specific product by ID
    /// </summary>
    /// <param name="id">The product ID to look for</param>
    /// <returns>The product if found, otherwise null</returns>
    Product? GetProductById(int id);
    
    /// <summary>
    /// Add a new product to the catalog
    /// </summary>
    /// <param name="product">The product to add</param>
    /// <returns>The added product with its assigned ID</returns>
    Product AddProduct(Product product);
    
    /// <summary>
    /// Delete a product by ID
    /// </summary>
    /// <param name="id">The product ID to delete</param>
    /// <returns>True if successful, false otherwise</returns>
    bool DeleteProduct(int id);
} 