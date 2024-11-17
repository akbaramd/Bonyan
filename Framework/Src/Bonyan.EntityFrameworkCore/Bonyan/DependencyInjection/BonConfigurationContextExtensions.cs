using System.Reflection;
using Bonyan.EntityFrameworkCore;
using Bonyan.EntityFrameworkCore.Abstractions;
using Bonyan.EntityFrameworkCore.Builders;
using Bonyan.Modularity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Bonyan.DependencyInjection;

public static class Extensions
{
    public static BonConfigurationContext AddBonDbContext<TDbContext>(this BonConfigurationContext context,
        Action<IBonDbContextRegistrationOptionBuilder>? optionsBuilder = null)
        where TDbContext : BonDbContext<TDbContext>
    {
        var option = CreateDbContextRegistrationOption<TDbContext>(context, optionsBuilder);

        RegisterDbContext<TDbContext>(context, option);
        RegisterAdditionalDbContexts(context, option);
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

    private static void RegisterAdditionalDbContexts(BonConfigurationContext context, BonDbContextRegistrationOptionBuilder option)
    {
        var interfaces = option.OriginalDbContextType.GetInterfaces();
        foreach (var implementedInterface in interfaces)
        {
            if (typeof(IEfDbContext).IsAssignableFrom(implementedInterface) && implementedInterface != typeof(IEfDbContext))
            {
                option.AdditionalDbContexts.Add(implementedInterface);
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
