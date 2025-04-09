using System;

namespace ProductService.Model;

public class Review
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public string Description { get; set; }
    public int Rating { get; set; } 
    public DateTime CreatedAt { get; set; }
}
