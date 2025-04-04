using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Dtos.Requests
{
    public class CreateOrderRequestDto
    {
        public int ProductId { get; set; }
        public string UserId { get; set; }
    }
}