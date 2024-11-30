namespace Bonyan.Messaging.Abstractions
{
    public interface IBonMessageSubscriber
    {
        void Subscribe<TMessage>(
            string queueName,
            Func<BonMessageContext<TMessage>, Task> handler)
            where TMessage : class;
    }
}