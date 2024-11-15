using Bonyan.Workers;

[CronJob("* * * * *")] // Run every minute
public class CronWorker : IBonWorker
{
    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("CronWorker is executing at scheduled time.");
        return Task.CompletedTask;
    }
}