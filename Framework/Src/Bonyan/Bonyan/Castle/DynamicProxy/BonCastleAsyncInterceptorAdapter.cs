using Bonyan.DependencyInjection;
using Castle.DynamicProxy;

namespace Bonyan.Castle.DynamicProxy;

/// <summary>
/// Adapts Bonyan <see cref="IBonInterceptor"/> to Castle's <see cref="AsyncInterceptorBase"/>.
/// Creates the appropriate <see cref="IBonMethodInvocation"/> adapter for void/Task vs Task&lt;TResult&gt;.
/// </summary>
/// <typeparam name="TInterceptor">The Bonyan interceptor type.</typeparam>
public class BonCastleAsyncInterceptorAdapter<TInterceptor> : AsyncInterceptorBase
    where TInterceptor : IBonInterceptor
{
    private readonly TInterceptor _interceptor;

    /// <summary>
    /// Creates an adapter that forwards Castle invocations to the Bonyan interceptor.
    /// </summary>
    public BonCastleAsyncInterceptorAdapter(TInterceptor interceptor)
    {
        _interceptor = interceptor ?? throw new ArgumentNullException(nameof(interceptor));
    }

    /// <inheritdoc />
    protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
    {
        var adapter = new BonCastleMethodInvocationAdapter(invocation, proceedInfo, proceed);
        await _interceptor.InterceptAsync(adapter).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
    {
        var adapter = new BonCastleMethodInvocationAdapterWithReturnValue<TResult>(invocation, proceedInfo, proceed);
        await _interceptor.InterceptAsync(adapter).ConfigureAwait(false);
        return (TResult)adapter.ReturnValue!;
    }
}
