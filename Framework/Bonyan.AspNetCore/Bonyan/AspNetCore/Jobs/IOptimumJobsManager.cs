namespace Bonyan.AspNetCore.Jobs;

public interface IOptimumJobsManager
{
  void AddCronJob<TJob>(string cronExpression, IServiceScope scope) where TJob : IJob;
  void AddBackgroundJob<TJob>(IServiceScope scope) where TJob : IJob;
}