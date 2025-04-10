using System;
using System.Security.Cryptography.X509Certificates;
using Dapper;
using MySql.Data.MySqlClient;
using ProductService.Interfaces;
using ProductService.Model;

namespace ProductService.Repository;

public class ReviewRepository :IReviewRepository
{
    private readonly MySqlConnection _connection;

    public ReviewRepository(MySqlConnection connection){
        _connection = connection;
    }

    public async Task<int> AddAsync(Review review){

        var sql = "Insert into Reviews (ProductId,UserId,Description,Rating) values (@ProductID, @UserId, @Description, @Rating); Select LAST_INSERT_ID();";

        return await _connection.ExecuteScalarAsync<int>(sql, review);
    }

    public async Task<List<Review>> GetByProductIdAsync(int productId)
    {
        var sql = "SELECT * FROM Reviews WHERE ProductId = @ProductId";
        return (await _connection.QueryAsync<Review>(sql, new { ProductId = productId })).ToList();
    }
    public async Task<Review> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM Reviews WHERE Id = @Id";
        return await _connection.QueryFirstOrDefaultAsync<Review>(sql, new { Id = id });
    }


    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var sql = "DELETE FROM Reviews WHERE Id = @Id AND UserId = @UserId";
        var affectedRows = await _connection.ExecuteAsync(sql, new { Id = id, UserId = userId });
        return affectedRows > 0;
    }

    
}
