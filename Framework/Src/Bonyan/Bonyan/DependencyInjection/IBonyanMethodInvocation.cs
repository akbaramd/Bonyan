using System.Reflection;

namespace Bonyan.DependencyInjection;

public interface IBonMethodInvocation
{
  object[] Arguments { get; }

  IReadOnlyDictionary<string, object> ArgumentsDictionary { get; }

  Type[] GenericArguments { get; }

  object TargetObject { get; }

  MethodInfo Method { get; }

  object ReturnValue { get; set; }

  Task ProceedAsync();
}
