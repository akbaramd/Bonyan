using Bonyan.AutoMapper;
using Bonyan.Job.Hangfire;
using Bonyan.Modularity;
using Bonyan.TenantManagement.Application;
using BonyanTemplate.Application.Dtos;
using BonyanTemplate.Application.Jobs;
using BonyanTemplate.Domain;
using Module = Bonyan.Modularity.Abstractions.Module;


namespace BonyanTemplate.Application
{

  public class BonyanTemplateApplicationModule : Module
  {
    public BonyanTemplateApplicationModule()
    {
      DependOn<BonyanTenantManagementApplicationModule>();
      DependOn<BonyanTemplateDomainModule>();
      DependOn<BonyanJobHangfireModule>();
    }
    public override Task OnConfigureAsync(ServiceConfigurationContext context)
    {
      context.ConfigureOptions<BonyanAutoMapperOptions>(options =>
      {
        options.AddProfile<BookMapper>();
      });
      context.AddJob<TestJob>();
      return base.OnConfigureAsync(context);
    }
  }
}
