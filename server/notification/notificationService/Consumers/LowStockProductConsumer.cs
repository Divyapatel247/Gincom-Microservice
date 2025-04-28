using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Events;
using MassTransit;
using notificationService.Services;

namespace notificationService.Consumers
{
    public class LowStockProductConsumer : IConsumer<ILowStockProduct>
    {
        private readonly ILogger<OrderCreatedConsumer> _logger;
        private readonly NotificationServiceForOrderCreated _notificationService;
        public LowStockProductConsumer(ILogger<OrderCreatedConsumer> logger, NotificationServiceForOrderCreated notificationService)
        {
            _notificationService = notificationService;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<ILowStockProduct> context)
        {
            var message = context.Message;
            _logger.LogInformation("Received LowStockProductConsumer " + message.lowStock);
                await _notificationService.LowStock(message);

        }
    }
}