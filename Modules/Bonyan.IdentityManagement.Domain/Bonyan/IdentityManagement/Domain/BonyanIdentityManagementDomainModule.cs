using Bonyan.Modularity;
using Bonyan.MultiTenant;
using Bonyan.UserManagement.Domain;
using Microsoft;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.Domain;


public class BonyanIdentityManagementDomainModule<TUser> : Modularity.Abstractions.Module where TUser : BonyanUser
{
  public BonyanIdentityManagementDomainModule()
  {
    DependOn([
      typeof(BonyanUserManagementDomainModule<TUser>),
    ]);
  }
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    context.Services.AddTransient<UserManager<TUser>>();
    return base.OnConfigureAsync(context);
  }
}
