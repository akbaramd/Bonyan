namespace Bonyan.Workers;

public interface IBonWorker 
{
    Task ExecuteAsync(CancellationToken cancellationToken = default!);
}