using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Layer.Domain
{
    /// <summary>
    /// Domain Layer Module - represents the core business domain.
    /// Domain Layer should be independent and not depend on infrastructure concerns.
    /// </summary>
    public class BonLayerDomainModule : BonModule
    {
        // Domain Layer should NOT depend on infrastructure modules like Mediator
        // Domain Events are dispatched via IBonDomainEventDispatcher abstraction
        // The concrete implementation (BonDomainEventDispatcher) can use Mediator,
        // but Domain Layer itself remains independent
        public BonLayerDomainModule()
        {
            // No dependencies - Domain Layer is independent
        }


        public override ValueTask OnPostConfigureAsync(BonPostConfigurationContext context, CancellationToken cancellationToken = default)
        {
            context.AddDomain();
            return base.OnPostConfigureAsync(context, cancellationToken);
        }
    }
}