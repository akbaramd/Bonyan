using Bonyan.DependencyInjection;
using Castle.DynamicProxy;

namespace Bonyan.Castle.DynamicProxy;

public class CastleBonyanMethodInvocationAdapterWithReturnValue<TResult> : CastleBonyanMethodInvocationAdapterBase, IBonyanMethodInvocation
{
    protected IInvocationProceedInfo ProceedInfo { get; }
    protected Func<IInvocation, IInvocationProceedInfo, Task<TResult>> Proceed { get; }

    public CastleBonyanMethodInvocationAdapterWithReturnValue(IInvocation invocation,
        IInvocationProceedInfo proceedInfo,
        Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        : base(invocation)
    {
        ProceedInfo = proceedInfo;
        Proceed = proceed;
    }

    public override async Task ProceedAsync()
    {
        ReturnValue = (await Proceed(Invocation, ProceedInfo))!;
    }
}
