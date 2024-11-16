using Bonyan.DependencyInjection;
using Bonyan.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users.Entities;
using Bonyan.UserManagement.Domain.Users.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.UserManagement.EntityFrameworkCore;


public class BonUserManagementEntityFrameworkModule<TUser> : Modularity.Abstractions.BonModule where TUser : class, IBonUser
{

  public BonUserManagementEntityFrameworkModule()
  {
    DependOn([
      typeof(BonEntityFrameworkModule),
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
    
    
    
    return base.OnConfigureAsync(context);
  }
}
