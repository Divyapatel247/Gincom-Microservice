using System;

namespace ProductService.DTOs;

public class CreateProductDTO
{
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public decimal DiscountPercentage { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string CategoryName { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
}
