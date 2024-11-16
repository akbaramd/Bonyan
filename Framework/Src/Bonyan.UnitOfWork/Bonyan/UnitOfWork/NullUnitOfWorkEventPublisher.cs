namespace Bonyan.UnitOfWork;

public class NullUnitOfWorkEventPublisher : IUnitOfWorkEventPublisher
{
    public Task PublishLocalEventsAsync(IEnumerable<UnitOfWorkEventRecord> localEvents)
    {
        return Task.CompletedTask;
    }

    public Task PublishDistributedEventsAsync(IEnumerable<UnitOfWorkEventRecord> distributedEvents)
    {
        return Task.CompletedTask;
    }
}
