using Bonyan.DependencyInjection;
using JetBrains.Annotations;

namespace Bonyan.UnitOfWork;

public interface IDatabaseApiContainer : IServiceProviderAccessor
{
    IDatabaseApi? FindDatabaseApi([NotNull] string key);

    void AddDatabaseApi([NotNull] string key, [NotNull] IDatabaseApi api);

    [NotNull]
    IDatabaseApi GetOrAddDatabaseApi([NotNull] string key, [NotNull] Func<IDatabaseApi> factory);
}
