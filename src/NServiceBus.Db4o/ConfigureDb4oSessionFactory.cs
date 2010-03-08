using System;
using Db4objects.Db4o.CS;
using Db4objects.Db4o.CS.Config;
using Db4oFramework;
using NServiceBus.Db4o;

namespace NServiceBus
{
    /// <summary>
    /// Helper methods to spin up a db4o session factory and register it with NSB's IoC
    /// </summary>
    public static class ConfigureDb4oSessionFactory
    {
        public static Configure Db4oHostedDatabase(this Configure config, 
            ICurrentSessionContext currentSessionContext, IServerConfiguration serverConfig, 
            string dbFileName, int port, params HostedServerSessionFactory.Access[] access)
        {
            var sessionFactory = new HostedServerSessionFactory(currentSessionContext, serverConfig, dbFileName, port, access);
            config.Configurer.RegisterSingleton<ISessionFactory>(sessionFactory);
            return config;
        }

        public static Configure Db4oHostedDatabase(this Configure config, params HostedServerSessionFactory.Access[] access)
        {
            var configSettings = Configure.GetConfigSection<Db4oConnectionConfig>();
            var currentSessionContextType = Type.GetType(configSettings.CurrentSessionContext);
            var currentSessionContext = (ICurrentSessionContext)Activator.CreateInstance(currentSessionContextType);

            Db4oHostedDatabase(config, currentSessionContext, Db4oClientServer.NewServerConfiguration(), configSettings.DatabaseFile, configSettings.Port, access);

            return config;
        }

        public static Configure Db4oRemoteDatabase(this Configure config,
            ICurrentSessionContext currentSessionContext, Func<IClientConfiguration> clientConfig, 
            string host, int port, string username, string password)
        {
            var sessionFactory = new RemoteServerSessionFactory(currentSessionContext, clientConfig, host, port, username, password);
            config.Configurer.RegisterSingleton<ISessionFactory>(sessionFactory);
            return config;
        }

        public static Configure Db4oRemoteDatabase(this Configure config, Func<IClientConfiguration> clientConfig)
        {
            var configSettings = Configure.GetConfigSection<Db4oConnectionConfig>();
            var currentSessionContextType = Type.GetType(configSettings.CurrentSessionContext);
            var currentSessionContext = (ICurrentSessionContext)Activator.CreateInstance(currentSessionContextType);
            return Db4oRemoteDatabase(config, currentSessionContext, clientConfig, configSettings.Host, configSettings.Port, configSettings.Username, configSettings.Password);
        }
    }
}