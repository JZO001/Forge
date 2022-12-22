/* *********************************************************************
 * Date: 05 Aug 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Forge.Configuration;
using Forge.Configuration.Shared;
using Forge.DatabaseManagement.SqlServer2008.Properties;
using Forge.Logging.Abstraction;
using Forge.Shared;

namespace Forge.DatabaseManagement.SqlServer2008
{

    /// <summary>
    /// Represents a database manager which ensure the underlying database integrity is valid.
    /// </summary>
    [Serializable]
    public class MsSql2008Manager : IDatabaseManager
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger<MsSql2008Manager>();

        private const string CONNECTION_STRING_CONFIG_ID = "ConnectionStringForAdministration";

        private string mConnectionStringForAdmin = string.Empty;

        /// <summary>
        /// Connection string key
        /// </summary>
        protected const string CONNECTION_STRING = "connection.connection_string";

        /// <summary>
        /// Connection string name key
        /// </summary>
        protected const string CONNECTION_STRING_NAME = "connection.connection_string_name";

        /// <summary>
        /// The user id
        /// </summary>
        protected const string USER_ID = "user id";

        /// <summary>
        /// The initial catalog
        /// </summary>
        protected const string INITIAL_CATALOG = "initial catalog";

        /// <summary>
        /// The password
        /// </summary>
        protected const string PASSWORD = "password";

        /// <summary>
        /// The server
        /// </summary>
        protected const string SERVER = "server";

        /// <summary>
        /// The database
        /// </summary>
        protected const string DATABASE = "database";

        /// <summary>
        /// The database collation
        /// </summary>
        protected const string DATABASE_COLLATION = "sqlserver.collation";

        private int mDefaultCommandTimeout = 60;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="MsSql2008Manager" /> class.
        /// </summary>
        public MsSql2008Manager()
        {
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MsSql2008Manager" /> class.
        /// </summary>
        ~MsSql2008Manager()
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

        /// <summary>
        /// Gets or sets the default command timeout.
        /// </summary>
        /// <value>
        /// The default command timeout.
        /// </value>
        public int DefaultCommandTimeout
        {
            get { return mDefaultCommandTimeout; }
            set
            {
                if (value < 0)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }
                mDefaultCommandTimeout = value;
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes the specified config item.
        /// </summary>
        /// <param name="configItem">The config item.</param>
        /// <exception cref="InitializationException">Administration connection string was not set.</exception>
        /// <exception cref="Forge.Shared.InitializationException">Administration connection string was not set.</exception>
        public void Initialize(IPropertyItem configItem)
        {
            if (configItem == null)
            {
                ThrowHelper.ThrowArgumentNullException("configItem");
            }

            mConnectionStringForAdmin = ConfigurationAccessHelper.GetValueByPath(configItem, CONNECTION_STRING_CONFIG_ID);

            if (string.IsNullOrEmpty(mConnectionStringForAdmin))
            {
                throw new InitializationException("Administration connection string was not set.");
            }

            IsInitialized = true;
        }

        /// <summary>
        /// Ensures the database integrity.
        /// </summary>
        /// <param name="systemId">The system id.</param>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="mode">The mode.</param>
        /// <exception cref="InitializationException">MsSql2008 Manager has not been initialized.</exception>
        /// <exception cref="InvalidConfigurationException">Unable to connection information in NHibernate Descriptor settings.</exception>
        /// <exception cref="InvalidConfigurationValueException">Connection string is empty.
        /// or
        /// Connection string does not contains DATABASE nor DATA SOURCE definition.
        /// or
        /// Connection string does not contains PASSWORD definition.
        /// or
        /// Connection string does not contains information about the database name.</exception>
        public void EnsureDatabaseIntegrity(long systemId, Dictionary<string, string> descriptor, SchemaFactoryModeEnum mode)
        {
            if (!this.IsInitialized)
            {
                throw new InitializationException("MsSql2008 Manager has not been initialized.");
            }
            if (descriptor == null)
            {
                ThrowHelper.ThrowArgumentNullException("descriptor");
            }

            string connectionString = string.Empty;

            if (descriptor.ContainsKey(CONNECTION_STRING))
            {
                connectionString = descriptor[CONNECTION_STRING];
            }
            else if (descriptor.ContainsKey(CONNECTION_STRING_NAME))
            {
                connectionString = ConfigurationManager.ConnectionStrings[descriptor[CONNECTION_STRING_NAME]].ConnectionString;
            }
            else
            {
                throw new InvalidConfigurationException("Unable to connection information in NHibernate Descriptor settings.");
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidConfigurationValueException("Connection string is empty.");
            }

            Dictionary<string, string> data = ParseConnectionString(connectionString);
            if (!data.ContainsKey(INITIAL_CATALOG) && !data.ContainsKey(DATABASE))
            {
                throw new InvalidConfigurationValueException("Connection string does not contains DATABASE nor DATA SOURCE definition.");
            }

            string collation = string.Empty;
            string database = string.Empty;
            string password = string.Empty;
            string userId = string.Empty;
            if (data.ContainsKey(USER_ID))
            {
                userId = data[USER_ID];
                if (data.ContainsKey(PASSWORD))
                {
                    password = data[PASSWORD];
                }
                else
                {
                    throw new InvalidConfigurationValueException("Connection string does not contains PASSWORD definition.");
                }
            }
            else
            {
                // add current windows principal
                userId = string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName);
            }

            if (data.ContainsKey(DATABASE))
            {
                database = data[DATABASE];
            }
            else if (data.ContainsKey(INITIAL_CATALOG))
            {
                database = data[INITIAL_CATALOG];
            }
            else
            {
                throw new InvalidConfigurationValueException("Connection string does not contains information about the database name.");
            }

            if (descriptor.ContainsKey(DATABASE_COLLATION))
            {
                collation = descriptor[DATABASE_COLLATION];
            }

            using (SqlConnection connection = new SqlConnection(mConnectionStringForAdmin))
            {
                connection.Open();
                SqlCommand command = null;

                bool isExist = false;

                if (!string.IsNullOrEmpty(password))
                {
                    using (command = new SqlCommand(Resources.CheckLogin, connection))
                    {
                        command.CommandTimeout = mDefaultCommandTimeout;
                        command.Parameters.Add(new SqlParameter("@P0", System.Data.SqlDbType.VarChar) { Value = userId });
                        isExist = (int)command.ExecuteScalar() > 0;
                    }

                    if (!isExist)
                    {
                        // CREATE LOGIN {0} WITH PASSWORD = '{1}', CHECK_EXPIRATION = OFF, CHECK_POLICY = OFF;
                        using (command = new SqlCommand(string.Format(Resources.CreateLogin, userId, password), connection))
                        {
                            command.CommandTimeout = mDefaultCommandTimeout;
                            command.ExecuteNonQuery();
                        }
                    }
                }

                // SELECT COUNT(*) FROM master.dbo.sysdatabases db WHERE db.name = @P0
                using (command = new SqlCommand(string.Format(Resources.DatabaseCheck, database), connection))
                {
                    command.CommandTimeout = mDefaultCommandTimeout;
                    command.Parameters.Add(new SqlParameter("@P0", System.Data.SqlDbType.VarChar) { Value = database });
                    isExist = (int)command.ExecuteScalar() > 0;
                }

                if (!isExist)
                {
                    // CREATE DATABASE {0} [COLLATE collation_name]
                    string commandText = string.IsNullOrEmpty(collation) ? string.Format(Resources.CreateDatabase, database) : (string.Format("{0} COLLATE {1}", string.Format(Resources.CreateDatabase, database), collation));
                    using (command = new SqlCommand(commandText, connection))
                    {
                        command.CommandTimeout = mDefaultCommandTimeout;
                        command.ExecuteNonQuery();
                    }
                }
                else if (!string.IsNullOrEmpty(collation))
                {
                    // SELECT DATABASEPROPERTYEX('{0}', 'Collation') SQLCollation
                    bool isEqual = false;
                    using (command = new SqlCommand(string.Format(Resources.GetDatabaseCollation, database), connection))
                    {
                        command.CommandTimeout = mDefaultCommandTimeout;
                        isEqual = collation.Equals(command.ExecuteScalar());
                    }
                    if (!isEqual)
                    {
                        // ALTER DATABASE [{0}] COLLATE {1}
                        using (command = new SqlCommand(string.Format(Resources.AlterDatabaseCollation, database, collation), connection))
                        {
                            try
                            {
                                LOGGER.Info(string.Format("MSSQL2008MANAGER, trying to alter database '{0}' collation. If this transaction freeze, please other services and transaction which are possible keep lock on this database object.", database));
                                command.CommandTimeout = mDefaultCommandTimeout;
                                command.ExecuteNonQuery();
                                LOGGER.Info(string.Format("MSSQL2008MANAGER, database '{0}' collation successfully modified.", database));
                            }
                            catch (Exception ex)
                            {
                                LOGGER.Error(string.Format("MSSQL2008MANAGER, failed to alter database '{0}' collation to '{1}'.", database, collation), ex);
                            }
                        }
                    }
                }

                // ALTER AUTHORIZATION ON DATABASE::{0} TO [{1}]
                using (command = new SqlCommand(string.Format(Resources.SetDatabaseOwner, database, userId), connection))
                {
                    command.CommandTimeout = mDefaultCommandTimeout;
                    command.ExecuteNonQuery();
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
        /// Parses the connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        protected Dictionary<string, string> ParseConnectionString(string connectionString)
        {
            // Data Source=yourSQLServer; Initial Catalog=yourDB; User Id=yourUserName; Password=yourPwd;
            // Server=yourServer;Database=yourDB;User ID=yourUserName;Password=yourPwd;Trusted_Connection=False;
            // Data Source=yourSQLServer; Initial Catalog=yourDB; Integrated Security=SSPI;
            // Server=yourSQLServer; Database=yourDB; Trusted_Connection=True;

            Dictionary<string, string> result = new Dictionary<string, string>();

            string[] keyValuePairs = connectionString.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string kv in keyValuePairs)
            {
                string[] pair = kv.Trim().Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (pair.Length > 1)
                {
                    string key = pair[0].Trim().ToLower();
                    switch (key)
                    {
                        case USER_ID:
                        case PASSWORD:
                        case INITIAL_CATALOG:
                        case SERVER:
                        case DATABASE:
                            result[key] = pair[1];
                            break;
                    }
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
