namespace Bonyan.Workers.Tests.Mock;

public class TestWorkerA : IBonWorker
{
    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

public class TestWorkerB : IBonWorker
{
    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}