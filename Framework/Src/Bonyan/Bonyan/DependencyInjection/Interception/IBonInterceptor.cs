namespace Bonyan.DependencyInjection;

/// <summary>
/// Interceptor for method invocations (e.g. validation, unit of work).
/// </summary>
public interface IBonInterceptor
{
    /// <summary>Intercepts the invocation (e.g. run before/after or call ProceedAsync).</summary>
    Task InterceptAsync(IBonMethodInvocation invocation);
}
