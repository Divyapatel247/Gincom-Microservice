using System;
using Common.Events;
using MassTransit;

namespace ProductService.Service;

public class RabbitMQPublisher
{
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly ILogger<RabbitMQPublisher> _logger;

    public RabbitMQPublisher(ISendEndpointProvider sendEndpointProvider, ILogger<RabbitMQPublisher> logger)
    {
        _sendEndpointProvider = sendEndpointProvider;
        _logger = logger;
    }

    public async Task PublishLowStockProduct(int count)
    {
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:PublishLowStockProduct"));
        await endpoint.Send<ILowStockProduct>(new
        {
            lowStock = count
        });
        _logger.LogInformation("Published ILowStockProduct ", count);
    }
}
