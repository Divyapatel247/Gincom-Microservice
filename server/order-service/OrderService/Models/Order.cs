using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}