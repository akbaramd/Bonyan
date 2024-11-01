namespace Bonyan.DependencyInjection
{
    /// <summary>
    /// Implementation of <see cref="ILayServiceProviderConfigurator"/> for managing and accessing services within a lazy-loaded service provider context.
    /// </summary>
    public class LayServiceProviderConfigurator : ILayServiceProviderConfigurator
    {
        /// <summary>
        /// Gets or sets the lazy service provider used for resolving services.
        /// </summary>
        public IBonyanLazyServiceProvider LazyServiceProvider { get; set; } = default!;


        public object? GetService(Type serviceType)
        {
            return LazyServiceProvider.GetService(serviceType);
        }

        public T GetService<T>(T defaultValue)
        {
            return LazyServiceProvider.GetService(defaultValue);
        }

        public object GetService(Type serviceType, object defaultValue)
        {
            return LazyServiceProvider.GetService(serviceType,defaultValue);
        }

        public T GetService<T>(Func<IServiceProvider, object> factory)
        {
            return LazyServiceProvider.GetService<T>(factory);
        }

        public object GetService(Type serviceType, Func<IServiceProvider, object> factory)
        {
            return LazyServiceProvider.GetService(serviceType,factory);
        }

        public T LazyGetRequiredService<T>()
        {
            return LazyServiceProvider.LazyGetRequiredService<T>();
        }

        public object LazyGetRequiredService(Type serviceType)
        {
            return LazyServiceProvider.LazyGetRequiredService(serviceType);
        }

        public T? LazyGetService<T>()
        {
            return LazyServiceProvider.LazyGetService<T>();
        }

        public object? LazyGetService(Type serviceType)
        {
            return LazyServiceProvider.LazyGetService(serviceType);
        }

        public T LazyGetService<T>(T defaultValue)
        {
            return LazyServiceProvider.LazyGetService<T>(defaultValue);
        }

        public object LazyGetService(Type serviceType, object defaultValue)
        {
            return LazyServiceProvider.LazyGetService(serviceType,defaultValue);
        }

        public object LazyGetService(Type serviceType, Func<IServiceProvider, object> factory)
        {
            return LazyServiceProvider.LazyGetService(serviceType,factory);
        }

        public T LazyGetService<T>(Func<IServiceProvider, object> factory)
        {
            return LazyServiceProvider.LazyGetService<T>(factory);
        }
    }
}
