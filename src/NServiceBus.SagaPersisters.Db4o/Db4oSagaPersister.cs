using System;
using System.Linq;
using Db4objects.Db4o.Linq;
using Db4oFramework;
using NServiceBus.Saga;

namespace NServiceBus.SagaPersisters.Db4o
{
    public class Db4oSagaPersister : ISagaPersister
    {
        private readonly ISessionFactory _sessionFactory;

        public Db4oSagaPersister(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public void Save(ISagaEntity saga)
        {
            _sessionFactory.GetCurrentSession().Store(saga);
        }

        public void Update(ISagaEntity saga)
        {
            _sessionFactory.GetCurrentSession().Store(saga);
        }

        public T Get<T>(Guid sagaId) where T : ISagaEntity
        {
            return _sessionFactory.GetCurrentSession().AsQueryable<T>().SingleOrDefault(x => x.Id == sagaId);
        }

        public T Get<T>(string property, object value) where T : ISagaEntity
        {
            var query = _sessionFactory.GetCurrentSession().Query();
            query.Constrain(typeof (T));
            query.Descend(property).Constrain(value);
            return query.Execute().Cast<T>().SingleOrDefault();
        }

        public void Complete(ISagaEntity saga)
        {
            _sessionFactory.GetCurrentSession().Delete(saga);
        }
    }
}