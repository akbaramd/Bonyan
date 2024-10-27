using Bonyan.AspNetCore.Job;
using Bonyan.Extensions;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Modularity.Attributes;
using Microsoft.Extensions.Options;

namespace Bonyan.AspNetCore.Persistence
{
  [DependOn(typeof(BonyanAspNetCoreModule),
    typeof(BonyanJobModule))]
  public class BonyanPersistenceModule : Module
  {
    public override Task OnConfigureAsync(ModularityContext context)
    {
      var options = context.RequireService<IOptions<PersistenceConfiguration>>();
      var optionsValue = options.Value;

      foreach (var seeder in optionsValue.Seeders)
      {
        context.Services.AddSingleton(seeder);
      }

      
      return base.OnConfigureAsync(context);
    }

    public override Task OnPreConfigureAsync(ModularityContext context)
    {
    
      return base.OnPreConfigureAsync(context);
    }

  }
}
