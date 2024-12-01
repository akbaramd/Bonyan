namespace Bonyan.Messaging.Saga
{
    public interface IBonStateStore
    {
        Task SaveStateAsync(BonSagaState state);
        Task<BonSagaState?> LoadStateAsync(string correlationId);
        Task RemoveStateAsync(string correlationId);
    }
}