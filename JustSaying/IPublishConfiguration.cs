using System.Collections.Generic;
using JustSaying.AwsTools.MessageHandling;

namespace JustSaying
{
    public interface IPublishConfiguration
    {
        int PublishFailureReAttempts { get; set; }
        int PublishFailureBackoffMilliseconds { get; set; }
        MessageResponseHandler RequestLogger { get; set; }
        IReadOnlyCollection<string> AdditionalSubscriberAccounts { get; set; }
    }
}
