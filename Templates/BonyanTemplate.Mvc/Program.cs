using System.Diagnostics;
using Bonyan.Plugins;
using BonyanTemplate.Mvc;
using Microsoft.Extensions.DependencyInjection;

var builder = BonyanApplication.CreateModularBuilder<BonyanTemplateMvcModule>(
    "BonyanTemplate.Mvc",
    "BonyanTemplate MVC",
    _ => { });

// Optional: dump suspicious descriptors (DiagnosticListener/DiagnosticSource or ImplementationInstance is Type) before Build
DumpSuspicious(builder.Services);

var app = await builder.BuildAsync();

static void DumpSuspicious(IServiceCollection services)
{
    foreach (var d in services)
    {
        var isDiag =
            d.ServiceType == typeof(DiagnosticListener) ||
            d.ServiceType == typeof(DiagnosticSource);

        var instanceIsType = d.ImplementationInstance is Type;

        if (isDiag || instanceIsType)
        {
            Console.WriteLine(
                "[{0}] {1} | ImplType={2} | ImplInstance={3} | ImplFactory={4}",
                d.Lifetime,
                d.ServiceType.FullName,
                d.ImplementationType?.FullName ?? "-",
                d.ImplementationInstance?.GetType().FullName ?? "-",
                d.ImplementationFactory != null ? "YES" : "NO");

            if (d.ImplementationInstance is Type t)
                Console.WriteLine("    ⚠️ ImplementationInstance is Type: {0}", t.FullName);
        }
    }
}
await app.RunAsync();
