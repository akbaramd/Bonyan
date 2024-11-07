using Bonyan.AspNetCore.MultiTenant;
using Bonyan.DependencyInjection;
using Bonyan.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.UserManagement.EntityFrameworkCore;


public class BonUserManagementEntityFrameworkModule<TUser> : Modularity.Abstractions.BonModule where TUser : BonUser
{

  public BonUserManagementEntityFrameworkModule()
  {
    DependOn([
      typeof(BonEntityFrameworkModule),
      typeof(BonAspNetCoreMultiTenantModule)
    ]);
  }
  public override Task OnConfigureAsync(BonConfigurationContext context)
  {

  
    
    context.Services.AddTransient<BonUserEfRepository<TUser>>();
    context.Services.AddTransient<IBonUserRepository<TUser>>(c=>c.GetRequiredService<BonUserEfRepository<TUser>>());
    context.Services.AddTransient<IBonUserReadOnlyRepository<TUser>>(c=>c.GetRequiredService<BonUserEfReadOnlyRepository<TUser>>());
    
    context.AddBonDbContext<BonUserManagementDbContext<TUser>>(c =>
    {
      c.AddRepository<TUser, BonUserEfRepository<TUser>>();
    });
    
    return base.OnConfigureAsync(context);
  }
}
