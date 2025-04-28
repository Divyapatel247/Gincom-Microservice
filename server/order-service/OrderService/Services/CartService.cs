using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.Dtos.Requests;
using OrderService.Dtos.Responses;
using OrderService.Interfaces;
using OrderService.Mappers;
using OrderService.Models;

namespace OrderService.Services
{
public class CartService
    {
        private readonly IOrderRepository _repository;
        private readonly ProductServiceClient _productService;

        public CartService(IOrderRepository repository, ProductServiceClient productService)
        {
            _repository = repository;
            _productService = productService;
        }

        public async Task<BasketResponseDto?> GetCartAsync(string userId)
        {
            var basket = await _repository.GetBasketAsync(userId);
            if (basket == null) return null;
            return BasketMapper.ToBasketResponse(basket);
        }

        public async Task<BasketResponseDto> AddToCartAsync(string userId, AddToCartRequestDto request)
        {
            var product = await _productService.GetProductAsync(request.ProductId, null);
            if (product == null)
                throw new Exception("Product not found");

            // Check stock before adding to cart
            if (product.Stock <= 0 || product.Stock < request.Quantity)
                throw new Exception($"Insufficient stock for Product ID {request.ProductId}. Available: {product.Stock}, Requested: {request.Quantity}");

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

            return BasketMapper.ToBasketResponse(basket);
        }

        public async Task<BasketResponseDto> AddMultipleToCartAsync(string userId, AddMultipleToCartRequestDto request)
        {
            if (request.Items == null || !request.Items.Any())
                throw new Exception("Items cannot be empty");

            var basket = await _repository.GetBasketAsync(userId);
            if (basket == null)
            {
                basket = new Basket { UserId = userId };
                basket = await _repository.CreateBasketAsync(basket);
            }

            foreach (var itemDto in request.Items)
            {
                var product = await _productService.GetProductAsync(itemDto.ProductId, null);
                if (product == null)
                    throw new Exception($"Product with ID {itemDto.ProductId} not found");

                // Check stock before adding to cart
                if (product.Stock <= 0 || product.Stock < itemDto.Quantity)
                    throw new Exception($"Insufficient stock for Product ID {itemDto.ProductId}. Available: {product.Stock}, Requested: {itemDto.Quantity}");

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

            return BasketMapper.ToBasketResponse(basket);
        }

        public async Task<BasketResponseDto> UpdateCartItemAsync(string userId, int itemId, UpdateBasketItemRequestDto request)
        {
            var basket = await _repository.GetBasketAsync(userId);
            if (basket == null)
                throw new Exception("Basket not found");

            var item = basket.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new Exception("Item not found in basket");

            await _repository.UpdateBasketItemAsync(itemId, request.Quantity);
            item.Quantity = request.Quantity;

            return BasketMapper.ToBasketResponse(basket);
        }

        public async Task<BasketResponseDto> RemoveCartItemAsync(string userId, int itemId)
        {
            var basket = await _repository.GetBasketAsync(userId);
            if (basket == null)
                throw new Exception("Basket not found");

            var item = basket.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new Exception("Item not found in basket");

            await _repository.RemoveBasketItemAsync(itemId);
            basket.Items.Remove(item);

            return BasketMapper.ToBasketResponse(basket);
        }

        public async Task ClearCartAsync(string userId)
        {
            var basket = await _repository.GetBasketAsync(userId);
            if (basket == null)
                throw new Exception("Basket not found");

            await _repository.ClearBasketAsync(userId);
        }
    }
}