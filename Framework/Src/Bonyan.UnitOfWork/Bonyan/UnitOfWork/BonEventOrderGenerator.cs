namespace Bonyan.UnitOfWork;

public static class BonEventOrderGenerator
{
    private static long _lastOrder;

    public static long GetNext()
    {
        return Interlocked.Increment(ref _lastOrder);
    }
}
