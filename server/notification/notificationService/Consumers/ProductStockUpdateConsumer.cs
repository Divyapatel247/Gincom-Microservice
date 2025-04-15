using System;
using Common.Events;
using MassTransit;

namespace notificationService.Consumers;

public class ProductStockUpdateConsumer : IConsumer<ProductUpdatedStock>

{
     
    public  Task Consume(ConsumeContext<ProductUpdatedStock> context)
    {
        var message = context.Message;
        Console.WriteLine($"[x] {message.ProductId} stock is updated to {message.NewStock}");
            // TODO: Send email or notification
            return Task.CompletedTask;
    }


}
