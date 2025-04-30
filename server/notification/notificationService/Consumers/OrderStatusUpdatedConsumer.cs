using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Events;
using MassTransit;
using notificationService.Services;

namespace notificationService.Consumers
{
    public class OrderStatusUpdatedConsumer : IConsumer<OrderStatusUpdatedEvent>
    {
        private readonly ILogger<OrderStatusUpdatedConsumer> _logger;
        private readonly NotificationServiceForOrderCreated _notificationService;

        public OrderStatusUpdatedConsumer(
            ILogger<OrderStatusUpdatedConsumer> logger,
            NotificationServiceForOrderCreated notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        public async Task Consume(ConsumeContext<OrderStatusUpdatedEvent> context)
        {
            var orderEvent = context.Message;
            _logger.LogInformation(
                "Received OrderStatusUpdatedEvent for OrderId: {OrderId}, UserId: {UserId}, OldStatus: {OldStatus}, NewStatus: {NewStatus}",
                orderEvent.OrderId, orderEvent.UserId, orderEvent.OldStatus, orderEvent.NewStatus);

            await _notificationService.NotifyOrderStatusUpdated(orderEvent);
            _logger.LogInformation("Notifications sent for OrderId: {OrderId}", orderEvent.OrderId);
        }
    }
}