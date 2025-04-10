using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public class Basket
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public List<BasketItem> Items { get; set; } = new();
    }

    public class BasketItem
    {
        public int Id { get; set; }
        public int BasketId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}