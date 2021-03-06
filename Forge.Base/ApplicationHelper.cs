﻿/* *********************************************************************
 * Date: 25 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Configuration;
using System.Threading;
using System.Web;

namespace Forge
{

    /// <summary>
    /// Represents an application with custom extensions
    /// </summary>
    public static class ApplicationHelper
    {

        private static readonly string APP_CONFIG_ID = "ApplicationID";

        /// <summary>
        /// Represents the constant of the machine name
        /// </summary>
        public static readonly string MachineName = "$MachineName";

        /// <summary>
        /// Represents the constant of the user domain name
        /// </summary>
        public static readonly string UserDomainName = "$UserDomainName";

        /// <summary>
        /// Represents the constant of the user name
        /// </summary>
        public static readonly string UserName = "$UserName";

        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        /// <value>
        /// The application id.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public static string ApplicationId
        {
            get
            {
                string appId = ConfigurationManager.AppSettings[APP_CONFIG_ID];

                if (String.IsNullOrEmpty(appId))
                {
                    throw new InitializationException("Unable to find application identifier in configuration.");
                }

                if (appId.Contains(MachineName))
                {
                    appId = appId.Replace(MachineName, Environment.MachineName);
                }

                if (appId.Contains(UserDomainName))
                {
                    appId = appId.Replace(UserDomainName, Environment.UserDomainName);
                }

                if (appId.Contains(UserName))
                {
                    appId = appId.Replace(UserName, Environment.UserName);
                }

                return appId;
            }
            set
            {
                ConfigurationManager.AppSettings[APP_CONFIG_ID] = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the thread is the UI thread.
        /// </summary>
        /// <value>
        /// <c>true</c> if [is unique identifier thread]; otherwise, <c>false</c>.
        /// </value>
#if NETCOREAPP3_1
        public static bool IsUIThread()
        {
            return System.Windows.Forms.Application.MessageLoop ||
                System.Windows.Threading.Dispatcher.FromThread(Thread.CurrentThread) != null;
        }
#else
        public static bool IsUIThread()
        {
            return System.Windows.Forms.Application.MessageLoop ||
                (HttpRuntime.AppDomainAppId == null ?
                    System.Windows.Threading.Dispatcher.FromThread(Thread.CurrentThread) != null
                    :
                    false);
        }
#endif

    }

}
