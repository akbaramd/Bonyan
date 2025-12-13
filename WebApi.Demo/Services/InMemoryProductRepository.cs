using WebApi.Demo.Models;

namespace WebApi.Demo.Services;

/// <summary>
/// In-memory product repository for demo purposes.
/// </summary>
public class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();
    private int _nextId = 1;

    public InMemoryProductRepository()
    {
        // Seed with sample data
        _products.AddRange(new[]
        {
            new Product { Id = _nextId++, Name = "Laptop", Description = "High-performance laptop", Price = 999.99m, CreatedAt = DateTime.UtcNow },
            new Product { Id = _nextId++, Name = "Mouse", Description = "Wireless mouse", Price = 29.99m, CreatedAt = DateTime.UtcNow },
            new Product { Id = _nextId++, Name = "Keyboard", Description = "Mechanical keyboard", Price = 79.99m, CreatedAt = DateTime.UtcNow }
        });
    }

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Product>>(_products);
    }

    public Task<Product?> GetByIdAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(product);
    }

    public Task<Product> CreateAsync(Product product)
    {
        product.Id = _nextId++;
        product.CreatedAt = DateTime.UtcNow;
        _products.Add(product);
        return Task.FromResult(product);
    }

    public Task<Product> UpdateAsync(Product product)
    {
        var existing = _products.FirstOrDefault(p => p.Id == product.Id);
        if (existing != null)
        {
            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.UpdatedAt = DateTime.UtcNow;
            return Task.FromResult(existing);
        }
        return Task.FromResult(product);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product != null)
        {
            _products.Remove(product);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}

