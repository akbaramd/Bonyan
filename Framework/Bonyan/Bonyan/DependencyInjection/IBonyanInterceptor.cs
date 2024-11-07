namespace Bonyan.DependencyInjection;

public interface IBonInterceptor
{
  Task InterceptAsync(IBonMethodInvocation invocation);
}
