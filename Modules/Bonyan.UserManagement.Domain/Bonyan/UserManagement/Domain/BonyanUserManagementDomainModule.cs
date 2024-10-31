using Bonyan.Layer.Domain;
using Bonyan.Modularity;
using Bonyan.MultiTenant;
using Bonyan.UserManagement.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bonyan.UserManagement.Domain;


public class BonyanUserManagementDomainModule<TUser> : Modularity.Abstractions.Module where TUser : BonyanUser
{

  public BonyanUserManagementDomainModule()
  {
    DependOn([
      typeof(BonyanLayerDomainModule),
    ]);
  }
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    context.Services.AddIdentityCore<TUser>();
    context.Services.TryAddTransient<BonUserManager<TUser>>();
    context.Services.TryAddTransient<UserManager<TUser>>(c=>c.GetRequiredService<BonUserManager<TUser>>());
    return base.OnConfigureAsync(context);
  }
}
