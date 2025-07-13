using Bonyan.EntityFrameworkCore;
using Bonyan.EntityFrameworkCore.Abstractions;
using Bonyan.EntityFrameworkCore.Builders;
using Bonyan.Modularity;
using Bonyan.Plugins;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bonyan.DependencyInjection;

public static class Extensions
{
    public static BonConfigurationContext AddDbContext<TDbContext>(this BonConfigurationContext context,
        Action<IBonDbContextRegistrationOptionBuilder>? optionsBuilder = null)
        where TDbContext : BonDbContext<TDbContext>
    {
        var option = CreateDbContextRegistrationOption<TDbContext>(context, optionsBuilder);

        RegisterDbContext<TDbContext>(context, option);
        RegisterAdditionalDbContexts<TDbContext>(context, option);
        ConfigureRepositories(option);

        return context;
    }

    private static BonDbContextRegistrationOptionBuilder CreateDbContextRegistrationOption<TDbContext>(BonConfigurationContext context,
        Action<IBonDbContextRegistrationOptionBuilder>? optionsBuilder)
        where TDbContext : BonDbContext<TDbContext>
    {
        var option = new BonDbContextRegistrationOptionBuilder(context.Services, typeof(TDbContext));
        optionsBuilder?.Invoke(option);
        return option;
    }

    private static void RegisterDbContext<TDbContext>(BonConfigurationContext context, BonDbContextRegistrationOptionBuilder option)
        where TDbContext : BonDbContext<TDbContext>
    {
        context.Services.AddTransient<TDbContext>();

        context.Services.TryAddTransient<DbContextOptions<TDbContext>>(sp =>
        {
            var builder = new DbContextOptionsBuilder<TDbContext>();
            option.DbContextOptionsAction.Invoke(builder);
            return builder.Options;
        });
    }

    private static void RegisterAdditionalDbContexts<TDbContext>(BonConfigurationContext context, BonDbContextRegistrationOptionBuilder option) where TDbContext : IEfDbContext
    {
        var interfaces = option.OriginalDbContextType.GetInterfaces();
        foreach (var implementedInterface in interfaces)
        {
            if (typeof(IEfDbContext).IsAssignableFrom(implementedInterface) && implementedInterface != typeof(IEfDbContext))
            {
                context.Services.Replace(ServiceDescriptor.Transient(implementedInterface, sp =>
                {
                    return sp.GetRequiredService<TDbContext>();
                }));
            }
        }

        foreach (var additionalDbContext in option.AdditionalDbContexts)
        {
            context.Services.TryAddTransient(additionalDbContext, c => c.GetRequiredService(option.OriginalDbContextType));
        }
    }

    private static void ConfigureRepositories(BonDbContextRegistrationOptionBuilder option)
    {
        new BonEfCoreRepositoryRegistrar(option).ConfigureRepository();
    }
}
