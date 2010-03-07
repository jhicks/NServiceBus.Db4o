using NServiceBus.ObjectBuilder;
using NServiceBus.Unicast.Subscriptions.Db4o;

namespace NServiceBus
{
    public static class ConfigureDb4oSubscriptionStorage
    {
        public static Configure Db4oSubscriptionStorage(this Configure config)
        {
            config.Configurer.ConfigureComponent<Db4oSubscriptionStorage>(ComponentCallModelEnum.Singlecall);
            return config;
        }
    }
}