using authService.Repositories;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Security.Claims;
using System.Security.Cryptography;

public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
    private readonly IUserRepository _userRepository;

    public ResourceOwnerPasswordValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        var user = await _userRepository.GetByUsernameAsync(context.UserName);
        if (user != null && user.PasswordHash == HashPassword(context.Password))
        {
            context.Result = new GrantValidationResult(
                subject: user.Id.ToString(),
                authenticationMethod: "password",
                claims: new[]
                {
                    new Claim("email", user.Email),
                    new Claim("username", user.Username),
                    new Claim(ClaimTypes.Role, user.Role) 
                });
        }
        else
        {
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid credentials");
        }
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}