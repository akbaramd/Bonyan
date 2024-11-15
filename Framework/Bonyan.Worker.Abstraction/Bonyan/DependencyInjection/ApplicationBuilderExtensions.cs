using Bonyan.AspNetCore.Job;
using Bonyan.Core;
using Bonyan.Modularity;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.DependencyInjection;

public static class ApplicationBuilderExtensions
{
    public static Task<IApplicationBuilder> AddBackgroundWorkerAsync<TWorker>([NotNull] this IApplicationBuilder context)
        where TWorker : IBonWorker
    {
        Check.NotNull(context, nameof(context));
         context
            .ApplicationServices.GetRequiredService<IBonWorkerManager>()
            .Enqueue<TWorker>();

        return Task.FromResult(context);
    }
    
    public static Task<IApplicationBuilder> AddCronWorkerAsync<TWorker>([NotNull] this IApplicationBuilder context, string cronExpression)
        where TWorker : IBonWorker
    {
        Check.NotNull(context, nameof(context));
        context
            .ApplicationServices.GetRequiredService<IBonWorkerManager>()
            .ScheduleRecurring<TWorker>(cronExpression);

        return Task.FromResult(context);
    }
   
}