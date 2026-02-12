using Bonyan.AutoMapper;
using Bonyan.IdentityManagement.Application.Roles;
using Bonyan.IdentityManagement.Application.UserMeta;
using Bonyan.IdentityManagement.Application.Users;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UserManagement.Application;
using Bonyan.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.Application;

/// <summary>
/// Application module for identity (non-generic). Registers <see cref="IRoleAppService"/>,
/// <see cref="IIdentityUserAppService"/>, <see cref="IUserMetaService"/>, and AutoMapper profiles.
/// </summary>
public class BonIdentityManagementApplicationModule : BonModule
{
    public BonIdentityManagementApplicationModule()
    {
        DependOn<BonUserManagementApplicationModule<Bonyan.IdentityManagement.Domain.Users.BonIdentityUser>>();
        DependOn<BonIdentityManagementModule>();
        DependOn<BonWorkersModule>();
    }

    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        context.Services.Configure<BonAutoMapperOptions>(c =>
            c.AddProfile<RoleAppServiceMappingProfile>());

        context.Services.AddTransient<RoleAppService>();
        context.Services.AddTransient<IRoleAppService, RoleAppService>();
        context.Services.AddTransient<IdentityUserAppService>();
        context.Services.AddTransient<IIdentityUserAppService, IdentityUserAppService>();
        context.Services.AddTransient<UserMetaService>();
        context.Services.AddTransient<IUserMetaService, UserMetaService>();

        return base.OnConfigureAsync(context, cancellationToken);
    }
}
