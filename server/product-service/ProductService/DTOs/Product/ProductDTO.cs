using System;
using ProductService.DTOs.Review;
using ProductService.Model;

namespace ProductService.DTOs;

public class ProductDTO
{
    public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal Rating { get; set; }
        public int Stock { get; set; }
        public List<string> Tags { get; set; } = new List<string>(); 
        public string Brand { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public Dimensions Dimensions { get; set; } 
        public string WarrantyInformation { get; set; } = string.Empty;
        public string ShippingInformation { get; set; } = string.Empty;
        public string AvailabilityStatus { get; set; } = string.Empty;
        public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>(); 
        public string ReturnPolicy { get; set; } = string.Empty;
        public int MinimumOrderQuantity { get; set; }
        public List<string> Images { get; set; } = new List<string>(); 
        public string Thumbnail { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public List<ProductDTO> RelatedProducts { get; set; } = new List<ProductDTO>();
}
