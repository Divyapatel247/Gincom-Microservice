using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace notificationService.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly ITokenRepository _tokenRepository;

        public NotificationsController(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
            Console.WriteLine("NotificationsController initialized.");
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterToken([FromBody] RegisterTokenRequest request)
        {
            Console.WriteLine($"Received token registration request - UserId: {request.UserId}, Token: {request.Token}");
            try
            {
                await _tokenRepository.SaveTokenAsync(request.UserId, request.Token);
                Console.WriteLine($"Token saved successfully for UserId: {request.UserId}");
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save token for UserId: {request.UserId}, Error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

public class RegisterTokenRequest
{
    public string UserId { get; set; }
    public string Token { get; set; }
}

