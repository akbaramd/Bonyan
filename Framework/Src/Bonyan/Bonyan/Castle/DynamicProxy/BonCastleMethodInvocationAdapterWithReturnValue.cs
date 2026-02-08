using Bonyan.DependencyInjection;
using Castle.DynamicProxy;

namespace Bonyan.Castle.DynamicProxy;

/// <summary>
/// Adapter for Task&lt;TResult&gt;-returning methods. Captures return value into <see cref="IBonMethodInvocation.ReturnValue"/>.
/// </summary>
/// <typeparam name="TResult">The method return type.</typeparam>
public class BonCastleMethodInvocationAdapterWithReturnValue<TResult> : BonCastleMethodInvocationAdapterBase, IBonMethodInvocation
{
    /// <summary>Castle proceed info for chaining to the next interceptor or target.</summary>
    protected IInvocationProceedInfo ProceedInfo { get; }

    /// <summary>Proceed callback that invokes the next interceptor or target and returns the result.</summary>
    protected Func<IInvocation, IInvocationProceedInfo, Task<TResult>> Proceed { get; }

    /// <summary>
    /// Creates an adapter for Task&lt;TResult&gt;-returning methods.
    /// </summary>
    public BonCastleMethodInvocationAdapterWithReturnValue(
        IInvocation invocation,
        IInvocationProceedInfo proceedInfo,
        Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        : base(invocation)
    {
        ProceedInfo = proceedInfo ?? throw new ArgumentNullException(nameof(proceedInfo));
        Proceed = proceed ?? throw new ArgumentNullException(nameof(proceed));
    }

    /// <inheritdoc />
    public override async Task ProceedAsync()
    {
        var result = await Proceed(Invocation, ProceedInfo).ConfigureAwait(false);
        ReturnValue = result!;
    }
}
