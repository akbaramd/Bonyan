using Bonyan.AspNetCore.Job;

namespace Bonyan.Modularity;

public static class JobModularityContextExtensions
{
  public static void AddJob<TJob>(this ServiceConfigurationContext context)where TJob : class, IJob
  {
    context.Services.AddTransient<TJob>();
    context.Services.AddTransient<IJob,TJob>();
  }
  
}
