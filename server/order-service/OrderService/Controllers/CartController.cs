using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderService.Dtos.Requests;
using OrderService.Interfaces;
using OrderService.Mappers;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Controllers
{
    [Route("api/orders/{userId}/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IOrderRepository _repository;
        private readonly ProductServiceClient _productService;

        public CartController(IOrderRepository repository, ProductServiceClient productService)
        {
            _repository = repository;
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart(string userId)
        {
            var basket = await _repository.GetBasketAsync(userId);
            if (basket == null) return NotFound();
            return Ok(BasketMapper.ToBasketResponse(basket));
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToCart(string userId, [FromBody] AddToCartRequestDto request)
        {
            // Validate product
            // var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            // var product = await _productService.GetProductAsync(request.ProductId, token);
            var product = await _productService.GetProductAsync(request.ProductId, null); // No token needed
            if (product == null) return BadRequest("Product not found");
            if (product.Stock < request.Quantity) return BadRequest("Insufficient stock");

            // Get or create basket
            var basket = await _repository.GetBasketAsync(userId);
            if (basket == null)
            {
                basket = new Basket { UserId = userId };
                basket = await _repository.CreateBasketAsync(basket);
            }

            // Add item
            var item = BasketMapper.ToBasketItem(request);
            await _repository.AddBasketItemAsync(item, basket.Id);
            basket.Items.Add(item);

            return Ok(BasketMapper.ToBasketResponse(basket));
        }

        [HttpPut("items/{itemId}")]
        public async Task<IActionResult> UpdateCartItem(string userId, int itemId, [FromBody] UpdateBasketItemRequestDto request)
        {
            // if (User.Identity.Name != userId) return Forbid();

            var basket = await _repository.GetBasketAsync(userId);
            if (basket == null) return NotFound("Basket not found");

            var item = basket.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return NotFound("Item not found in basket");

            // Validate stock (mocked Product Service)
            var product = await _productService.GetProductAsync(item.ProductId, null);
            if (product == null) return BadRequest("Product not found");
            if (product.Stock < request.Quantity) return BadRequest("Insufficient stock");

            await _repository.UpdateBasketItemAsync(itemId, request.Quantity);
            item.Quantity = request.Quantity;

            return Ok(BasketMapper.ToBasketResponse(basket));
        }

        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> RemoveCartItem(string userId, int itemId)
        {
            // if (User.Identity.Name != userId) return Forbid();

            var basket = await _repository.GetBasketAsync(userId);
            if (basket == null) return NotFound("Basket not found");

            var item = basket.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return NotFound("Item not found in basket");

            await _repository.RemoveBasketItemAsync(itemId);
            basket.Items.Remove(item);

            return Ok(BasketMapper.ToBasketResponse(basket));
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart(string userId)
        {
            // if (User.Identity.Name != userId) return Forbid();

            var basket = await _repository.GetBasketAsync(userId);
            if (basket == null) return NotFound("Basket not found");

            await _repository.ClearBasketAsync(userId);
            return Ok("Basket cleared");
        }

        
    }
}