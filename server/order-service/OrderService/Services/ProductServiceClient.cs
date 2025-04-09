using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using OrderService.Handlers;

namespace OrderService.Services
{
    public class ProductServiceClient
    {
        private readonly HttpClient _client;

        public ProductServiceClient(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _client.BaseAddress = new Uri("http://localhost:5002/api/products/");
        }

        public async Task<ProductDto> GetProductAsync(int productId, string token = null)
        {
            try
            {
                _client.DefaultRequestHeaders.Authorization = token != null ? new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token) : null;
                var response = await _client.GetAsync($"{productId}");
                response.EnsureSuccessStatusCode();
                return await JsonHandler.DeserializeAsync<ProductDto>(response);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching product {productId}: {ex.Message}");
                return null;
            }
        }
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal Rating { get; set; }
        public int Stock { get; set; }
        public List<string> Tags { get; set; }
        public string Brand { get; set; }
        public string Sku { get; set; }
        public decimal Weight { get; set; }
        public Dimensions Dimensions { get; set; }
        public string WarrantyInformation { get; set; }
        public string ShippingInformation { get; set; }
        public string AvailabilityStatus { get; set; }
        public List<object> Reviews { get; set; }
        public string ReturnPolicy { get; set; }
        public int MinimumOrderQuantity { get; set; }
        public List<string> Images { get; set; }
        public string Thumbnail { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class Dimensions
    {
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Depth { get; set; }
    }
}