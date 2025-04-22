using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using notificationService.Hubs;

namespace notificationService.Services
{
    public class NotificationServiceForOrderCreated
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly HttpClient _httpClient;
        public NotificationServiceForOrderCreated(IHubContext<NotificationHub> hubContext, IHttpClientFactory httpClientFactory)
        {
            _hubContext = hubContext;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task NotifyOrderCreated(Common.Events.OrderCreatedEvent order)
        {
            // Notification for Admin
            var adminMessage = new
            {
                MessageType = "AdminNotification",
                OrderId = order.OrderId,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
                Status = order.Status ?? "Pending",
                CreatedAt = order.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                Details = $"New order placed by User {order.UserId} for ${order.TotalAmount}. Items: {string.Join(", ", order.Items.Select(i => $"{i.Quantity} x Product {i.ProductId}"))}"
            };
            await _hubContext.Clients.Group("Admin").SendAsync("ReceiveNotification", adminMessage);
            Console.WriteLine($"Admin notification sent for OrderId: {order.OrderId}");

            // Notification for User
            var userMessage = new
            {
                MessageType = "UserNotification",
                OrderId = order.OrderId,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
                Status = order.Status ?? "Pending",
                CreatedAt = order.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                Details = $"Your order (ID: {order.OrderId}) worth ${order.TotalAmount} has been successfully placed!"
            };
            await _hubContext.Clients.Group($"User_{order.UserId}").SendAsync("ReceiveNotification", userMessage);
            Console.WriteLine($"User notification sent to User_{order.UserId} for OrderId: {order.OrderId}");
        }

        public async Task NotifyStockUpdated(Common.Events.ProductUpdatedStock update, List<int> userIds)
        {
            // Notification for Admin
            var adminMessage = new
            {
                MessageType = "AdminNotification",
                NewStock = update.NewStock,
                ProductId = update.ProductId,
                ProductTitle = update.Title
            };
            await _hubContext.Clients.Group("Admin").SendAsync("ReceiveNotification", adminMessage);
            Console.WriteLine($"Admin notification sent for OrderId: {update.Title}");

            
            // Notification for User
            // UPDATED: Notify each interested user dynamically
            foreach (var userId in userIds)
            {
                var userMessage = new
                {
                    MessageType = "UserNotification",
                    NewStock = update.NewStock,
                    ProductId = update.ProductId,
                    Details = $"Product #{update.Title} is back in stock!"
                };
                
                await _hubContext.Clients.Group($"User_{userId}").SendAsync("ReceiveNotification", userMessage);
                Console.WriteLine($"User notification sent to User_{userId} for ProductId: {update.ProductId}");


                var response = await _httpClient.PostAsync(
                $"http://localhost:5002/api/products/mark-notified?productId={update.ProductId}&userId={userId}",
                null);

                if (response.IsSuccessStatusCode)
                Console.WriteLine($"User {userId} marked as notified.");
                else Console.WriteLine("database not updated");
            }


        }


    }
}

