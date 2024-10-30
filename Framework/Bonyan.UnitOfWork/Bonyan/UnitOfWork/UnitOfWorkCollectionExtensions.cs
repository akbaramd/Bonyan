using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bonyan.UnitOfWork;

public static class UnitOfWorkCollectionExtensions
{
    public static IServiceCollection AddAlwaysDisableUnitOfWorkTransaction(this IServiceCollection services)
    {
        return services.Replace(ServiceDescriptor.Singleton<IUnitOfWorkManager, AlwaysDisableTransactionsUnitOfWorkManager>());
    }
}
