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

        public ProductServiceClient(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://localhost:5000/api/products/");

            if (_mockProducts.IsEmpty)
            {
                _mockProducts.TryAdd(1, new ProductDto { Id = 1, ProductDetail = "Dummy Product 1", Price = 10m, Stock = 5 });
                _mockProducts.TryAdd(2, new ProductDto { Id = 2, ProductDetail = "Dummy Product 2", Price = 15m, Stock = 30 });
            }
        }

        public async Task<ProductDto> GetProductAsync(int productId, string token = null)
        {
            if (_isProductServiceDown)
            {
                if (_mockProducts.TryGetValue(productId, out var product))
                {
                    return new ProductDto
                    {
                        Id = product.Id,
                        ProductDetail = product.ProductDetail,
                        Price = product.Price,
                        Stock = product.Stock
                    };
                }
                return null;
            }

            _client.DefaultRequestHeaders.Authorization = new("Bearer", token);
            var response = await _client.GetAsync($"{productId}");
            response.EnsureSuccessStatusCode();
            return await JsonHandler.DeserializeAsync<ProductDto>(response);
        }

        internal bool DeductStock(int productId, int quantity)
        {
            lock (_mockProducts) // Prevent concurrent deductions
            {
                if (_mockProducts.TryGetValue(productId, out var product) && product.Stock >= quantity)
                {
                    _mockProducts[productId] = new ProductDto
                    {
                        Id = product.Id,
                        ProductDetail = product.ProductDetail,
                        Price = product.Price,
                        Stock = product.Stock - quantity
                    };
                    return true;
                }
                return false;
            }
        }

        internal bool RestoreStock(int productId, int quantity)
        {
            lock (_mockProducts) // Ensure atomic restoration
            {
                if (_mockProducts.TryGetValue(productId, out var product))
                {
                    _mockProducts[productId] = new ProductDto
                    {
                        Id = product.Id,
                        ProductDetail = product.ProductDetail,
                        Price = product.Price,
                        Stock = product.Stock + quantity
                    };
                    return true;
                }
                return false;
            }
        }

        internal void ResetStock()
        {
            lock (_mockProducts)
            {
                _mockProducts.Clear();
                _mockProducts.TryAdd(1, new ProductDto { Id = 1, ProductDetail = "Dummy Product 1", Price = 1000m, Stock = 5 });
                _mockProducts.TryAdd(2, new ProductDto { Id = 2, ProductDetail = "Dummy Product 2", Price = 1500m, Stock = 30 });
            }
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