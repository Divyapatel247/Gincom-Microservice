using System;
using ProductService.Model;

namespace ProductService.Interfaces;

public interface IProductRepository
{
Task<List<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product, List<int> relatedProductIds);
        Task<Category> GetCategoryByNameAsync(string name);
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<List<Product>> GetProductsByCategoryAsync(string categoryName);
        Task<bool> DeductStockAsync(int productId, int quantity);

        Task RegisterNotificationRequestAsync(int userId, int productId);
        Task UpdateProductStockAsync(int productId, int newStock);
        Task <bool> CheckNotificationExistsAsync(int productId, int userId);

        Task<IEnumerable<int>> GetNotifiedProductIdsAsync(int userId); 
         Task<IEnumerable<int>> GetUsersToNotifyAsync(int productId);
         Task MarkNotified(int productId, int userId);
        Task<List<int>> WhomToNotify(int productId);
}
