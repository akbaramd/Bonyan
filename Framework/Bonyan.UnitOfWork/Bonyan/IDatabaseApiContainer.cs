using Bonyan.DependencyInjection;
using Bonyan.UnitOfWork;
using JetBrains.Annotations;

namespace Bonyan;

public interface IDatabaseApiContainer : IServiceProviderAccessor
{
  IDatabaseApi? FindDatabaseApi([NotNull] string key);

  void AddDatabaseApi([NotNull] string key, [NotNull] IDatabaseApi api);

  [NotNull]
  IDatabaseApi GetOrAddDatabaseApi([NotNull] string key, [NotNull] Func<IDatabaseApi> factory);
}
