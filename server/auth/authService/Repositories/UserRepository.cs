using System;
using authService.Models;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace authService.Repositories;

public class UserRepository(string connectionString) : IUserRepository
{
    private readonly string _connectionString = connectionString;

    public async Task<User> GetByUsernameAsync(string username)
    {
        using var connection = new MySqlConnection(_connectionString);
        var query = "SELECT * FROM Users WHERE Username = @Username";
        return await connection.QueryFirstOrDefaultAsync<User>(
           query, new { Username = username });
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        using var connection = new MySqlConnection(_connectionString);
        var query = "SELECT * FROM Users WHERE Email = @Email";
        return await connection.QueryFirstOrDefaultAsync<User>(
           query, new { Email = email });
    }

    public async Task AddAsync(User user)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "INSERT INTO Users (Email, Username, PasswordHash, Role) VALUES (@Email, @Username, @PasswordHash, @Role)",
            user);
    }

    public async Task<User> GetByIdAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Id = @Id", new { Id = id });
    }

    public async Task<int> GetUserCountAsync()
    {
        using var connection = new MySqlConnection(_connectionString);
        var query = "SELECT COUNT(*) FROM Users WHERE Role = @Role";
        var result = await connection.ExecuteScalarAsync(query, new { Role = "User" });
        return result != null ? Convert.ToInt32(result) : 0;
    }
}
