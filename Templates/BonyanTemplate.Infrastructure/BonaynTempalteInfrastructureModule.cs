﻿using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.MultiTenant;
using Bonyan.TenantManagement.EntityFrameworkCore;
using BonyanTemplate.Domain;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BonyanTemplate.Infrastructure;


public class BonaynTempalteInfrastructureModule : Module
{

  public BonaynTempalteInfrastructureModule()
  {
    DependOn<BonyanTenantManagementEntityFrameworkModule>();
    DependOn<BonyanIdentityManagementEntityFrameworkCoreModule<User>>();
    DependOn<BonyanTemplateDomainModule>();
  }

  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    
    context.ConfigureOptions<BonyanMultiTenancyOptions>(options =>
    {
      options.IsEnabled = true;
    });
    
    context.AddBonyanDbContext<BonyanTemplateBookManagementDbContext>(c =>
    {
      c.AddDefaultRepositories(true);
    });

    context.Services.Configure<EntityFrameworkDbContextOptions>(configuration =>
    {
      configuration.UseSqlite("Data Source=BonyanTemplate.db");
    });

    return base.OnConfigureAsync(context);
  }
}

