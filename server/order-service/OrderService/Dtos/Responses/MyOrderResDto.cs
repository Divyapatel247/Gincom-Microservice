using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.Services;

namespace OrderService.Dtos.Responses
{
    public class MyOrderResDto
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public List<MyOrderItemDetailResDto> Items { get; set; } = new List<MyOrderItemDetailResDto>();
        public DateTime? CreatedAt { get; set; }
    }

    public class MyOrderItemDetailResDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public ProductDto Product { get; set; }
    }
}