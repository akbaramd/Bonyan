namespace Bonyan.AspNetCore.Job;

public  interface IJob
{
  Task ExecuteAsync(CancellationToken cancellationToken = default!);
}



