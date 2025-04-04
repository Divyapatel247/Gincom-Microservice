using System;
using ProductService.DTOs;
using ProductService.Model;

namespace ProductService.Mapper;

public static class ProductMapper
{
public static ProductDTO ToProductDto(this Product productModel)
        {
            return new ProductDTO
            {
                Id = productModel.Id,
                Title = productModel.Title,
                Description = productModel.Description,
                Price = productModel.Price,
                DiscountPercentage = productModel.DiscountPercentage,
                Rating = productModel.Rating,
                Stock = productModel.Stock,
                Tags = productModel.Tags ?? new List<string>(), 
                Brand = productModel.Brand,
                Sku = productModel.Sku,
                Weight = productModel.Weight,
                Dimensions = productModel.Dimensions, 
                WarrantyInformation = productModel.WarrantyInformation,
                ShippingInformation = productModel.ShippingInformation,
                AvailabilityStatus = productModel.AvailabilityStatus,
                Reviews = productModel.Reviews ?? new List<Review>(), 
                ReturnPolicy = productModel.ReturnPolicy,
                MinimumOrderQuantity = productModel.MinimumOrderQuantity,
                Images = productModel.Images ?? new List<string>(), 
                Thumbnail = productModel.Thumbnail,
                CategoryId = productModel.CategoryId,
                CategoryName = productModel.Category?.Name ?? string.Empty
            };
        }

         public static Product ToProductFromCreate(this CreateProductDTO createProductDto)
        {
            return new Product
            {
                Title = createProductDto.Title,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                Stock = createProductDto.Stock,
                DiscountPercentage = createProductDto.DiscountPercentage,
                Tags = createProductDto.Tags            
            };
        }

        public static void UpdateFromDto(this Product product, UpdateProductDTO updateProductDto)
        {
            product.Title = updateProductDto.Title;
            product.Description = updateProductDto.Description;
            product.Price = updateProductDto.Price;
            product.Stock = updateProductDto.Stock;
        }
}
