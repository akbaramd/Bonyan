using Bonyan.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.UnitOfWork;

public class UnitOfWorkInterceptor : BonyanInterceptor
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public UnitOfWorkInterceptor(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task InterceptAsync(IBonyanMethodInvocation invocation)
    {
        Console.WriteLine("ss1");
        await invocation.ProceedAsync();
        Console.WriteLine("ss2");
        await invocation.ProceedAsync();
        Console.WriteLine("ss3");
        
    }

}
