using System;

namespace Common.Events;

public interface ProductUpdatedStock
{
    int ProductId { get; }
    int NewStock { get; }
    string Title{ get; set; }
    List<int> UserIds { get; }
    DateTime UpdatedAt { get; }
}

public class ProductUpdatedStockEvent : ProductUpdatedStock
    {
        public int ProductId { get; set; }
        public int NewStock { get; set; }
        public string Title{ get; set; } = "";
        public List<int> UserIds { get; set; } = new List<int>();
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

