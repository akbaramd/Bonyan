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
        
        
        public BonMessagingEventHandler<TInstance, TEvent> Then(Action<TInstance, BonMessageContext<TEvent>, TEvent> action)
        {
            base.Then((c, context, b) =>
            {
                if (context is not BonMessageContext<TEvent> ctx)
                    throw new InvalidOperationException("Invalid context type.");
                
                action.Invoke(c,ctx,b);
            });
            return this;
        }

    
        public ContextAwareEventHandler<TInstance, TEvent> ThenAsync(Func<TInstance,  BonMessageContext<TEvent>, TEvent, Task> asyncAction)
        {
            base.ThenAsync((c, context, b) =>
            {
                if (context is not BonMessageContext<TEvent> ctx)
                    throw new InvalidOperationException("Invalid context type.");
                
                return asyncAction.Invoke(c,ctx,b);
            });
            return this;
        }

      
        
        /// <summary>
        /// Publishes an event to all subscribers, ensuring the correlation ID is included.
        /// </summary>
        public BonMessagingEventHandler<TInstance, TEvent> Publish<TEventMessage>(
            Func<TInstance, BonMessageContext<TEvent>, TEventMessage> eventFactory)
            where TEventMessage : class, IMessageEvent
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
        /// Sends a message to a specific service without expecting a response, ensuring the correlation ID is included and handled.
        /// </summary>
        public BonMessagingEventHandler<TInstance, TEvent> Send<TMessage>(
            string serviceName,
            Func<TInstance, BonMessageContext<TEvent>, TMessage> messageFactory)
            where TMessage : class, IMessageRequest
        {
            Then(async (instance, context, eventData) =>
            {
                if (context is not BonMessageContext<TEvent> ctx)
                    throw new InvalidOperationException("Invalid context type.");

                var message = messageFactory(instance, ctx);
                if (message == null)
                    throw new InvalidOperationException("Message factory produced a null message.");

                await ctx.SendAsync(serviceName, message);
            });

            return this;
        }

        /// <summary>
        /// Sends a message to a specific service and waits for a response, ensuring the correlation ID is included and handled.
        /// </summary>
        public BonMessagingEventHandler<TInstance, TEvent> Send<TMessage, TResponse>(
            string serviceName,
            Func<TInstance, BonMessageContext<TEvent>, TMessage> messageFactory,
            Action<TInstance, BonMessageContext<TEvent>, TResponse> responseHandler)
            where TMessage : class, IMessageRequest<TResponse>
            where TResponse : class
        {
            Then(async (instance, context, eventData) =>
            {
                if (context is not BonMessageContext<TEvent> ctx)
                    throw new InvalidOperationException("Invalid context type.");

                var message = messageFactory(instance, ctx);
                if (message == null)
                    throw new InvalidOperationException("Message factory produced a null message.");

                var response = await ctx.SendAsync<TMessage, TResponse>(serviceName, message);
                responseHandler(instance, ctx, response);
            });

            return this;
        }

     
        public BonMessagingEventHandler(BonMessagingSagaMachine<TInstance> stateMachine) : base(stateMachine)
        {
        }
    }
}