namespace Bonyan.Workers.Tests.Mock;

public class SimpleWorker : IBonWorker
{
    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("SimpleWorker is executing.");
        return Task.CompletedTask;
    }
}