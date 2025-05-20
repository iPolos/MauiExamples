using MauiExamples.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Diagnostics;

namespace MauiExamples.Services;

/// <summary>
/// API implementation of IProductService
/// Makes actual HTTP requests to the backend API
/// </summary>
public class ApiProductService : IProductService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiProductService()
    {
        _httpClient = new HttpClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // Get the device-specific base URL
        _baseUrl = GetApiUrl();
        
        // For debugging
        Debug.WriteLine($"API Service initialized with URL: {_baseUrl}");
    }

    private string GetApiUrl()
    {
        // Default API URL (works for local debugging on Windows/Mac)
        string apiUrl = "http://localhost:5000/api/products";

#if ANDROID
        // When running on Android Emulator, localhost refers to the emulator's own loopback interface
        // 10.0.2.2 is a special alias to the host's localhost
        apiUrl = "http://10.0.2.2:5000/api/products";
#elif IOS
        // For iOS simulators, we can use a special localhost alias
        apiUrl = "http://localhost:5000/api/products";
#endif

        return apiUrl;
    }

    /// <inheritdoc />
    public List<Product> GetAllProducts()
    {
        try
        {
            // Convert to async/await pattern
            var task = Task.Run(async () => await GetAllProductsAsync());
            return task.Result;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in GetAllProducts: {ex.Message}");
            return new List<Product>();
        }
    }
    
    private async Task<List<Product>> GetAllProductsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(_baseUrl);
            
            if (response.IsSuccessStatusCode)
            {
                var products = await response.Content.ReadFromJsonAsync<List<Product>>(_jsonOptions);
                return products ?? new List<Product>();
            }
            
            Debug.WriteLine($"Failed to get products: {response.StatusCode}");
            return new List<Product>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception getting products: {ex.Message}");
            if (ex.InnerException != null)
            {
                Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            return new List<Product>();
        }
    }

    /// <inheritdoc />
    public Product? GetProductById(int id)
    {
        try
        {
            var task = Task.Run(async () => await GetProductByIdAsync(id));
            return task.Result;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in GetProductById: {ex.Message}");
            return null;
        }
    }
    
    private async Task<Product?> GetProductByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Product>(_jsonOptions);
            }
            
            Debug.WriteLine($"Failed to get product {id}: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception getting product {id}: {ex.Message}");
            return null;
        }
    }
    
    /// <inheritdoc />
    public Product AddProduct(Product product)
    {
        try
        {
            var task = Task.Run(async () => await AddProductAsync(product));
            return task.Result;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in AddProduct: {ex.Message}");
            return product;
        }
    }
    
    private async Task<Product> AddProductAsync(Product product)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUrl, product);
            
            if (response.IsSuccessStatusCode)
            {
                var createdProduct = await response.Content.ReadFromJsonAsync<Product>(_jsonOptions);
                return createdProduct ?? product;
            }
            
            Debug.WriteLine($"Failed to add product: {response.StatusCode}");
            var content = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"Response content: {content}");
            return product;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception adding product: {ex.Message}");
            return product;
        }
    }

    /// <inheritdoc />
    public bool DeleteProduct(int id)
    {
        try
        {
            var task = Task.Run(async () => await DeleteProductAsync(id));
            return task.Result;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in DeleteProduct: {ex.Message}");
            return false;
        }
    }
    
    private async Task<bool> DeleteProductAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
            
            // For successful delete, API returns 204 No Content
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception deleting product {id}: {ex.Message}");
            return false;
        }
    }
} 