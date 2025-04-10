using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int OrderId { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
    }
}