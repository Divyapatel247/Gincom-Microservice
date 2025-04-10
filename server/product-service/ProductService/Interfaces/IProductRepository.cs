using System;
using ProductService.Model;

namespace ProductService.Interfaces;

public interface IProductRepository
{
Task<List<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product, List<int> relatedProductIds);
        Task<Category> GetCategoryByNameAsync(string name);
        Task<Product> UpdateProductAsync(Product product, List<int> relatedProductIds);
        Task<bool> DeleteProductAsync(int id);
        Task<List<Product>> GetProductsByCategoryAsync(string categoryName);
        Task<bool> DeductStockAsync(int productId, int quantity);
}
