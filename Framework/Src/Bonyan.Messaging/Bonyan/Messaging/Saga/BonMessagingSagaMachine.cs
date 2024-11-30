using Bonyan.Messaging.Abstractions;
using Bonyan.StateMachine;

namespace Bonyan.Messaging.Saga
{
    public abstract class BonMessagingSagaMachine<TStateInstance> : BonStateMachine<TStateInstance>
        where TStateInstance : class, IStateInstance
    {
        /// <summary>
        /// Overrides the base OnEvent method to return a context-aware messaging event handler.
        /// </summary>
        public new BonMessagingEventHandler<TStateInstance, TEvent> OnEvent<TEvent>() where TEvent : class
        {
            // Get the base event handler and wrap it in the messaging handler
            var baseHandler = base.OnEvent<TEvent>();

            return new BonMessagingEventHandler<TStateInstance, TEvent>(this);
        }

        /// <summary>
        /// Processes an event with a context by delegating the logic to the base class.
        /// </summary>
        public async Task ProcessAsync<TEvent, TContext>(TStateInstance instance, TContext context, TEvent eventData)
            where TEvent : class where TContext : BonMessageContext<TEvent>
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (eventData == null) throw new ArgumentNullException(nameof(eventData));

            await base.ProcessAsync(instance, context, eventData);
        }
    }

    public class BonMessagingEventHandler<TInstance, TEvent> : ContextAwareEventHandler<TInstance, TEvent>
        where TInstance : class, IStateInstance where TEvent : class
    {
        /// <summary>
        /// Publishes an event to all subscribers, ensuring the correlation ID is included.
        /// </summary>
        public BonMessagingEventHandler<TInstance, TEvent> Publish<TEventMessage>(
            Func<TInstance, BonMessageContext<TEvent>, TEventMessage> eventFactory)
            where TEventMessage : class
        {
            Then(async (instance, context, eventData) =>
            {
                if (context is not BonMessageContext<TEvent> ctx)
                    throw new InvalidOperationException("Invalid context type.");

                var @event = eventFactory(instance, ctx);
                if (@event == null)
                    throw new InvalidOperationException("Event factory produced a null event.");

                await ctx.PublishAsync(@event);
            });

            return this;
        }

        /// <summary>
        /// Sends a message to a specific service, ensuring the correlation ID is included and handled.
        /// </summary>
        public BonMessagingEventHandler<TInstance, TEvent> Send<TMessage>(
            string serviceName,
            Func<TInstance, BonMessageContext<TEvent>, TMessage> messageFactory)
            where TMessage : class
        {
            Then(async (instance, context, eventData) =>
            {
                if (context is not BonMessageContext<TEvent> ctx)
                    throw new InvalidOperationException("Invalid context type.");

                var message = messageFactory(instance, ctx);
                if (message == null)
                    throw new InvalidOperationException("Message factory produced a null message.");

                await ctx.SendAsync(
                    serviceName,
                    message);
            });

            return this;
        }

        /// <summary>
        /// Sends a reply to a received message, using the correlation ID and reply-to information from the context.
        /// </summary>
        public BonMessagingEventHandler<TInstance, TEvent> Reply<TResponse>(
            Func<TInstance, TEvent, TResponse> responseFactory)
            where TResponse : class
        {
            Then(async (instance, context, eventData) =>
            {
                if (context is not BonMessageContext<TEvent> ctx)
                    throw new InvalidOperationException("Invalid context type.");

                var response = responseFactory(instance,eventData);
                if (response == null)
                    throw new InvalidOperationException("Response factory produced a null response.");

                await ctx.ReplyAsync(response);
            });

            return this;
        }

        public BonMessagingEventHandler(BonMessagingSagaMachine<TInstance> stateMachine) : base(stateMachine)
        {
        }
    }
}