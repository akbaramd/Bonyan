using AutoMapper;
using AutoMapper.Internal;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bonyan.AutoMapper
{
    public class BonAutoMapperModule : BonModule
    {
        public override Task OnConfigureAsync(BonConfigurationContext context)
        {
            var assembliesToScan = context.DiscoverApplicationAssemblies();

            context.Services.Configure<MapperConfigurationExpression>(
                (Action<MapperConfigurationExpression>)(options => options.AddMaps(assembliesToScan)));
            
            Type[] openTypes = new Type[5]
            {
                typeof(IValueResolver<,,>),
                typeof(IMemberValueResolver<,,,>),
                typeof(ITypeConverter<,>),
                typeof(IValueConverter<,>),
                typeof(IMappingAction<,>)
            };

            foreach (var openType in openTypes)
            {
                context.RegisterTransientServicesFor(openType);
            }
            

            context.Services.AddSingleton<IConfigurationProvider>(sp =>
            {
                using (var scope = sp.CreateScope())
                {
                    var options = scope.ServiceProvider.GetRequiredService<IOptions<BonAutoMapperOptions>>().Value;

                    var mapperConfigurationExpression =
                        sp.GetRequiredService<IOptions<MapperConfigurationExpression>>().Value;
                    var autoMapperConfigurationContext =
                        new BonAutoMapperConfigurationContext(mapperConfigurationExpression, scope.ServiceProvider);

                    foreach (var configurator in options.Configurators)
                    {
                        configurator(autoMapperConfigurationContext);
                    }

                    var mapperConfiguration = new MapperConfiguration(mapperConfigurationExpression);

                    foreach (var profileType in options.ValidatingProfiles)
                    {
                        mapperConfiguration.Internal()
                            .AssertConfigurationIsValid(((Profile)Activator.CreateInstance(profileType)!).ProfileName);
                    }

                    return mapperConfiguration;
                }
            });

            context.Services.AddTransient<IMapper>(sp =>
                sp.GetRequiredService<IConfigurationProvider>().CreateMapper(sp.GetService));

            return base.OnConfigureAsync(context);
        }
    }
}