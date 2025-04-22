using System;
using Dapper;
using MySql.Data.MySqlClient;
using ProductService.DTOs;
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

    public async Task<Product> CreateProductAsync(Product product, List<int> relatedProductIds)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();



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
        var id = await connection.QuerySingleAsync<int>(sql, product);
        product.Id = id;


        product.Reviews ??= new List<Review>();
        product.RelatedProducts ??= new List<Product>();

        // Insert related products
        if (relatedProductIds != null && relatedProductIds.Any())
        {
            var relatedSql = "INSERT INTO RelatedProducts (ProductId, RelatedProductId) VALUES (@ProductId, @RelatedProductId)";
            foreach (var relatedId in relatedProductIds.Where(rid => rid != product.Id))
            {
                await connection.ExecuteAsync(relatedSql, new { ProductId = product.Id, RelatedProductId = relatedId }, transaction);
            }

            // Fetch related products with categories
            // product.RelatedProducts = await connection.QueryAsync<Product, Category, Product>(
            //     "SELECT p.*, c.Id, c.Name FROM Products p LEFT JOIN Categories c ON p.CategoryId = c.Id WHERE p.Id IN @Ids",
            //     (p, c) => { p.Category = c; return p; },
            //     new { Ids = relatedProductIds },
            //     transaction,
            //     splitOn: "Id"
            // ).AsList();
        }

        await transaction.CommitAsync();
        return product;
    }



    public async Task<bool> DeleteProductAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        var sql = "DELETE FROM Products WHERE Id = @Id";
        var product = await connection.ExecuteAsync(sql, new { Id = id });
        return product > 0;
    }



    // public async Task<List<Product>> GetAllProductsAsync()
    // {
    //     using (var connection = new MySqlConnection(_connectionString))
    //     {
    //         var sql = @"
    //             SELECT 
    //                 p.*, 
    //                 c.Id, c.Name,  -- Category columns
    //                 r.Id AS ReviewId, r.ProductId, r.UserId, r.Description, r.Rating, r.CreatedAt,  -- Review columns
    //                 rp.Id AS RelatedId, rp.RelatedProductId,  -- RelatedProducts columns
    //                 pr.*  -- Related Product details
    //             FROM Products p
    //             LEFT JOIN Categories c ON p.CategoryId = c.Id
    //             LEFT JOIN Reviews r ON p.Id = r.ProductId
    //             LEFT JOIN RelatedProducts rp ON p.Id = rp.ProductId
    //             LEFT JOIN Products pr ON rp.RelatedProductId = pr.Id";

    //         var productDict = new Dictionary<int, Product>();
    //         await connection.QueryAsync<Product, Category, Review, Product, Product, Product>(
    //             sql,
    //             (product, category, review, relatedProduct, relatedProductDetail) =>
    //             {
    //                 if (!productDict.TryGetValue(product.Id, out var productEntry))
    //                 {
    //                     productEntry = product;
    //                     productEntry.Category = category;
    //                     productEntry.CategoryId = category?.Id ?? product.CategoryId; // Sync CategoryId
    //                     productEntry.Reviews = new List<Review>();
    //                     productEntry.RelatedProducts = new List<Product>(); // Initialize RelatedProducts
    //                     productDict[product.Id] = productEntry;
    //                 }
    //                 if (review != null && review.Id != 0)
    //                 {
    //                     productEntry.Reviews.Add(review);
    //                 }
    //                 if (relatedProductDetail != null && relatedProductDetail.Id != 0)
    //                 {
    //                     productEntry.RelatedProducts.Add(relatedProductDetail);
    //                 }
    //                 return productEntry;
    //             },
    //             splitOn: "Id,ReviewId,RelatedId,Id"  // Updated splitOn
    //         );
    //         return productDict.Values.ToList();
    //     }
    // }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            var sql = @"
            SELECT 
                p.*, 
                c.Id, c.Name,  
                r.Id AS ReviewId, r.ProductId, r.UserId, r.Description, r.Rating, r.CreatedAt, 
                rp.Id AS RelatedId, rp.RelatedProductId,  
                pr.*, 
                rc.Id, rc.Name  
            FROM Products p
            LEFT JOIN Categories c ON p.CategoryId = c.Id
            LEFT JOIN Reviews r ON p.Id = r.ProductId
            LEFT JOIN RelatedProducts rp ON p.Id = rp.ProductId
            LEFT JOIN Products pr ON rp.RelatedProductId = pr.Id
            LEFT JOIN Categories rc ON pr.CategoryId = rc.Id";

            var productDict = new Dictionary<int, Product>();
            await connection.QueryAsync<Product, Category, Review, Product, Product, Category, Product>(
                sql,
                (product, category, review, relatedProduct, relatedProductDetail, relatedCategory) =>
                {
                    if (!productDict.TryGetValue(product.Id, out var productEntry))
                    {
                        productEntry = product;
                        productEntry.Category = category;
                        productEntry.CategoryId = category?.Id ?? product.CategoryId;
                        productEntry.Reviews = new List<Review>();
                        productEntry.RelatedProducts = new List<Product>();
                        productDict[product.Id] = productEntry;
                    }
                    if (review != null && review.Id != 0)
                    {
                        productEntry.Reviews.Add(review);
                    }
                    if (relatedProductDetail != null && relatedProductDetail.Id != 0)
                    {
                        relatedProductDetail.Category = relatedCategory;  // Directly use relatedCategory
                        relatedProductDetail.CategoryId = relatedCategory?.Id ?? relatedProductDetail.CategoryId;
                        productEntry.RelatedProducts.Add(relatedProductDetail);
                    }
                    return productEntry;
                },
                splitOn: "Id,ReviewId,RelatedId,Id,Id"  // Adjusted splitOn
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
        var sql = @"
            SELECT 
                p.*, 
                c.Id AS CategoryId, c.Name,
                r.Id AS ReviewId, r.ProductId, r.UserId, r.Description, r.Rating, r.CreatedAt,
                rp.RelatedProductId,
                pr.*
            FROM Products p
            LEFT JOIN Categories c ON p.CategoryId = c.Id
            LEFT JOIN Reviews r ON p.Id = r.ProductId
            LEFT JOIN RelatedProducts rp ON p.Id = rp.ProductId
            LEFT JOIN Products pr ON rp.RelatedProductId = pr.Id
            WHERE p.Id = @Id";

        var productDict = new Dictionary<int, Product>();
        await connection.QueryAsync<Product, Category, Review, Product, Product, Product>(
            sql,
            (product, category, review, relatedProduct, relatedProductDetail) =>
            {
                if (!productDict.TryGetValue(product.Id, out var productEntry))
                {
                    productEntry = product;
                    productEntry.Category = category;
                    productEntry.Reviews = new List<Review>();
                    productEntry.RelatedProducts = new List<Product>();
                    productDict[product.Id] = productEntry;
                }
                if (review != null && review.Id != 0)
                {
                    productEntry.Reviews.Add(review);
                }
                if (relatedProductDetail != null && relatedProductDetail.Id != 0)
                {
                    productEntry.RelatedProducts.Add(relatedProductDetail);
                }
                return productEntry;
            },
            new { Id = id },
            splitOn: "CategoryId,ReviewId,RelatedProductId,Id"
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
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();
        var sql = @"
            UPDATE Products
            SET 
                Title = @Title,
                Description = @Description,
                Price = @Price,
                Stock = @Stock,
                CategoryId = @CategoryId,
                discountPercentage = @DiscountPercentage,
                tags = @tags
            WHERE Id = @Id";
        await connection.ExecuteAsync(sql, product, transaction);

        var deleteSql = "DELETE FROM RelatedProducts WHERE ProductId = @ProductId";
        await connection.ExecuteAsync(deleteSql, new { ProductId = product.Id }, transaction);



        await transaction.CommitAsync();
        return product;

    }

    public async Task<bool> DeductStockAsync(int productId, int quantity)
    {
        using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync(); // Explicitly open the connection

        try
        {
            using var transaction = conn.BeginTransaction();

            // Fetch current stock
            var product = await conn.QueryFirstOrDefaultAsync<Product>(
                "SELECT Stock FROM Products WHERE Id = @ProductId",
                new { ProductId = productId }, transaction);

            if (product == null)
            {
                Console.WriteLine($"Product {productId} not found");
                transaction.Rollback();
                return false;
            }

            if (product.Stock < quantity)
            {
                Console.WriteLine($"Insufficient stock for Product ID {productId}. Available: {product.Stock}, Required: {quantity}");
                transaction.Rollback();
                return false;
            }

            // Update stock
            var affectedRows = await conn.ExecuteAsync(
                "UPDATE Products SET Stock = Stock - @Quantity WHERE Id = @ProductId AND Stock >= @Quantity",
                new { ProductId = productId, Quantity = quantity }, transaction);

            if (affectedRows == 0)
            {
                Console.WriteLine($"Failed to deduct stock for Product ID {productId}. Possible concurrent update.");
                transaction.Rollback();
                return false;
            }

            transaction.Commit();
            Console.WriteLine($"Deducted {quantity} from Product ID {productId}. New stock: {product.Stock - quantity}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deducting stock for Product ID {productId}: {ex.Message}");
            // Rollback is handled by the using statement if transaction is still active
            return false;
        }
        finally
        {
            conn.Close(); // Ensure connection is closed
        }
    }

    public async Task RegisterNotificationRequestAsync(int userId, int productId)
    {
        using var connection = new MySqlConnection(_connectionString);
        var sql = @"
        INSERT IGNORE INTO StockNotificationRequests 
        (UserId, ProductId) 
        VALUES (@UserId, @ProductId)";
        await connection.ExecuteAsync(sql, new { UserId = userId, ProductId = productId });
    }

    public async Task UpdateProductStockAsync(int productId, int newStock)
    {
        using var connection = new MySqlConnection(_connectionString);
        var sql = "UPDATE Products SET Stock = @NewStock WHERE Id = @ProductId";
        await connection.ExecuteAsync(sql, new { ProductId = productId, NewStock = newStock });
    }

    public async Task<List<int>> WhomToNotify(int productId)
    {
        using var connection = new MySqlConnection(_connectionString);
        var sql = @"
            SELECT UserId 
            FROM StockNotificationRequests 
            WHERE ProductId = @ProductId 
            AND IsNotified = FALSE";

        var userIds = await connection.QueryAsync<int>(sql, new { ProductId = productId });
        return userIds.AsList();
    }

    public async Task<bool> CheckNotificationExistsAsync(int productId, int userId)
    {
        using var connection = new MySqlConnection(_connectionString);
        var sql = @"SELECT COUNT(*) 
                FROM StockNotificationRequests 
                WHERE UserId = @UserId 
                  AND ProductId = @ProductId 
                  AND IsNotified = false"; // only check unnotified entries

        var count = await connection.ExecuteScalarAsync<int>(sql, new { UserId = userId, ProductId = productId });
        return count > 0;
    }

    public async Task<IEnumerable<int>> GetNotifiedProductIdsAsync(int userId)
    {
        using var connection = new MySqlConnection(_connectionString);
        var sql = @"Select ProductId from StockNotificationRequests where userId = @userId and Isnotified = false";
        var result = await connection.QueryAsync<int>(sql, new { userid = userId });
        return result.ToList();
    }

    public async Task<IEnumerable<int>> GetUsersToNotifyAsync(int productId)
    {
        using var conn = new MySqlConnection(_connectionString);
        var sql = "SELECT UserId FROM StockNotificationRequests WHERE ProductId = @ProductId AND IsNotified = false";
        return await conn.QueryAsync<int>(sql, new { ProductId = productId });
    }

    public async Task MarkNotified(int productId, int userId)
    {
        using var conn = new MySqlConnection(_connectionString);
        var sql = "UPDATE StockNotificationRequests SET IsNotified = true WHERE ProductId = @ProductId AND UserId = @UserId";
        await conn.ExecuteAsync(sql, new { ProductId = productId, UserId = userId });

        
        var deleteSql = "DELETE FROM StockNotificationRequests WHERE ProductId = @ProductId AND UserId = @UserId";
        await conn.ExecuteAsync(deleteSql, new { ProductId = productId, UserId = userId });
        Console.WriteLine("deleting noti req deleted");
    }


}

