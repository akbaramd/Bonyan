using Bonyan.DomainDrivenDesign.Domain.Events;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Module = Bonyan.Modularity.Abstractions.Module;

namespace Bonyan.DomainDrivenDesign.Domain
{
    public class BonyanDomainDrivenDesignDomainModule : Module
    {
        public override Task OnConfigureAsync(ModularityContext context)
        {
            var services = context.Services;
            
            // Register the InMemoryDomainEventDispatcher as a singleton
            services.AddSingleton<IDomainEventDispatcher, InMemoryDomainEventDispatcher>();

            // Find all types that implement IDomainEventHandler<T> from assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            // Get all types that are classes and not abstract, and implement IDomainEventHandler<>
            var handlerTypes = assemblies.SelectMany(x=>x.GetTypes())
              .Where(type => type.IsClass 
                             && !type.IsAbstract
                             && type.GetInterfaces()
                               .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)))
              .ToList();

            // Register each handler as a transient service
            foreach (var handlerType in handlerTypes)
            {
              // Get the interface type that the handler implements (IDomainEventHandler<>)
              var interfaceType = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));
                    
              // Register the handler with the service collection as transient
              services.AddTransient(interfaceType, handlerType);
            }

            return base.OnConfigureAsync(context);
        }
    }
}
