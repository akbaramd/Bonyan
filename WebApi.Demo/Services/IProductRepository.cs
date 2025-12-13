using WebApi.Demo.Models;

namespace WebApi.Demo.Services;

/// <summary>
/// Product repository interface.
/// </summary>
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
}

