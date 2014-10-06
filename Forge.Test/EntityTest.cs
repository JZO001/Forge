using System;
using Forge.Collections;
using Forge.ORM.NHibernateExtension;
using Forge.ORM.NHibernateExtension.Criterias;
using Forge.Test.EntitiesIntNative;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Mapping.Attributes;
using NHibernate.Tool.hbm2ddl;

namespace Forge.Test
{

    [TestClass]
    public class EntityTest
    {

        private static ISessionFactory mSessionFactory = null;

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            // define mapping schema
            HbmSerializer.Default.HbmAssembly = typeof(Product).Assembly.GetName().FullName;
            //HbmSerializer.Default.HbmNamespace = typeof( Product ).Namespace;
            HbmSerializer.Default.HbmAutoImport = true;
            HbmSerializer.Default.Validate = true;
            HbmSerializer.Default.WriteDateComment = false;
            HbmSerializer.Default.HbmDefaultAccess = "field";
            //HbmSerializer.Default.Serialize(typeof(Product).Assembly, "output.hbm.xml"); // serialize mapping xml into file to spectate it

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
            mSessionFactory = cfg.BuildSessionFactory();

        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            if (mSessionFactory != null)
            {
                mSessionFactory.Dispose();
            }
        }

        [TestMethod]
        public void TestEntitySave()
        {
            using (ISession session = mSessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    // create products if they are not exist...
                    Product apple = null;
                    Product pear = null;
                    Product peach = null;

                    IListSpecialized<Product> productList = QueryHelper.Query<Product>(session, new QueryParams<Product>(new ArithmeticCriteria("name", "Apple"), 1));
                    if (productList.Count > 0)
                    {
                        apple = productList[0];
                    }
                    else
                    {
                        apple = new Product();
                        //tp.Id = 1;
                        apple.Name = "Apple";
                        ORMUtils.SaveEntity(apple, session);
                    }

                    productList = QueryHelper.Query<Product>(session, new QueryParams<Product>(new ArithmeticCriteria("name", "Pear"), 1));
                    if (productList.Count > 0)
                    {
                        pear = productList[0];
                    }
                    else
                    {
                        pear = new Product();
                        //tp.Id = 1;
                        pear.Name = "Pear";
                        ORMUtils.SaveEntity(pear, session);
                    }

                    productList = QueryHelper.Query<Product>(session, new QueryParams<Product>(new ArithmeticCriteria("name", "Peach"), 1));
                    if (productList.Count > 0)
                    {
                        peach = productList[0];
                    }
                    else
                    {
                        peach = new Product();
                        //tp.Id = 1;
                        peach.Name = "Peach";
                        ORMUtils.SaveEntity(peach, session);
                    }

                    Customer customer1 = null;
                    Customer customer2 = null;

                    IListSpecialized<Customer> customerList = QueryHelper.Query<Customer>(session, new QueryParams<Customer>(new ArithmeticCriteria("name", "David"), 1));
                    if (customerList.Count > 0)
                    {
                        customer1 = customerList[0];
                    }
                    else
                    {
                        customer1 = new Customer();
                        //tp.Id = 1;
                        customer1.Name = "David";
                        ORMUtils.SaveEntity(customer1, session);
                    }

                    customerList = QueryHelper.Query<Customer>(session, new QueryParams<Customer>(new ArithmeticCriteria("name", "Thomas"), 1));
                    if (customerList.Count > 0)
                    {
                        customer2 = customerList[0];
                    }
                    else
                    {
                        customer2 = new Customer();
                        //tp.Id = 1;
                        customer2.Name = "Thomas";
                        ORMUtils.SaveEntity(customer2, session);
                    }

                    // create a shopping cart
                    ShoppingCart cart1 = new ShoppingCart();
                    cart1.Customer = customer1;
                    cart1.Products.Add(apple);
                    cart1.Products.Add(pear);
                    ORMUtils.SaveEntity(cart1, session);

                    ShoppingCart cart2 = new ShoppingCart();
                    cart2.Customer = customer2;
                    cart2.Products.Add(apple);
                    cart2.Products.Add(peach);
                    ORMUtils.SaveEntity(cart2, session);

                    transaction.Commit();
                }
            }
        }

    }

}
