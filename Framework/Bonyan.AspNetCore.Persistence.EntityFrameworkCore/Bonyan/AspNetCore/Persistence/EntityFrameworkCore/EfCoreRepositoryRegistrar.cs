using Bonyan.AspNetCore.Persistence.EntityFrameworkCore.Abstractions;
using Bonyan.AspNetCore.Persistence.EntityFrameworkCore.Helpers;
using Bonyan.DomainDrivenDesign.Domain;
using Bonyan.DomainDrivenDesign.Domain.Aggregates;
using Bonyan.DomainDrivenDesign.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Bonyan.AspNetCore.Persistence.EntityFrameworkCore;

public class EfCoreRepositoryRegistrar
{
  private readonly IDbContextRegistrationOptionBuilder _optionBuilder;

  public EfCoreRepositoryRegistrar(IDbContextRegistrationOptionBuilder optionBuilder)
  {
    _optionBuilder = optionBuilder;
  }

 public void ConfigureRepository()
  {
    foreach (var customRepository in _optionBuilder.CustomRepositories)
    {
      _optionBuilder.Services.AddDefaultRepository(customRepository.Key, customRepository.Value, replaceExisting: true);
    }

    foreach (var type in _optionBuilder.SpecifiedDefaultRepositories)
    {
      _optionBuilder.Services.AddDefaultRepository(type, GetDefaultRepositoryImplementationType(type));
    }

    RegisterDefaultRepositories();
  }

  protected virtual void RegisterDefaultRepositories()
  {
    if (!_optionBuilder.RegisterDefaultRepositories)
    {
      return;
    }

    foreach (var entityType in GetEntityTypes(_optionBuilder.OriginalDbContextType))
    {
      if (!ShouldRegisterDefaultRepositoryFor(entityType))
      {
        continue;
      }

      RegisterDefaultRepository(entityType);
    }
  }

  protected virtual void RegisterDefaultRepository(Type entityType)
  {
    _optionBuilder.Services.AddDefaultRepository(
      entityType,
      GetDefaultRepositoryImplementationType(entityType)
    );
  }

  protected virtual Type GetDefaultRepositoryImplementationType(Type entityType)
  {
    var primaryKeyType = EntityHelper.FindPrimaryKeyType(entityType);

    if (primaryKeyType == null)
    {
      return GetRepositoryType(_optionBuilder.DefaultRepositoryDbContextType, entityType);
    }

    return GetRepositoryType(_optionBuilder.DefaultRepositoryDbContextType, entityType, primaryKeyType);
  }


  protected IEnumerable<Type> GetEntityTypes(Type dbContextType)
  {
    return DbContextHelper.GetEntityTypes(dbContextType);
  }

  protected Type GetRepositoryType(Type dbContextType, Type entityType)
  {
    return typeof(EfCoreRepository<,>).MakeGenericType( entityType,dbContextType);
  }

  protected Type GetRepositoryType(Type dbContextType, Type entityType, Type primaryKeyType)
  {
    return typeof(EfCoreRepository<,,>).MakeGenericType( entityType, primaryKeyType,dbContextType);
  }

  protected virtual bool ShouldRegisterDefaultRepositoryFor(Type entityType)
  {
    if (!_optionBuilder.RegisterDefaultRepositories)
    {
      return false;
    }

    if (_optionBuilder.CustomRepositories.ContainsKey(entityType))
    {
      return false;
    }

    if (!_optionBuilder.IncludeAllEntitiesForDefaultRepositories &&
        !typeof(IAggregateRoot).IsAssignableFrom(entityType))
    {
      return false;
    }

    return true;
  }
}
