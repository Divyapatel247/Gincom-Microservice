using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.Dtos.Requests;
using OrderService.Dtos.Responses;
using OrderService.Models;

namespace OrderService.Mappers
{
    public static class BasketMapper
    {
        public static BasketItem ToBasketItem(AddToCartRequestDto request)
        {
            return new BasketItem
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };
        }

        public static BasketResponseDto ToBasketResponse(Basket basket)
        {
            return new BasketResponseDto
            {
                Id = basket.Id,
                UserId = basket.UserId,
                Items = basket.Items.Select(i => new BasketItemResponseDto
                {
                    Id = i.Id,  
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };
        }
        
    }
}