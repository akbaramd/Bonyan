using Bonyan.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.MultiTenant;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;


public class BonyanIdentityManagementEntityFrameworkCoreModule<TUser> : Module  where TUser:BonyanUser 
{

  public BonyanIdentityManagementEntityFrameworkCoreModule()
  {
    DependOn([
      typeof(BonyanMultiTenantModule),
      typeof(BonyanEntityFrameworkModule),
    ]);
  }
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    context.AddBonyanDbContext<BonyanIdentityDbContext<TUser>>(c =>
    {
      c.AddRepository<TUser, BonyanUserEfRepository<TUser>>();
    });

    context.Services.AddTransient<BonyanUserEfRepository<TUser>>();
    
    context.Services.AddIdentityCore<TUser>()
      .AddUserManager<UserManager<TUser>>()
      .AddUserStore<BonyanUserEfRepository<TUser>>();
    
    context.Services.AddTransient<UserManager<TUser>>();
    return base.OnConfigureAsync(context);
  }
}
