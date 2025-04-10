using authService.Repositories;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Security.Claims;

namespace authService
{
    public class CustomProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;

        public CustomProfileService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.FindFirst("sub")?.Value;
            if (sub != null)
            {
                var user = await _userRepository.GetByIdAsync(int.Parse(sub)); // Assume you add this method
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim("role", user.Role)
                    };
                        // new Claim("email", user.Email),
                        // new Claim("username", user.Username),

                    context.IssuedClaims.AddRange(claims);
                }
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.FindFirst("sub")?.Value;
            if (sub != null)
            {
                var user = await _userRepository.GetByIdAsync(int.Parse(sub));
                context.IsActive = user != null;
            }
        }
    }
}