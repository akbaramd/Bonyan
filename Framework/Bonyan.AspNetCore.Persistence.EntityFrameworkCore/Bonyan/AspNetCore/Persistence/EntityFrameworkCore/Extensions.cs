using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Bonyan.AspNetCore.Persistence.EntityFrameworkCore;

public static class Extensions
{
    public static ModularityContext AddDbContext<TDbContext>(this ModularityContext context) where TDbContext : DbContext
    {
        
        EntityFrameworkCoreConfiguration coreConfiguration;

        var options = context.RequireService<IOptions<EntityFrameworkCoreConfiguration>>();
        coreConfiguration = options.Value;

        if (coreConfiguration == null)
        {
            throw new InvalidOperationException(
                $"EfCoreConfiguration is not configured. Please configure it using context.Services.Configure<EfCoreConfiguration>() in your main module. {nameof(IModule.OnPreConfigureAsync)}");
        }
        
         context.Services.AddDbContext<TDbContext>(coreConfiguration.DbContextOptionsAction);

         
         
         return context;
    }
}