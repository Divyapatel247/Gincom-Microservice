using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Events
{
    public class OrderStatusUpdatedEvent
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}