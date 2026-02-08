using Bonyan.DependencyInjection;
using Castle.DynamicProxy;

namespace Bonyan.Castle.DynamicProxy;

/// <summary>
/// Adapter for void or Task-returning methods. Delegates to Castle proceed callback.
/// </summary>
public class BonCastleMethodInvocationAdapter : BonCastleMethodInvocationAdapterBase, IBonMethodInvocation
{
    /// <summary>Castle proceed info for chaining to the next interceptor or target.</summary>
    protected IInvocationProceedInfo ProceedInfo { get; }

    /// <summary>Proceed callback that invokes the next interceptor or target.</summary>
    protected Func<IInvocation, IInvocationProceedInfo, Task> Proceed { get; }

    /// <summary>
    /// Creates an adapter for void/Task methods.
    /// </summary>
    public BonCastleMethodInvocationAdapter(
        IInvocation invocation,
        IInvocationProceedInfo proceedInfo,
        Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        : base(invocation)
    {
        ProceedInfo = proceedInfo ?? throw new ArgumentNullException(nameof(proceedInfo));
        Proceed = proceed ?? throw new ArgumentNullException(nameof(proceed));
    }

    /// <inheritdoc />
    public override async Task ProceedAsync()
    {
        await Proceed(Invocation, ProceedInfo).ConfigureAwait(false);
    }
}
