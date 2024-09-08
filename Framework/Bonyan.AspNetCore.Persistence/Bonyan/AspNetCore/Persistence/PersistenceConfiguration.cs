using Bonyan.DDD.Domain.Abstractions;
using Bonyan.DDD.Domain.Entities;
using Bonyan.EntityFrameworkCore;

namespace Bonyan.AspNetCore.Persistence;

public class PersistenceConfiguration(IBonyanApplicationBuilder builder)
{
  public IBonyanApplicationBuilder Builder { get; } = builder;
  
  
  public PersistenceConfiguration AddSeed<TSeed>() where TSeed : SeedBase
  {
    Builder.Services.AddTransient<SeedBase, TSeed>();
    return this;
  }

}
