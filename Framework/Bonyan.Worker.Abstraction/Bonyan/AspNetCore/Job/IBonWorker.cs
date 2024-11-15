namespace Bonyan.AspNetCore.Job;

public interface IBonWorker 
{
    Task ExecuteAsync(CancellationToken cancellationToken = default!);
}