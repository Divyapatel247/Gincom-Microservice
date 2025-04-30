
using System.ComponentModel.DataAnnotations;
using Common.Events;
using MassTransit;


namespace authService.Services
{
    public class RabbitMQPublisher
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly ILogger<RabbitMQPublisher> _logger;

        public RabbitMQPublisher(ISendEndpointProvider sendEndpointProvider, ILogger<RabbitMQPublisher> logger)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _logger = logger;
        }

        public async Task PublishUserLoggedIn(string userId, string username)
        {
            _logger.LogInformation("Publishing IUserLoggedInEvent for user {Username}", username);
           var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:xxxx"));
            await endpoint.Send<IUserLoggedInEvent>(new
            {
                UserId = userId,
                Username = username,
                Email = "medivyapatel27@gmail.com",
                LoginTime = DateTime.UtcNow
            });
            _logger.LogInformation("Published IUserLoggedInEvent for user {Username}", username);
        }

        public async Task PublishUserRegister(int count)
        {
             var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:PublishUserRegister"));
            await endpoint.Send<IUserRegisterEvent>(new
            {
                count
            });
            _logger.LogInformation("Published IUserRegisterEvent for user {Username}", count);
        }

        
    }

    // public interface IUserLoggedInEvent
    // {
    //     string UserId { get; }
    //     string Username { get; }
    //     DateTime LoginTime { get; }
    // }
}
