namespace Bonyan.Messaging.Saga
{
    public class BonSagaState
    {
        public string CorrelationId { get; set; } = string.Empty; // Unique ID for the saga instance
        public string? Namespace { get; set; } = string.Empty;    // Namespace of the state machine
        public string Name { get; set; } = string.Empty;         // Name of the state machine
        public string StateData { get; set; } = string.Empty;           // Serialized state data
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow; // Last update timestamp
    }
}