using System;
using Dapper;
using MySql.Data.MySqlClient;
using ProductService.Interfaces;
using ProductService.Model;
using ProductService.Service;

namespace ProductService.Repository;

public class ProductsRepository : IProductRepository
{
    private readonly string _connectionString;


        public ProductsRepository(DatabaseConfig dbConfig)
        {
            _connectionString = dbConfig.GetConnectionString();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                INSERT INTO Products (
                    Title, Description, Price, DiscountPercentage, Rating, Stock, Tags,
                    Brand, Sku, Weight, Dimensions, WarrantyInformation, ShippingInformation,
                    AvailabilityStatus, ReturnPolicy, MinimumOrderQuantity, 
                    Thumbnail, CategoryId
                )
                VALUES (
                    @Title, @Description, @Price, @DiscountPercentage, @Rating, @Stock, @Tags,
                    @Brand, @Sku, @Weight, @Dimensions, @WarrantyInformation, @ShippingInformation,
                    @AvailabilityStatus,  @ReturnPolicy, @MinimumOrderQuantity,
                    @Thumbnail, @CategoryId
                );
                SELECT LAST_INSERT_ID();";
            var id = await connection.QuerySingleAsync<int>(sql,product);
            product.Id = id;
            return product;          
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "DELETE FROM Products WHERE Id = @Id";
            var product = await connection.ExecuteAsync(sql, new { Id = id });
            return product > 0;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"SELECT p.*, c.Id, c.Name FROM Products p LEFT JOIN Categories c ON p.CategoryId = c.Id LEFT JOIN Reviews r ON p.Id = r.ProductId";
            

                var productDict = new Dictionary<int, Product>();
                 await connection.QueryAsync<Product, Category, Review, Product>(
                    sql,
                    (product, category, Review) =>
                    {
                        if (!productDict.TryGetValue(product.Id, out var productEntry))
                {
                    productEntry = product;
                    productEntry.Category = category;
                    productDict.Add(productEntry.Id, productEntry);
                }
                if (Review != null)
                {
                    productEntry.Reviews.Add(Review);
                }
                return productEntry;
            },
            splitOn: "CategoryId,Id" // Split on Category.Id and Review.Id
        );

        return productDict.Values.ToList();
            }
        }

        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "SELECT Id, Name FROM Categories WHERE Name = @Name";
            var category = await connection.QueryFirstOrDefaultAsync<Category>(sql,
                new { Name = name });
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with name '{name}' not found.");
            }
            return category;
        }

        public async Task<Product> GetProductByIdAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        // CHANGED: Simplified and ensured Reviews are included
        var sql = @"
            SELECT p.*, c.Id AS CategoryId, c.Name AS CategoryName, r.*
            FROM Products p
            LEFT JOIN Categories c ON p.CategoryId = c.Id
            LEFT JOIN Reviews r ON p.Id = r.ProductId
            WHERE p.Id = @Id";
        var productDict = new Dictionary<int, Product>();
        await connection.QueryAsync<Product, Category, Review, Product>(
            sql,
            (product, category, review) =>
            {
                if (!productDict.TryGetValue(product.Id, out var existingProduct))
                {
                    existingProduct = product;
                    existingProduct.Category = category;
                    productDict[product.Id] = product;
                }
                if (review != null)
                {
                    existingProduct.Reviews.Add(review);
                }
                return existingProduct;
            },
            new { Id = id },
            splitOn: "CategoryId,Id"
        );
        return productDict.Values.FirstOrDefault();
    }
        
        
        //     using var connection = new MySqlConnection(_connectionString);
        //     var sql = @"
        // SELECT 
        //     p.*, c.Id, c.Name
        // FROM Products p
        // LEFT JOIN Categories c ON p.CategoryId = c.Id
        // WHERE p.Id = @Id";
        //     var results = await connection.QueryAsync<Product, Category, Product>(
        //         sql,
        //         (product, category) =>
        //         {
        //             product.Category = category;
        //             return product;
        //         },
        //         new { Id = id },
        //         splitOn: "Id"
        //     );
        //     return results.FirstOrDefault()!;


        

public async Task<List<Product>> GetProductsByCategoryAsync(string categoryName)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var sqlGetCategoryId = "SELECT Id FROM Categories WHERE Name = @CategoryName";
            var categoryId = await connection.QuerySingleOrDefaultAsync<int?>(sqlGetCategoryId, new { CategoryName = categoryName });

            if (!categoryId.HasValue)
            {
                return new List<Product>(); 
            }
            var sqlGetProducts = @"
                SELECT 
                    p.*, c.Id AS CategoryId, c.Name AS CategoryName
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryId = c.Id
                WHERE p.CategoryId = @CategoryId";
            return (await connection.QueryAsync<Product, Category, Product>(
                sqlGetProducts,
                (product, category) =>
                {
                    product.Category = category;
                    return product;
                },
                new { CategoryId = categoryId.Value },
                splitOn: "CategoryId"
            )).AsList();
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
            UPDATE Products
            SET 
                Title = @Title,
                Description = @Description,
                Price = @Price,
                Stock = @Stock,
                CategoryId = @CategoryId
            WHERE Id = @Id";
        await connection.ExecuteAsync(sql, product);
        return product;

        }


    }

