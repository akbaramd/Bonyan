using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.EntityFrameworkCore.Abstractions;

public interface IDbContextRegistrationOptionBuilder
{
  IServiceCollection Services { get; }
  public Dictionary<Type, Type> CustomRepositories { get; }
  List<Type> SpecifiedDefaultRepositories { get; }
  public bool RegisterDefaultRepositories { get;  }
  public bool IncludeAllEntitiesForDefaultRepositories { get;  }
  Type OriginalDbContextType { get; set; }
  Type DefaultRepositoryDbContextType { get; set; }
  IDbContextRegistrationOptionBuilder AddRepository<TEntity, TRepository>();
  IDbContextRegistrationOptionBuilder AddDefaultRepositories(bool includeAllEntities = false);

}
