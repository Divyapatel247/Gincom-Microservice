using System;
using ProductService.DTOs;
using ProductService.DTOs.Review;
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
            Reviews = productModel.Reviews?.Select(r => r.ToReviewDto()).ToList() ?? new List<ReviewDto>(),
            ReturnPolicy = productModel.ReturnPolicy,
            MinimumOrderQuantity = productModel.MinimumOrderQuantity,
            Images = productModel.Images ?? new List<string>(),
            Thumbnail = productModel.Thumbnail,
            CategoryId = productModel.CategoryId,
            CategoryName = productModel.Category?.Name,

            RelatedProducts = productModel.RelatedProducts?
        .Where(p => p != null)
        .Select(p => new ProductDTO
        {
            Id = p.Id,
        Title = p.Title,
        Description = p.Description,
        Price = p.Price,
        DiscountPercentage = p.DiscountPercentage,
        Rating = p.Rating,
        Stock = p.Stock,
        Tags = p.Tags ?? new List<string>(),
        Brand = p.Brand,
        Sku = p.Sku,
        Weight = p.Weight,
        Dimensions = p.Dimensions,
        WarrantyInformation = p.WarrantyInformation,
        ShippingInformation = p.ShippingInformation,
        AvailabilityStatus = p.AvailabilityStatus,
        Reviews = new List<ReviewDto>(), // Optional: avoid fetching nested reviews
        ReturnPolicy = p.ReturnPolicy,
        MinimumOrderQuantity = p.MinimumOrderQuantity,
        Images = p.Images ?? new List<string>(),
        Thumbnail = p.Thumbnail,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name,
         RelatedProducts = new List<ProductDTO>() // <== Avoid recursion here
        }).ToList() ?? new List<ProductDTO>()
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
            Tags = createProductDto.Tags ?? new List<string>(),
            Thumbnail = createProductDto.Thumbnail,
            RelatedProducts = new List<Product>()
        };
    }

    public static void UpdateFromDto(this Product product, UpdateProductDTO updateProductDto)
    {
        product.Title = updateProductDto.Title;
        product.Description = updateProductDto.Description;
        product.Price = updateProductDto.Price;
        product.Stock = updateProductDto.Stock;
        product.DiscountPercentage = updateProductDto.DiscountPercentage;
        product.Tags = updateProductDto.Tags;
    }
}
