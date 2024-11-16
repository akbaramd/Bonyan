using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.EntityFrameworkCore.Abstractions;

public interface IBonDbContextRegistrationOptionBuilder
{
  IServiceCollection Services { get; }
  public Dictionary<Type, Type> CustomRepositories { get; }
  List<Type> SpecifiedDefaultRepositories { get; }
  public bool RegisterDefaultRepositories { get;  }
  public bool IncludeAllEntitiesForDefaultRepositories { get;  }
  Type OriginalDbContextType { get; set; }
  Type DefaultRepositoryDbContextType { get; set; }
  IBonDbContextRegistrationOptionBuilder Configure(Action<DbContextOptionsBuilder> action);
  IBonDbContextRegistrationOptionBuilder AddRepository<TEntity, TRepository>();
  IBonDbContextRegistrationOptionBuilder AsDbContext<TDbContext>() where TDbContext : DbContext;
  IBonDbContextRegistrationOptionBuilder AddDefaultRepositories(bool includeAllEntities = false);

}
