using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MauiExamples.API.Data;
using MauiExamples.API.Models;

namespace MauiExamples.API.Services;

/// <summary>
/// Service for handling authentication operations
/// </summary>
public class AuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Constructor for AuthService
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="configuration">Application configuration</param>
    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    /// <summary>
    /// Authenticate a user with username and password
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    /// <returns>Authentication response with token if successful, null otherwise</returns>
    public async Task<AuthResponse?> AuthenticateAsync(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        
        // Check if user exists and password is correct
        if (user == null || !VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }
        
        // Generate token
        var token = GenerateJwtToken(user);
        var expiration = DateTimeOffset.UtcNow.AddHours(3).ToUnixTimeSeconds(); // Token valid for 3 hours
        
        return new AuthResponse
        {
            Token = token,
            Expiration = expiration,
            Username = user.Username,
            Role = user.Role
        };
    }
    
    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="registerRequest">Registration details</param>
    /// <returns>True if registration was successful, false otherwise</returns>
    public async Task<bool> RegisterAsync(RegisterRequest registerRequest)
    {
        // Check if username already exists
        if (await _context.Users.AnyAsync(u => u.Username == registerRequest.Username))
        {
            return false;
        }
        
        // Create new user
        var user = new User
        {
            Username = registerRequest.Username,
            PasswordHash = ComputeSha256Hash(registerRequest.Password),
            Email = registerRequest.Email,
            Role = "User" // Default role
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        return true;
    }
    
    /// <summary>
    /// Verify a password against a hash
    /// </summary>
    /// <param name="password">Raw password</param>
    /// <param name="storedHash">Stored password hash</param>
    /// <returns>True if password matches hash</returns>
    private bool VerifyPassword(string password, string storedHash)
    {
        var computedHash = ComputeSha256Hash(password);
        return computedHash == storedHash;
    }
    
    /// <summary>
    /// Generate a JWT token for a user
    /// </summary>
    /// <param name="user">User to generate token for</param>
    /// <returns>JWT token string</returns>
    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "MY_FALLBACK_KEY_DO_NOT_USE_IN_PRODUCTION_PLEASE_SET_IN_CONFIG");
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(3),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };
        
        Console.WriteLine($"Creating token for user {user.Username} with role {user.Role}");
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    /// <summary>
    /// Compute SHA256 hash of a string
    /// </summary>
    /// <param name="rawData">Raw string to hash</param>
    /// <returns>Hashed string</returns>
    private static string ComputeSha256Hash(string rawData)
    {
        using var sha256Hash = SHA256.Create();
        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        
        var builder = new StringBuilder();
        foreach (var t in bytes)
        {
            builder.Append(t.ToString("x2"));
        }
        return builder.ToString();
    }
} 