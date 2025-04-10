using System;
using authService.Models;

namespace authService.Repositories;

public interface IUserRepository
{
    Task<User> GetByUsernameAsync(string username);
    Task<User> GetByEmailAsync(string email);
    Task AddAsync(User user);

    Task<User> GetByIdAsync(int id);
}
