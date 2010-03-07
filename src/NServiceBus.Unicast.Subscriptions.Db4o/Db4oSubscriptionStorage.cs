using System.Collections.Generic;
using System.Linq;
using Db4objects.Db4o.Linq;
using Db4oFramework;

namespace NServiceBus.Unicast.Subscriptions.Db4o
{
    /// <summary>
    /// Stores subscriptions in db4o
    /// </summary>
    public class Db4oSubscriptionStorage : ISubscriptionStorage
    {
        private readonly ISessionFactory _sessionFactory;

        public Db4oSubscriptionStorage(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public void Subscribe(string client, IList<string> messageTypes)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var existingSubscription = session.AsQueryable<Subscription>().SingleOrDefault(x => x.SubscriberEndpoint == client) 
                    ?? new Subscription { SubscriberEndpoint = client, MessageTypes = new List<string>() };

                var messageTypesToSubscribe = messageTypes.Union(existingSubscription.MessageTypes).ToList();

                existingSubscription.MessageTypes = messageTypesToSubscribe;
                session.Store(existingSubscription);
                session.Commit();
            }
        }

        public void Unsubscribe(string client, IList<string> messageTypes)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var existingSubscription = session.AsQueryable<Subscription>().SingleOrDefault(x => x.SubscriberEndpoint == client);

                if (existingSubscription == null)
                {
                    return;
                }

                session.Delete(existingSubscription);
                session.Commit();
            }
        }

        public IList<string> GetSubscribersForMessage(IList<string> messageTypes)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var db = session.AsQueryable<Subscription>();
                return db.Where(s => s.MessageTypes.Any(mt => messageTypes.Contains(mt))).Select(s => s.SubscriberEndpoint).Distinct().ToList();
            }
        }

        public void Init() {}
    }
}