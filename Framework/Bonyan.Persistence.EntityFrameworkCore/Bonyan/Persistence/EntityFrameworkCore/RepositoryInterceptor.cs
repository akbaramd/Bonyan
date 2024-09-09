using Castle.DynamicProxy;

namespace Bonyan.Persistence.EntityFrameworkCore;

public class RepositoryInterceptor : IInterceptor
{
  public void Intercept(IInvocation invocation)
  {
    // Logic before method execution
    Console.WriteLine($"Intercepting call to: {invocation.Method.Name}");

    // Continue with the actual method call
    invocation.Proceed();

    // Logic after method execution
    Console.WriteLine($"Method {invocation.Method.Name} completed.");
  }
}
