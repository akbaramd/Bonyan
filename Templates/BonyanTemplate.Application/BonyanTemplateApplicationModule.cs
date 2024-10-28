using Bonyan.AutoMapper;
using Bonyan.Modularity;
using Bonyan.Modularity.Attributes;
using BonyanTemplate.Application.Dtos;
using BonyanTemplate.Application.Jobs;
using BonyanTemplate.Domain;
using Module = Bonyan.Modularity.Abstractions.Module;


namespace BonyanTemplate.Application
{
  [DependOn(typeof(BonyanTemplateDomainModule),typeof(BonyanAutoMapperModule))]
  public class BonyanTemplateApplicationModule : Module
  {
    public override Task OnConfigureAsync(ServiceConfigurationContext context)
    {
      context.Configure<BonyanAutoMapperOptions>(options =>
      {
        options.AddProfile<BookMapper>();
      });
      context.AddJob<TestJob>();
      return base.OnConfigureAsync(context);
    }
  }
}
