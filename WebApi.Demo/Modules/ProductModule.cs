using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using WebApi.Demo.Controllers;
using WebApi.Demo.Services;

namespace WebApi.Demo.Modules;

/// <summary>
/// Product management module demonstrating modular architecture.
/// </summary>
public class ProductModule : BonModule
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        // Register product services
        context.Services.AddScoped<IProductService, ProductService>();
        context.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();

        // Configure product-specific options
        context.ConfigureOptions<ProductModuleOptions>(options =>
        {
            options.MaxProductsPerPage = 100;
            options.EnableCaching = true;
            options.CacheExpirationMinutes = 5;
        });

        return ValueTask.CompletedTask;
    }

    public override ValueTask OnPostConfigureAsync(BonPostConfigurationContext context, CancellationToken cancellationToken = default)
    {
        // Register controllers from this module
        // Note: Controllers are auto-discovered by ASP.NET Core, but we can register additional services here
        
        return ValueTask.CompletedTask;
    }
}

/// <summary>
/// Configuration options for the Product module.
/// </summary>
public class ProductModuleOptions
{
    public int MaxProductsPerPage { get; set; } = 50;
    public bool EnableCaching { get; set; } = false;
    public int CacheExpirationMinutes { get; set; } = 5;
}

