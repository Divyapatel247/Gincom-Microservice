using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Common.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using notificationService.Hubs;

namespace notificationService.Services
{
    public class NotificationServiceForOrderCreated
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly HttpClient _httpClient;
        private readonly EmailService _emailService;
        public NotificationServiceForOrderCreated(IHubContext<NotificationHub> hubContext, IHttpClientFactory httpClientFactory, EmailService emailService)
        {
            _hubContext = hubContext;
            _httpClient = httpClientFactory.CreateClient();
            _emailService = emailService;

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
                notificationType = "totalOrder",
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

            try
            {
                var subject = $"Order #{order.OrderId} Confirmed";
                var body = $@"
                    <h2>Order Confirmation</h2>
                    <p>Thank you for your order! Your order #{order.OrderId} has been confirmed.</p>
                    <p><strong>Status:</strong> {order.Status ?? "Pending"}</p>
                    <p><strong>Total Amount:</strong> ${order.TotalAmount}</p>
                    <h3>Items:</h3>
                    <ul>
                        {string.Join("", order.Items.Select(item => $"<li>Product ID: {item.ProductId}, Quantity: {item.Quantity}</li>"))}
                    </ul>
                    <p>For any questions, contact us at support@ecommerce.com.</p>
                ";
                await _emailService.SendEmailAsync(order.Email, subject, body);
                Console.WriteLine($"Email sent to {order.Email} for OrderId: {order.OrderId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email for OrderId: {order.OrderId}, Error: {ex.Message}");
            }

        }

        

        public async Task NotifyStockUpdated(Common.Events.ProductUpdatedStock update, List<int> userIds)
        {
            // Notification for Admin
            var adminMessage = new
            {
                MessageType = "AdminNotification",
                NewStock = update.NewStock,
                ProductId = update.ProductId,
                ProductTitle = update.Title,
                Details = $"You updated the stock for Product #{update.Title} to {update.NewStock}."
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

        public async Task NotifyOrderStatusUpdated(OrderStatusUpdatedEvent orderEvent)
        {
            // Notification for Admin
            var adminMessage = new
            {
                MessageType = "AdminNotification",
                OrderId = orderEvent.OrderId,
                UserId = orderEvent.UserId,
                OldStatus = orderEvent.OldStatus,
                NewStatus = orderEvent.NewStatus,
                UpdatedAt = orderEvent.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                Details = $"Order {orderEvent.OrderId} status updated from {orderEvent.OldStatus} to {orderEvent.NewStatus} for User {orderEvent.UserId}."
            };
            await _hubContext.Clients.Group("Admin").SendAsync("ReceiveNotification", adminMessage);
            Console.WriteLine($"Admin notification sent for OrderId: {orderEvent.OrderId}");

            // Notification for User
            var userMessage = new
            {
                MessageType = "UserNotification",
                OrderId = orderEvent.OrderId,
                UserId = orderEvent.UserId,
                OldStatus = orderEvent.OldStatus,
                NewStatus = orderEvent.NewStatus,
                UpdatedAt = orderEvent.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                Details = $"Your order (ID: {orderEvent.OrderId}) status has been updated from {orderEvent.OldStatus} to {orderEvent.NewStatus}."
            };
            await _hubContext.Clients.Group($"User_{orderEvent.UserId}").SendAsync("ReceiveNotification", userMessage);
            Console.WriteLine($"User notification sent to User_{orderEvent.UserId} for OrderId: {orderEvent.OrderId}");

        }

         public async Task NotifyUserCreated(IUserRegisterEvent message)
        {
            // Notification for Admin
            var adminMessage = new
            {
                MessageType = "AdminNotification",
                notificationType = "regisrterCustomer",
                count = message.count
            };
            await _hubContext.Clients.Group("Admin").SendAsync("ReceiveNotification", adminMessage);
            Console.WriteLine($"Admin notification sent for count: {message.count}");
        }

         public async Task LowStock(ILowStockProduct message)
        {
            // Notification for Admin
            var adminMessage = new
            {
                MessageType = "AdminNotification",
                notificationType = "lowStockProduct",
                lowStock = message.lowStock
            };
            await _hubContext.Clients.Group("Admin").SendAsync("ReceiveNotification", adminMessage);
            Console.WriteLine($"Admin notification sent for count: {message.lowStock}");
        }

    }
}

