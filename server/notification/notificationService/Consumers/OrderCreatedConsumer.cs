using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Events;
using MassTransit;

namespace notificationService.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var order = context.Message;
            _logger.LogInformation("Received OrderCreatedEvent for OrderId: {OrderId}, UserId: {UserId}, TotalAmount: {TotalAmount}",
                order.OrderId, order.UserId, order.TotalAmount);

            // Log details of items
            foreach (var item in order.Items)
            {
                _logger.LogInformation("Item - ProductId: {ProductId}, Quantity: {Quantity}", item.ProductId, item.Quantity);
            }

            // Placeholder for notification logic (WebSocket to be added later)
            _logger.LogInformation("Notification to be sent for OrderId: {OrderId} (WebSocket implementation pending)", order.OrderId);

            // Acknowledge the message (optional, depending on your error handling strategy)
            await Task.CompletedTask;
        }
    }
}