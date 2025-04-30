using System;

namespace ProductService.DTOs.Review;

public class CreateReviewDto
{
    public int ProductId { get; set; }
    public string Description { get; set; }
    public int Rating { get; set; }
}
