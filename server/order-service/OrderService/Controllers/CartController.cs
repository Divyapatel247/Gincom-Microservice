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
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart(string userId)
        {
            var basket = await _cartService.GetCartAsync(userId);
            if (basket == null) return NotFound();
            return Ok(basket);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToCart(string userId, [FromBody] AddToCartRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await _cartService.AddToCartAsync(userId, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("items/bulk")]
        public async Task<IActionResult> AddMultipleToCart(string userId, [FromBody] AddMultipleToCartRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await _cartService.AddMultipleToCartAsync(userId, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("items/{itemId}")]
        public async Task<IActionResult> UpdateCartItem(string userId, int itemId, [FromBody] UpdateBasketItemRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await _cartService.UpdateCartItemAsync(userId, itemId, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> RemoveCartItem(string userId, int itemId)
        {
            try
            {
                var response = await _cartService.RemoveCartItemAsync(userId, itemId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart(string userId)
        {
            try
            {
                await _cartService.ClearCartAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}