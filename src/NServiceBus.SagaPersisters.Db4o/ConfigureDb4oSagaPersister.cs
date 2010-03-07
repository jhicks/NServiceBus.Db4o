using NServiceBus.ObjectBuilder;
using NServiceBus.SagaPersisters.Db4o;

namespace NServiceBus
{
    public static class ConfigureDb4oSagaPersister
    {
        public static Configure Db4oSagaPersister(this Configure config)
        {
            config.Configurer.ConfigureComponent<Db4oSagaPersister>(ComponentCallModelEnum.Singlecall);
            return config;
        }
    }
}