namespace Bonyan.UnitOfWork;

public interface IUnitOfWorkEventPublisher
{
    Task PublishLocalEventsAsync(IEnumerable<UnitOfWorkEventRecord> localEvents);

    Task PublishDistributedEventsAsync(IEnumerable<UnitOfWorkEventRecord> distributedEvents);
}
