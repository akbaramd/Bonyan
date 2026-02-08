using Bonyan.Localization.External;
using Bonyan.Localization.Resources.BonLocalization;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.VirtualFileSystem;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Localization;

public class BonLocalizationModule : BonModule
{
    public BonLocalizationModule()
    {
        DependOn<BonLocalizationAbstractionsModule>();
        DependOn<BonVirtualFileSystemModule>();
    }
    
    
    public override ValueTask OnConfigureAsync(BonConfigurationContext context , CancellationToken cancellationToken = default)
    {
        BonStringLocalizerFactory.Replace(context.Services);

        context.Services.AddTransient<DefaultLanguageProvider>();
        context.Services.AddTransient<ILanguageProvider,DefaultLanguageProvider>();
        
        context.Services.AddTransient<IBonEnumLocalizer,BonEnumLocalizer>();
        context.Services.AddTransient<BonEnumLocalizer>();

        context.Services.AddTransient<NullExternalLocalizationStore>();
        context.Services.AddTransient<IExternalLocalizationStore,NullExternalLocalizationStore>();

        context.Services.AddTransient<LocalizableStringSerializer>();
        context.Services.AddTransient<ILocalizableStringSerializer,LocalizableStringSerializer>();

        context.Services.Configure<BonVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<BonLocalizationModule>("Bonayn", "Bonayn");
        });
        
        context.Services.Configure<BonLocalizationOptions>(options =>
        {
            options
                .Resources
                .Add<DefaultResource>("en");

            options
                .Resources
                .Add<BonLocalizationResource>("en")
                .AddVirtualJson("/Localization/Resources/BonLocalization");
        });
        return base.OnConfigureAsync(context);
    }

    
}
