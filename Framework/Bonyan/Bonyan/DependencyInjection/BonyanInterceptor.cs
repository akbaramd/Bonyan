namespace Bonyan.DependencyInjection;

public abstract class BonyanInterceptor : IBonyanInterceptor
{
    public abstract Task InterceptAsync(IBonyanMethodInvocation invocation);
}