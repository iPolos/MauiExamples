using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Diagnostics;
using MauiExamples.Models;

namespace MauiExamples.Services;

/// <summary>
/// Service for handling authentication with the API
/// </summary>
public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions;
    
    // Store the authentication response when logged in
    private AuthResponse? _currentAuth;
    
    /// <summary>
    /// Currently logged in user's username, or empty if not logged in
    /// </summary>
    public string CurrentUsername => _currentAuth?.Username ?? string.Empty;
    
    /// <summary>
    /// Currently logged in user's role, or empty if not logged in
    /// </summary>
    public string CurrentRole => _currentAuth?.Role ?? string.Empty;
    
    /// <summary>
    /// Indicates whether the user is currently logged in
    /// </summary>
    public bool IsAuthenticated => _currentAuth != null;
    
    /// <summary>
    /// The current authentication token, or null if not logged in
    /// </summary>
    public string? AuthToken => _currentAuth?.Token;
    
    /// <summary>
    /// Fires when the authentication state changes
    /// </summary>
    public event EventHandler? AuthenticationChanged;
    
    public AuthService()
    {
        _httpClient = new HttpClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        // Set the base URL for auth endpoints
        _baseUrl = GetApiUrl();
        
        Debug.WriteLine($"Auth Service initialized with URL: {_baseUrl}");
        
        // Try to load saved authentication
        LoadSavedAuthentication();
    }
    
    private string GetApiUrl()
    {
        // Default API URL (works for local debugging on Windows/Mac)
        string apiUrl = "http://localhost:5001/api/auth";

#if ANDROID
        // When running on Android Emulator, localhost refers to the emulator's own loopback interface
        // 10.0.2.2 is a special alias to the host's localhost
        apiUrl = "http://10.0.2.2:5001/api/auth";
#elif IOS
        // For iOS simulators, we can use a special localhost alias
        apiUrl = "http://localhost:5001/api/auth";
#endif

        return apiUrl;
    }
    
    /// <summary>
    /// Attempt to login with the provided credentials
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    /// <returns>True if login was successful, false otherwise</returns>
    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            var loginRequest = new LoginRequest
            {
                Username = username,
                Password = password
            };
            
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/login", loginRequest);
            
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(_jsonOptions);
                
                if (authResponse != null)
                {
                    Debug.WriteLine("Login successful");
                    Debug.WriteLine($"Username: {authResponse.Username}");
                    Debug.WriteLine($"Role: {authResponse.Role}");
                    
                    if (string.IsNullOrEmpty(authResponse.Token))
                    {
                        Debug.WriteLine("WARNING: Received token is null or empty!");
                    }
                    else
                    {
                        Debug.WriteLine($"Token preview: {authResponse.Token.Substring(0, Math.Min(15, authResponse.Token.Length))}...");
                        Debug.WriteLine($"Token length: {authResponse.Token.Length} characters");
                    }
                    
                    var expiration = DateTimeOffset.FromUnixTimeSeconds(authResponse.Expiration);
                    Debug.WriteLine($"Token expires: {expiration} (UTC)");
                    
                    // Save the authentication response
                    _currentAuth = authResponse;
                    
                    // Save to secure storage
                    await SaveAuthenticationAsync();
                    
                    // Notify that auth state changed
                    AuthenticationChanged?.Invoke(this, EventArgs.Empty);
                    
                    return true;
                }
            }
            
            Debug.WriteLine($"Login failed: {response.StatusCode}");
            var content = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"Response content: {content}");
            
            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Login exception: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                Debug.WriteLine($"Inner stack trace: {ex.InnerException.StackTrace}");
            }
            return false;
        }
    }
    
    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    /// <param name="email">Email</param>
    /// <returns>True if registration was successful, false otherwise</returns>
    public async Task<bool> RegisterAsync(string username, string password, string email)
    {
        try
        {
            var registerRequest = new RegisterRequest
            {
                Username = username,
                Password = password,
                Email = email
            };
            
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/register", registerRequest);
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Registration exception: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Logout the current user
    /// </summary>
    public async Task LogoutAsync()
    {
        _currentAuth = null;
        
        // Remove from secure storage
        try
        {
            await SecureStorage.Default.SetAsync("auth_token", string.Empty);
            await SecureStorage.Default.SetAsync("auth_data", string.Empty);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error clearing secure storage: {ex.Message}");
        }
        
        // Notify that auth state changed
        AuthenticationChanged?.Invoke(this, EventArgs.Empty);
    }
    
    /// <summary>
    /// Get a preconfigured HttpClient with the auth token set
    /// </summary>
    /// <returns>HttpClient with auth header</returns>
    public HttpClient GetAuthenticatedHttpClient()
    {
        var client = new HttpClient();
        
        if (_currentAuth != null)
        {
            client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _currentAuth.Token);
        }
        
        return client;
    }
    
    /// <summary>
    /// Add the authentication token to an existing HttpClient
    /// </summary>
    /// <param name="client">HttpClient to configure</param>
    public void ConfigureHttpClient(HttpClient client)
    {
        // First, remove any existing Authorization header to avoid duplicates
        if (client.DefaultRequestHeaders.Contains("Authorization"))
        {
            client.DefaultRequestHeaders.Remove("Authorization");
            Debug.WriteLine("Removed existing Authorization header");
        }
        
        if (_currentAuth != null && !string.IsNullOrEmpty(_currentAuth.Token))
        {
            client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _currentAuth.Token);
            
            Debug.WriteLine($"Added Authorization header with token: Bearer {_currentAuth.Token.Substring(0, 15)}...");
            Debug.WriteLine($"Token length: {_currentAuth.Token.Length} characters");
            
            // Check if token might be malformed
            if (_currentAuth.Token.Contains(" "))
            {
                Debug.WriteLine("WARNING: Token contains spaces which may cause issues!");
            }
        }
        else
        {
            Debug.WriteLine("No authentication token available to configure HttpClient");
            if (_currentAuth == null)
            {
                Debug.WriteLine("_currentAuth is null");
            }
            else if (string.IsNullOrEmpty(_currentAuth.Token))
            {
                Debug.WriteLine("_currentAuth.Token is null or empty");
            }
        }
    }
    
    /// <summary>
    /// Save authentication to secure storage
    /// </summary>
    private async Task SaveAuthenticationAsync()
    {
        if (_currentAuth == null) return;
        
        try
        {
            // Save the token itself for quick access
            await SecureStorage.Default.SetAsync("auth_token", _currentAuth.Token);
            
            // Save the full auth response as JSON
            var authJson = JsonSerializer.Serialize(_currentAuth);
            await SecureStorage.Default.SetAsync("auth_data", authJson);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error saving authentication: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Load saved authentication from secure storage
    /// </summary>
    private void LoadSavedAuthentication()
    {
        try
        {
            var authJson = SecureStorage.Default.GetAsync("auth_data").Result;
            
            if (!string.IsNullOrEmpty(authJson))
            {
                Debug.WriteLine("Found saved authentication data");
                var authResponse = JsonSerializer.Deserialize<AuthResponse>(authJson, _jsonOptions);
                
                // Check if the token has expired
                if (authResponse != null)
                {
                    Debug.WriteLine($"Loaded auth for user: {authResponse.Username}, role: {authResponse.Role}");
                    
                    if (string.IsNullOrEmpty(authResponse.Token))
                    {
                        Debug.WriteLine("WARNING: Loaded token is null or empty!");
                    }
                    else
                    {
                        Debug.WriteLine($"Token preview: {authResponse.Token.Substring(0, Math.Min(15, authResponse.Token.Length))}...");
                    }
                    
                    var expiration = DateTimeOffset.FromUnixTimeSeconds(authResponse.Expiration);
                    Debug.WriteLine($"Token expiration: {expiration} (UTC)");
                    Debug.WriteLine($"Current time: {DateTimeOffset.UtcNow} (UTC)");
                    Debug.WriteLine($"Token {(expiration > DateTimeOffset.UtcNow ? "is valid" : "has expired")}");
                    
                    if (expiration > DateTimeOffset.UtcNow)
                    {
                        _currentAuth = authResponse;
                        
                        // Notify that auth state changed
                        AuthenticationChanged?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        Debug.WriteLine("Token has expired, clearing it from storage");
                        // Token has expired, clear it
                        SecureStorage.Default.Remove("auth_token");
                        SecureStorage.Default.Remove("auth_data");
                    }
                }
                else
                {
                    Debug.WriteLine("Failed to deserialize saved auth data");
                }
            }
            else
            {
                Debug.WriteLine("No saved authentication data found");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading authentication: {ex.Message}");
            if (ex.InnerException != null)
            {
                Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
        }
    }
} 