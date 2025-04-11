using authService.Models;
using authService.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

[Route("auth")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await _userRepository.GetByUsernameAsync(request.Username) != null)
            return BadRequest("Username already exists");
        if (await _userRepository.GetByEmailAsync(request.Email) != null)
            return BadRequest("Email already exists");

        var user = new User
        {
            Email = request.Email,
            Username = request.Username,
            PasswordHash = HashPassword(request.Password),
            Role = request.Role ?? "User" // Default to "User" if not specified
        };

        if (user.Role != "User" && user.Role != "Admin")
            return BadRequest("Invalid role. Must be 'User' or 'Admin'.");

        await _userRepository.AddAsync(user);
       return Ok(new { message = "User registered successfully" });

    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}

public class RegisterRequest
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string? Role { get; set; } // Optional, defaults to "User"
}