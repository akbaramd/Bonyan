namespace Bonyan.MultiTenant;

public interface ITenantResolveContributor
{
    string Name { get; }

    Task ResolveAsync(ITenantResolveContext context);
}
