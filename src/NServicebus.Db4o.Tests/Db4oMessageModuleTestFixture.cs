using Db4oFramework;
using NServiceBus;
using NServiceBus.Db4o;
using NUnit.Framework;
using Rhino.Mocks;

namespace NServicebus.Db4o.Tests
{
    [TestFixture]
    public class when_handling_begin_message
    {
        private ISessionFactory _sessionFactory;
        private ISession _session;
        private IMessageModule _mod;

        [SetUp]
        public void SetupContext()
        {
            _sessionFactory = MockRepository.GenerateStrictMock<ISessionFactory>();
            _session = MockRepository.GenerateStrictMock<ISession>();

            _sessionFactory.Expect(x => x.OpenSession()).Return(_session);
            _sessionFactory.Expect(x => x.Bind(_session));

            _mod = new Db4oMessageModule(_sessionFactory);
        }

        [Test]
        public void it_should_open_a_new_session()
        {
            _mod.HandleBeginMessage();
            _sessionFactory.AssertWasCalled(x => x.OpenSession());
        }

        [Test]
        public void it_should_bind_the_session_to_the_session_factory()
        {
            _mod.HandleBeginMessage();
            _sessionFactory.AssertWasCalled(x => x.Bind(_session));
        }
    }

    [TestFixture]
    public class when_handling_end_message
    {
        private ISessionFactory _sessionFactory;
        private ISession _session;
        private IMessageModule _mod;

        [SetUp]
        public void SetupContext()
        {
            _sessionFactory = MockRepository.GenerateStrictMock<ISessionFactory>();
            _session = MockRepository.GenerateStrictMock<ISession>();

            _sessionFactory.Expect(x => x.Unbind()).Return(_session);
            _session.Expect(x => x.Close()).Return(true);
            _session.Expect(x => x.Dispose());

            _mod = new Db4oMessageModule(_sessionFactory);
        }

        [Test]
        public void it_should_unbind_the_session_and_close_it()
        {
            _mod.HandleEndMessage();
            _sessionFactory.AssertWasCalled(x => x.Unbind());
        }

        [Test]
        public void it_should_close_the_session()
        {
            _mod.HandleEndMessage();
            _session.AssertWasCalled(x => x.Close());
        }

        [Test]
        public void it_should_dispose_the_session()
        {
            _mod.HandleEndMessage();
            _session.AssertWasCalled(x => x.Dispose());
        }
    }

    [TestFixture]
    public class when_handling_error_message
    {
        private IMessageModule _mod;
        private ISessionFactory _sessionFactory;
        private ISession _session;

        [SetUp]
        public void SetupContext()
        {
            _session = MockRepository.GenerateStrictMock<ISession>();
            _sessionFactory = MockRepository.GenerateStrictMock<ISessionFactory>();

            _sessionFactory.Expect(x => x.Unbind()).Return(_session);
            _session.Expect(x => x.Rollback());
            _session.Expect(x => x.Close()).Return(true);
            _session.Expect(x => x.Dispose());

            _mod = new Db4oMessageModule(_sessionFactory);
        }

        [Test]
        public void it_should_unbind_the_session()
        {
            _mod.HandleError();
            _sessionFactory.AssertWasCalled(x => x.Unbind());
        }

        [Test]
        public void it_should_rollback_the_transaction()
        {
            _mod.HandleError();
            _session.AssertWasCalled(x => x.Rollback());
        }

        [Test]
        public void it_should_close_the_session()
        {
            _mod.HandleError();
            _session.AssertWasCalled(x => x.Close());
        }

        [Test]
        public void it_should_dispose_the_session()
        {
            _mod.HandleError();
            _session.AssertWasCalled(x => x.Dispose());
        }
    }
}