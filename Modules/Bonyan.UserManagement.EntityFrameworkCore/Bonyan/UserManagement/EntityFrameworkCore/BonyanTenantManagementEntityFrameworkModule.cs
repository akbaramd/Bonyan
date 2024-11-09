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

  
    
    context.Services.AddTransient<BonEfCoreUserRepository<TUser>>();
    context.Services.AddTransient<IBonUserRepository<TUser>,BonEfCoreUserRepository<TUser>>();
    context.Services.AddTransient<IBonUserReadOnlyRepository<TUser>,BonEfCoreUserReadOnlyRepository<TUser>>();
    
    context.Services.AddTransient<BonEfCoreUserRepository>();
    context.Services.AddTransient<IBonUserRepository,BonEfCoreUserRepository>();
    context.Services.AddTransient<IBonUserReadOnlyRepository,BonEfCoreUserReadOnlyRepository>();
    
    context.AddBonDbContext<BonUserManagementDbContext<TUser>>(c =>
    {
      c.AddRepository<TUser, BonEfCoreUserRepository<TUser>>();
    });
    
    context.AddBonDbContext<BonUserManagementDbContext>(c =>
    {
      c.AddRepository<TUser, BonEfCoreUserRepository>();
    });
    
    return base.OnConfigureAsync(context);
  }
}
