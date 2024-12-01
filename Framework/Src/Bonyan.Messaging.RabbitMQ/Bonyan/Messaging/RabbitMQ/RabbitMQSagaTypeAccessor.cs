namespace Bonyan.Messaging.RabbitMQ
{
    public class RabbitMQSagaTypeAccessor
    {
        private readonly List<SagaRegistration> _sagaRegistrations = new();

        /// <summary>
        /// Registers a saga type and its associated queue name.
        /// </summary>
        public void AddSaga(Type sagaType, Type instanceType, string queueName)
        {
            if (_sagaRegistrations.Exists(x => x.SagaType == sagaType))
                throw new InvalidOperationException($"Saga {sagaType.Name} is already registered.");

            _sagaRegistrations.Add(new SagaRegistration
            {
                SagaType = sagaType,
                InstanceType = instanceType,
                QueueName = queueName
            });
        }

        /// <summary>
        /// Retrieves all saga registrations.
        /// </summary>
        public IEnumerable<SagaRegistration> GetAllSagas() => _sagaRegistrations;

        public class SagaRegistration
        {
            public Type SagaType { get; set; }
            public Type InstanceType { get; set; }
            public string QueueName { get; set; }
        }
    }
}