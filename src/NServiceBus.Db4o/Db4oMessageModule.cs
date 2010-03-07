using Db4oFramework;

namespace NServiceBus.Db4o
{
    public class Db4oMessageModule : IMessageModule
    {
        private readonly ISessionFactory _sessionFactory;

        public Db4oMessageModule(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public void HandleBeginMessage()
        {
            var session = _sessionFactory.OpenSession();
            _sessionFactory.Bind(session);
        }

        public void HandleEndMessage()
        {
            var session = _sessionFactory.Unbind();
            if(session == null)
            {
                return;
            }

            session.Close();
            session.Dispose();
        }

        public void HandleError()
        {
            var session = _sessionFactory.Unbind();
            if (session == null)
            {
                return;
            }

            session.Rollback();
            session.Close();
            session.Dispose();
        }
    }
}