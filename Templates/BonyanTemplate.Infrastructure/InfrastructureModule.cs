using Bonyan.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.MultiTenant;
using Bonyan.TenantManagement.EntityFrameworkCore;
using BonyanTemplate.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BonyanTemplate.Infrastructure;

[DependOn(
  typeof(BonyanEntityFrameworkModule),
  typeof(BonyanTenantManagementEntityFrameworkModule))]
public class InfrastructureModule : Module
{
  public override Task OnPreConfigureAsync(ServiceConfigurationContext context)
  {
    return base.OnPreConfigureAsync(context);
  }

  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    
    context.Configure<BonyanMultiTenancyOptions>(options =>
    {
      options.IsEnabled = true;
    });
    
    context.AddBonyanDbContext<BonyanTemplateBookDbContext>(c =>
    {
      c.AddDefaultRepositories(true);
    });

    context.Services.Configure<EntityFrameworkDbContextOptions>(configuration =>
    {
      configuration.UseSqlite("Data Source=BonyanTemplate.db");
    });

    return base.OnConfigureAsync(context);
  }
}

