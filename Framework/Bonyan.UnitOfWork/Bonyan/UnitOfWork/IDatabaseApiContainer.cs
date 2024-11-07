using Bonyan.DependencyInjection;
using JetBrains.Annotations;

namespace Bonyan.UnitOfWork;

public interface IDatabaseApiContainer : IBonServiceProviderAccessor
{
    IBonDatabaseApi? FindDatabaseApi([NotNull] string key);

    void AddDatabaseApi([NotNull] string key, [NotNull] IBonDatabaseApi api);

    [NotNull]
    IBonDatabaseApi GetOrAddDatabaseApi([NotNull] string key, [NotNull] Func<IBonDatabaseApi> factory);
}
