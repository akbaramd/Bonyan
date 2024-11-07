using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.AspNetCore;

namespace Bonyan.AspNetCore.Job;

public class BonJobModule : BonModule
{
  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    return base.OnConfigureAsync(context);
  }

  public override Task OnPostConfigureAsync(BonConfigurationContext context)
  {
    return base.OnPostConfigureAsync(context);
  }
  


}