namespace MauiExamples.API.Models;

/// <summary>
/// Represents a user in the system
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier for the user
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Username for authentication
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Hashed password
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// Email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// User's role (e.g., "Admin", "User")
    /// </summary>
    public string Role { get; set; } = "User";
} 