using System.Collections.Concurrent;

namespace Bonyan.Messaging.Saga;

public class BonInMemoryStateStore: IBonStateStore
{
    private readonly ConcurrentDictionary<string, BonSagaState> _store = new();


    public Task SaveStateAsync(BonSagaState state)
    {
        _store[state.CorrelationId] = state;
        return Task.CompletedTask;
    }

    public Task<BonSagaState?> LoadStateAsync(string correlationI)
    {
        _store.TryGetValue(correlationI, out var state);
        return Task.FromResult(state);
    }

    public Task RemoveStateAsync(string correlationId)
    {
        _store.TryRemove(correlationId, out _);
        return Task.CompletedTask;
    }
}