using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

using Bonyan.Modularity;

namespace Bonyan.Messaging
{
    /// <summary>
    /// Provides configuration options for registering message consumers and dispatchers in the messaging system.
    /// </summary>
    public class BonMediatorConfiguration
    {
        public BonMediatorConfiguration(BonConfigurationContext services)
        {
            Context = services ?? throw new ArgumentNullException(nameof(services));
          

            // Register the configuration instance
            Context.Services.AddSingleton(this);
        }

        public BonConfigurationContext Context { get; set; }

     
    }
}
