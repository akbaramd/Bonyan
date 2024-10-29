using Bonyan.EntityFrameworkCore.Abstractions;
using Bonyan.EntityFrameworkCore.Builders;
using Bonyan.Layer.Application;
using Bonyan.Modularity;
using Bonyan.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Bonyan.EntityFrameworkCore;

public static class Extensions
{
  public static ServiceConfigurationContext AddBonyanDbContext<TDbContext>(this ServiceConfigurationContext context,
    Action<IDbContextRegistrationOptionBuilder>? optionsBuilder = null) where TDbContext : BonyanDbContext<TDbContext>
  {
    var option = new DbContextRegistrationOptionBuilder(context.Services,typeof(TDbContext));
    optionsBuilder?.Invoke(option);
    
    context.Services.AddTransient<TDbContext>();


    context.Services.TryAddTransient(sp =>
    {
      var options = sp.GetRequiredService<IOptions<EntityFrameworkDbContextOptions>>();

      var builder = new DbContextOptionsBuilder<TDbContext>();
      options.Value.DbContextOptionsAction.Invoke(builder);
      return builder.Options;
    });

    new EfCoreRepositoryRegistrar(option).ConfigureRepository();
    
    return context;
  }
}
