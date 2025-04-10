using System;
using ProductService.Model;

namespace ProductService.Interfaces;

public interface ICategoryRepository
{
Task<List<Category>> GetAllCategoriesAsync();
}
