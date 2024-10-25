using Bonyan.AspNetCore.Job;
using Bonyan.AspNetCore.Job.Hangfire;
using Bonyan.AspNetCore.Persistence;
using Bonyan.FastEndpoints.Security;
using Bonyan.FastEndpoints.Swagger;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Modularity.Attributes;
using BonyanTemplate.Application.Jobs;
using BonyanTemplate.Infrastructure;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Hangfire;

namespace BonyanTemplate.Api;

[DependOn(
  typeof(BonyanFastEndpointSecurityModule),
  // typeof(BonyanJobHangfireModule),
  typeof(BonyanPersistenceModule),
  typeof(BonyanFastEndpointSwaggerModule),
  typeof(InfrastructureModule)
  )]
public class BonyanTemplateModule : WebModule
{
  public override Task OnPreConfigureAsync(ModularityContext context)
  {
    context.Services.Configure<JwtSigningOptions>(c =>
    {
      c.SigningKey = "adfdfadasdadsasdasdasdad";
    });

    context.Services.Configure<DocumentOptions>(c =>
    {
      c.DocumentSettings = s =>
      {
        s.Title = "My API";
        s.Version = "v1";
      };
    });
    
    return base.OnPreConfigureAsync(context);
  }

  public override Task OnConfigureAsync(ModularityContext context)
  {
    context.Services.Configure<JobConfiguration>(c =>
    {
      c.AddCronJob<TestJob>("*/1 * * * *");
    });
    return base.OnConfigureAsync(context);
  }

  
}
