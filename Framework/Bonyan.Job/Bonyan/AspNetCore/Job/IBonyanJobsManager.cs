namespace Bonyan.AspNetCore.Job;

public interface IBonyanJobsManager
{
  void AddCronJob<TJob>(string cronExpression) where TJob : IJob;
  void AddBackgroundJob<TJob>() where TJob : IJob;
}
