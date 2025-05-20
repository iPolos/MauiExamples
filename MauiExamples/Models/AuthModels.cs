namespace MauiExamples.Models;

/// <summary>
/// Represents a login request to the API
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Username for authentication
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Password for authentication
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Represents a registration request to the API
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Username for the new account
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Password for the new account
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// Email address for the new account
    /// </summary>
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Represents an authentication response with token from the API
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// JWT token for authenticated requests
    /// </summary>
    public string Token { get; set; } = string.Empty;
    
    /// <summary>
    /// When the token expires (Unix timestamp)
    /// </summary>
    public long Expiration { get; set; }
    
    /// <summary>
    /// Logged in user's username
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Logged in user's role
    /// </summary>
    public string Role { get; set; } = string.Empty;
} 