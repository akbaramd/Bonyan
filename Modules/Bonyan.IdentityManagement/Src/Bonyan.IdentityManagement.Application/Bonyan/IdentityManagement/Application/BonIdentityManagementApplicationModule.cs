using Bonyan.IdentityManagement.Domain;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UserManagement.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.Application;

public class BonIdentityManagementApplicationModule<TUser> : BonModule where TUser : BonIdentityUser
{
    public BonIdentityManagementApplicationModule()
    {
        DependOn<BonUserManagementApplicationModule<TUser>>();
        DependOn<BonIdentityManagementDomainModule<TUser>>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddTransient<IBonAuthService, BonAuthService<TUser>>();
        return base.OnConfigureAsync(context);
    }
}