using System;
using Common.Events;
using MassTransit;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ProductService.DTOs;
using ProductService.DTOs.Review;
using ProductService.Interfaces;
using ProductService.Mapper;
using System.Text.Json;

using ProductService.Service;


namespace ProductService.Controllers;


[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _repository;
    private readonly IReviewRepository _reviewRepo;

    private readonly IPublishEndpoint _publishEndpoint;

    public ProductController(IProductRepository repository, IReviewRepository reviewRepo, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _reviewRepo = reviewRepo;
        _publishEndpoint = publishEndpoint;
    }


    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _repository.GetAllProductsAsync();
        var productDtos = products.Select(p => p.ToProductDto()).ToList();
        return Ok(productDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _repository.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        var productDto = product.ToProductDto();
        return Ok(productDto);
    }


    [HttpPost("add")]

    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO createProductDto)
    {
        var category = await _repository.GetCategoryByNameAsync(createProductDto.CategoryName);
        if (category == null)
        {
            return BadRequest("Category not found");
        }
        var product = createProductDto.ToProductFromCreate();
        product.CategoryId = category.Id;

        var createdProduct = await _repository.CreateProductAsync(product, createProductDto.RelatedProductIds);
        var productWithCategory = await _repository.GetProductByIdAsync(createdProduct.Id);
        var productDto = productWithCategory.ToProductDto();
        //  return Ok();
        return CreatedAtAction(nameof(GetProductById), new { id = productDto.Id }, productDto);
    }



    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDTO updateProductDto)
    {
        var existingProduct = await _repository.GetProductByIdAsync(id);

        if (existingProduct == null)
        {
            return NotFound();
        }

        var category = await _repository.GetCategoryByNameAsync(updateProductDto.CategoryName);
        if (category == null)
        {
            return BadRequest("Category not found");
        }

        //  Storing old stock before update
        int oldStock = existingProduct.Stock;

        existingProduct.UpdateFromDto(updateProductDto);
        existingProduct.CategoryId = category.Id;



        await _repository.UpdateProductAsync(existingProduct);
        var updatedProduct = await _repository.GetProductByIdAsync(id);
        var productDto = updatedProduct.ToProductDto();

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int userId = userIdClaim != null ? int.Parse(userIdClaim) : 0;

        Console.WriteLine("Updated Product: " + JsonSerializer.Serialize(updatedProduct));

        var count = await _repository.lowStokProductAsync();
        Console.WriteLine("count :" + count);

        if (oldStock != updatedProduct.Stock)
        {
            await _publishEndpoint.Publish<ILowStockProduct>(new
            {
                lowStock = count
            });
        }


        if (oldStock == 0 && updatedProduct.Stock > 0)
        {
            await _publishEndpoint.Publish<ProductUpdatedStock>(new ProductUpdatedStockEvent
            {
                ProductId = id,
                Title = updatedProduct.Title,
                NewStock = updatedProduct.Stock,
                UserIds = new List<int> { userId },
                UpdatedAt = DateTime.UtcNow
            });
            Console.WriteLine($"event generated for updating stock of{updatedProduct.Title} to {updatedProduct.Stock}");
        }
        return Ok(productDto);
    }



    [HttpDelete("{id}")]

    public async Task<IActionResult> DeleteProduct(int id)
    {
        var deleted = await _repository.DeleteProductAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }


    [HttpGet("category/{categoryName}")]
    public async Task<IActionResult> GetProductsByCategory(string categoryName)
    {
        var products = await _repository.GetProductsByCategoryAsync(categoryName);
        if (products == null || !products.Any())
        {
            return NotFound($"No products found for category '{categoryName}'");
        }

        var productsWithEmptyCategory = products.Where(p => string.IsNullOrEmpty(p.Category?.Name)).ToList();
        if (productsWithEmptyCategory.Any())
        {
            foreach (var product in products)
            {
                if (string.IsNullOrEmpty(product.Category?.Name))
                {
                    var category = await _repository.GetCategoryByNameAsync(categoryName);
                    product.Category = category;
                }
            }
        }
        var productDtos = products.Select(p => p.ToProductDto()).ToList();
        return Ok(productDtos);
    }



    // New Review Endpoints
    // New Review Endpoints
    [HttpPost("{productId}/reviews")]
    public async Task<IActionResult> AddReview(int productId, [FromBody] CreateReviewDto dto)
    {
        if (dto.ProductId != productId) return BadRequest("Product ID mismatch");
        if (dto.Rating < 1 || dto.Rating > 5) return BadRequest("Rating must be between 1 and 5");

        var product = await _repository.GetProductByIdAsync(productId);
        if (product == null) return NotFound("Product not found");

        // int userId = 1; // Hardcoded; replace with auth later
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int userId = int.Parse(userIdClaim);
        var review = dto.ToReviewFromCreate(userId);
        var id = await _reviewRepo.AddAsync(review);
        review.Id = id;
        return CreatedAtAction(nameof(GetReview), new { reviewId = id }, review.ToReviewDto());
    }

    [HttpGet("{productId}/reviews")]
    public async Task<IActionResult> GetReviews(int productId)
    {
        var product = await _repository.GetProductByIdAsync(productId);
        if (product == null) return NotFound("Product not found");

        var reviews = await _reviewRepo.GetByProductIdAsync(productId);
        return Ok(reviews.Select(r => r.ToReviewDto()));
    }

    [HttpGet("reviews/{reviewId}")]
    public async Task<IActionResult> GetReview(int reviewId)
    {
        var review = await _reviewRepo.GetByIdAsync(reviewId);
        if (review == null) return NotFound("Review not found");
        return Ok(review.ToReviewDto());
    }

    [HttpDelete("reviews/{reviewId}")]
    public async Task<IActionResult> DeleteReview(int reviewId)
    {
        // int userId = 1; // Hardcoded; replace with auth later
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        Console.WriteLine("User Claims:", claims);
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Use 'sub' or 'nameid'
        Console.WriteLine("Extracted userIdClaim:", userIdClaim);
        int userId = int.Parse(userIdClaim);
        Console.WriteLine($"Attempting to delete reviewId: {reviewId}, userId: {userId}");
        var success = await _reviewRepo.DeleteAsync(reviewId, userId);
        if (!success) return NotFound("Review not found or not owned by user");
        return NoContent();
    }

    [HttpPost("notifyMe")]
    public async Task<IActionResult> RegisterNotification([FromBody] notifyMeRequest request)
    {
        await _repository.RegisterNotificationRequestAsync(request.userId, request.productId);
        Console.WriteLine("notifyme clicked and data added in table");
        return Ok();
    }

    [HttpPut("{id}/stock")]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockRequest request)
    {
        Console.WriteLine($"Received productId: {id}, newStock: {request.NewStock}");
        await _repository.UpdateProductStockAsync(id, request.NewStock);
        if (request.NewStock > 0)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userIds = userIdClaim != null ? int.Parse(userIdClaim) : 0;

            await _publishEndpoint.Publish<ProductUpdatedStock>(new
            {
                ProductId = id,
                NewStock = request.NewStock,
                UserIds = userIds,
                Title = request.Title,
                UpdatedAt = DateTime.UtcNow,
            });
            Console.WriteLine($"event publish for stock added by admin product id {id}  userid {userIds}");
        }

        return Ok();
    }
    [HttpGet("check")]
    public async Task<IActionResult> CheckNotificationExists(int productId, int userId)
    {
        var exists = await _repository.CheckNotificationExistsAsync(productId, userId);
        return Ok(exists); // returns true or false
    }

    [HttpGet("user-notified-products")]
    public async Task<IActionResult> GetNotifiedProductIds([FromQuery] int userId)
    {
        if (userId == null)
            return BadRequest("userId is required.");
        var productIds = await _repository.GetNotifiedProductIdsAsync(userId);
        return Ok(productIds);
    }

    [HttpGet("users-to-notify")]
    public async Task<IActionResult> GetUsersToNotify([FromQuery] int productId)
    {
        var userIds = await _repository.WhomToNotify(productId);
        return Ok(userIds);
    }

    [HttpPost("mark-notified")]
    public async Task<IActionResult> MarkAsNotified(int productId, int userId)
    {
        await _repository.MarkNotified(productId, userId);
        return Ok();
    }


    [HttpGet("search")]
    public async Task<IActionResult> searchProduct([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Query cannot be empty.");

        var results = await _repository.SearchProductAsync(query);
        return Ok(results);
    }


    
    [HttpGet("lowStok")]
    public async Task<IActionResult> lowStokProduct()
    {
        var lowstock = await _repository.lowStokProductAsync();
        return Ok(lowstock);

    }

}

public class notifyMeRequest
{
    public int userId { get; set; }
    public int productId { get; set; }
}

public class UpdateStockRequest
{
    public int NewStock { get; set; }
    public string Title { get; set; }
}



