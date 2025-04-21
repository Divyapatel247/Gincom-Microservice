using System;
using Common.Events;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using notificationService.Hubs;
using notificationService.Services;

namespace notificationService.Consumers;

public class ProductStockUpdateConsumer : IConsumer<ProductUpdatedStock>

{
    private readonly ILogger<OrderCreatedConsumer> _logger;
    
     private readonly NotificationServiceForOrderCreated _notificationService;

    public ProductStockUpdateConsumer(NotificationServiceForOrderCreated notificationService, ILogger<OrderCreatedConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<ProductUpdatedStock> context)
    {
        var update = context.Message;
        Console.WriteLine($"[x] {update.ProductId} stock is updated to {update.NewStock}");
        _logger.LogInformation("Received update stock event: {ProductId}, newstock: {NewStock}",
               update.ProductId, update.NewStock);

               
                await _notificationService.NotifyStockUpdated(update);
        // string notificationMessage = $"{message.ProductId} stock updated to {message.NewStock}";
        // await _hubContext.Clients.All.SendAsync("RecieveNotification", new
        // {
        //     messageType = "UserNotification",
        //     details = notificationMessage
        // });
        // TODO: Send email or notification

    }


}
