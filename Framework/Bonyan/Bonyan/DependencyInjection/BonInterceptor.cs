namespace Bonyan.DependencyInjection;

public abstract class BonInterceptor : IBonInterceptor
{
    public abstract Task InterceptAsync(IBonMethodInvocation invocation);
}