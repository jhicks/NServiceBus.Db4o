using System.Collections.Generic;
using System.IO;
using System.Linq;
using Db4objects.Db4o.Linq;
using Db4oFramework;
using NServiceBus.Saga;
using NServiceBus.Unicast.Subscriptions;
using NServiceBus.Unicast.Subscriptions.Db4o;
using NUnit.Framework;

namespace NServiceBus.Unitcast.Subscriptions.Db4o.Tests
{
    public abstract class SubscriptionStorageTestFixture
    {
        protected const string DbFileName = ".\\Db4oSagaPersisterTestDb.yap";

        protected ISessionFactory SessionFactory;
        protected ICurrentSessionContext CurrentSessionContext;
        protected ISagaPersister SagaPersister;
        protected ISubscriptionStorage _storage;

        [SetUp]
        public void SetupSagaPersisterContext()
        {
            DeleteFile();
            CurrentSessionContext = new ThreadStaticCurrentSessionContext();
            SessionFactory = new HostedServerSessionFactory(CurrentSessionContext, DbFileName);
            _storage = new Db4oSubscriptionStorage(SessionFactory);
        }

        [TearDown]
        public void DestroySagaPersisterContext()
        {
            SagaPersister = null;
            SessionFactory.Dispose();
            SessionFactory = null;
            DeleteFile();
        }

        private static void DeleteFile()
        {
            if (File.Exists(DbFileName))
            {
                File.Delete(DbFileName);
            }
        }

    }

    [TestFixture]
    public class When_receiving_a_subscription_message : SubscriptionStorageTestFixture
    {
        [Test]
        public void A_subscription_entry_should_be_added_to_the_database()
        {
            const string clientEndpoint = "TestEndpoint";
            _storage.Subscribe(clientEndpoint, new List<string> { "MessageType1" });

            using (var db = SessionFactory.OpenSession())
            {
                var subscription = db.AsQueryable<Subscription>().FirstOrDefault(s => s.SubscriberEndpoint == clientEndpoint);
                Assert.That(subscription, Is.Not.Null);
                Assert.That(subscription.MessageTypes.Count, Is.EqualTo(1));
                Assert.That(subscription.MessageTypes[0], Is.EqualTo("MessageType1"));
            }
        }

        [Test]
        public void duplicate_subcriptions_shouldnt_create_aditional_db_entries()
        {
            _storage.Subscribe("testendpoint", new List<string> { "SomeMessageType" });
            _storage.Subscribe("testendpoint", new List<string> { "SomeMessageType" });

            using (var db = SessionFactory.OpenSession())
            {
                var subscriptions = db.AsQueryable<Subscription>().Where(s => s.SubscriberEndpoint == "testendpoint");
                Assert.That(subscriptions.Count(), Is.EqualTo(1));
                Assert.That(subscriptions.First().MessageTypes.Count(), Is.EqualTo(1));
            }
        }
    }

    [TestFixture]
    public class when_listing_subscribers_for_message_type : SubscriptionStorageTestFixture
    {
        [Test]
        public void The_names_of_all_subscribers_should_be_returned()
        {
            // arrange
            const string clientEndpoint = "TestEndpoint";
            _storage.Subscribe(clientEndpoint, new List<string> { "MessageType1" });
            _storage.Subscribe(clientEndpoint, new List<string> { "MessageType2" });
            _storage.Subscribe("some other endpoint", new List<string> { "MessageType1" });

            // act
            var subscriptionsForMessageType = _storage.GetSubscribersForMessage(new List<string> { "MessageType1" });

            // assert
            Assert.That(subscriptionsForMessageType.Count, Is.EqualTo(2));
            Assert.That(subscriptionsForMessageType.Contains(clientEndpoint), Is.True);
            Assert.That(subscriptionsForMessageType.Contains("some other endpoint"), Is.True);
        }

        [Test]
        public void The_names_of_non_subscribers_should_not_be_returned()
        {
            // arrange
            const string clientEndpoint = "TestEndpoint";
            _storage.Subscribe(clientEndpoint, new List<string> { "MessageType1" });
            _storage.Subscribe(clientEndpoint, new List<string> { "MessageType2" });
            _storage.Subscribe("some other endpoint", new List<string> { "MessageType1" });

            // act
            var subscriptionsForMessageType = _storage.GetSubscribersForMessage(new List<string> { "MessageType2" });

            // assert
            Assert.That(subscriptionsForMessageType.Count, Is.EqualTo(1));
            Assert.That(subscriptionsForMessageType.Contains(clientEndpoint), Is.True);
            Assert.That(subscriptionsForMessageType.Contains("some other endpoint"), Is.False);
        }
    }

    [TestFixture]
    public class When_receiving_a_unsubscription_message : SubscriptionStorageTestFixture
    {
        [Test]
        public void All_subscription_entries_for_specfied_message_types_should_be_removed()
        {
            // arrange
            string clientEndpoint = "TestEndpoint";

            var messageTypes = new List<string> { "MessageType1", "MessageType2" };
            _storage.Subscribe(clientEndpoint, messageTypes);

            // act
            _storage.Unsubscribe(clientEndpoint, messageTypes);

            // assert
            using (var db = SessionFactory.OpenSession())
            {
                var subscriptionCountForEndpoint = db.AsQueryable<Subscription>().Count(x => x.SubscriberEndpoint == clientEndpoint);
                Assert.That(subscriptionCountForEndpoint, Is.EqualTo(0));
            }
        }
    }
}