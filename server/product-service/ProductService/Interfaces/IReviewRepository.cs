using System;
using ProductService.Model;

namespace ProductService.Interfaces;

public interface IReviewRepository
{
    Task<int> AddAsync(Review review);
    Task<List<Review>> GetByProductIdAsync(int productId);
    Task<Review> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id, int userId);
    
    
} 
