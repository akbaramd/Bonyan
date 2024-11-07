using Bonyan.AspNetCore.Job;
using Bonyan.Core;
using Bonyan.Exceptions;
using Bonyan.Modularity;
using JetBrains.Annotations;

namespace Bonyan.DependencyInjection;

public static class ServiceInitializationContextExtensions
{
    public static Task<ServiceInitializationContext> AddBackgroundWorkerAsync<TWorker>([NotNull] this ServiceInitializationContext context)
        where TWorker : IBonWorker
    {
        Check.NotNull(context, nameof(context));
         context
            .RequireService<IBonWorkerManager>()
            .Enqueue<TWorker>();

        return Task.FromResult(context);
    }
    
    public static Task<ServiceInitializationContext> AddCronWorkerAsync<TWorker>([NotNull] this ServiceInitializationContext context, string cronExpression)
        where TWorker : IBonWorker
    {
        Check.NotNull(context, nameof(context));
        context
            .RequireService<IBonWorkerManager>()
            .ScheduleRecurring<TWorker>(cronExpression);

        return Task.FromResult(context);
    }
   
}