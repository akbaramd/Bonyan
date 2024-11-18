using Bonyan.DependencyInjection;
using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.IdentityManagement.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.MultiTenant;
using Bonyan.TenantManagement.EntityFrameworkCore;
using Bonyan.UserManagement.Domain.Users.DomainServices;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Bonyan.UserManagement.EntityFrameworkCore;
using BonyanTemplate.Domain;
using BonyanTemplate.Domain.Authors;
using BonyanTemplate.Domain.Books;
using BonyanTemplate.Domain.Users;
using BonyanTemplate.Infrastructure.Data;
using BonyanTemplate.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BonyanTemplate.Infrastructure;

public class BonaynTempalteInfrastructureModule : BonModule
{
    public BonaynTempalteInfrastructureModule()
    {
        DependOn<BonTenantManagementEntityFrameworkModule>();
        DependOn<BonIdentityManagementEntityFrameworkCoreModule<User>>();
        DependOn<BonyanTemplateDomainModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.ConfigureOptions<BonMultiTenancyOptions>(options => { options.IsEnabled = true; });
        context.Services.AddTransient<IBooksRepository, EfBookRepository>();
        context.Services.AddTransient<IAuthorsBonRepository, EfAuthorBonRepository>();
        context.AddBonDbContext<TemplateBookManagementBonDbContext>(c =>
        {
            c.UseSqlite("Data Source=BonyanTemplate.db");
            c.AddDefaultRepositories(true);
        });


        return base.OnConfigureAsync(context);
    }

    public override async Task OnPostInitializeAsync(BonInitializedContext context)
    {
        var userRepo = context.RequireService<IBonIdentityUserManager<User>>();
        if ((await userRepo.FindByUserNameAsync("admin")).IsSuccess)
        {
            var user = new User(BonUserId.NewId(), "admin");
            await userRepo.CreateAsync(user, "Aa@123456");
        }

        await base.OnPostInitializeAsync(context);
    }
}