namespace Bonyan.DependencyInjection
{
    /// <summary>
    /// Provides methods for accessing and managing services within a service provider.
    /// </summary>
    public interface IBonLayServiceProviderConfigurator :IBonLazyServiceProvider
    {

        public IBonLazyServiceProvider LazyServiceProvider { get; set; }
        
       
    }
}
