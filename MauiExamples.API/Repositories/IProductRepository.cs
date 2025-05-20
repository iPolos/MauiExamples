using MauiExamples.API.Models;

namespace MauiExamples.API.Repositories;

/// <summary>
/// Repository for product operations
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Get all products
    /// </summary>
    /// <returns>A list of all products</returns>
    Task<IEnumerable<Product>> GetAllProductsAsync();
    
    /// <summary>
    /// Get a product by ID
    /// </summary>
    /// <param name="id">The product ID</param>
    /// <returns>The product if found, otherwise null</returns>
    Task<Product?> GetProductByIdAsync(int id);
    
    /// <summary>
    /// Add a new product
    /// </summary>
    /// <param name="product">The product to add</param>
    /// <returns>The added product with its assigned ID</returns>
    Task<Product> AddProductAsync(Product product);
    
    /// <summary>
    /// Update an existing product
    /// </summary>
    /// <param name="product">The product to update</param>
    /// <returns>True if update was successful, otherwise false</returns>
    Task<bool> UpdateProductAsync(Product product);
    
    /// <summary>
    /// Delete a product by ID
    /// </summary>
    /// <param name="id">The product ID to delete</param>
    /// <returns>True if deletion was successful, otherwise false</returns>
    Task<bool> DeleteProductAsync(int id);
} 