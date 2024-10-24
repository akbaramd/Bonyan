namespace Bonyan.AspNetCore.Job;

public interface IOptimumJobsManager
{
  void AddCronJob<TJob>(string cronExpression) where TJob : IJob;
  void AddBackgroundJob<TJob>() where TJob : IJob;
}