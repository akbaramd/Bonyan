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
      var options = sp.GetRequiredService<IOptions<BonEntityFrameworkDbContextOptions>>();

      var builder = new DbContextOptionsBuilder<TDbContext>();
      options.Value.DbContextOptionsAction.Invoke(builder);
      return builder.Options;
    });

    new BonEfCoreRepositoryRegistrar(option).ConfigureRepository();
    
    return context;
  }
}
