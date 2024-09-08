using Bonyan.AspNetCore.Jobs;

namespace Bonyan.Demo.Api.Jobs;


public class DemoJobs : IJob
{
  public Task ExecuteAsync()
  {
    Console.WriteLine("Demo Jobs Started");
    return Task.CompletedTask;
  }

  public string CronExpression => "*/1 * * * *";
}
