using System;

namespace ProductService.Model;


public class Product
{
    public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal Rating { get; set; } = 0.0m;
        public int Stock { get; set; }
        public List<string> Tags { get; set; } = new List<string>(); 
        public string Brand { get; set; } = "DefaultBrand";
        public string Sku { get; set; } = "SKU-1000";
        public decimal Weight { get; set; } = 1.0m;
        public Dimensions Dimensions { get; set; } = new Dimensions { Width = 10, Height = 12, Depth = 10 };
        public string WarrantyInformation { get; set; } = "No warranty";
        public string ShippingInformation { get; set; } = "Ships in 7 days";
        public string AvailabilityStatus { get; set; } = "In Stock";
        public List<Review> Reviews { get; set; } = new List<Review>();
        public string ReturnPolicy { get; set; } = "No return";
        public int MinimumOrderQuantity { get; set; } = 1;
        public List<string> Images { get; set; } = new List<string> { "https://placehold.co/600x400.png"};
        public string Thumbnail { get; set; } = "https://placehold.co/600x400.png";

        public int CategoryId { get; set; }
        public  Category Category { get; set; }
        public List<Product> RelatedProducts { get; set; } = new List<Product>();
}


public class Dimensions
    {
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Depth { get; set; }
    }

// public class Review
//     {
//         public string ReviewerName { get; set; } = string.Empty;
//         public string Comment { get; set; } = string.Empty;
//     }
