using Bonyan.DependencyInjection;
using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.MultiTenant;
using Bonyan.TenantManagement.EntityFrameworkCore;
using Bonyan.UserManagement.Domain.Users.DomainServices;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Bonyan.UserManagement.EntityFrameworkCore;
using BonyanTemplate.Domain;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Repositories;
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
        context.Services.AddTransient<IBooksBonRepository, EfBookBonRepository>();
        context.Services.AddTransient<IAuthorsBonRepository, EfAuthorBonRepository>();
        context.AddBonDbContext<BonTemplateBookManagementDbContext>(c =>
        {
            c.UseSqlite("Data Source=BonyanTemplate.db");
            c.AddDefaultRepositories(true);

            c.AsDbContext<BonTenantDbContext>();
            c.AsDbContext<BonUserManagementDbContext<User>>();
            c.AsDbContext<BonUserManagementDbContext>();
            c.AsDbContext<BonIdentityManagementDbContext<User>>();
            c.AsDbContext<BonIdentityManagementDbContext>();
        });


        return base.OnConfigureAsync(context);
    }

    public override async Task OnPostInitializeAsync(BonInitializedContext context)
    {
        var userRepo = context.RequireService<BonUserManager<User>>();
        if (await userRepo.FindByUserNameAsync("admin") == null)
        {
            var user = new User(BonUserId.CreateNew(), "admin");
            await userRepo.CreateAsync(user, "Aa@123456");
        }

        await base.OnPostInitializeAsync(context);
    }
}