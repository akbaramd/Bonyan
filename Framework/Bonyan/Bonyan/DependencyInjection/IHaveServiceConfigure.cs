namespace Bonyan.DependencyInjection
{
    /// <summary>
    /// Provides methods for accessing and managing services within a service provider.
    /// </summary>
    public interface ILayServiceProviderConfigurator :IBonyanLazyServiceProvider
    {

        public IBonyanLazyServiceProvider LazyServiceProvider { get; set; }
        
       
    }
}
