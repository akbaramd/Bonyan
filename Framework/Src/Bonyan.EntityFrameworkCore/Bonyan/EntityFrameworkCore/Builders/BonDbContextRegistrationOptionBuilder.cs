using Bonyan.EntityFrameworkCore.Abstractions;
using Bonyan.Layer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.EntityFrameworkCore.Builders;

public class BonDbContextRegistrationOptionBuilder : IBonDbContextRegistrationOptionBuilder
{
  public BonDbContextRegistrationOptionBuilder(IServiceCollection services, Type originalDbContextType)
  {
    Services = services;
    OriginalDbContextType = originalDbContextType;
    DefaultRepositoryDbContextType = originalDbContextType;
  }

  
  public Action<DbContextOptionsBuilder> DbContextOptionsAction { get; set; } = _ => { };


  public IBonDbContextRegistrationOptionBuilder Configure(Action<DbContextOptionsBuilder> action) 
  {
    DbContextOptionsAction = action;
    return this;
  }
  public List<Type> AdditionalDbContexts { get; } = new();
  public IServiceCollection Services { get; }
  public Dictionary<Type, Type> CustomRepositories { get; } = new();
  public List<Type> SpecifiedDefaultRepositories { get; } = new();
  public bool RegisterDefaultRepositories { get; private set; } = true;
  public bool IncludeAllEntitiesForDefaultRepositories { get; private set; } = true;
  public Type OriginalDbContextType { get; set; }
  public Type DefaultRepositoryDbContextType { get; set; }

  public IBonDbContextRegistrationOptionBuilder AddRepository<TEntity,TRepository>()
  {
    CustomRepositories.Add(typeof(TEntity), typeof(TRepository));
    return this;
  }
  
  
  /// <summary>
  /// Adds another DbContext type that will use the same configuration as the primary DbContext.
  /// </summary>
  public IBonDbContextRegistrationOptionBuilder AsDbContext<TAnotherDbContext>()
    where TAnotherDbContext : DbContext
  {
    AdditionalDbContexts.Add(typeof(TAnotherDbContext));
    return this;
  }
  public IBonDbContextRegistrationOptionBuilder AddDefaultRepository(Type entityType)
  {
    BonEntityHelper.CheckEntity(entityType);

    SpecifiedDefaultRepositories.AddIfNotContains(entityType);

    return this;
  }
  
  public IBonDbContextRegistrationOptionBuilder AddDefaultRepositories(bool includeAllEntities = false)
  {
    RegisterDefaultRepositories = true;
    IncludeAllEntitiesForDefaultRepositories = includeAllEntities;

    return this;
  }
}
