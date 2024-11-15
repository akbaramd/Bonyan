using System;
using Microsoft.Extensions.Logging;

namespace Bonyan.Exceptions
{
    [Serializable]
    public class MessagingNotConfiguredException : BusinessException
    {
        public MessagingNotConfiguredException()
            : base(
                code: "Messaging.NotConfigured",
                message: "The Messaging Layer is not configured. Please configure the messaging layer by calling AddMessaging(...) in your AddBonyan(c=>...) method or if use module ensure your module depends on BonMessagingModule.",
                details: "Domain events require the messaging layer to be configured.",
                logLevel: LogLevel.Error)
        {
        }
    }
}