namespace Bonyan.Workers.Tests.Mock;

public class ExceptionWorker : IBonWorker
{
    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("ExceptionWorker is about to throw an exception.");
        throw new InvalidOperationException("An error occurred in ExceptionWorker.");
    }
}