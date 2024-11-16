using Bonyan.Core;
using JetBrains.Annotations;

namespace Bonyan.UnitOfWork;

public static class UnitOfWorkExtensions
{
    public static bool IsReservedFor([NotNull] this IBonUnitOfWork bonUnitOfWork, string reservationName)
    {
        Check.NotNull(bonUnitOfWork, nameof(bonUnitOfWork));

        return bonUnitOfWork.IsReserved && bonUnitOfWork.ReservationName == reservationName;
    }

    public static void AddItem<TValue>([NotNull] this IBonUnitOfWork bonUnitOfWork, string key, TValue value)
        where TValue : class
    {
        Check.NotNull(bonUnitOfWork, nameof(bonUnitOfWork));

        bonUnitOfWork.Items[key] = value;
    }

    public static TValue GetItemOrDefault<TValue>([NotNull] this IBonUnitOfWork bonUnitOfWork, string key)
        where TValue : class
    {
        Check.NotNull(bonUnitOfWork, nameof(bonUnitOfWork));

        return bonUnitOfWork.Items.FirstOrDefault(x => x.Key == key).Value.As<TValue>();
    }

    public static TValue GetOrAddItem<TValue>([NotNull] this IBonUnitOfWork bonUnitOfWork, string key, Func<string, TValue> factory)
        where TValue : class
    {
        Check.NotNull(bonUnitOfWork, nameof(bonUnitOfWork));

        return bonUnitOfWork.Items.GetOrAdd(key, factory).As<TValue>();
    }

    public static void RemoveItem([NotNull] this IBonUnitOfWork bonUnitOfWork, string key)
    {
        Check.NotNull(bonUnitOfWork, nameof(bonUnitOfWork));

        bonUnitOfWork.Items.RemoveAll(x => x.Key == key);
    }
}
