using Bonyan.AspNetCore.Localization.External;
using Bonyan.AspNetCore.Localization.Resources.AbpLocalization;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.VirtualFileSystem;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.AspNetCore.Localization;

public class AbpLocalizationModule : BonModule
{
    public AbpLocalizationModule()
    {
        DependOn<AbpLocalizationAbstractionsModule>();
        DependOn<AbpVirtualFileSystemModule>();
    }
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        AbpStringLocalizerFactory.Replace(context.Services);

        context.Services.AddTransient<DefaultLanguageProvider>();
        context.Services.AddTransient<ILanguageProvider,DefaultLanguageProvider>();
        
        context.Services.AddTransient<IAbpEnumLocalizer,AbpEnumLocalizer>();
        context.Services.AddTransient<AbpEnumLocalizer>();

        context.Services.AddTransient<NullExternalLocalizationStore>();
        context.Services.AddTransient<IExternalLocalizationStore,NullExternalLocalizationStore>();

        context.Services.AddTransient<LocalizableStringSerializer>();
        context.Services.AddTransient<ILocalizableStringSerializer,LocalizableStringSerializer>();

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpLocalizationModule>("Volo.Abp", "Volo/Abp");
        });
        
        Configure<AbpLocalizationOptions>(options =>
        {
            options
                .Resources
                .Add<DefaultResource>("en");

            options
                .Resources
                .Add<AbpLocalizationResource>("en")
                .AddVirtualJson("/Localization/Resources/AbpLocalization");
        });
        return base.OnConfigureAsync(context);
    }

    
}
