using Bonyan.EntityFrameworkCore.Abstractions;
using Bonyan.EntityFrameworkCore.Helpers;
using Bonyan.Layer.Domain;
using Bonyan.Layer.Domain.Aggregates;
using Bonyan.Layer.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.EntityFrameworkCore;

public class BonEfCoreRepositoryRegistrar
{
  private readonly IBonDbContextRegistrationOptionBuilder _optionBuilder;

  public BonEfCoreRepositoryRegistrar(IBonDbContextRegistrationOptionBuilder optionBuilder)
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
    var primaryKeyType = BonEntityHelper.FindPrimaryKeyType(entityType);

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
    return typeof(EfCoreBonRepository<,>).MakeGenericType( entityType,dbContextType);
  }

  protected Type GetRepositoryType(Type dbContextType, Type entityType, Type primaryKeyType)
  {
    return typeof(EfCoreBonRepository<,,>).MakeGenericType( entityType, primaryKeyType,dbContextType);
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
        !typeof(IBonAggregateRoot).IsAssignableFrom(entityType))
    {
      return false;
    }

    return true;
  }
}
