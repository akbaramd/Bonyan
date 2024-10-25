using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Modularity.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Bonyan.AspNetCore.Persistence.EntityFrameworkCore
{
  [DependOn(typeof(BonyanPersistenceModule))]
  public class BonyanPersistenceEntityFrameworkModule : Module
  {
      public override Task OnConfigureAsync(ModularityContext context)
      {
          var options = context.RequireService<IOptions<EntityFrameworkCoreConfiguration>>();
          var optionsValue = options.Value;
          
          
          return base.OnConfigureAsync(context);
      }
  }
}
