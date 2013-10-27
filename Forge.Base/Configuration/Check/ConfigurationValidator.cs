/* *********************************************************************
 * Date: 20 Feb 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Configuration;
using System.Diagnostics;
using log4net;

namespace Forge.Configuration.Check
{

    /// <summary>
    /// Configuration validator which validates the application xml configuration.
    /// </summary>
    public static class ConfigurationValidator
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(ConfigurationValidator));

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static String mLog = "Application";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static String mEventLogSource = String.Empty;

        #endregion

        #region Public properties

        /// <summary>
        /// Get or set the Log where the EventLogEntry will be written
        /// Default is the "Application"
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        [DebuggerHidden]
        public static String Log
        {
            get { return ConfigurationValidator.mLog; }
            set { ConfigurationValidator.mLog = value; }
        }

        /// <summary>
        /// Get or set the EventLog Source name in the Application
        /// Do not specify existing Source name
        /// </summary>
        /// <value>
        /// The event log source.
        /// </value>
        [DebuggerHidden]
        public static String EventLogSource
        {
            get { return ConfigurationValidator.mEventLogSource; }
            set { ConfigurationValidator.mEventLogSource = value; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Create the EventLog log if it does not exist
        /// </summary>
        [DebuggerStepThrough]
        public static void CreateEventLog()
        {
            try
            {
                //DnsPermission dnsPerm = new DnsPermission( System.Security.Permissions.PermissionState.Unrestricted );
                //dnsPerm.Demand( );
                //EventLogPermission eventLogPerm = new EventLogPermission( EventLogPermissionAccess.Administer, System.Net.Dns.GetHostName( ) );
                //eventLogPerm.Demand( );

                bool found = false;
                foreach (EventLog el in EventLog.GetEventLogs())
                {
                    if (el.Log.Equals(mLog))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    EventLog.CreateEventSource(mEventLogSource, mLog);
                }
            }
            //catch ( SecurityException se ) 
            //{
            //    StaticLogger.Logger.WriteError( String.Format( "[CHECK] {0}", se.ToString( ) ) );
            //}
            catch (Exception e)
            {
                if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format("[CHECK] {0}", e.ToString()));
            }
        }

        /// <summary>
        /// Validate the configuration
        /// </summary>
        /// <param name="configFile">The config file.</param>
        /// <returns>True, if the configuration file content is valid, otherwise False.</returns>
        [DebuggerStepThrough]
        public static bool ValidateConfiguration(String configFile)
        {
            bool success = true;

            try
            {
                ConfigurationManager.OpenExeConfiguration(configFile);
            }
            catch (Exception ex)
            {
                success = false;
                WriteEventLog(ex.ToString());
                if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format("[CHECK] {0}", ex.ToString()));
            }

            return success;
        }

        /// <summary>
        /// Write an eventlog entry to the specified Log
        /// </summary>
        /// <param name="message">Message to write</param>
        [DebuggerStepThrough]
        public static void WriteEventLog(String message)
        {
            try
            {
                //EventLogPermission perm = new EventLogPermission( EventLogPermissionAccess.Administer, System.Net.Dns.GetHostName( ) );
                //perm.Demand( );

                foreach (EventLog el in EventLog.GetEventLogs())
                {
                    if (el.Log.Equals(mLog))
                    {
                        el.Source = mEventLogSource;
                        el.WriteEntry(message, EventLogEntryType.Error);
                        break;
                    }
                }

            }
            //catch ( SecurityException se ) 
            //{
            //    StaticLogger.Logger.WriteError( String.Format( "[CHECK] {0}", se.ToString( ) ) );
            //}
            catch (Exception e)
            {
                if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format("[CHECK] {0}", e.ToString()));
            }
        }

        #endregion

    }

}
