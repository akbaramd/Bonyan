using System.Reflection;
using AutoMapper;
using AutoMapper.Internal;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Module = Bonyan.Modularity.Abstractions.Module;

namespace Bonyan.AutoMapper
{
    public class BonyanAutoMapperModule : Module
    {
        public override Task OnConfigureAsync(ServiceConfigurationContext context)
        {
          var assembliesToScan = AppDomain.CurrentDomain.GetAssemblies();

          context.Services.Configure<MapperConfigurationExpression>((Action<MapperConfigurationExpression>) (options => options.AddMaps(assembliesToScan)));
          Type[] openTypes = new Type[5]
          {
            typeof (IValueResolver<,,>),
            typeof (IMemberValueResolver<,,,>),
            typeof (ITypeConverter<,>),
            typeof (IValueConverter<,>),
            typeof (IMappingAction<,>)
          };
          foreach (Type service in assembliesToScan.SelectMany<Assembly, Type>((Func<Assembly, IEnumerable<Type>>) (a => ((IEnumerable<Type>) a.GetTypes()).Where<Type>((Func<Type, bool>) (type => type.IsClass && !type.IsAbstract && Array.Exists<Type>(openTypes, (Predicate<Type>) (openType => type.GetGenericInterface(openType) != (Type) null)))))))
            context.Services.TryAddTransient(service);
          
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
