using Bonyan.AspNetCore.Mvc.Localization;
using Bonyan.IdentityManagement.Application;
using Bonyan.Localization;
using Bonyan.Modularity;
using Bonyan.Ui.BonWeb.Mvc;
using Bonyan.Ui.BonWeb.Mvc.Contracts;
using Bonyan.VirtualFileSystem;
using Microsoft.AspNetCore.Mvc;
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
        context.Services.PreConfigure<BonMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(IdentityManagementResource), typeof(Models.Account.LoginInputModel).Assembly);
        });
        return base.OnPreConfigureAsync(context, cancellationToken);
    }

    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        context.Services.Configure<BonVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<BonIdentityManagementBonWebMvcModule>("Bonyan.Module.IdentityManagement.BonWeb.Mvc");
        });
        context.Services.Configure<BonLocalizationOptions>(options =>
        {
            options.Resources
                .Add<IdentityManagementResource>("en")
                .AddVirtualJson("/Localization/Resources/IdentityManagement");
        });
        context.Services.AddSingleton<IBonWebMenuProvider, IdentityBonWebMenuProvider>();
        return base.OnConfigureAsync(context, cancellationToken);
    }
}
