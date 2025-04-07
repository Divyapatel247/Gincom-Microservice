// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using OrderService.Handlers;

// namespace OrderService.Services
// {
//     public class ProductServiceClient
//     {
//        private readonly HttpClient _client;

//         public ProductServiceClient(HttpClient client)
//         {
//             _client = client;
//             _client.BaseAddress = new Uri("https://localhost:5000/api/products/");
//         }

//         public async Task<ProductDto> GetProductAsync(int productId, string token)
//         {
//             _client.DefaultRequestHeaders.Authorization = new("Bearer", token);
//             var response = await _client.GetAsync($"{productId}");
//             response.EnsureSuccessStatusCode();
//             return await JsonHandler.DeserializeAsync<ProductDto>(response);
//         }
//     }

//     public class ProductDto
//     {
//         public int Id { get; set; }
//         public string ProductDetail { get; set; }
//         public decimal Price { get; set; }
//         public int Stock { get; set; }
//     }
// } 

using System.Collections.Concurrent;
using System.Text.Json;
using OrderService.Handlers;

namespace OrderService.Services
{
    public class ProductServiceClient
    {
        private readonly HttpClient _client;
        private readonly bool _isProductServiceDown = true;
        private readonly ConcurrentDictionary<int, ProductDto> _mockProducts = new();
        private readonly ConcurrentDictionary<int, int> _reservedStock = new(); // Track reserved stock

        public ProductServiceClient(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://localhost:5000/api/products/");

            // Initialize mock products
            if (_mockProducts.IsEmpty)
            {
                _mockProducts.TryAdd(1, new ProductDto { Id = 1, ProductDetail = "Dummy Product 1", Price = 1000m, Stock = 4 });
                _mockProducts.TryAdd(2, new ProductDto { Id = 2, ProductDetail = "Dummy Product 2", Price = 1500m, Stock = 30 });
            }
        }

        public async Task<ProductDto> GetProductAsync(int productId, string token = null)
        {
            if (_isProductServiceDown)
            {
                if (_mockProducts.TryGetValue(productId, out var product))
                {
                    int reserved = _reservedStock.GetValueOrDefault(productId, 0);
                    return new ProductDto
                    {
                        Id = product.Id,
                        ProductDetail = product.ProductDetail,
                        Price = product.Price,
                        Stock = product.Stock - reserved // Return available (unreserved) stock
                    };
                }
                return null;
            }

            _client.DefaultRequestHeaders.Authorization = new("Bearer", token);
            var response = await _client.GetAsync($"{productId}");
            response.EnsureSuccessStatusCode();
            return await JsonHandler.DeserializeAsync<ProductDto>(response);
        }

        private readonly ConcurrentDictionary<int, SemaphoreSlim> _locks = new();
        private SemaphoreSlim GetLock(int productId)
        {
            return _locks.GetOrAdd(productId, _ => new SemaphoreSlim(1, 1));
        }

        internal bool ReserveStock(int productId, int quantity)
        {
            var productLock = GetLock(productId);
            productLock.Wait(); // lock per product
            try
            {
                if (!_mockProducts.TryGetValue(productId, out var product))
                    return false;

                int reserved = _reservedStock.GetValueOrDefault(productId, 0);
                int available = product.Stock - reserved;

                if (available < quantity)
                    return false;

                _reservedStock.AddOrUpdate(productId, quantity, (key, oldVal) => oldVal + quantity);
                return true;
            }
            finally
            {
                productLock.Release();
            }
        }


        internal void CommitStock(int productId, int quantity)
        {
            if (_mockProducts.TryGetValue(productId, out var product))
            {
                int reserved = _reservedStock.GetValueOrDefault(productId, 0);
                if (reserved >= quantity)
                {
                    // Update the actual stock
                    _mockProducts[productId] = new ProductDto
                    {
                        Id = product.Id,
                        ProductDetail = product.ProductDetail,
                        Price = product.Price,
                        Stock = product.Stock - quantity
                    };

                    // Subtract from reserved
                    int remaining = reserved - quantity;

                    if (remaining <= 0)
                    {
                        // Remove reservation if none left
                        _reservedStock.TryRemove(productId, out _);
                    }
                    else
                    {
                        _reservedStock[productId] = remaining;
                    }
                }
            }
        }


        internal void ReleaseStock(int productId, int quantity)
        {
            _reservedStock.AddOrUpdate(productId, 0, (key, oldValue) => Math.Max(0, oldValue - quantity));
        }

        internal void ResetStock()
        {
            _mockProducts.Clear();
            _reservedStock.Clear();
            _mockProducts.TryAdd(1, new ProductDto { Id = 1, ProductDetail = "Dummy Product 1", Price = 1000m, Stock = 4 });
            _mockProducts.TryAdd(2, new ProductDto { Id = 2, ProductDetail = "Dummy Product 2", Price = 1500m, Stock = 30 });
        }

        internal Dictionary<int, ProductDto> GetMockProducts()
        {
            return _mockProducts.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string ProductDetail { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}