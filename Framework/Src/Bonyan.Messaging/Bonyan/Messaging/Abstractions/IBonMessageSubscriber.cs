namespace Bonyan.Messaging.Abstractions
{
    public interface IBonMessageSubscriber: IDisposable
    {
        void Subscribe<TMessage>(
            string queueName,
            Func<BonMessageContext<TMessage>, Task> handler,
            bool isTemporary = false)
            where TMessage : class;


    }
}