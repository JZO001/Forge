using System.Collections.Generic;
using Forge.Configuration.Shared;
using Forge.DatabaseManagement.SqlServer2008;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing.DBManagersTest
{

    /// <summary>
    /// Test the MS Sql 2008 Manager
    /// </summary>
    [TestClass]
    public class Sql2008ManagerTest
    {

        /// <summary>
        /// Tests the manager.
        /// </summary>
        [TestMethod]
        public void TestInitManager()
        {
            CategoryPropertyItem item = new CategoryPropertyItem();
            item.Id = "DatabaseManager";
            item.EntryValue = "Forge.DatabaseManagement.SqlServer2008.MsSql2008Manager, Forge.DatabaseManagement.SqlServer2008";
            CategoryPropertyItem connectionString = new CategoryPropertyItem();
            connectionString.Id = "ConnectionStringForAdministration";
            connectionString.EntryValue = @"Data Source=.\SQLEXPRESS2008; Integrated Security=SSPI";
            item.PropertyItems.Add(connectionString);

            MsSql2008Manager manager = new MsSql2008Manager();
            manager.Initialize(item);

            Dictionary<string, string> descrition = new Dictionary<string,string>();
            descrition.Add("connection.connection_string", @"Data Source=.\SQLEXPRESS2014; Initial Catalog=TestDB; User Id=TestUser; Password=idsonwhite36743");
            descrition.Add("sqlserver.collation", "Hungarian_CI_AI");
            manager.EnsureDatabaseIntegrity(10, descrition, DatabaseManagement.SchemaFactoryModeEnum.Create);

        }

    }

}
