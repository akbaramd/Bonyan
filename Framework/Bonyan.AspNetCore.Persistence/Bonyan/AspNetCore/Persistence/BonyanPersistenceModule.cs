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
    public override Task OnPreConfigureAsync(ModularityContext context)
    {
      context.Services.Configure<JobConfiguration>(c =>
      {
        c.AddBackgroundJob<SeedBackgroundJobs>();
      });
      
      return base.OnPreConfigureAsync(context);
    }

  }
}
