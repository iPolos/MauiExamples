using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MauiExamples.API.Models;
using MauiExamples.API.Repositories;

namespace MauiExamples.API.Controllers;

/// <summary>
/// Products API Controller - provides endpoints for managing product data
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductsController> _logger;

    /// <summary>
    /// Constructor for the ProductsController
    /// </summary>
    /// <param name="productRepository">Product repository</param>
    /// <param name="logger">Logger instance injected by DI</param>
    public ProductsController(IProductRepository productRepository, ILogger<ProductsController> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets all products
    /// </summary>
    /// <returns>A list of all products</returns>
    /// <response code="200">Returns the list of products</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        _logger.LogInformation("Getting all products");
        var products = await _productRepository.GetAllProductsAsync();
        return Ok(products);
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
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        _logger.LogInformation("Getting product with ID: {Id}", id);
        var product = await _productRepository.GetProductByIdAsync(id);

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
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        if (product == null)
        {
            return BadRequest("Product data is null");
        }

        try
        {
            var createdProduct = await _productRepository.AddProductAsync(product);
            _logger.LogInformation("Created new product with ID: {Id}", createdProduct.Id);

            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return BadRequest($"Error creating product: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="id">The ID of the product to update</param>
    /// <param name="product">The updated product data</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the product was successfully updated</response>
    /// <response code="400">If the product data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    /// <response code="404">If the product to update is not found</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (id != product.Id)
        {
            return BadRequest("ID in URL does not match ID in product data");
        }

        try
        {
            var success = await _productRepository.UpdateProductAsync(product);
            if (!success)
            {
                _logger.LogWarning("Cannot update - product with ID: {Id} not found", id);
                return NotFound();
            }
            
            _logger.LogInformation("Updated product with ID: {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product with ID {Id}", id);
            return BadRequest($"Error updating product: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes a product by ID
    /// </summary>
    /// <param name="id">The ID of the product to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the product was successfully deleted</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    /// <response code="404">If the product to delete is not found</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var success = await _productRepository.DeleteProductAsync(id);
        if (!success)
        {
            _logger.LogWarning("Cannot delete - product with ID: {Id} not found", id);
            return NotFound();
        }
        
        _logger.LogInformation("Deleted product with ID: {Id}", id);
        return NoContent();
    }
} 