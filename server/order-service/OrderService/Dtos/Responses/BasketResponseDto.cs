using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Dtos.Responses
{
    public class BasketResponseDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public List<BasketItemResponseDto> Items { get; set; }
    }

    public class BasketItemResponseDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}