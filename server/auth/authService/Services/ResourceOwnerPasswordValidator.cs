using authService.Repositories;
using authService.Services;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Security.Claims;
using System.Security.Cryptography;

public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
    private readonly IUserRepository _userRepository;
    private readonly RabbitMQPublisher _publisher;

    public ResourceOwnerPasswordValidator(IUserRepository userRepository,RabbitMQPublisher publisher)
    {
        _userRepository = userRepository;
        _publisher = publisher;
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
                    new Claim("role", user.Role) // Add role claim
                }
                // );
                //  claims: new[]
                // {
                //     new Claim("role", user.Role) // Add role claim
                // }
                );
                // context.IssuedClaims.AddRange(claims.Where(c => context.RequestedClaimTypes.Contains(c.Type)));

                // Publish event
            await _publisher.PublishUserLoggedIn(user.Id.ToString(), user.Username);
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