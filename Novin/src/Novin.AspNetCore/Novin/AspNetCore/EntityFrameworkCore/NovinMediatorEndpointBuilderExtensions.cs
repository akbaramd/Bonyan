using Bonyan.DependencyInjection;
using Bonyan.EntityFrameworkCore;
using Bonyan.EntityFrameworkCore.Abstractions;

namespace Novin.AspNetCore.Novin.AspNetCore.EntityFrameworkCore
{
    public static class NovinEntityFrameworkCoreExtensions
    {
        /// <summary>
        /// Adds Mediator services to the configuration context.
        /// </summary>
        public static NovinConfigurationContext AddEntityFrameworkCore<TDbContext>(this NovinConfigurationContext builder,
            Action<IBonDbContextRegistrationOptionBuilder> configureOptions ) where TDbContext : BonDbContext<TDbContext>
        {
            builder.BonContext.AddDbContext<TDbContext>(configureOptions);
            return builder;
        }

      
        
    }
}
