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
        }
    }
}

        