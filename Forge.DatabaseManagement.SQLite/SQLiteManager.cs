/* *********************************************************************
 * Date: 08 Aug 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using Forge.Configuration;
using Forge.Configuration.Shared;
using Forge.Shared;

namespace Forge.DatabaseManagement.SQLite
{

    /// <summary>
    /// Represents a database manager which ensure the underlying database integrity is valid.
    /// </summary>
    [Serializable]
    public class SQLiteManager : IDatabaseManager
    {

        #region Field(s)

        /// <summary>
        /// Dialect key
        /// </summary>
        protected static readonly string DIALECT = "dialect";

        /// <summary>
        /// Dialect value
        /// </summary>
        protected static readonly string DIALECT_EXPECTED_VALUE = "NHibernate.Dialect.SQLiteDialect";

        /// <summary>
        /// Driver class key
        /// </summary>
        protected static readonly string CONNECTION_DRIVER = "connection.driver_class";

        /// <summary>
        /// Driver class value
        /// </summary>
        protected static readonly string CONNECTION_DRIVER_EXPECTED_VALUE = "NHibernate.Driver.SQLite20Driver";

        /// <summary>
        /// Connection string key
        /// </summary>
        protected static readonly string CONNECTION_STRING = "connection.connection_string";

        /// <summary>
        /// Connection string name key
        /// </summary>
        protected static readonly string CONNECTION_STRING_NAME = "connection.connection_string_name";

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteManager"/> class.
        /// </summary>
        public SQLiteManager()
        {
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="SQLiteManager"/> is reclaimed by garbage collection.
        /// </summary>
        ~SQLiteManager()
        {
            Dispose(false);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized
        {
            get;
            protected set;
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes the specified config item.
        /// </summary>
        /// <param name="configItem">The config item.</param>
        public virtual void Initialize(IPropertyItem configItem)
        {
            IsInitialized = true;
        }

        /// <summary>
        /// Ensures the database integrity.
        /// </summary>
        /// <param name="systemId">The system id.</param>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="mode">The mode.</param>
        /// <exception cref="UnexpectedNHibernateConfigurationException">Unexpected dialect.
        /// or
        /// Unexpected connection driver.</exception>
        /// <exception cref="InvalidConfigurationException">
        /// Unable to connection information in NHibernate Descriptor settings.
        /// or
        /// Unable to find data source in connection string.
        /// </exception>
        /// <exception cref="Forge.Configuration.Shared.InvalidConfigurationException">Unable to connection information in NHibernate Descriptor settings.
        /// or
        /// Unable to find data source in connection string.</exception>
        public virtual void EnsureDatabaseIntegrity(long systemId, Dictionary<string, string> descriptor, SchemaFactoryModeEnum mode)
        {
            if (descriptor == null)
            {
                ThrowHelper.ThrowArgumentNullException("descriptor");
            }

            if (descriptor.ContainsKey(DIALECT))
            {
                if (!DIALECT_EXPECTED_VALUE.Equals(descriptor[DIALECT]))
                {
                    throw new UnexpectedNHibernateConfigurationException("Unexpected dialect.");
                }
            }

            if (descriptor.ContainsKey(CONNECTION_DRIVER))
            {
                if (!CONNECTION_DRIVER_EXPECTED_VALUE.Equals(descriptor[CONNECTION_DRIVER]))
                {
                    throw new UnexpectedNHibernateConfigurationException("Unexpected connection driver.");
                }
            }

            string databaseFile = string.Empty;
            string connectionString = string.Empty;

            if (descriptor.ContainsKey(CONNECTION_STRING))
            {
                connectionString = descriptor[CONNECTION_STRING];
                databaseFile = GetDatabaseFile(connectionString);
            }
            else if (descriptor.ContainsKey(CONNECTION_STRING_NAME))
            {
                connectionString = ConfigurationManager.ConnectionStrings[descriptor[CONNECTION_STRING_NAME]].ConnectionString;
                databaseFile = GetDatabaseFile(connectionString);
            }
            else
            {
                throw new InvalidConfigurationException("Unable to connection information in NHibernate Descriptor settings.");
            }

            if (string.IsNullOrEmpty(databaseFile))
            {
                throw new InvalidConfigurationException("Unable to find data source in connection string.");
            }

            using (SQLiteFactory factory = new SQLiteFactory())
            {
                FileInfo dbFileInfo = new FileInfo(databaseFile);
                if (!dbFileInfo.Exists)
                {
                    using (DbConnection connection = factory.CreateConnection())
                    {
                        connection.ConnectionString = connectionString;
                        connection.Open();
                    }
                }
                else
                {
                    if (mode == SchemaFactoryModeEnum.Create || mode == SchemaFactoryModeEnum.Create_And_Drop)
                    {
                        dbFileInfo.Delete();
                        using (DbConnection connection = factory.CreateConnection())
                        {
                            connection.ConnectionString = connectionString;
                            connection.Open();
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Gets the database file.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        protected string GetDatabaseFile(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidConfigurationException("Unable to connection information in NHibernate Descriptor settings.");
            }

            string result = string.Empty;

            string[] configItems = connectionString.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in configItems)
            {
                string[] keyValue = item.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                string key = keyValue[0].Trim().ToLower();
                if (key.Equals("data source"))
                {
                    result = keyValue[1].Trim();
                }
            }

            return result;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="dispose"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool dispose)
        {
        }

        #endregion

    }

}
