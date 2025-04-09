using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
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
            var product = await _productService.GetProductAsync(request.ProductId, null);
            if (product == null)
                return BadRequest("Product not found");

            var basket = await _repository.GetBasketAsync(userId);
            if (basket == null)
            {
                basket = new Basket { UserId = userId };
                basket = await _repository.CreateBasketAsync(basket);
            }

            var existingItem = basket.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            int totalRequested = (existingItem?.Quantity ?? 0) + request.Quantity;

            if (existingItem != null)
            {
                await _repository.UpdateBasketItemAsync(existingItem.Id, totalRequested);
                existingItem.Quantity = totalRequested;
            }
            else
            {
                var item = BasketMapper.ToBasketItem(request);
                await _repository.AddBasketItemAsync(item, basket.Id);
                basket.Items.Add(item);
            }

            return Ok(BasketMapper.ToBasketResponse(basket));
        }

        [HttpPost("items/bulk")]
        public async Task<IActionResult> AddMultipleToCart(string userId, [FromBody] AddMultipleToCartRequestDto request)
        {
            if (request.Items == null || !request.Items.Any())
                return BadRequest("Items cannot be empty");

            var basket = await _repository.GetBasketAsync(userId);
            if (basket == null)
            {
                basket = new Basket { UserId = userId };
                basket = await _repository.CreateBasketAsync(basket);
            }

            foreach (var itemDto in request.Items)
            {
                var existingItem = basket.Items.FirstOrDefault(i => i.ProductId == itemDto.ProductId);
                if (existingItem != null)
                {
                    int updatedQty = existingItem.Quantity + itemDto.Quantity;
                    await _repository.UpdateBasketItemAsync(existingItem.Id, updatedQty);
                    existingItem.Quantity = updatedQty;
                }
                else
                {
                    var item = new BasketItem
                    {
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity
                    };
                    await _repository.AddBasketItemAsync(item, basket.Id);
                    basket.Items.Add(item);
                }
            }

            return Ok(BasketMapper.ToBasketResponse(basket));
        }

        [HttpPut("items/{itemId}")]
        public async Task<IActionResult> UpdateCartItem(string userId, int itemId, [FromBody] UpdateBasketItemRequestDto request)
        {
            var basket = await _repository.GetBasketAsync(userId);
            if (basket == null) return NotFound("Basket not found");

            var item = basket.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return NotFound("Item not found in basket");

            await _repository.UpdateBasketItemAsync(itemId, request.Quantity);
            item.Quantity = request.Quantity;

            return Ok(BasketMapper.ToBasketResponse(basket));
        }

        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> RemoveCartItem(string userId, int itemId)
        {
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
            var basket = await _repository.GetBasketAsync(userId);
            if (basket == null) return NotFound("Basket not found");

            await _repository.ClearBasketAsync(userId);
            return NoContent();
        }

        // [HttpGet("debug/stock")]
        // public IActionResult DebugStock()
        // {
        //     var stocks = _productService.GetMockProducts();
        //     return Ok(stocks);
        // }

        // [HttpGet("reset/stock")]
        // public IActionResult ResetStock()
        // {
        //     _productService.ResetStock();
        //     return Ok("Stock reset successfully");
        // }
    }
}