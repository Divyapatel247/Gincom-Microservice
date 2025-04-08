using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;
using OrderService.Interfaces;
using OrderService.Models;

namespace OrderService.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection Connection => new MySqlConnection(_connectionString);

        public async Task<Basket> GetBasketAsync(string userId)
        {
            using var conn = Connection;
            var basket = await conn.QueryFirstOrDefaultAsync<Basket>(
                "SELECT * FROM Basket WHERE UserId = @UserId", new { UserId = userId });
            if (basket == null) return null;

            basket.Items = (await conn.QueryAsync<BasketItem>(
                "SELECT Id, BasketId, ProductId AS ProductId, Quantity FROM BasketItems WHERE BasketId = @BasketId",
                new { BasketId = basket.Id }))
                .ToList();

            return basket;
        }

        public async Task<Basket> CreateBasketAsync(Basket basket)
        {
            using var conn = Connection;
            basket.Id = await conn.ExecuteScalarAsync<int>(
                "INSERT INTO Basket (UserId) VALUES (@UserId); SELECT LAST_INSERT_ID();",
                new { basket.UserId });
            return basket;
        }

        public async Task AddBasketItemAsync(BasketItem item, int basketId)
        {
            using var conn = Connection;
            item.BasketId = basketId;
            item.Id = await conn.ExecuteScalarAsync<int>(
                "INSERT INTO BasketItems (BasketId, ProductId, Quantity) VALUES (@BasketId, @ProductId, @Quantity); SELECT LAST_INSERT_ID();",
                item);
        }

        public async Task UpdateBasketItemAsync(int itemId, int quantity)
        {
            using var conn = Connection;
            await conn.ExecuteAsync(
                "UPDATE BasketItems SET Quantity = @Quantity WHERE Id = @Id",
                new { Quantity = quantity, Id = itemId });
        }

        public async Task RemoveBasketItemAsync(int itemId)
        {
            using var conn = Connection;
            await conn.ExecuteAsync(
                "DELETE FROM BasketItems WHERE Id = @Id",
                new { Id = itemId });
        }

        public async Task ClearBasketAsync(string userId)
        {
            using var conn = Connection;
            var basket = await GetBasketAsync(userId);
            if (basket != null)
            {
                await conn.ExecuteAsync(
                    "DELETE FROM Basket WHERE UserId = @UserId",
                    new { UserId = userId });
            }
        }

        public async Task<List<Order>> GetOrdersAsync(string userId)
        {
            using var conn = Connection;
            var orders = await conn.QueryAsync<Order>(
                "SELECT * FROM `Order` WHERE UserId = @UserId", new { UserId = userId });
            foreach (var order in orders)
            {
                order.Items = (await conn.QueryAsync<OrderItem>(
                    "SELECT * FROM OrderItems WHERE OrderId = @OrderId", new { OrderId = order.Id }))
                    .ToList();
            }
            return orders.ToList();
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            using var conn = Connection;
            var order = await conn.QueryFirstOrDefaultAsync<Order>(
                "SELECT * FROM `Order` WHERE Id = @Id", new { Id = orderId });
            if (order != null)
            {
                order.Items = (await conn.QueryAsync<OrderItem>(
                    "SELECT * FROM OrderItems WHERE OrderId = @OrderId", new { OrderId = orderId }))
                    .ToList();
            }
            return order;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            using var conn = Connection;

            order.Id = await conn.ExecuteScalarAsync<int>(
                "INSERT INTO `Order` (UserId, Status) VALUES (@UserId, @Status); SELECT LAST_INSERT_ID();",
                new { order.UserId, order.Status });

            foreach (var item in order.Items)
            {
                Console.WriteLine($"[Repository] Inserting OrderItem -> ProductId: {item.ProductId}, Quantity: {item.Quantity}");

                if (item.ProductId <= 0)
                    throw new Exception("ProductId is missing or invalid while inserting OrderItem");

                await AddOrderItemAsync(item, order.Id);
            }

            return order;
        }

        public async Task UpdateOrderAsync(Order order)
        {
            using var conn = Connection;
            await conn.ExecuteAsync(
                "UPDATE `Order` SET Status = @Status WHERE Id = @Id",
                new { order.Status, order.Id });
        }

        public async Task AddOrderItemAsync(OrderItem item, int orderId)
        {
            using var conn = Connection;
            await conn.ExecuteAsync(
                "INSERT INTO OrderItems (OrderId, ProductId, Quantity) VALUES (@OrderId, @ProductId, @Quantity)",
                new
                {
                    OrderId = orderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            using var conn = Connection;
            payment.Id = await conn.ExecuteScalarAsync<int>(
                "INSERT INTO Payment (UserId, OrderId, Status, TransactionId) VALUES (@UserId, @OrderId, @Status, @TransactionId); SELECT LAST_INSERT_ID();",
                payment);
            return payment;
        }

        public async Task<Payment> GetPaymentByOrderIdAsync(int orderId)
        {
            using var conn = Connection;
            return await conn.QueryFirstOrDefaultAsync<Payment>(
                "SELECT * FROM Payment WHERE OrderId = @OrderId", new { OrderId = orderId });
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            using var conn = Connection;
            await conn.ExecuteAsync(
                "UPDATE Payment SET UserId = @UserId, Status = @Status, TransactionId = @TransactionId WHERE Id = @Id",
                new { payment.UserId, payment.Status, payment.TransactionId, payment.Id });
        }
    }
}