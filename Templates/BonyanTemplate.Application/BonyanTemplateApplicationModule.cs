using Bonyan.AspNetCore.Job;
using Bonyan.AutoMapper;
using Bonyan.DependencyInjection;
using Bonyan.Job.Hangfire;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.TenantManagement.Application;
using BonyanTemplate.Application.Dtos;
using BonyanTemplate.Application.Jobs;
using BonyanTemplate.Domain;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;


namespace BonyanTemplate.Application
{

  public class BonyanTemplateApplicationModule : BonModule
  {
    public BonyanTemplateApplicationModule()
    {
      DependOn<BonTenantManagementApplicationModule>();
      DependOn<BonyanTemplateDomainModule>();
      DependOn<BonAspNetCoreWorkersHangfireModule>();
    }

    public override Task OnPreConfigureAsync(BonConfigurationContext context)
    {
      PreConfigure<IGlobalConfiguration>(c =>
      {
        c.ToString();
      });
      
      
      return base.OnPreConfigureAsync(context);
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
      
      context.ConfigureOptions<BonAutoMapperOptions>(options =>
      {
        options.AddProfile<BookMapper>();
      });

      context.Services.AddSingleton<TestBonWorker>();

      return base.OnConfigureAsync(context);
    }


    public override Task OnInitializeAsync(ServiceInitializationContext context)
    {
      context.AddBackgroundWorkerAsync<TestBonWorker>();
      return base.OnInitializeAsync(context);
    }
  }
}
