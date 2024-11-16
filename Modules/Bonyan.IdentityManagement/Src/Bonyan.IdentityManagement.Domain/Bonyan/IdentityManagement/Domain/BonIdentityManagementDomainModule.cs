using Bonyan.IdentityManagement.Domain.Abstractions.Roles;
using Bonyan.IdentityManagement.Domain.Abstractions.Users;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Modularity;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.Domain;

public class BonIdentityManagementDomainModule<TUser> : Modularity.Abstractions.BonModule where TUser : class, IBonIdentityUser
{
    public BonIdentityManagementDomainModule()
    {
        DependOn([
            typeof(BonUserManagementDomainModule<TUser>),
        ]);
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddTransient<BonIdentityRoleManager>();
        context.Services.AddTransient<IBonIdentityRoleManager,BonIdentityRoleManager>();
        context.Services.AddTransient<BonIdentityUserManager>();
        context.Services.AddTransient<IBonIdentityUserManager,BonIdentityUserManager>();
        context.Services.AddTransient(typeof(IBonIdentityUserManager<TUser>),typeof(BonIdentityUserManager<TUser>));
   
        return base.OnConfigureAsync(context);
    }
}