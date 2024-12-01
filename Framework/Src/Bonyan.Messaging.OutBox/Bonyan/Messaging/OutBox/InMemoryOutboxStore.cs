using Bonyan.Layer.Domain;

namespace Bonyan.Messaging.OutBox;

public class InMemoryOutboxStore : IOutboxStore
{
    private readonly List<BonOutboxMessage> _store = new();

    public Task AddAsync(BonOutboxMessage message,CancellationToken cancellationToken = default)
    {
        _store.Add(message);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<BonOutboxMessage>> GetPendingMessagesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<BonOutboxMessage>>(_store.ToList());
    }

    public Task DeleteAsync(Guid id,CancellationToken cancellationToken = default)
    {
        var message = _store.FirstOrDefault(m => m.Id == id);
        if (message != null)
        {
            _store.Remove(message);
        }
        return Task.CompletedTask;
    }
}