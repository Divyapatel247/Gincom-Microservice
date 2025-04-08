using System;

namespace ProductService.DTOs.Review;

public class ReviewDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public string Description { get; set; }
    public int Rating { get; set; } // New
    public DateTime CreatedAt { get; set; }
}
