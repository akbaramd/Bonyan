using System.Reflection;
using Bonyan.DependencyInjection;
using Castle.DynamicProxy;

namespace Bonyan.Castle.DynamicProxy;

/// <summary>
/// Base adapter that bridges Castle <see cref="IInvocation"/> to Bonyan <see cref="IBonMethodInvocation"/>.
/// Subclasses implement <see cref="ProceedAsync"/> for void/Task vs Task&lt;TResult&gt;.
/// </summary>
public abstract class BonCastleMethodInvocationAdapterBase : IBonMethodInvocation
{
    /// <inheritdoc />
    public object[] Arguments => Invocation.Arguments;

    /// <inheritdoc />
    public IReadOnlyDictionary<string, object> ArgumentsDictionary => _lazyArgumentsDictionary.Value;

    /// <inheritdoc />
    public Type[] GenericArguments => Invocation.GenericArguments;

    /// <inheritdoc />
    public object TargetObject => Invocation.InvocationTarget ?? Invocation.Proxy ?? Invocation.MethodInvocationTarget!;

    /// <inheritdoc />
    public MethodInfo Method => Invocation.MethodInvocationTarget ?? Invocation.Method;

    /// <inheritdoc />
    public object ReturnValue { get; set; } = default!;

    /// <summary>Castle invocation being adapted.</summary>
    protected IInvocation Invocation { get; }

    private readonly Lazy<IReadOnlyDictionary<string, object>> _lazyArgumentsDictionary;

    /// <summary>
    /// Initializes the adapter with the Castle invocation.
    /// </summary>
    protected BonCastleMethodInvocationAdapterBase(IInvocation invocation)
    {
        Invocation = invocation ?? throw new ArgumentNullException(nameof(invocation));
        _lazyArgumentsDictionary = new Lazy<IReadOnlyDictionary<string, object>>(BuildArgumentsDictionary);
    }

    /// <inheritdoc />
    public abstract Task ProceedAsync();

    private IReadOnlyDictionary<string, object> BuildArgumentsDictionary()
    {
        var methodParameters = Method.GetParameters();
        var dict = new Dictionary<string, object>(methodParameters.Length);

        for (var i = 0; i < methodParameters.Length && i < Invocation.Arguments.Length; i++)
        {
            var paramName = methodParameters[i].Name ?? $"arg{i}";
            dict[paramName] = Invocation.Arguments[i]!;
        }

        return dict;
    }
}
