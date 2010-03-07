using System.Collections.Generic;

namespace NServiceBus.Unicast.Subscriptions.Db4o
{
    public class Subscription
    {
        public string SubscriberEndpoint { get; set; }
        public IList<string> MessageTypes { get; set; }
    }
}