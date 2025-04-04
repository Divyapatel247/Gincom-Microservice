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

using OrderService.Handlers;

namespace OrderService.Services
{
    public class ProductServiceClient
    {
        private readonly HttpClient _client;
        private readonly bool _isProductServiceDown = true;

        public ProductServiceClient(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://localhost:5000/api/products/");
        }

        public async Task<ProductDto> GetProductAsync(int productId, string token = null)
        {
            if (_isProductServiceDown)
            {
                return new ProductDto
                {
                    Id = productId,
                    ProductDetail = $"Dummy Product {productId}",
                    Price = 1000m,
                    Stock = 50
                };
            }

            var response = await _client.GetAsync($"{productId}");
            response.EnsureSuccessStatusCode();
            return await JsonHandler.DeserializeAsync<ProductDto>(response);
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