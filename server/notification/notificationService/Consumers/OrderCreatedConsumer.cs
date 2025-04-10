using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Events;
using MassTransit;
using notificationService.Services;

namespace notificationService.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedConsumer> _logger;
        private readonly NotificationServiceForOrderCreated _notificationService;

        public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger, NotificationServiceForOrderCreated notificationService)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

       public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var order = context.Message;
            _logger.LogInformation("Received OrderCreatedEvent for OrderId: {OrderId}, UserId: {UserId}, TotalAmount: {TotalAmount}",
                order.OrderId, order.UserId, order.TotalAmount);

            foreach (var item in order.Items)
            {
                _logger.LogInformation("Item - ProductId: {ProductId}, Quantity: {Quantity}", item.ProductId, item.Quantity);
            }

            // Delegate notification to the service
            await _notificationService.NotifyOrderCreated(order);
            _logger.LogInformation("Notifications sent for OrderId: {OrderId}", order.OrderId);
        }
    }
}