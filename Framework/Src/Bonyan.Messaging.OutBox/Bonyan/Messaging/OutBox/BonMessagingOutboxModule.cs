using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Messaging.OutBox
{
    public class BonMessagingOutboxModule : BonModule
    {
        public BonMessagingOutboxModule()
        {
            DependOn<BonMessagingModule>();
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