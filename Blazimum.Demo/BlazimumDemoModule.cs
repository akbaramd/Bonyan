using Blazimum.Demo.Components;
using Bonyan.Modularity;
using Bonyan.Ui.Blazimum;

namespace Blazimum.Demo;

public class BlazimumDemoModule : BonWebModule
{
    public BlazimumDemoModule()
    {
        DependOn<BonUiBlazimumModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        return base.OnConfigureAsync(context);
    }


    public override Task OnPostApplicationAsync(BonWebApplicationContext context)
    {
        // Configure the HTTP request pipeline.
        if (!context.Application.Environment.IsDevelopment())
        {
            context.Application.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            context.Application.UseHsts();
        }

        context.Application.UseHttpsRedirection();

        context.Application.UseStaticFiles();
        context.Application.UseAntiforgery();

        context.Application.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();


        return base.OnPostApplicationAsync(context);
    }
}