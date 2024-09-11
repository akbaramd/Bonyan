using Castle.DynamicProxy;

namespace Bonyan.AspNetCore.Persistence;

public class PersistenceConfiguration
{
  private readonly ProxyGenerator _proxyGenerator;

  public PersistenceConfiguration(IBonyanApplicationBuilder builder)
  {
    Builder = builder;
    _proxyGenerator = new ProxyGenerator();
  }

  public IBonyanApplicationBuilder Builder { get; }

  // Add repository dynamically based on whether the entity has a key (IEntity<TKey>) or not
  
  public PersistenceConfiguration AddSeed<TSeed>() where TSeed : class, ISeeder
  {
    Builder.GetServicesCollection().AddScoped<ISeeder, TSeed>();
    
    return this;
  }
  
  public PersistenceConfiguration AddInMemory( Action<InMemoryConfiguration> configure)
  {
    var configuration = new InMemoryConfiguration(Builder);
    configure(configuration);
    
    return this;
  }
 
}

// Example of a simple Castle DynamicProxy interceptor (this can be customized)
public class RepositoryInterceptor : IInterceptor
{
  public void Intercept(IInvocation invocation)
  {
    // Pre-invocation logic (e.g., logging)
    Console.WriteLine($"Calling method {invocation.Method.Name}");

    // Proceed with the method call
    invocation.Proceed();

    // Post-invocation logic (e.g., logging)
    Console.WriteLine($"Finished method {invocation.Method.Name}");
  }
}
