using System.Reflection;

namespace Bonyan.DependencyInjection;

/// <summary>
/// Encapsulates a method invocation for interception.
/// </summary>
public interface IBonMethodInvocation
{
    /// <summary>Method arguments.</summary>
    object[] Arguments { get; }

    /// <summary>Arguments by parameter name.</summary>
    IReadOnlyDictionary<string, object> ArgumentsDictionary { get; }

    /// <summary>Generic type arguments when the method is generic.</summary>
    Type[] GenericArguments { get; }

    /// <summary>Target instance.</summary>
    object TargetObject { get; }

    /// <summary>Method being invoked.</summary>
    MethodInfo Method { get; }

    /// <summary>Return value (set after invocation).</summary>
    object ReturnValue { get; set; }

    /// <summary>Proceeds to the next interceptor or target.</summary>
    Task ProceedAsync();
}
