using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bonyan.DependencyInjection;

/// <summary>
/// Template Method base for contributors: orchestrates GetCandidateTypes, CanRegister, GetRegistration, then registers (Template Method pattern).
/// </summary>
public abstract class ServiceRegistrationContributorBase : IServiceRegistrationContributor
{
    public abstract IEnumerable<Type> GetCandidateTypes(IServiceRegistrationContext context);
    public abstract bool CanRegister(Type type, IServiceRegistrationContext context);
    public abstract ServiceRegistrationDescriptor? GetRegistration(Type type, IServiceRegistrationContext context);

    /// <summary>
    /// Registers all types that pass CanRegister and have a non-null GetRegistration into the context's Services.
    /// </summary>
    public virtual void Contribute(IServiceRegistrationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        foreach (var type in GetCandidateTypes(context))
        {
            if (type == null)
                continue;
            if (!CanRegister(type, context))
                continue;

            var descriptor = GetRegistration(type, context);
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

    private static void TryAdd(IServiceCollection services, ServiceDescriptor sd)
    {
        services.TryAdd(sd);
    }
}
