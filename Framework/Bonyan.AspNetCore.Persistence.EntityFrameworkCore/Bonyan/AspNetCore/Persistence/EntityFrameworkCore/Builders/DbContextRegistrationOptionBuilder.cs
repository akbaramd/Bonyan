using Bonyan.AspNetCore.Persistence.EntityFrameworkCore.Abstractions;
using Bonyan.DomainDrivenDesign.Domain.Entities;

namespace Bonyan.AspNetCore.Persistence.EntityFrameworkCore.Builders;

public class DbContextRegistrationOptionBuilder : IDbContextRegistrationOptionBuilder
{
  public DbContextRegistrationOptionBuilder(IServiceCollection services, Type originalDbContextType)
  {
    Services = services;
    OriginalDbContextType = originalDbContextType;
    DefaultRepositoryDbContextType = originalDbContextType;
  }

  public IServiceCollection Services { get; }
  public Dictionary<Type, Type> CustomRepositories { get; } = new();
  public List<Type> SpecifiedDefaultRepositories { get; } = new();
  public bool RegisterDefaultRepositories { get; private set; } = true;
  public bool IncludeAllEntitiesForDefaultRepositories { get; private set; } = true;
  public Type OriginalDbContextType { get; set; }
  public Type DefaultRepositoryDbContextType { get; set; }

  public IDbContextRegistrationOptionBuilder AddRepository<TEntity,TRepository>()
  {
    CustomRepositories.Add(typeof(TEntity), typeof(TRepository));
    return this;
  }
  
  
  public IDbContextRegistrationOptionBuilder AddDefaultRepository(Type entityType)
  {
    EntityHelper.CheckEntity(entityType);

    SpecifiedDefaultRepositories.AddIfNotContains(entityType);

    return this;
  }
  
  public IDbContextRegistrationOptionBuilder AddDefaultRepositories(bool includeAllEntities = false)
  {
    RegisterDefaultRepositories = true;
    IncludeAllEntitiesForDefaultRepositories = includeAllEntities;

    return this;
  }
}
