using JetBrains.Annotations;

namespace Bonyan.UnitOfWork;

public interface ITransactionApiContainer
{
    IBonTransactionApi? FindTransactionApi([NotNull] string key);

    void AddTransactionApi([NotNull] string key, [NotNull] IBonTransactionApi api);

    [NotNull]
    IBonTransactionApi GetOrAddTransactionApi([NotNull] string key, [NotNull] Func<IBonTransactionApi> factory);
}
