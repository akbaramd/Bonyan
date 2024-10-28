using AutoMapper;
using AutoMapper.Internal;
using Bonyan.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Module = Bonyan.Modularity.Abstractions.Module;

namespace Bonyan.AutoMapper
{
    public class BonyanAutoMapperModule : Module
    {
        public override Task OnConfigureAsync(ServiceConfigurationContext context)
        {

          context.Services.AddSingleton<IConfigurationProvider>(sp =>
          {
            using (var scope = sp.CreateScope())
            {
              var options = scope.ServiceProvider.GetRequiredService<IOptions<BonyanAutoMapperOptions>>().Value;

              var mapperConfigurationExpression = sp.GetRequiredService<IOptions<MapperConfigurationExpression>>().Value;
              var autoMapperConfigurationContext = new BonyanAutoMapperConfigurationContext(mapperConfigurationExpression, scope.ServiceProvider);

              foreach (var configurator in options.Configurators)
              {
                configurator(autoMapperConfigurationContext);
              }
              var mapperConfiguration = new MapperConfiguration(mapperConfigurationExpression);

              foreach (var profileType in options.ValidatingProfiles)
              {
                mapperConfiguration.Internal().AssertConfigurationIsValid(((Profile)Activator.CreateInstance(profileType)!).ProfileName);
              }

              return mapperConfiguration;
            }
          });

          context.Services.AddTransient<IMapper>(sp => sp.GetRequiredService<IConfigurationProvider>().CreateMapper(sp.GetService));

            return base.OnConfigureAsync(context);
        }
    }
}
