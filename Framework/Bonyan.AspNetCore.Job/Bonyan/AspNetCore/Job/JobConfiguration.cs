namespace Bonyan.AspNetCore.Job;

public class JobConfiguration
{
  public  List<(Type, string)> CronJobTypes = new(); // Store cron jobs with their expressions
  public  List<Type> BackgroundJobTypes = new(); // Store background jobs
  
  public void AddCronJob<TJob>(string cronExpression) where TJob : class, IJob
  {
    CronJobTypes.Add((typeof(TJob), cronExpression));
  }

  // Method for adding background jobs
  public void AddBackgroundJob<TJob>() where TJob : class, IJob
  {
    BackgroundJobTypes.Add(typeof(TJob));
  
  }
}
