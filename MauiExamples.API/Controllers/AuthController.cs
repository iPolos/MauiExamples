using Microsoft.AspNetCore.Mvc;
using MauiExamples.API.Models;
using MauiExamples.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace MauiExamples.API.Controllers;

/// <summary>
/// Authentication API Controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;
    
    /// <summary>
    /// Constructor for AuthController
    /// </summary>
    /// <param name="authService">Authentication service</param>
    /// <param name="logger">Logger instance</param>
    public AuthController(AuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }
    
    /// <summary>
    /// Authenticate a user and get a JWT token
    /// </summary>
    /// <param name="request">Login request with username and password</param>
    /// <returns>Authentication response with token</returns>
    /// <response code="200">Returns the authentication token</response>
    /// <response code="401">If authentication fails</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        _logger.LogInformation("Login attempt for user: {Username}", request.Username);
        
        var response = await _authService.AuthenticateAsync(request.Username, request.Password);
        
        if (response == null)
        {
            _logger.LogWarning("Login failed for user: {Username} with password: {Password}", request.Username, request.Password);
            return Unauthorized(new { message = "Username or password is incorrect" });
        }
        
        _logger.LogInformation("Login successful for user: {Username}", request.Username);
        return Ok(response);
    }
    
    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <returns>Success message</returns>
    /// <response code="200">If registration is successful</response>
    /// <response code="400">If registration fails</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        _logger.LogInformation("Registration attempt for user: {Username}", request.Username);
        
        // Validate input - this is a basic validation
        if (string.IsNullOrWhiteSpace(request.Username) || 
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new { message = "Username, password, and email are required" });
        }
        
        var success = await _authService.RegisterAsync(request);
        
        if (!success)
        {
            _logger.LogWarning("Registration failed for user: {Username} - Username already exists", request.Username);
            return BadRequest(new { message = "Username already exists" });
        }
        
        _logger.LogInformation("Registration successful for user: {Username}", request.Username);
        return Ok(new { message = "Registration successful" });
    }
    
    /// <summary>
    /// Test endpoint to verify authentication and role
    /// </summary>
    /// <returns>User information from the token</returns>
    [HttpGet("verify")]
    [Authorize]
    public IActionResult VerifyAuth()
    {
        var username = User.Identity?.Name;
        var claims = User.Claims.Select(c => new { Type = c.Type, Value = c.Value }).ToList();
        var isAdmin = User.IsInRole("Admin");
        
        _logger.LogInformation("Auth verification for user: {Username}, IsAdmin: {IsAdmin}", username, isAdmin);
        
        return Ok(new 
        { 
            Username = username,
            IsAdmin = isAdmin,
            Claims = claims
        });
    }
} 