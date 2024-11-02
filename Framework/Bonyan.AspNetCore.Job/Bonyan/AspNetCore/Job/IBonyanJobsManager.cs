namespace Bonyan.AspNetCore.Job;

public interface IBonyanJobsManager
{
  void AddCronJob<TJob>(TJob job,string cronExpression) where TJob : IJob;
  void AddBackgroundJob<TJob>(TJob job) where TJob : IJob;
}
