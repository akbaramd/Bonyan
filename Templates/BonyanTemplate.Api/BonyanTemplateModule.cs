using Bonyan.Job.Hangfire;
using Bonyan.Modularity;
using Bonyan.Modularity.Attributes;
using BonyanTemplate.Application;
using BonyanTemplate.Infrastructure;

namespace BonyanTemplate.Api;

[DependOn(
  // typeof(BonyanJobHangfireModule),
  typeof(BonyanTemplateApplicationModule),
  typeof(BonyanJobHangfireModule),
  typeof(InfrastructureModule)
  )]
public class BonyanTemplateModule : WebModule
{
  
}
