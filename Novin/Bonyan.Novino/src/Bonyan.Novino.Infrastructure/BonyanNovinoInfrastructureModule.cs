using Bonyan.DependencyInjection;
using Bonyan.IdentityManagement.EntityFrameworkCore;
using Bonyan.Localization;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Novino.Application;
using Bonyan.Novino.Domain;
using Bonyan.Novino.Domain.Entities;
using Bonyan.Novino.Infrastructure.Data;
using Bonyan.TenantManagement.EntityFrameworkCore;
using Bonyan.VirtualFileSystem;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Bonyan.Novino.Infrastructure;

public class BonyanNovinoInfrastructureModule : BonModule
{
    public BonyanNovinoInfrastructureModule()
    {
        DependOn<BonyanNovinoDomainModule>();
        DependOn<BonIdentityManagementEntityFrameworkCoreModule<Domain.Entities.User,Role>>();
        DependOn<BonTenantManagementEntityFrameworkModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
       
        
        // Configure database context
        context.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = "Server=localhost\\AHMADI,1433;Database=BonyanNovino;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
            options.UseSqlServer(connectionString);
        });

    
        return base.OnConfigureAsync(context);
    }

    public override Task OnPostConfigureAsync(BonConfigurationContext context)
    {
        Configure<BonLocalizationOptions>(c =>
        {
            c.Languages.Clear();
            c.Languages.Add(new LanguageInfo("fa", "فارسی"));
        });
        return base.OnPostConfigureAsync(context);
    }
} 