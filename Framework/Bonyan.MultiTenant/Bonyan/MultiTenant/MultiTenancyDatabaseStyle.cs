namespace Bonyan.MultiTenant;

[Flags]
public enum MultiTenancyDatabaseStyle
{
    Shared = 1,
    PerTenant = 2,
    Hybrid = Shared | PerTenant
}
