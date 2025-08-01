﻿using System.Data;
using Bonyan.DependencyInjection;
using Bonyan.IdentityManagement.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.MultiTenant;
using Bonyan.TenantManagement.EntityFrameworkCore;
using Bonyan.UnitOfWork;
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
        DependOn<BonIdentityManagementEntityFrameworkCoreModule<User,Role>>();
        DependOn<BonyanTemplateDomainModule>();
      
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {


 
        context.ConfigureOptions<BonMultiTenancyOptions>(options => { options.IsEnabled = true; });
        context.Services.AddTransient<IBooksRepository, EfBookRepository>();
        context.Services.AddTransient<IAuthorsBonRepository, EfAuthorBonRepository>();
        context.AddDbContext<BonyanTemplateDbContext>(c =>
        {
            c.AddDefaultRepositories();
            c.UseSqlite("Data Source=BonyanTemplate.db;Mode=ReadWrite;");
        });


        return base.OnConfigureAsync(context);
    }

 
}