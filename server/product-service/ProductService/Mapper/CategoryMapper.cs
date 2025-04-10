using System;
using ProductService.DTOs.Category;
using ProductService.Model;

namespace ProductService.Mapper;

public static class CategoryMapper
{
    public static CategoryDTO ToCategoryDto(this Category category)
        {
            return new CategoryDTO
            {
                Name = category.Name
            };
        }
}
