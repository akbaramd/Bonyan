using Bonyan.Layer.Domain.Aggregate;

namespace Bonyan.Layer.Domain
{
    public class BonOutboxMessage : BonFullAggregateRoot<Guid>
    {
        // Private fields
        private string _destination;
        private string _payload;
        private string _messageType;
        private string _correlationId;
        private string _headers;
        private string _replyQueueName;

        // Public properties with getters and setters
        public string Destination
        {
            get => _destination;
            private set => _destination = value ?? throw new ArgumentNullException(nameof(Destination), "Destination cannot be null.");
        }

        public string Payload
        {
            get => _payload;
            private set => _payload = value ?? throw new ArgumentNullException(nameof(Payload), "Payload cannot be null.");
        }

        public string MessageType
        {
            get => _messageType;
            private set => _messageType = value ?? throw new ArgumentNullException(nameof(MessageType), "MessageType cannot be null.");
        }

        public string CorrelationId
        {
            get => _correlationId;
            private set => _correlationId = value ?? throw new ArgumentNullException(nameof(CorrelationId), "CorrelationId cannot be null.");
        }

        public string Headers
        {
            get => _headers;
            private set => _headers = value ?? throw new ArgumentNullException(nameof(Headers), "Headers cannot be null.");
        }

        public string ReplyQueueName
        {
            get => _replyQueueName;
            private set => _replyQueueName = value ?? string.Empty;
        }

        // Private constructor for EF Core
        private BonOutboxMessage() { }

        // Constructor for creating a new instance
        private BonOutboxMessage(string destination, string payload, string messageType, string headers, string? replyQueueName, string? correlationId = null)
        {
            Id = Guid.NewGuid(); // Generate a new ID
            Destination = destination;
            Payload = payload;
            MessageType = messageType;
            Headers = headers;
            ReplyQueueName = replyQueueName ?? string.Empty;
            CorrelationId = correlationId ?? Guid.NewGuid().ToString();
        }

        // Static factory method for creating a new instance
        public static BonOutboxMessage Create(string destination, string payload, string messageType, string headers, string? replyQueueName, string? correlationId = null)
        {
            return new BonOutboxMessage(destination, payload, messageType, headers, replyQueueName, correlationId);
        }

        // Example behavior: A method to update the payload
        public void UpdatePayload(string newPayload)
        {
            if (string.IsNullOrEmpty(newPayload))
                throw new ArgumentException("New payload cannot be null or empty.", nameof(newPayload));

            _payload = newPayload;
        }

        // Example behavior: A method to update the correlation ID
        public void UpdateCorrelationId(string? newCorrelationId)
        {
            _correlationId = newCorrelationId;
        }

        
    }
}
