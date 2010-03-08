using Db4objects.Db4o.CS;
using Db4oFramework;
using NUnit.Framework;

namespace NServiceBus.Unitcast.Subscriptions.Db4o.Tests
{
    public abstract class ConfigurationTestFixture
    {
        protected Configure config;

        [Test]
        public void The_session_source_factory_should_be_registered_as_singleton()
        {
            var sessionFactory = config.Builder.Build<ISessionFactory>();
            Assert.That(sessionFactory, Is.SameAs(config.Builder.Build<ISessionFactory>()));
        }
    }

    [TestFixture]
    public class when_configuring_the_subscription_storage_to_use_a_hosted_database : ConfigurationTestFixture
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            var sessionContext = new ThreadStaticCurrentSessionContext();
            var serverConfig = Db4oClientServer.NewServerConfiguration();
            var dbFileName = "ConfigTest.yap";
            var port = 17432;
            var access = new HostedServerSessionFactory.Access() { Username = "nservicebus", Password = "nservicebus" };

            config = Configure.With()
                .SpringBuilder()
                .Db4oHostedDatabase(sessionContext, serverConfig, dbFileName, port, access)
                .Db4oSubscriptionStorage();
        }

        [Test]
        public void the_session_source_factory_should_be_HostedServerSessionFactory()
        {
            var sessionFactory = config.Builder.Build<ISessionFactory>();
            Assert.That(sessionFactory, Is.TypeOf<HostedServerSessionFactory>());
        }
    }

    [TestFixture]
    public class when_configuring_the_subscription_storage_to_use_a_remote_database : ConfigurationTestFixture
    {
        [SetUp]
        public void SetUp()
        {
            var sessionContext = new ThreadStaticCurrentSessionContext();
            var host = "localhost";
            var port = 17432;
            var username = "nservicebus";
            var password = "nservicebus";
            config = Configure.With()
                .SpringBuilder()
                .Db4oRemoteDatabase(sessionContext, () => Db4oClientServer.NewClientConfiguration(),host,port,username,password)
                .Db4oSubscriptionStorage();
        }

        [Test]
        public void the_session_source_factory_should_be_RemoteServerSessionFactory()
        {
            var sessionFactory = config.Builder.Build<ISessionFactory>();
            Assert.That(sessionFactory, Is.TypeOf<RemoteServerSessionFactory>());
        }
    }
}