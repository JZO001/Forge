/* *********************************************************************
 * Date: 05 Aug 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlServerCe;
using System.IO;
using Forge.Configuration.Shared;

namespace Forge.DatabaseManagement.SqlServerCe40
{

    /// <summary>
    /// Represents a database manager which ensure the underlying database integrity is valid.
    /// </summary>
    [Serializable]
    public class SqlServerCe40Manager : IDatabaseManager
    {

        #region Field(s)

        /// <summary>
        /// Dialec key
        /// </summary>
        protected static readonly string DIALECT = "dialect";

        /// <summary>
        /// Dialect builtin value
        /// </summary>
        protected static readonly string DIALECT_EXPECTED_VALUE_BUILTIN = "NHibernate.Dialect.MsSqlCe40Dialect";

        /// <summary>
        /// Dialect custom value
        /// </summary>
        protected static readonly string DIALECT_EXPECTED_VALUE_CUSTOM = "Forge.DatabaseManagement.SqlServerCe40.SqlServerCe40Dialect";

        /// <summary>
        /// Connection driver key
        /// </summary>
        protected static readonly string CONNECTION_DRIVER = "connection.driver_class";

        /// <summary>
        /// Connection driver value
        /// </summary>
        protected static readonly string CONNECTION_DRIVER_EXPECTED_VALUE = "NHibernate.Driver.SqlServerCeDriver";

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
        /// Initializes a new instance of the <see cref="SqlServerCe40Manager"/> class.
        /// </summary>
        public SqlServerCe40Manager()
        {
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="SqlServerCe40Manager"/> is reclaimed by garbage collection.
        /// </summary>
        ~SqlServerCe40Manager()
        {
            Dispose(false);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
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
        public virtual void Initialize(CategoryPropertyItem configItem)
        {
            this.IsInitialized = true;
        }

        /// <summary>
        /// Ensures the database integrity.
        /// </summary>
        /// <param name="systemId">The system id.</param>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="mode">The mode.</param>
        /// <exception cref="UnexpectedNHibernateConfigurationException">
        /// Unexpected dialect.
        /// or
        /// Unexpected connection driver.
        /// </exception>
        /// <exception cref="InvalidConfigurationException">
        /// Unable to find connection information in NHibernate Descriptor settings.
        /// or
        /// Unable to find data source in connection string.
        /// </exception>
        /// <exception cref="DatabaseVerificationErrorException"></exception>
        public virtual void EnsureDatabaseIntegrity(long systemId, Dictionary<string, string> descriptor, SchemaFactoryModeEnum mode)
        {
            if (descriptor == null)
            {
                ThrowHelper.ThrowArgumentNullException("descriptor");
            }

            if (descriptor.ContainsKey(DIALECT))
            {
                string dialect = descriptor[DIALECT];
                if (!dialect.StartsWith(DIALECT_EXPECTED_VALUE_BUILTIN) && !dialect.StartsWith(DIALECT_EXPECTED_VALUE_CUSTOM))
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
                throw new InvalidConfigurationException("Unable to find connection information in NHibernate Descriptor settings.");
            }

            if (string.IsNullOrEmpty(databaseFile))
            {
                throw new InvalidConfigurationException("Unable to find data source in connection string.");
            }

            using (SqlCeEngine en = new SqlCeEngine(connectionString))
            {
                FileInfo dbFileInfo = new FileInfo(databaseFile);
                if (!dbFileInfo.Exists)
                {
                    en.CreateDatabase();
                }
                else
                {
                    if (mode == SchemaFactoryModeEnum.Create || mode == SchemaFactoryModeEnum.Create_And_Drop)
                    {
                        dbFileInfo.Delete();
                        en.CreateDatabase();
                    }
                    else
                    {
                        if (!en.Verify(VerifyOption.Enhanced))
                        {
                            throw new DatabaseVerificationErrorException();
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
            Dispose(true);
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
