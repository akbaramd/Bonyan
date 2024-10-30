using Bonyan.MultiTenant;

namespace Bonyan.AspNetCore.MultiTenant;

public class BonyanAspNetCoreMultiTenancyOptions
{
    /// <summary>
    /// Default: <see cref="TenantResolverConsts.DefaultTenantKey"/>.
    /// </summary>
    public string TenantKey { get; set; }

    /// <summary>
    /// Return true to stop the pipeline, false to continue.
    /// </summary>

    public BonyanAspNetCoreMultiTenancyOptions()
    {
        TenantKey = TenantResolverConsts.DefaultTenantKey;
      
    }
}
