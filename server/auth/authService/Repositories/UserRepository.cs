using System;
using authService.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace authService.Repositories;

public class UserRepository(string connectionString) : IUserRepository
{
    private readonly string _connectionString = connectionString;

    public async Task<User> GetByUsernameAsync(string username)
    {
        using var connection = new MySqlConnection(_connectionString);
        var query =  "SELECT * FROM Users WHERE Username = @Username";
        return await connection.QueryFirstOrDefaultAsync<User>(
           query, new { Username = username });
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        using var connection = new MySqlConnection(_connectionString);
        var query =  "SELECT * FROM Users WHERE Email = @Email";
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
}
