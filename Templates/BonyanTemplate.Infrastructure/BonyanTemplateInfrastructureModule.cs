using Bonyan.DependencyInjection;
using Bonyan.IdentityManagement.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.MultiTenant;
using Bonyan.TenantManagement.EntityFrameworkCore;
using BonyanTemplate.Domain;
using BonyanTemplate.Domain.Authors;
using BonyanTemplate.Domain.Books;
using BonyanTemplate.Infrastructure.Data;
using BonyanTemplate.Infrastructure.Data.Identity;
using BonyanTemplate.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BonyanTemplate.Infrastructure;

/// <summary>
/// Infrastructure module for the Bonyan Template. Registers data access, DbContext, and repositories.
/// </summary>
public class BonyanTemplateInfrastructureModule : BonModule
{
    /// <inheritdoc />
    public BonyanTemplateInfrastructureModule()
    {
        DependOn<BonTenantManagementEntityFrameworkModule>();
        DependOn<BonIdentityManagementEntityFrameworkCoreModule>();
        DependOn<BonyanTemplateDomainModule>();
    }

    /// <inheritdoc />
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        context.ConfigureOptions<BonMultiTenancyOptions>(options => options.IsEnabled = true);
        context.Services.AddTransient<IBooksRepository, EfBookRepository>();
        context.Services.AddTransient<IAuthorsBonRepository, EfAuthorBonRepository>();
        context.AddDbContext<BonyanTemplateDbContext>(options =>
        {
            options.AddDefaultRepositories();
            options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=BonyanTemplate;Trusted_Connection=True;TrustServerCertificate=True;");
        });

        context.Services.AddHostedService<BonyanTemplateIdentityDataSeeder>();

        return base.OnConfigureAsync(context, cancellationToken);
    }
}
