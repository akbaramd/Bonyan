using Bonyan.AspNetCore.Persistence.EntityFrameworkCore.Abstractions;
using Bonyan.AspNetCore.Persistence.EntityFrameworkCore.Builders;
using Bonyan.DomainDrivenDesign.Application;
using Bonyan.Exceptions;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Bonyan.AspNetCore.Persistence.EntityFrameworkCore;

public static class Extensions
{
  public static ModularityContext AddBonyanDbContext<TDbContext>(this ModularityContext context,
    Action<IDbContextRegistrationOptionBuilder>? optionsBuilder = null) where TDbContext : BonyanDbContext<TDbContext>
  {
    var option = new DbContextRegistrationOptionBuilder(context.Services,typeof(TDbContext));
    optionsBuilder?.Invoke(option);
    
    context.Services.AddTransient<TDbContext>();
    context.Services.AddTransient<IUnitOfWork, EfCoreUnitOfWork<TDbContext>>();

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
