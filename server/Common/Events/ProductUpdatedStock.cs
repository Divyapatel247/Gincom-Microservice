using System;

namespace Common.Events;

public interface ProductUpdatedStock
{
    int ProductId { get; }
    int NewStock { get; }
    List<int> UserIds { get; }
    DateTime UpdatedAt { get; }
}
