using Bonyan.AspNetCore.MultiTenant;
using Bonyan.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Repositories;
using Microsoft;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bonyan.UserManagement.EntityFrameworkCore;


public class BonyanUserManagementEntityFrameworkModule<TUser> : Modularity.Abstractions.Module where TUser : BonyanUser
{

  public BonyanUserManagementEntityFrameworkModule()
  {
    DependOn([
      typeof(BonyanEntityFrameworkModule),
      typeof(BonyanAspNetCoreMultiTenantModule)
    ]);
  }
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {

  
    
    context.Services.AddTransient<BonyanUserEfRepository<TUser>>();
    context.Services.AddTransient<IBonyanUserRepository<TUser>>(c=>c.GetRequiredService<BonyanUserEfRepository<TUser>>());
    
    context.AddBonyanDbContext<BonUserManagementDbContext<TUser>>(c =>
    {
      c.AddRepository<TUser, BonyanUserEfRepository<TUser>>();
    });
    
    return base.OnConfigureAsync(context);
  }
}
