using Bonyan.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Messaging.OutBox
{
    public class BonMessagingOutboxEntityFrameworkModule : BonModule
    {
        public BonMessagingOutboxEntityFrameworkModule()
        {
            DependOn<BonMessagingOutboxModule>();
            DependOn<BonEntityFrameworkModule>();
        }

      public override Task OnPreConfigureAsync(BonConfigurationContext context)
      {
          PreConfigure<BonMessagingConfiguration>(c =>
          {
              c.AddOutbox(c =>
              {
                  context.Services.ExecutePreConfiguredActions(c);
              });
          });
          return base.OnPreConfigureAsync(context);
      }

   
    }
}