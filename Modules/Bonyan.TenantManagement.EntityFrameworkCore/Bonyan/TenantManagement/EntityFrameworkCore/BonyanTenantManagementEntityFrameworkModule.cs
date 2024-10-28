
using Bonyan.AspNetCore;
using Bonyan.AspNetCore.MultiTenant;
using Bonyan.AspNetCore.Persistence.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Modularity.Attributes;
using Bonyan.TenantManagement.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.TenantManagement.EntityFramework;

[DependOn(typeof(BonyanPersistenceEntityFrameworkModule),
  typeof(BonyanAspNetCoreMultiTenantModule))]
public class BonyanTenantManagementEntityFrameworkModule : Modularity.Abstractions.Module
{
  public override Task OnConfigureAsync(ModularityContext context)
  {
    context.Services.AddTransient<ITenantRepository, TenantRepository>();
    context.AddBonyanDbContext<BonyanTenantDbContext>(c =>
    {
      c.AddDefaultRepositories();
    });
    
    return base.OnConfigureAsync(context);
  }
}
