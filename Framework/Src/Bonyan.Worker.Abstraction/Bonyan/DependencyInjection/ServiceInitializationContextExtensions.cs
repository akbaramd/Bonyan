using Bonyan.Core;
using Bonyan.Modularity;
using Bonyan.Workers;
using JetBrains.Annotations;

namespace Bonyan.DependencyInjection;

public static class ServiceInitializationContextExtensions
{
    public static Task<BonInitializedContext> AddBackgroundWorkerAsync<TWorker>([NotNull] this BonInitializedContext context)
        where TWorker : IBonWorker
    {
        Check.NotNull(context, nameof(context));
         context
            .GetRequireService<IBonWorkerManager>()
            .Enqueue<TWorker>();

        return Task.FromResult(context);
    }
    
    public static Task<BonInitializedContext> AddCronWorkerAsync<TWorker>([NotNull] this BonInitializedContext context, string cronExpression)
        where TWorker : IBonWorker
    {
        Check.NotNull(context, nameof(context));
        context
            .GetRequireService<IBonWorkerManager>()
            .ScheduleRecurring<TWorker>(cronExpression);

        return Task.FromResult(context);
    }
   
}