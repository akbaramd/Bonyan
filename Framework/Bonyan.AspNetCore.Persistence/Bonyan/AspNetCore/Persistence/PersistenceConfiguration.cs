
using Castle.DynamicProxy;

namespace Bonyan.AspNetCore.Persistence;

public class PersistenceConfiguration
{

  public List<Type> Seeders { get; set; }= new List<Type>();
  public PersistenceConfiguration()
  {
  }

  // Add repository dynamically based on whether the entity has a key (IEntity<TKey>) or not
  public PersistenceConfiguration AddSeed<TSeed>() where TSeed : class, ISeeder
  {
    Seeders.Add(typeof(TSeed));
    return this;
  }
  
  
  // public PersistenceConfiguration AddInMemory( Action<InMemoryConfiguration> configure)
  // {
  //   var configuration = new InMemoryConfiguration(Configuration.Context);
  //   configure(configuration);
  //   
  //   return this;
  // }
  //
  // // EnableTenant method: registers ITenantAccessor when called
  // public PersistenceConfiguration EnableTenant()
  // {
  //   Configuration.Context.AddScoped<ITenantAccessor, TenantAccessor>();
  //   
  //   Configuration.Context.AddBeforeInitializer(app =>
  //   {
  //     app.Application.Use(async (context, next) =>
  //     {
  //       // Global tenant header key
  //       const string tenantHeaderKey = "X-Tenant";
  //
  //       // Check if the tenant is in the request headers
  //       if (context.Request.Headers.TryGetValue(tenantHeaderKey, out var tenantValue))
  //       {
  //         // Access the ITenantAccessor service from the request scope
  //         var tenantAccessor = context.RequestServices.GetRequiredService<ITenantAccessor>();
  //
  //         // Set the tenant in the ITenantAccessor (assuming SetTenant is a method on your TenantAccessor)
  //         tenantAccessor.CurrentTenant = tenantValue.ToString().Split(',').ToList();
  //       }
  //
  //       // Call the next middleware in the pipeline
  //       await next.Invoke();
  //     });
  //     
  //   });
  //       
  //   return this;
  // }
 
}

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
