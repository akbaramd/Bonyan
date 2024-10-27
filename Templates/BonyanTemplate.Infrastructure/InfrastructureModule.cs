using Bonyan.AspNetCore.Persistence;
using Bonyan.AspNetCore.Persistence.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Modularity.Attributes;
using Bonyan.Persistence.EntityFrameworkCore.Sqlite;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Repositories;
using BonyanTemplate.Infrastructure.Data;
using BonyanTemplate.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BonyanTemplate.Infrastructure;

[DependOn(typeof(BonyanPersistenceEntityFrameworkModule))]
public class InfrastructureModule : Module
{
  public override Task OnPreConfigureAsync(ModularityContext context)
  {
    return base.OnPreConfigureAsync(context);
  }

  public override Task OnConfigureAsync(ModularityContext context)
  {
    context.AddBonyanDbContext<BonyanTemplateBookDbContext>(c =>
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
