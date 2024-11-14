using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Bonyan.Messaging.Abstractions;

namespace Bonyan.Messaging
{
    public class BonMessagingOptions
    {
        public List<Assembly> Assemblies { get; } = new List<Assembly>();
        public List<Type> ConsumerTypes { get; } = new List<Type>();

        public BonMessagingOptions RegisterConsumersFromAssembly(Assembly assembly)
        {
            Assemblies.Add(assembly);
            return this;
        }

        public BonMessagingOptions RegisterConsumer<TConsumer>() where TConsumer : class, IBonMessageConsumer
        {
            ConsumerTypes.Add(typeof(TConsumer));
            return this;
        }

        // Register all discovered consumers into the IServiceCollection
        public void RegisterConsumers(IServiceCollection services)
        {
            foreach (var assembly in Assemblies)
            {
                var consumerTypes = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && ImplementsGenericInterface(t, typeof(IBonMessageConsumer<>)));

                foreach (var consumerType in consumerTypes)
                {
                    RegisterConsumerType(services, consumerType);
                }
            }

            foreach (var consumerType in ConsumerTypes)
            {
                RegisterConsumerType(services, consumerType);
            }
        }

        private static bool ImplementsGenericInterface(Type type, Type interfaceType)
        {
            return type.GetInterfaces()
                       .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
        }

        private static void RegisterConsumerType(IServiceCollection services, Type consumerType)
        {
            var messageInterface = consumerType.GetInterfaces()
                                               .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBonMessageConsumer<>));

            if (messageInterface != null)
            {
                services.AddTransient(messageInterface, consumerType);
            }
        }
    }
}
