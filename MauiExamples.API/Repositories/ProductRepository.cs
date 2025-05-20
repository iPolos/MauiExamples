using Microsoft.EntityFrameworkCore;
using MauiExamples.API.Data;
using MauiExamples.API.Models;

namespace MauiExamples.API.Repositories;

/// <summary>
/// Implementation of product repository using Entity Framework Core
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductRepository> _logger;

    /// <summary>
    /// Constructor for product repository
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="logger">Logger</param>
    public ProductRepository(ApplicationDbContext context, ILogger<ProductRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        try
        {
            return await _context.Products.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all products");
            return Enumerable.Empty<Product>();
        }
    }

    /// <inheritdoc />
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        try
        {
            return await _context.Products.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product with ID {Id}", id);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<Product> AddProductAsync(Product product)
    {
        try
        {
            // Generate a new ID if not specified
            if (product.Id <= 0)
            {
                // Find the maximum ID and increment
                var maxId = await _context.Products.MaxAsync(p => (int?)p.Id) ?? 100;
                product.Id = maxId + 1;
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding product: {ProductName}", product.Name);
            throw; // Re-throw to let the controller handle it
        }
    }

    /// <inheritdoc />
    public async Task<bool> UpdateProductAsync(Product product)
    {
        try
        {
            var existingProduct = await _context.Products.FindAsync(product.Id);
            if (existingProduct == null)
            {
                _logger.LogWarning("Product with ID {Id} not found for update", product.Id);
                return false;
            }

            _context.Entry(existingProduct).State = EntityState.Detached;
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product with ID {Id}", product.Id);
            throw; // Re-throw to let the controller handle it
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteProductAsync(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {Id} not found for deletion", id);
                return false;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product with ID {Id}", id);
            return false;
        }
    }
} 