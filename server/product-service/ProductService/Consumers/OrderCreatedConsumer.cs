using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Events;
using MassTransit;
using ProductService.Interfaces;

namespace ProductService.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly IProductRepository _repository;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedConsumer(IProductRepository repository,IPublishEndpoint publishEndpoint)
        {
            _repository = repository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var order = context.Message;
            foreach (var item in order.Items)
            {
                var success = await _repository.DeductStockAsync(item.ProductId, item.Quantity);
                if (!success)
                {
                    Console.WriteLine($"Failed to deduct stock for Product ID {item.ProductId}");
                    // Optionally, reject the message or notify OrderService to rollback
                    continue;
                }

                //increment soldProduct
                var updateSuccess = await _repository.IncrementSoldCountAsync(item.ProductId, item.Quantity);
                if(!updateSuccess){
                    Console.WriteLine("stock deducted but failed to increment sold count");
                }
                var count = await _repository.lowStokProductAsync();
                Console.WriteLine("count :" + count);

                await _publishEndpoint.Publish<ILowStockProduct>(new
                {
                    lowStock = count
                });
                Console.WriteLine($"Successfully deducted {item.Quantity} from Product ID {item.ProductId}");
            }
        }
    }
}