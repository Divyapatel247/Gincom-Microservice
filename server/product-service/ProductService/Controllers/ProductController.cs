using System;
using Microsoft.AspNetCore.Mvc;
using ProductService.DTOs;
using ProductService.DTOs.Review;
using ProductService.Interfaces;
using ProductService.Mapper;

namespace ProductService.Controllers;


[ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly IReviewRepository _reviewRepo;
        public ProductController(IProductRepository repository, IReviewRepository reviewRepo)
        {
            _repository = repository;
            _reviewRepo = reviewRepo;
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
            existingProduct.UpdateFromDto(updateProductDto);
            existingProduct.CategoryId = category.Id;

            await _repository.UpdateProductAsync(existingProduct);
            var updatedProduct = await _repository.GetProductByIdAsync(id);
            var productDto = updatedProduct.ToProductDto();
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
    [HttpPost("{productId}/reviews")]
    public async Task<IActionResult> AddReview(int productId, [FromBody] CreateReviewDto dto)
    {
        if (dto.ProductId != productId) return BadRequest("Product ID mismatch");
        if (dto.Rating < 1 || dto.Rating > 5) return BadRequest("Rating must be between 1 and 5");

        var product = await _repository.GetProductByIdAsync(productId);
        if (product == null) return NotFound("Product not found");

        int userId = 1; // Hardcoded; replace with auth later
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
    public async Task<IActionResult> DeleteReview(int reviewId, [FromQuery] int userId)
    {
        // int userId = 1; // Hardcoded; replace with auth later
        Console.WriteLine($"Attempting to delete reviewId: {reviewId}, userId: {userId}");
        var success = await _reviewRepo.DeleteAsync(reviewId, userId);
        if (!success) return NotFound("Review not found or not owned by user");
        return NoContent();
    }
        
    }

