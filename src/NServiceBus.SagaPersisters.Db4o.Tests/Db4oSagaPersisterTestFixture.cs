using System;
using System.IO;
using System.Linq;
using Db4objects.Db4o.Linq;
using Db4oFramework;
using NServiceBus.Saga;
using NUnit.Framework;

namespace NServiceBus.SagaPersisters.Db4o.Tests
{
    public class BasicSagaData : ISagaEntity
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }
    }

    public abstract class Db4oSagaPersisterTestFixture
    {
        protected const string DbFileName = ".\\Db4oSagaPersisterTestDb.yap";

        protected ISessionFactory SessionFactory;
        protected ICurrentSessionContext CurrentSessionContext;
        protected ISagaPersister SagaPersister;

        [SetUp]
        public void SetupSagaPersisterContext()
        {
            DeleteFile();
            CurrentSessionContext = new ThreadStaticCurrentSessionContext();
            SessionFactory = new HostedServerSessionFactory(CurrentSessionContext, DbFileName);
            SagaPersister = new Db4oSagaPersister(SessionFactory);
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

        protected static BasicSagaData CreateBasicData()
        {
            return new BasicSagaData {
                                         Id = Guid.NewGuid(),
                                         Originator = String.Format("Originator{0}",Guid.NewGuid()),
                                         OriginalMessageId = Guid.NewGuid().ToString()
                                     };
        }
    }

    [TestFixture]
    public class when_completing_a_saga : Db4oSagaPersisterTestFixture
    {
        [Test]
        public void it_should_remove_the_saga_from_the_database()
        {
            Guid id;
            using (var session = SessionFactory.OpenSession())
            {
                var sagaEntity = CreateBasicData();
                id = sagaEntity.Id;
                session.Store(sagaEntity);
            }

            using (var session = SessionFactory.OpenSession())
            {
                SessionFactory.Bind(session);
                var data = SagaPersister.Get<BasicSagaData>(id);
                SagaPersister.Complete(data);
                SessionFactory.Unbind();
            }

            using (var session = SessionFactory.OpenSession())
            {
                var data = session.AsQueryable<BasicSagaData>().SingleOrDefault(x => x.Id == id);

                Assert.That(data, Is.Null);
            }
        }
    }

    [TestFixture]
    public class when_getting_a_saga_by_id : Db4oSagaPersisterTestFixture
    {
        [Test]
        public void it_should_return_the_entity_previously_saved()
        {
            Guid id;
            using (var session = SessionFactory.OpenSession())
            {
                var sagaEntity = CreateBasicData();
                id = sagaEntity.Id;
                session.Store(sagaEntity);
            }

            using (var client = SessionFactory.OpenSession())
            {
                SessionFactory.Bind(client);
                var data = SagaPersister.Get<BasicSagaData>(id);
                Assert.That(data, Is.Not.Null);
                SessionFactory.Unbind();
            }
        }
    }

    [TestFixture]
    public class when_saving_a_saga : Db4oSagaPersisterTestFixture
    {
        [Test]
        public void it_should_store_the_saga_in_the_database()
        {
            Guid id;
            
            using(var session = SessionFactory.OpenSession())
            {
                SessionFactory.Bind(session);
                var sagaEntity = CreateBasicData();
                id = sagaEntity.Id;
                SagaPersister.Save(sagaEntity);
                SessionFactory.Unbind();
            }

            using (var session = SessionFactory.OpenSession())
            {
                var data = session.AsQueryable<BasicSagaData>().SingleOrDefault(x => x.Id == id);

                Assert.That(data, Is.Not.Null);
            }
        }
    }
}