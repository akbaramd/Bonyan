using Bonyan.AspNetCore.Job;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Modularity;

public static class JobModularityContextExtensions
{
  public static void AddJob<TJob>(this ModularityContext context)where TJob : class, IJob
  {
    context.Services.AddTransient<TJob>();
    context.Services.AddTransient<IJob,TJob>();
  }
  
}
