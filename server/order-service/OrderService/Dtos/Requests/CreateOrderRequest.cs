using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Dtos.Requests
{
    public class CreateOrderRequest
    {
        public string UserEmail { get; set; } = string.Empty;
        
    }
}