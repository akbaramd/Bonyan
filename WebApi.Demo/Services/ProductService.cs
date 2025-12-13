using Microsoft.Extensions.Options;
using WebApi.Demo.Models;
using WebApi.Demo.Modules;

namespace WebApi.Demo.Services;

/// <summary>
/// Product service implementation.
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ProductModuleOptions _options;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository repository,
        IOptions<ProductModuleOptions> options,
        ILogger<ProductService> logger)
    {
        _repository = repository;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        _logger.LogInformation("Getting all products (MaxPerPage: {MaxPerPage})", _options.MaxProductsPerPage);
        return await _repository.GetAllAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        _logger.LogInformation("Getting product by ID: {ProductId}", id);
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        _logger.LogInformation("Creating product: {ProductName}", product.Name);
        product.CreatedAt = DateTime.UtcNow;
        return await _repository.CreateAsync(product);
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        _logger.LogInformation("Updating product: {ProductId}", product.Id);
        product.UpdatedAt = DateTime.UtcNow;
        return await _repository.UpdateAsync(product);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        _logger.LogInformation("Deleting product: {ProductId}", id);
        return await _repository.DeleteAsync(id);
    }
}

