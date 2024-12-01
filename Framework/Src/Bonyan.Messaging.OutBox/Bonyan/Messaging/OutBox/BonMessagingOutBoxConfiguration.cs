namespace Bonyan.Messaging.OutBox
{
    public class BonMessagingOutBoxConfiguration
    {

        public BonMessagingOutBoxConfiguration(BonMessagingConfiguration configuration)
        {
            Configuration = configuration;
        }

        public BonMessagingConfiguration Configuration { get; set; }
    }
}
