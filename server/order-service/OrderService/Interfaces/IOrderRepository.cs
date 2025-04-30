using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.Models;

namespace OrderService.Interfaces
{
public interface IOrderRepository
    {
        Task<Basket> GetBasketAsync(string userId);
        Task<Basket> CreateBasketAsync(Basket basket);
        Task AddBasketItemAsync(BasketItem item, int basketId);
        Task UpdateBasketItemAsync(int itemId, int quantity);
        Task RemoveBasketItemAsync(int itemId);
        Task ClearBasketAsync(string userId);
        Task<List<Order>> GetOrdersAsync(string userId);
        Task<List<Order>> GetAllOrdersAsync(); 
        Task<List<Order>> GetOrdersByProductIdAsync(int productId); 
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<Order> CreateOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task AddOrderItemAsync(OrderItem item, int orderId);
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Payment> GetPaymentByOrderIdAsync(int orderId);
        Task UpdatePaymentAsync(Payment payment);
    }
}