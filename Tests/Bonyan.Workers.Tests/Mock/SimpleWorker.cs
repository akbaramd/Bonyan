using System;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.Workers;

public class SimpleWorker : IBonWorker
{
    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("SimpleWorker is executing.");
        return Task.CompletedTask;
    }
}