using Bonyan.IdentityManagement.Application;
using Bonyan.Modularity;
using Bonyan.Ui.BonWeb.Mvc;
using Microsoft.AspNetCore.Mvc;
using Bonyan.Ui.BonWeb.Mvc.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.BonWeb.Mvc;

/// <summary>
/// BonWeb MVC integration for Identity Management - adds Identity pages and menu to the dashboard.
/// </summary>
public class BonIdentityManagementBonWebMvcModule : BonWebModule
{
    public BonIdentityManagementBonWebMvcModule()
    {
        DependOn<BonWebMvcModule>();
        DependOn<BonIdentityManagementApplicationModule>();
    }

    public override ValueTask OnPreConfigureAsync(BonPreConfigurationContext context, CancellationToken cancellationToken = default)
    {
        context.Services.PreConfigure<IMvcBuilder>(builder =>
        {
            builder.AddApplicationPart(typeof(BonIdentityManagementBonWebMvcModule).Assembly);
        });
        return base.OnPreConfigureAsync(context, cancellationToken);
    }

    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        context.Services.AddSingleton<IBonWebMenuProvider, IdentityBonWebMenuProvider>();
        return base.OnConfigureAsync(context, cancellationToken);
    }
}
