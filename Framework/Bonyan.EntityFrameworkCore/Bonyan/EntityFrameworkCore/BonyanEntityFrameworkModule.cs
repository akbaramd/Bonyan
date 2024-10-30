using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Modularity.Attributes;
using Bonyan.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.EntityFrameworkCore
{
  [DependOn(typeof(BonyanUnitOfWorkModule))]
  public class BonyanEntityFrameworkModule : Module
  {
      public override Task OnConfigureAsync(ServiceConfigurationContext context)
      {
        context.Services.AddTransient(typeof(IDbContextProvider<>),typeof(UnitOfWorkDbContextProvider<>));
          return base.OnConfigureAsync(context);
      }
  }
}
