using Bonyan.DependencyInjection;
using JetBrains.Annotations;

namespace Bonyan.UnitOfWork;

public interface IDatabaseApiContainer : IBonServiceProviderAccessor
{
    IBonDatabaseApi? FindDatabaseApi([NotNull] string key);

    void AddDatabaseApi([NotNull] string key, [NotNull] IBonDatabaseApi api);

    [NotNull]
    IBonDatabaseApi GetOrAddDatabaseApi([NotNull] string key, [NotNull] Func<IBonDatabaseApi> factory);

    /// <summary>
    /// Gets or adds a database API using an async factory. Ensures a single creation per key per UoW (async-safe).
    /// </summary>
    Task<IBonDatabaseApi> GetOrAddDatabaseApiAsync(
        [NotNull] string key,
        [NotNull] Func<CancellationToken, Task<IBonDatabaseApi>> factory,
        CancellationToken cancellationToken = default);
}
