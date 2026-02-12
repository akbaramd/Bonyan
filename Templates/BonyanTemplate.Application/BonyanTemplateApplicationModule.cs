using Bonyan.AutoMapper;
using Bonyan.IdentityManagement;
using Bonyan.IdentityManagement.Application;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.TenantManagement.Application;
using Bonyan.Workers;
using Bonyan.Workers.Hangfire;
using BonyanTemplate.Application.Authors;
using BonyanTemplate.Application.Books;
using BonyanTemplate.Application.Books.Jobs;
using BonyanTemplate.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace BonyanTemplate.Application;

/// <summary>
/// Application layer module for the Bonyan Template. Registers application services, AutoMapper, and workers.
/// </summary>
public class BonyanTemplateApplicationModule : BonModule
{
    /// <inheritdoc />
    public BonyanTemplateApplicationModule()
    {
        DependOn<BonTenantManagementApplicationModule>();
        DependOn<BonIdentityManagementApplicationModule>();
        DependOn<BonyanTemplateDomainModule>();
        DependOn<BonWorkersHangfireModule>();
    }

    /// <inheritdoc />
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        context.Services.AddTransient<IBookAppService, BookAppService>();
        context.Services.AddTransient<IAuthorAppService, AuthorAppService>();
        context.ConfigureOptions<BonAutoMapperOptions>(options =>
        {
            options.AddProfile<BookMapper>();
            options.AddProfile<AuthorMapper>();
        });
        context.Services.PreConfigure<BonWorkerConfiguration>(c => c.RegisterWorker<BookOutOfStockNotifierWorker>());

        return base.OnConfigureAsync(context, cancellationToken);
    }
}
