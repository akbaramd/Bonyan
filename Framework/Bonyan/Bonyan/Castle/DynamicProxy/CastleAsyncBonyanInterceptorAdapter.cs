using Bonyan.DependencyInjection;
using Castle.DynamicProxy;

namespace Bonyan.Castle.DynamicProxy;

public class CastleAsyncBonyanInterceptorAdapter<TInterceptor> : AsyncInterceptorBase
    where TInterceptor : IBonyanInterceptor
{
    private readonly TInterceptor _bonyanInterceptor;

    public CastleAsyncBonyanInterceptorAdapter(TInterceptor bonyanInterceptor)
    {
        _bonyanInterceptor = bonyanInterceptor;
    }

    protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
    {
        await _bonyanInterceptor.InterceptAsync(
            new CastleBonyanMethodInvocationAdapter(invocation, proceedInfo, proceed)
        );
    }

    protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
    {
        var adapter = new CastleBonyanMethodInvocationAdapterWithReturnValue<TResult>(invocation, proceedInfo, proceed);

        await _bonyanInterceptor.InterceptAsync(
            adapter
        );

        return (TResult)adapter.ReturnValue;
    }
}
