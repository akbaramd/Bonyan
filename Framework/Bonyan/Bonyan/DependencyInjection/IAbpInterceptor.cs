namespace Bonyan.DependencyInjection;

public interface IBonyanInterceptor
{
  Task InterceptAsync(IBonyanMethodInvocation invocation);
}
