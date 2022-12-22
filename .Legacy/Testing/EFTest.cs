using System;
using System.Collections.Generic;
using System.Diagnostics;
using Forge.Reflection;
using Forge.Testing.Entities;
using Forge.Testing.Entities.NETSetTest.MTM_Bi;
using Forge.Testing.Entities.NETSetTest.MTM_Uni;
using Forge.Testing.Entities.NETSetTest.MTO;
using Forge.Testing.Entities.NETSetTest.OTM;
using Forge.Testing.Entities.NETSetTest.OTO;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Mapping.Attributes;
using NHibernate.SqlCommand;
using NHibernate.Tool.hbm2ddl;
using Forge.ORM.NHibernateExtension;
using Forge.ORM.NHibernateExtension.Model.Distributed;
using Forge.ORM.NHibernateExtension.Model.Distributed.Serialization;
using Forge.ORM.NHibernateExtension.Criterias;

namespace Forge.Testing
{

    /// <summary>
    /// Summary description for EFTest
    /// </summary>
    [TestClass]
    public class EFTest
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(EFTest));

        private static ISessionFactory sessionFactory = null;

        private TestContext testContextInstance;

        #endregion

        #region Constructor(s)

        static EFTest()
        {
            Forge.Logging.Log4net.Log4NetManager.InitializeFromAppConfig();
            LOGGER.Warn("START::WARN");
            LOGGER.Debug("START::DEBUG");
            LOGGER.Info("START::INFO");
            LOGGER.Error("START::ERROR");
            LOGGER.Fatal("START::FATAL");
        }

        public EFTest()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #endregion

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void Initialize(TestContext testContext)
        {
            // define mapping schema
            HbmSerializer.Default.HbmAssembly = typeof(Product).Assembly.GetName().FullName;
            //HbmSerializer.Default.HbmNamespace = typeof( Product ).Namespace;
            HbmSerializer.Default.HbmAutoImport = true;
            HbmSerializer.Default.Validate = true;
            HbmSerializer.Default.WriteDateComment = false;
            HbmSerializer.Default.HbmDefaultAccess = "field";
            HbmSerializer.Default.Serialize(typeof(Product).Assembly, "output.hbm.xml"); // serialize mapping xml into file to spectate it

            // create configuration and load assembly
            NHibernate.Cfg.Configuration cfg = new NHibernate.Cfg.Configuration();
            //cfg.Properties[NHibernate.Cfg.Environment.CollectionTypeFactoryClass] = typeof(Net4CollectionTypeFactory).AssemblyQualifiedName;
            cfg.Configure();
            //cfg.AddAssembly( typeof( Product ).Assembly ); // use this only, if hbm.xml exists in the assembly
            cfg.AddInputStream(HbmSerializer.Default.Serialize(typeof(Product).Assembly)); // ez bármikor müxik, de lassabb

            try
            {
                SchemaValidator schemaValidator = new SchemaValidator(cfg);
                schemaValidator.Validate(); // validate the database schema
            }
            catch (Exception)
            {
                SchemaUpdate schemaUpdater = new SchemaUpdate(cfg); // try to update schema
                schemaUpdater.Execute(false, true);
                if (schemaUpdater.Exceptions.Count > 0)
                {
                    throw new Exception("FAILED TO UPDATE SCHEMA");
                }
            }

            //SchemaExport export = new SchemaExport(cfg);
            //export.Execute( false, true, false );
            //new SchemaExport( cfg ).Execute( false, true, false );

            sessionFactory = cfg.BuildSessionFactory();
        }

        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            if (sessionFactory != null)
            {
                sessionFactory.Dispose();
            }
        }

        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        #region Test Method(s)

        [TestMethod]
        public void ConcurrencyTest()
        {
            //EntityId id = new EntityId(1, 1, DateTime.Now.Ticks);
            //using (ISession session = sessionFactory.OpenSession())
            //{
            //    using (ITransaction transaction = session.BeginTransaction())
            //    {
            //        Product p = new Product();
            //        p.Id = id;
            //        p.Version = new EntityVersion(1);
            //        p.Category = "category_" + Guid.NewGuid().ToString();
            //        p.Name = "name_" + Guid.NewGuid().ToString();
            //        p.Price = new decimal(102.23);
            //        p.Deleted = true;
            //        ORMUtils.SaveEntity(p, session);
            //        transaction.Commit();
            //    }
            //}

            //using (ISession session1 = sessionFactory.OpenSession())
            //{
            //    using (ISession session2 = sessionFactory.OpenSession())
            //    {
            //        using (ITransaction transaction1 = session1.BeginTransaction(System.Data.IsolationLevel.Snapshot))
            //        {
            //            using (ITransaction transaction2 = session2.BeginTransaction(System.Data.IsolationLevel.Snapshot))
            //            {
            //                Product p1 = session1.Get<Product>(id);
            //                p1.Name = "A";

            //                Product p2 = session1.Get<Product>(id);
            //                p2.Name = "B";

            //                session1.SaveOrUpdate(p1);
            //                session2.SaveOrUpdate(p2);

            //                try
            //                {
            //                    //session1.Flush();
            //                    //session2.Flush();

            //                    transaction1.Commit();
            //                    transaction2.Commit();
            //                }
            //                catch (Exception ex)
            //                {
            //                    Console.WriteLine();
            //                }
            //            }
            //        }
            //    }
            //}
        }

        [TestMethod]
        public void TestMedadata()
        {
            ICollection<NHibernate.Metadata.IClassMetadata> values = sessionFactory.GetAllClassMetadata().Values;
            foreach (NHibernate.Metadata.IClassMetadata m in values)
            {
                //Type type = m.GetMappedClass(EntityMode.Poco);
            }

        }

        [TestMethod]
        public void TestSimpleEntity()
        {

            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    //session.CreateCriteria<Product>();
                    //Expression.Like("", "a", MatchMode.Anywhere);

                    //DetachedCriteria.For<
                    //Projections.Property()

                    Product p = new Product();
                    p.Id = new EntityId(1, 1, DateTime.Now.Ticks);
                    p.Version = new EntityVersion(1);
                    p.Category = "category_" + Guid.NewGuid().ToString();
                    p.Name = "name_" + Guid.NewGuid().ToString();
                    p.Price = new decimal(102.23);
                    p.Deleted = true;
                    ORMUtils.SaveEntity(p, session);

                    User u = new User();
                    u.Id = new EntityId(1, 2, DateTime.Now.Ticks + 1);
                    u.Version = new EntityVersion(2);
                    u.Name = "JZO_" + Guid.NewGuid().ToString();
                    ORMUtils.SaveEntity(u, session);

                    Guest g = new Guest();
                    g.Id = new EntityId(1, 2, DateTime.Now.Ticks + 2);
                    g.Version = new EntityVersion(2);
                    g.Name = "JZO_" + Guid.NewGuid().ToString();
                    ORMUtils.SaveEntity(g, session);

                    transaction.Commit();
                }
            }
        }

        [TestMethod]
        public void TestOneToOne()
        {
            EntityId girlId = null;
            EntityId boyId = null;
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    Girl girl = new Girl();
                    girl.Id = new EntityId(1, 1, DateTime.Now.Ticks);
                    girl.Version = new EntityVersion(1);
                    girl.Name = "Viki_" + Guid.NewGuid().ToString();
                    girlId = girl.Id;
                    ORMUtils.SaveEntity(girl, session);

                    Boy boy = new Boy();
                    boy.Id = new EntityId(1, 1, DateTime.Now.Ticks + 1);
                    boy.Version = new EntityVersion(1);
                    boy.Name = "Zoli_" + Guid.NewGuid().ToString();
                    boyId = boy.Id;
                    ORMUtils.SaveEntity(boy, session);

                    girl.Boy = boy;
                    ORMUtils.SaveEntity(girl, session);

                    boy.Girl = girl;
                    ORMUtils.SaveEntity(boy, session);

                    transaction.Commit();
                }
            }
            using (ISession session = sessionFactory.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria<Girl>();
                criteria.Add(Expression.Eq("id", girlId));
                IList<Girl> girls = criteria.List<Girl>();
                Assert.IsTrue(girls.Count > 0);
            }
        }

        [TestMethod]
        public void TestOneToMany()
        {
            EntityId id = null;
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    OTMList otmList = new OTMList();
                    otmList.Id = new EntityId(1, 1, DateTime.Now.Ticks);
                    otmList.Version = new EntityVersion(1);
                    id = otmList.Id;

                    ORMUtils.SaveEntity(otmList, session);

                    for (int i = 1; i < 6; i++)
                    {
                        OTMItem item = new OTMItem();
                        item.Id = new EntityId(1, 2, DateTime.Now.Ticks + i);
                        item.Version = new EntityVersion(2);
                        item.OTMList = otmList;
                        otmList.OTMItems.Add(item);

                        ORMUtils.SaveEntity(item, session);

                    }

                    ORMUtils.SaveEntity(otmList, session);
                    transaction.Commit();
                }
            }

            using (ISession session = sessionFactory.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria<OTMList>();
                criteria.Add(Expression.Eq("id", id));
                IList<OTMList> resultList = criteria.List<OTMList>();
                Assert.IsTrue(resultList.Count > 0);
                if (resultList.Count > 0)
                {
                    OTMList list = resultList[0];
                    Assert.IsTrue(list.OTMItems.Count > 0);
                }
            }

        }

        [TestMethod]
        public void TestManyToOne()
        {
            EntityId id = null;
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    MTOList list = new MTOList();
                    list.Id = new EntityId(1, 1, DateTime.Now.Ticks);
                    list.Version = new EntityVersion(1);
                    id = list.Id;
                    ORMUtils.SaveEntity(list, session);

                    for (int i = 1; i < 6; i++)
                    {
                        MTOItem item = new MTOItem();
                        item.Id = new EntityId(1, 1, DateTime.Now.Ticks + i);
                        item.Version = new EntityVersion(1);
                        item.MTOList = list;
                        ORMUtils.SaveEntity(item, session);
                    }

                    transaction.Commit();
                }
            }

            using (ISession session = sessionFactory.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria<MTOItem>();
                criteria.Add(Expression.Eq("mtoList.id", id));
                IList<MTOItem> resultList = criteria.List<MTOItem>();
                Assert.IsTrue(resultList.Count > 0);
            }

        }

        [TestMethod]
        public void TestManyToManyBi()
        {
            List<EntityId> driverIds = new List<EntityId>();
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    List<Driver> drivers = new List<Driver>();
                    List<Car> cars = new List<Car>();

                    for (int i = 1; i < 6; i++)
                    {
                        Driver item = new Driver();
                        item.Id = new EntityId(1, 1, DateTime.Now.Ticks + i);
                        item.Version = new EntityVersion(1);
                        driverIds.Add(item.Id);
                        ORMUtils.SaveEntity(item, session);
                        drivers.Add(item);
                    }

                    for (int i = 6; i < 11; i++)
                    {
                        Car item = new Car();
                        item.Id = new EntityId(1, 1, DateTime.Now.Ticks + i);
                        item.Version = new EntityVersion(1);
                        ORMUtils.SaveEntity(item, session);
                        cars.Add(item);
                    }

                    foreach (Driver d in drivers)
                    {
                        foreach (Car c in cars)
                        {
                            d.Cars.Add(c);
                        }
                        ORMUtils.SaveEntity(d, session);
                    }
                    foreach (Car c in cars)
                    {
                        foreach (Driver d in drivers)
                        {
                            c.Drivers.Add(d);
                        }
                        ORMUtils.SaveEntity(c, session);
                    }

                    transaction.Commit();
                }
            }

            using (ISession session = sessionFactory.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria<Driver>();
                //criteria.Add(Expression.In("id", driverIds.ToArray()));
                IList<Driver> resultList = criteria.List<Driver>();
                Assert.IsTrue(resultList.Count > 0);
            }

        }

        [TestMethod]
        public void TestManyToManyUni()
        {
            List<EntityId> driverIds = new List<EntityId>();
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    List<Player> players = new List<Player>();
                    List<Game> games = new List<Game>();

                    for (int i = 1; i < 6; i++)
                    {
                        Player item = new Player();
                        item.Id = new EntityId(1, 1, DateTime.Now.Ticks + i);
                        item.Version = new EntityVersion(1);
                        driverIds.Add(item.Id);
                        ORMUtils.SaveEntity(item, session);
                        players.Add(item);
                    }

                    for (int i = 6; i < 11; i++)
                    {
                        Game item = new Game();
                        item.Id = new EntityId(1, 1, DateTime.Now.Ticks + i);
                        item.Version = new EntityVersion(1);
                        ORMUtils.SaveEntity(item, session);
                        games.Add(item);
                    }

                    foreach (Game c in games)
                    {
                        foreach (Player d in players)
                        {
                            c.Players.Add(d);
                        }
                        ORMUtils.SaveEntity(c, session);
                    }

                    transaction.Commit();
                }
            }

            using (ISession session = sessionFactory.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria<Game>();
                IList<Game> resultList = criteria.List<Game>();
                Assert.IsTrue(resultList.Count > 0);
            }

        }

        [TestMethod]
        public void CloneTest()
        {
            //List<EntityId> driverIds = new List<EntityId>();
            //using (ISession session = sessionFactory.OpenSession())
            //{
            //    using (ITransaction transaction = session.BeginTransaction())
            //    {
            //        List<Driver> drivers = new List<Driver>();
            //        List<Car> cars = new List<Car>();

            //        for (int i = 1; i < 6; i++)
            //        {
            //            Driver item = new Driver();
            //            item.Id = new EntityId(1, 1, DateTime.Now.Ticks + i);
            //            item.Version = new EntityVersion(1);
            //            driverIds.Add(item.Id);
            //            ORMUtils.SaveEntity(item, session);
            //            drivers.Add(item);
            //        }

            //        for (int i = 6; i < 11; i++)
            //        {
            //            Car item = new Car();
            //            item.Id = new EntityId(1, 1, DateTime.Now.Ticks + i);
            //            item.Version = new EntityVersion(1);
            //            ORMUtils.SaveEntity(item, session);
            //            cars.Add(item);
            //        }

            //        foreach (Driver d in drivers)
            //        {
            //            foreach (Car c in cars)
            //            {
            //                d.Cars.Add(c);
            //            }
            //            ORMUtils.SaveEntity(d, session);
            //        }
            //        foreach (Car c in cars)
            //        {
            //            foreach (Driver d in drivers)
            //            {
            //                c.Drivers.Add(d);
            //            }
            //            ORMUtils.SaveEntity(c, session);
            //        }

            //        transaction.Commit();
            //    }
            //}

            EntityClone entityClone = null;
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    ICriteria criteria = session.CreateCriteria<Driver>();
                    //criteria.Add(Expression.In("id", driverIds.ToArray()));

                    IList<Driver> resultList = criteria.List<Driver>();
                    Assert.IsTrue(resultList.Count > 0);
                    Driver d = resultList[0];

                    Driver cloned = (Driver)d.Clone();
                    cloned.Id = new EntityId(1, 1, DateTime.Now.Ticks + 100);
                    cloned.Version = new EntityVersion(1);
                    ORMUtils.SaveEntity(cloned, session);

                    entityClone = EntityBase.CreateEntityClone(cloned, session.GetSessionImplementation().PersistenceContext);

                    transaction.Commit();
                }
            }

            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    Driver driver = new Driver();
                    EntityBase.RestoreFromEntityClone(driver, entityClone, session);
                    object count = ExtractObjectData.Create("cars.m_count").GetValue(driver);
                }
            }
        }

        [TestMethod]
        public void QueryTest()
        {
            //List<EntityId> driverIds = new List<EntityId>();
            //using (ISession session = sessionFactory.OpenSession())
            //{
            //    using (ITransaction transaction = session.BeginTransaction())
            //    {
            //        List<Driver> drivers = new List<Driver>();
            //        List<Car> cars = new List<Car>();
            //        List<string> str = new List<string>() { "A", "B", "B", "B", "C", "C", "D" };

            //        for (int i = 1; i < 6; i++)
            //        {
            //            Driver item = new Driver();
            //            item.Id = new EntityId(1, 1, DateTime.Now.Ticks + i);
            //            item.Version = new EntityVersion(1);
            //            item.BinaryData = new byte[] { 0, 1, 2 };
            //            item.Name = str[i];
            //            driverIds.Add(item.Id);
            //            ORMUtils.SaveEntity(item, session);
            //            drivers.Add(item);
            //        }

            //        for (int i = 6; i < 11; i++)
            //        {
            //            Car item = new Car();
            //            item.Id = new EntityId(1, 1, DateTime.Now.Ticks + i);
            //            item.Version = new EntityVersion(1);
            //            ORMUtils.SaveEntity(item, session);
            //            cars.Add(item);
            //        }

            //        foreach (Driver d in drivers)
            //        {
            //            foreach (Car c in cars)
            //            {
            //                d.Cars.Add(c);
            //            }
            //            ORMUtils.SaveEntity(d, session);
            //        }
            //        foreach (Car c in cars)
            //        {
            //            foreach (Driver d in drivers)
            //            {
            //                c.Drivers.Add(d);
            //            }
            //            ORMUtils.SaveEntity(c, session);
            //        }

            //        transaction.Commit();
            //    }
            //}

            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    QueryParams<Driver> qp = new QueryParams<Driver>();
                    IList<Driver> resultList = QueryHelper.Query<Driver>(session, qp);

                    transaction.Rollback();
                }
            }

            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    QueryParams<Driver> qp = new QueryParams<Driver>(
                        new GroupCriteria(
                            new ArithmeticCriteria("name", "B", ArithmeticOperandEnum.NotEqual),
                            new ArithmeticCriteria("orderMode", OrderModeEnum.Asc)
                        )
                    );
                    qp.MaxResults = 2;
                    qp.OrderBys.Add(new OrderBy("id.id", OrderModeEnum.Desc));
                    IList<Driver> resultList = QueryHelper.Query<Driver>(session, qp);

                    Assert.IsTrue(qp.Criteria.ResultForEntity(resultList[0]));

                    qp = new QueryParams<Driver>(
                        new BetweenCriteria("name", "A", "D")
                    );
                    qp.MaxResults = 2;
                    qp.OrderBys.Add(new OrderBy("id.id", OrderModeEnum.Desc));
                    resultList = QueryHelper.Query<Driver>(session, qp);

                    Assert.IsTrue(qp.Criteria.ResultForEntity(resultList[0]));

                    qp = new QueryParams<Driver>(
                        new InCriteria("name", "A", "B", "C")
                    );
                    //qp.MaxResults = 2;
                    qp.OrderBys.Add(new OrderBy("id.id", OrderModeEnum.Desc));
                    resultList = QueryHelper.Query<Driver>(session, qp);

                    Assert.IsTrue(qp.Criteria.ResultForEntity(resultList[0]));

                    qp = new QueryParams<Driver>(
                        new InCriteria("name", false, "A", "B")
                    );
                    //qp.MaxResults = 2;
                    qp.OrderBys.Add(new OrderBy("id.id", OrderModeEnum.Desc));
                    resultList = QueryHelper.Query<Driver>(session, qp);

                    Assert.IsTrue(qp.Criteria.ResultForEntity(resultList[0]));

                    qp = new QueryParams<Driver>(
                        new NullCriteria("name", false)
                    );
                    //qp.MaxResults = 2;
                    qp.OrderBys.Add(new OrderBy("id.id", OrderModeEnum.Desc));
                    resultList = QueryHelper.Query<Driver>(session, qp);

                    Assert.IsTrue(qp.Criteria.ResultForEntity(resultList[0]));

                    qp = new QueryParams<Driver>(
                        new LikeCriteria("name", "B", LikeMatchModeEnum.Anywhere)
                    );
                    //qp.MaxResults = 2;
                    qp.OrderBys.Add(new OrderBy("id.id", OrderModeEnum.Desc));
                    resultList = QueryHelper.Query<Driver>(session, qp);

                    Assert.IsTrue(qp.Criteria.ResultForEntity(resultList[0]));

                    transaction.Rollback();
                }
            }

        }

        [TestMethod]
        public void ComplexQueryTest()
        {
            //using (ISession session = sessionFactory.OpenSession())
            //{
            //    using (ITransaction transaction = session.BeginTransaction())
            //    {
            //        try
            //        {
            //            CreateSystemEnumPaymentModes(session);
            //            CreateSystemEnumConsumerModes(session);
            //        }
            //        catch (Exception) { }
            //        transaction.Commit();
            //    }
            //}

            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    QueryParams<EnumeratorItem> qp = null;
                    Criteria c = null;
                    IList<EnumeratorItem> resultItem1 = null;

                    c = new GroupCriteria(
                            new ArithmeticCriteria("value", 0L),
                            new ArithmeticCriteria("id.systemId", 1L),
                            new ArithmeticCriteria("enumeratorType.guest.id.systemId", 1L),
                            new ArithmeticCriteria("enumeratorType.guest.name", "G1"),
                            new ArithmeticCriteria("enumeratorType.guest.name", "G1"),
                            new ArithmeticCriteria("enumeratorType.systemEnumeratorType", SystemEnumeratorTypeEnum.PaymentModes));

                    qp = new QueryParams<EnumeratorItem>(c);
                    resultItem1 = QueryHelper.Query<EnumeratorItem>(session, qp);

                    Console.WriteLine();

                    c = new GroupCriteria(GroupCriteriaLogicEnum.Or,
                        new GroupCriteria(
                            new ArithmeticCriteria("value", 0L),
                            new ArithmeticCriteria("id.systemId", 1L),
                            new ArithmeticCriteria("enumeratorType.guest.id.systemId", 1L),
                            new ArithmeticCriteria("enumeratorType.guest.name", "G1"),
                            new ArithmeticCriteria("enumeratorType.guest.name", "G1"),
                            new ArithmeticCriteria("enumeratorType.systemEnumeratorType", SystemEnumeratorTypeEnum.PaymentModes)),
                        new GroupCriteria(
                            new ArithmeticCriteria("value", 0L),
                            new ArithmeticCriteria("id.systemId", 1L),
                            new ArithmeticCriteria("enumeratorType.guest.id.systemId", 1L),
                            new ArithmeticCriteria("enumeratorType.guest.name", "G1"),
                            new ArithmeticCriteria("enumeratorType.guest.name", "G1"),
                            new ArithmeticCriteria("enumeratorType.systemEnumeratorType", SystemEnumeratorTypeEnum.PaymentModes))
                            );

                    qp = new QueryParams<EnumeratorItem>(c);
                    IList<EnumeratorItem> resultItem2 = QueryHelper.Query<EnumeratorItem>(session, qp);

                    DetachedCriteria dc = DetachedCriteria.For<EnumeratorItem>("e");
                    dc.Add(Expression.Eq("value", 0L));

                    //DetachedCriteria dc1 = dc.CreateCriteria("enumeratorType", "t", JoinType.InnerJoin, Expression.Eq("systemEnumeratorType", SystemEnumeratorTypeEnum.PaymentModes));
                    DetachedCriteria dc1 = dc.CreateCriteria("enumeratorType", "t", JoinType.InnerJoin);
                    DetachedCriteria dc2 = dc.CreateCriteria("t.guest", "g", JoinType.InnerJoin);
                    DetachedCriteria dc3 = dc.Add(Expression.Eq("g.name", "G1"));
                    DetachedCriteria dc4 = dc.Add(Expression.Eq("g.name", "G1"));

                    ICriteria criteria = dc.GetExecutableCriteria(session);

                    criteria.List();

                    transaction.Commit();
                }
            }

        }

        #endregion

        #region Private method(s)

        private void CreateSystemEnumPaymentModes(ISession session)
        {
            // create payment modes
            Stopwatch sw = Stopwatch.StartNew();

            Guest g = new Guest();
            g.Id = new EntityId(1, 2, 1);
            g.Version = new EntityVersion(1);
            g.Name = "G1";
            ORMUtils.SaveEntity(g, session);

            EnumeratorType paymentMode = new EnumeratorType();
            paymentMode.Id = new EntityId(1, 1, 1);
            paymentMode.Version = new EntityVersion(1);
            paymentMode.SystemEnumeratorType = SystemEnumeratorTypeEnum.PaymentModes;
            paymentMode.Guest = g;
            ORMUtils.SaveEntity(paymentMode, session);

            // Payment realized with cash
            EnumeratorItem item = new EnumeratorItem();
            item.Id = new EntityId(1, 1, sw.ElapsedMilliseconds);
            item.Version = new EntityVersion(1);
            item.Name = "Cash";
            item.Value = 0;
            item.EnumeratorType = paymentMode;
            ORMUtils.SaveEntity(item, session);
            ISet<EnumeratorItem> items = paymentMode.Items;
            items.Add(item);
            paymentMode.Items = items;

            // Payment realized with credit card
            item = new EnumeratorItem();
            item.Id = new EntityId(1, 1, sw.ElapsedMilliseconds);
            item.Version = new EntityVersion(1);
            item.Name = "CreditCard";
            item.Value = 1;
            item.EnumeratorType = paymentMode;
            ORMUtils.SaveEntity(item, session);
            items = paymentMode.Items;
            items.Add(item);
            paymentMode.Items = items;

            // Payment realized with a bank transfer
            item = new EnumeratorItem();
            item.Id = new EntityId(1, 1, sw.ElapsedMilliseconds);
            item.Version = new EntityVersion(1);
            item.Name = "Transfer";
            item.Value = 2;
            item.EnumeratorType = paymentMode;
            ORMUtils.SaveEntity(item, session);
            items = paymentMode.Items;
            items.Add(item);
            paymentMode.Items = items;

            // Payment realized with a check
            item = new EnumeratorItem();
            item.Id = new EntityId(1, 1, sw.ElapsedMilliseconds);
            item.Version = new EntityVersion(1);
            item.Name = "Check";
            item.Value = 3;
            item.EnumeratorType = paymentMode;
            ORMUtils.SaveEntity(item, session);
            items = paymentMode.Items;
            items.Add(item);
            paymentMode.Items = items;

            // Payment realized with a voucher
            item = new EnumeratorItem();
            item.Id = new EntityId(1, 1, sw.ElapsedMilliseconds);
            item.Version = new EntityVersion(1);
            item.Name = "Voucher";
            item.Value = 4;
            item.EnumeratorType = paymentMode;
            ORMUtils.SaveEntity(item, session);
            items = paymentMode.Items;
            items.Add(item);
            paymentMode.Items = items;

            ORMUtils.SaveEntity(paymentMode, session);
        }

        private void CreateSystemEnumConsumerModes(ISession session)
        {
            // create consumer modes
            Stopwatch sw = Stopwatch.StartNew();

            Guest g = new Guest();
            g.Id = new EntityId(1, 3, 1);
            g.Version = new EntityVersion(1);
            g.Name = "G2";
            ORMUtils.SaveEntity(g, session);

            EnumeratorType consumerMode = new EnumeratorType();
            consumerMode.Id = new EntityId(1, 2, 2);
            consumerMode.Version = new EntityVersion(1);
            consumerMode.SystemEnumeratorType = SystemEnumeratorTypeEnum.ConsumerTypeModes;
            consumerMode.Guest = g;
            ORMUtils.SaveEntity(consumerMode, session);

            // Represents the type of the consumer is not definied
            EnumeratorItem item = new EnumeratorItem();
            item.Id = new EntityId(1, 2, sw.ElapsedMilliseconds);
            item.Version = new EntityVersion(1);
            item.Name = "NotDefinied";
            item.Value = 0;
            item.EnumeratorType = consumerMode;
            ORMUtils.SaveEntity(item, session);
            ISet<EnumeratorItem> items = consumerMode.Items;
            items.Add(item);
            consumerMode.Items = items;

            // Represents that the guest walked in the restaurant from the street, ad-hoc
            item = new EnumeratorItem();
            item.Id = new EntityId(1, 2, sw.ElapsedMilliseconds);
            item.Version = new EntityVersion(1);
            item.Name = "WalkIn";
            item.Value = 1;
            item.EnumeratorType = consumerMode;
            ORMUtils.SaveEntity(item, session);
            items = consumerMode.Items;
            items.Add(item);
            consumerMode.Items = items;

            // Represents that the guest walked in the restaurant from the street, ad-hoc
            item = new EnumeratorItem();
            item.Id = new EntityId(1, 2, sw.ElapsedMilliseconds);
            item.Version = new EntityVersion(1);
            item.Name = "Reservation";
            item.Value = 2;
            item.EnumeratorType = consumerMode;
            ORMUtils.SaveEntity(item, session);
            items = consumerMode.Items;
            items.Add(item);
            consumerMode.Items = items;

            // Represents that the consumption belongs to an event (Rendezvény)
            item = new EnumeratorItem();
            item.Id = new EntityId(1, 2, sw.ElapsedMilliseconds);
            item.Version = new EntityVersion(1);
            item.Name = "Event";
            item.Value = 3;
            item.EnumeratorType = consumerMode;
            ORMUtils.SaveEntity(item, session);
            items = consumerMode.Items;
            items.Add(item);
            consumerMode.Items = items;

            ORMUtils.SaveEntity(consumerMode, session);
        }

        #endregion

    }
}
