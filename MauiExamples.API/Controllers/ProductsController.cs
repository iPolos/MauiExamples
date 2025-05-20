using Microsoft.AspNetCore.Mvc;
using MauiExamples.API.Models;

namespace MauiExamples.API.Controllers;

/// <summary>
/// Products API Controller - provides endpoints for managing product data
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    // For demo purposes, we'll use an in-memory list
    // In a real application, this would come from a database
    private static readonly List<Product> _products = new()
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

    private readonly ILogger<ProductsController> _logger;

    /// <summary>
    /// Constructor for the ProductsController
    /// </summary>
    /// <param name="logger">Logger instance injected by DI</param>
    public ProductsController(ILogger<ProductsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets all products
    /// </summary>
    /// <returns>A list of all products</returns>
    /// <response code="200">Returns the list of products</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Product>> GetProducts()
    {
        _logger.LogInformation("Getting all products");
        return Ok(_products);
    }

    /// <summary>
    /// Gets a specific product by ID
    /// </summary>
    /// <param name="id">The ID of the product to retrieve</param>
    /// <returns>The requested product</returns>
    /// <response code="200">Returns the requested product</response>
    /// <response code="404">If the product is not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Product> GetProduct(int id)
    {
        _logger.LogInformation("Getting product with ID: {Id}", id);
        var product = _products.FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            _logger.LogWarning("Product with ID: {Id} not found", id);
            return NotFound();
        }

        return Ok(product);
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="product">The product data to create</param>
    /// <returns>The created product with its assigned ID</returns>
    /// <response code="201">Returns the newly created product</response>
    /// <response code="400">If the product data is invalid</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Product> CreateProduct(Product product)
    {
        if (product == null)
        {
            return BadRequest("Product data is null");
        }

        // Generate a new ID
        int newId = _products.Count > 0 ? _products.Max(p => p.Id) + 1 : 100;
        product.Id = newId;

        _products.Add(product);
        _logger.LogInformation("Created new product with ID: {Id}", product.Id);

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    /// <summary>
    /// Deletes a product by ID
    /// </summary>
    /// <param name="id">The ID of the product to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the product was successfully deleted</response>
    /// <response code="404">If the product to delete is not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteProduct(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            _logger.LogWarning("Cannot delete - product with ID: {Id} not found", id);
            return NotFound();
        }

        _products.Remove(product);
        _logger.LogInformation("Deleted product with ID: {Id}", id);

        return NoContent();
    }
} 