using System;
using Dapper;
using MySql.Data.MySqlClient;
using ProductService.Interfaces;
using ProductService.Model;
using ProductService.Service;

namespace ProductService.Repository;

public class CategoryRepository : ICategoryRepository
{
 private readonly string _connectionString;
        public CategoryRepository(DatabaseConfig dbConfig)
        {
            _connectionString = dbConfig.GetConnectionString();
        }
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "SELECT Id, Name FROM Categories";
            var categories = await connection.QueryAsync<Category>(sql);
            return categories.AsList();
        }
    }

