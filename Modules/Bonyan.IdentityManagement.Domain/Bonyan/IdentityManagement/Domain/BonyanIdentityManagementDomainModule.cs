using Bonyan.Modularity;
using Bonyan.MultiTenant;
using Bonyan.UserManagement;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Services;
using Microsoft;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bonyan.IdentityManagement.Domain;


public class BonyanIdentityManagementDomainModule<TUser,TRole> : Modularity.Abstractions.Module where TUser : BonyanUser where TRole : BonRole
{
  public BonyanIdentityManagementDomainModule()
  {
    DependOn([
      typeof(BonyanUserManagementDomainModule<TUser>),
    ]);
  }
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    context.Services.AddIdentityCore<TUser>();
    context.Services.TryAddTransient<BonUserManager<TUser>>();
    context.Services.TryAddScoped<UserManager<TUser>>(c=>c.GetRequiredService<BonUserManager<TUser>>());
    context.Services.TryAddScoped<IUserStore<TUser>,BonUserStore<TUser>>();
    context.Services.TryAddScoped<IRoleStore<TRole>,BonRoleStore<TRole>>();
    return base.OnConfigureAsync(context);
  }
}
