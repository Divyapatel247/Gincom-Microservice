using System;
using ProductService.Model;

namespace ProductService.Interfaces;

public interface IProductRepository
{
Task<List<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<Category> GetCategoryByNameAsync(string name);
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<List<Product>> GetProductsByCategoryAsync(string categoryName);
}
