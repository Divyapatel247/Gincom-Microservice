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
    private readonly HttpClient _httpClient;
    private readonly NotificationServiceForOrderCreated _notificationService;

    public ProductStockUpdateConsumer(NotificationServiceForOrderCreated notificationService, ILogger<OrderCreatedConsumer> logger, IHttpClientFactory httpClientFactory)
    {
        _notificationService = notificationService;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }
    public async Task Consume(ConsumeContext<ProductUpdatedStock> context)
    {
        var update = context.Message;
        var productId = context.Message.ProductId;
        var ProductTitle = context.Message.Title;
        var newStock = context.Message.NewStock;


        // 1. Call ProductService API to get userIds who requested notification
        var response = await _httpClient.GetAsync($"http://localhost:5002/api/products/users-to-notify?productId={productId}");
        Console.WriteLine("details ", productId, newStock);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("userids", response);
            Console.WriteLine("Failed to get users to notify.");
            return;
        }

        var userIds = await response.Content.ReadFromJsonAsync<List<int>>();
        Console.WriteLine("userids", userIds);
        Console.WriteLine($"[x] {update.ProductId} stock is updated to {update.NewStock}");
        _logger.LogInformation("Received update stock event: {ProductTitle}, newstock: {NewStock}",
               update.ProductId, update.NewStock);
        await _notificationService.NotifyStockUpdated(update, userIds);
        _logger.LogInformation("Product stock updated: ProductId={ProductId}, NewStock={NewStock}",
            productId, newStock);

    }


}

// string notificationMessage = $"{message.ProductId} stock updated to {message.NewStock}";
// await _hubContext.Clients.All.SendAsync("RecieveNotification", new
// {
//     messageType = "UserNotification",
//     details = notificationMessage
// });
// TODO: Send email or notification