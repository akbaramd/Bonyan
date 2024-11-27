using Bonyan.Mediators;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Mediator.Tests;

public class TestModule : BonModule
{
    public TestModule()
    {
        DependOn<BonMediatorModule>();
    }
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {

        context.Services.AddTransient(typeof(IBonMediatorBehavior<>), typeof(SampleEventMediatorBehavior<>));
        context.Services.AddTransient(typeof(IBonMediatorBehavior<,>), typeof(SampleCommandMediatorBehavior<,>));
        return Task.CompletedTask;
    }

  
}