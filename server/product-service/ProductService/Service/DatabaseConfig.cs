using System;

namespace ProductService.Service;

public class DatabaseConfig
{
private readonly IConfiguration _configuration;
        public DatabaseConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }
    }

