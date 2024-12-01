using Bonyan.Layer.Domain;

namespace Bonyan.Messaging.OutBox;

public interface IOutboxStore
{
    Task AddAsync(BonOutboxMessage message,CancellationToken cancellationToken = default);
    Task<IEnumerable<BonOutboxMessage>> GetPendingMessagesAsync(CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id,CancellationToken cancellationToken = default);
} 