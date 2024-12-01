namespace Bonyan.Messaging.RabbitMQ
{
    /// <summary>
    /// Provides access to the consumer-to-queue mappings.
    /// </summary>
    internal class RabbitMQCosnumerTypeAccessor
    {
        private readonly List<ConsumerQueueMapping> _consumerQueueMappings = new();

        /// <summary>
        /// Adds a consumer-to-queue mapping.
        /// </summary>
        public void AddMapping(Type consumerType, string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name cannot be null or empty.", nameof(queueName));

            _consumerQueueMappings.Add(new ConsumerQueueMapping
            {
                ConsumerType = consumerType,
                QueueName = queueName
            });
        }

        /// <summary>
        /// Gets all registered consumer-to-queue mappings.
        /// </summary>
        public IEnumerable<ConsumerQueueMapping> GetMappings()
        {
            return _consumerQueueMappings;
        }

        /// <summary>
        /// Represents a mapping between a consumer and a queue.
        /// </summary>
        public class ConsumerQueueMapping
        {
            public Type ConsumerType { get; set; }
            public string QueueName { get; set; }
        }
    }
}