using System;
using ProductService.DTOs.Review;
using ProductService.Model;

namespace ProductService.Mapper;

public static class ReviewMapper
{
    public static Review ToReviewFromCreate(this CreateReviewDto dto, int UserId)
    {
        return new Review{
            ProductId = dto.ProductId,
            UserId = UserId,
            Description = dto.Description,
            Rating = dto.Rating
        };
    }

    public static ReviewDto ToReviewDto(this Review review)
    {
        return new ReviewDto{
            Id = review.Id,
            ProductId = review.ProductId,
            UserId = review.UserId,
            Description = review.Description,
            Rating = review.Rating, // New
            CreatedAt = review.CreatedAt
        };
    }

}
