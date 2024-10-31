using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.EntityFrameworkCore
{
  
  public class BonyanEntityFrameworkModule : Module
  {
      public BonyanEntityFrameworkModule()
      {
          DependOn<BonyanUnitOfWorkModule>();
      }
      public override Task OnConfigureAsync(ServiceConfigurationContext context)
      {
        context.Services.AddTransient(typeof(IDbContextProvider<>),typeof(UnitOfWorkDbContextProvider<>));
          return base.OnConfigureAsync(context);
      }
  }
}
