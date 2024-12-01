using Bonyan.EntityFrameworkCore;
using Bonyan.Messaging.OutBox;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonMessagingOutboxServiceCollectionExtensions
    {
        public static BonMessagingOutBoxConfiguration UseEntityFrameworkCoreStore<TDbContext>(
            this BonMessagingOutBoxConfiguration configuration) where TDbContext : IBonOutBoxDbContext
        {
            configuration.Configuration.Context.Services.Replace(ServiceDescriptor
                .Singleton<IOutboxStore, EfCoreOutboxStore<TDbContext>>());
            return configuration;
        }
    }
}