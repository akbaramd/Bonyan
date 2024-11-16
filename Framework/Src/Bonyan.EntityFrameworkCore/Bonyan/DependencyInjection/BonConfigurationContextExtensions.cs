using Bonyan.EntityFrameworkCore;
using Bonyan.EntityFrameworkCore.Abstractions;
using Bonyan.EntityFrameworkCore.Builders;
using Bonyan.Modularity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Bonyan.DependencyInjection;

public static class Extensions
{
  public static BonConfigurationContext AddBonDbContext<TDbContext>(this BonConfigurationContext context,
    Action<IBonDbContextRegistrationOptionBuilder>? optionsBuilder = null) where TDbContext : BonDbContext<TDbContext>
  {
    var option = new BonDbContextRegistrationOptionBuilder(context.Services,typeof(TDbContext));
    optionsBuilder?.Invoke(option);

    context.Services.AddTransient<TDbContext>();
    
    context.Services.TryAddTransient(sp =>
    {
      var builder = new DbContextOptionsBuilder<TDbContext>();
      option.DbContextOptionsAction.Invoke(builder);
      return builder.Options;
    });
    
    foreach (var additionalDbContext in option.AdditionalDbContexts)
    {
      context.Services.AddTransient(additionalDbContext);
      
      // Dynamically create DbContextOptions for the additional DbContext
      var dbContextOptionsType = typeof(DbContextOptions<>).MakeGenericType(additionalDbContext);
      var dbContextOptionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(additionalDbContext);

      context.Services.TryAddTransient(dbContextOptionsType, sp =>
      {
        var builder = Activator.CreateInstance(dbContextOptionsBuilderType);
        if (builder is DbContextOptionsBuilder optionsBuilderInstance)
        {
          option.DbContextOptionsAction.Invoke(optionsBuilderInstance);
          return optionsBuilderInstance.Options;
        }

        throw new InvalidOperationException($"Failed to create DbContextOptions for {additionalDbContext.Name}");
      });
    }

    

    new BonEfCoreRepositoryRegistrar(option).ConfigureRepository();
    
    return context;
  }
}
