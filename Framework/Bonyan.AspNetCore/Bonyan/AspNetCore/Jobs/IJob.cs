namespace Bonyan.AspNetCore.Jobs;

public  interface IJob
{
  Task ExecuteAsync(CancellationToken cancellationToken = default!);
}



