using Bonyan.DependencyInjection;
using Castle.DynamicProxy;

namespace Bonyan.Castle.DynamicProxy;

public class BonCastleAsyncInterceptorAdapter<TInterceptor> : AsyncInterceptorBase
    where TInterceptor : IBonInterceptor
{
    private readonly TInterceptor _bonyanInterceptor;

    public BonCastleAsyncInterceptorAdapter(TInterceptor bonyanInterceptor)
    {
        _bonyanInterceptor = bonyanInterceptor;
    }

    protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
    {
        await _bonyanInterceptor.InterceptAsync(
            new BonCastleMethodInvocationAdapter(invocation, proceedInfo, proceed)
        );
    }

    protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
    {
        var adapter = new BonCastleMethodInvocationAdapterWithReturnValue<TResult>(invocation, proceedInfo, proceed);

        await _bonyanInterceptor.InterceptAsync(
            adapter
        );

        return (TResult)adapter.ReturnValue;
    }
}
