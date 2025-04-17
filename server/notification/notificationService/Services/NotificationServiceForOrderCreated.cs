using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using notificationService.Hubs;

namespace notificationService.Services
{
    public class NotificationServiceForOrderCreated
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationServiceForOrderCreated(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
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
                Details = $"New order placed by User {order.UserId} for ${order.TotalAmount}. Items: {string.Join(", ", order.Items.Select(i => $"{i.Quantity}x Product {i.ProductId}"))}"
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
                Details = $"Your order (ID: {order.OrderId}) worth ${order.TotalAmount} has been successfully placed!"
            };
            await _hubContext.Clients.Group($"User_{order.UserId}").SendAsync("ReceiveNotification", userMessage);
            Console.WriteLine($"User notification sent to User_{order.UserId} for OrderId: {order.OrderId}");
        }

        public async Task NotifyStockUpdated(Common.Events.ProductUpdatedStock update)
        {
            // Notification for Admin
            var adminMessage = new
            {
                MessageType = "AdminNotification",
                NewStock = update.NewStock,
                ProductId = update.ProductId,
            };
            await _hubContext.Clients.Group("Admin").SendAsync("ReceiveNotification", adminMessage);
            Console.WriteLine($"Admin notification sent for OrderId: {update.ProductId}");

            // Notification for User
            var userMessage = new
            {
               MessageType = "UserNotification",
                NewStock = update.NewStock,
                ProductId = update.ProductId,
                Details = $"Your stock (ID: 4) worth ${update.NewStock} has been successfully placed!"
            };
            await _hubContext.Clients.Group($"User_{4}").SendAsync("ReceiveNotification", userMessage);
            Console.WriteLine($"User notification sent to User_{4} for OrderId: {4}");
        }


    }
}

        