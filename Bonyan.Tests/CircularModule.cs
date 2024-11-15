using Bonyan.Modularity.Abstractions;

namespace Bonyan.Tests;

public class CircularModule : BonModule
{
    public CircularModule()
    {
        DependOn<CircularModuleB>();
    }
}

public class CircularModuleB : BonModule
{
    public CircularModuleB()
    {
        DependOn<CircularModule>();
    }
}