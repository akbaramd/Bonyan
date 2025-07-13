using Bonyan.Layer.Domain;
using Bonyan.Modularity;
using Bonyan.UserManagement.Domain.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.UserManagement.Domain;

public class BonUserManagementDomainModule<TUser> : Modularity.Abstractions.BonModule where TUser : class, IBonUser
{
    public BonUserManagementDomainModule()
    {
        DependOn([
            typeof(BonLayerDomainModule),
        ]);
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {

        return base.OnConfigureAsync(context);
    }
}