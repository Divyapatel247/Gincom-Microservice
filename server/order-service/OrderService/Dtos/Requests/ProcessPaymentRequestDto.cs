using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Dtos.Requests
{
    public class ProcessPaymentRequestDto
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
    }
}