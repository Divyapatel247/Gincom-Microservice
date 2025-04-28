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
        private readonly ITokenRepository _tokenRepository;
        public NotificationServiceForOrderCreated(IHubContext<NotificationHub> hubContext, IHttpClientFactory httpClientFactory, EmailService emailService, ITokenRepository tokenRepository)
        {
            _hubContext = hubContext;
            _httpClient = httpClientFactory.CreateClient();
            _emailService = emailService;
            _tokenRepository = tokenRepository;
            Console.WriteLine("NotificationServiceForOrderCreated initialized.");

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
            try
            {
                Console.WriteLine($"Retrieving tokens for UserId: {order.UserId}...");
                var tokens = await _tokenRepository.GetTokensForUserAsync(order.UserId);
                Console.WriteLine($"Retrieved tokens for UserId {order.UserId}: {string.Join(", ", tokens ?? new List<string>())}");

                if (tokens != null && tokens.Any())
                {
                    Console.WriteLine($"Preparing push notification for {tokens.Count} tokens for OrderId: {order.OrderId}...");
                    var message = new FirebaseAdmin.Messaging.MulticastMessage
                    {
                        Tokens = tokens.ToList(),
                        Notification = new FirebaseAdmin.Messaging.Notification
                        {
                            Title = $"Order #{order.OrderId} Confirmed",
                            Body = $"Your order worth ${order.TotalAmount} has been placed!"
                        },
                        Data = new Dictionary<string, string>
                {
                    { "orderId", order.OrderId.ToString() },
                    { "click_action", "http://localhost:4200/customer/myOrders" }
                }
                    };
                    Console.WriteLine($"Sending push notification with Title: {message.Notification.Title}, Body: {message.Notification.Body}, Click Action: {message.Data["click_action"]}...");
                    var response = await FirebaseAdmin.Messaging.FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
                    Console.WriteLine($"Push notification response: SuccessCount={response.SuccessCount}, FailureCount={response.FailureCount} for OrderId: {order.OrderId}");
                    if (response.FailureCount > 0)
                    {
                        Console.WriteLine($"Failed tokens details: {string.Join(", ", response.Responses.Where(r => !r.IsSuccess).Select(r => $"Error: {r.Exception?.Message}"))}");
                    }
                }
                else
                {
                    Console.WriteLine($"No valid tokens found for UserId: {order.UserId}. Push notification not sent.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send push notification for OrderId: {order.OrderId}, Error: {ex.Message}, StackTrace: {ex.StackTrace}");
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


    }
}

