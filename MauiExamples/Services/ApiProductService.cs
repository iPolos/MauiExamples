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
    private readonly AuthService _authService;

    public ApiProductService(AuthService authService)
    {
        _httpClient = new HttpClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        _authService = authService;
        
        // Add authentication token if available
        _authService.ConfigureHttpClient(_httpClient);
        
        // Subscribe to auth changes to update the HTTP client
        _authService.AuthenticationChanged += OnAuthenticationChanged;

        // Get the device-specific base URL
        _baseUrl = GetApiUrl();
        
        // For debugging
        Debug.WriteLine($"API Service initialized with URL: {_baseUrl}");
    }

    private string GetApiUrl()
    {
        // Default API URL (works for local debugging on Windows/Mac)
        string apiUrl = "http://localhost:5001/api/products";

#if ANDROID
        // When running on Android Emulator, localhost refers to the emulator's own loopback interface
        // 10.0.2.2 is a special alias to the host's localhost
        apiUrl = "http://10.0.2.2:5001/api/products";
#elif IOS
        // For iOS simulators, we can use a special localhost alias
        apiUrl = "http://localhost:5001/api/products";
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
            // Check if authenticated as admin user
            if (!IsAuthorizedForAdminActions())
            {
                Debug.WriteLine("User not authorized to add products. Need admin role.");
                return product;
            }
            
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
            // Ensure we have the most up-to-date token
            if (_httpClient.DefaultRequestHeaders.Authorization == null && _authService.IsAuthenticated)
            {
                _authService.ConfigureHttpClient(_httpClient);
                Debug.WriteLine("Re-applied auth token before product add");
            }
            
            // Log the current authorization header
            if (_httpClient.DefaultRequestHeaders.Authorization != null)
            {
                Debug.WriteLine($"Authorization header: {_httpClient.DefaultRequestHeaders.Authorization.Scheme} {_httpClient.DefaultRequestHeaders.Authorization.Parameter}");
            }
            else
            {
                Debug.WriteLine("WARNING: No Authorization header present in the request!");
            }
            
            var response = await _httpClient.PostAsJsonAsync(_baseUrl, product);
            
            if (response.IsSuccessStatusCode)
            {
                var createdProduct = await response.Content.ReadFromJsonAsync<Product>(_jsonOptions);
                return createdProduct ?? product;
            }
            
            Debug.WriteLine($"Failed to add product: {response.StatusCode}");
            var content = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"Response content: {content}");
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Debug.WriteLine("Unauthorized error. Current auth state:");
                Debug.WriteLine($"IsAuthenticated: {_authService.IsAuthenticated}");
                Debug.WriteLine($"Username: {_authService.CurrentUsername}");
                Debug.WriteLine($"Role: {_authService.CurrentRole}");
                Debug.WriteLine($"Token present: {!string.IsNullOrEmpty(_authService.AuthToken)}");
            }
            
            return product;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception adding product: {ex.Message}");
            if (ex.InnerException != null)
            {
                Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            return product;
        }
    }

    /// <inheritdoc />
    public bool DeleteProduct(int id)
    {
        try
        {
            // Check if authenticated as admin user
            if (!IsAuthorizedForAdminActions())
            {
                Debug.WriteLine("User not authorized to delete products. Need admin role.");
                return false;
            }
            
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
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to delete product {id}: {response.StatusCode}, {content}");
            }
            
            // For successful delete, API returns 204 No Content
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception deleting product {id}: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Checks if the current user is authorized for admin-only actions
    /// </summary>
    private bool IsAuthorizedForAdminActions()
    {
        // Check if user is authenticated
        if (!_authService.IsAuthenticated)
        {
            Debug.WriteLine("User not authenticated - admin action denied");
            return false;
        }
        
        // Log the current authentication details
        Debug.WriteLine($"User authentication details: Username='{_authService.CurrentUsername}', Role='{_authService.CurrentRole}'");
        
        // Check if user has admin role
        var isAdmin = _authService.CurrentRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        
        if (!isAdmin)
        {
            Debug.WriteLine($"User role '{_authService.CurrentRole}' is not Admin - admin action denied");
        }
        else
        {
            Debug.WriteLine("User is authorized as Admin");
        }
        
        return isAdmin;
    }

    // Handle authentication changes to ensure the token is always current
    private void OnAuthenticationChanged(object sender, EventArgs e)
    {
        // Clear any existing auth headers
        if (_httpClient.DefaultRequestHeaders.Authorization != null)
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
        }
        
        // Add the updated auth token
        _authService.ConfigureHttpClient(_httpClient);
        Debug.WriteLine("Updated API Service auth token after authentication change");
    }
} 