using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bonyan.DependencyInjection;

/// <summary>
/// Runs a set of contributors against a context (orchestrator). Order of contributors matters: first registration wins unless ReplaceExisting.
/// </summary>
public static class ServiceRegistrationContributorRunner
{
    /// <summary>
    /// Runs all contributors in order; each contributor's Contribute is called if it is a <see cref="ServiceRegistrationContributorBase"/>.
    /// </summary>
    public static void Run(IServiceRegistrationContext context, IEnumerable<IServiceRegistrationContributor> contributors)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(contributors);

        foreach (var contributor in contributors)
        {
            if (contributor == null)
                continue;

            if (contributor is ServiceRegistrationContributorBase baseContributor)
                baseContributor.Contribute(context);
            else
                RunSingle(context, contributor);
        }
    }

    private static void RunSingle(IServiceRegistrationContext context, IServiceRegistrationContributor contributor)
    {
        foreach (var type in contributor.GetCandidateTypes(context))
        {
            if (type == null || !contributor.CanRegister(type, context))
                continue;

            var descriptor = contributor.GetRegistration(type, context);
            if (descriptor == null)
                continue;

            var implementationType = type;
            var serviceTypes = descriptor.ServiceTypes.Count > 0
                ? descriptor.ServiceTypes
                : new[] { implementationType };

            foreach (var serviceType in serviceTypes)
            {
                if (serviceType == null || !serviceType.IsAssignableFrom(implementationType))
                    continue;

                var sd = new ServiceDescriptor(serviceType, implementationType, descriptor.Lifetime);
                if (descriptor.ReplaceExisting)
                {
                    RemoveExisting(context.Services, serviceType);
                    context.Services.Add(sd);
                }
                else
                {
                    TryAdd(context.Services, sd);
                }
            }
        }
    }

    private static void RemoveExisting(IServiceCollection services, Type serviceType)
    {
        for (var i = services.Count - 1; i >= 0; i--)
        {
            if (services[i].ServiceType == serviceType)
                services.RemoveAt(i);
        }
    }

    private static void TryAdd(IServiceCollection services, Microsoft.Extensions.DependencyInjection.ServiceDescriptor sd)
    {
        services.TryAdd(sd);
    }
}
