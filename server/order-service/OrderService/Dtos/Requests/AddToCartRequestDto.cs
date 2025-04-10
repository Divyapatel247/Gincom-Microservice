using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Dtos.Requests
{
    public class AddToCartRequestDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}