using Bonyan.Modularity;
using Bonyan.UserManagement.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.Domain;

public class BonIdentityManagementDomainModule<TUser> : Modularity.Abstractions.BonModule where TUser : BonIdentityUser
{
    public BonIdentityManagementDomainModule()
    {
        DependOn([
            typeof(BonUserManagementDomainModule<TUser>),
        ]);
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddTransient<BonUserManager<TUser>>();
        context.Services.AddTransient<BonRoleManager>();
        return base.OnConfigureAsync(context);
    }
}