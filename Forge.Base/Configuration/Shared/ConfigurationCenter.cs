/* *********************************************************************
 * Date: 20 Feb 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;

namespace Forge.Configuration.Shared
{

    /// <summary>
    /// Center properties of the configuration
    /// </summary>
    public static class ConfigurationCenter
    {

        #region Field(s)

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private static bool mOverride = false;
        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private static String mConfigurationFile = String.Empty;

        #endregion

        #region Public Static Properties

        /// <summary>
        /// Override local settings of configurations. Default is false.
        /// </summary>
        /// <value>
        /// <c>true</c> if [override local settings]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public static bool OverrideLocalSettings
        {
            get { return mOverride; }
            set { mOverride = value; }
        }

        /// <summary>
        /// Specify global configuration file. Local configuration handler may override this file,
        /// so if you want to order them to use this file, you have to set true the value of property 'OverrideLocalSettings'
        /// </summary>
        /// <value>
        /// The configuration file.
        /// </value>
        public static String ConfigurationFile
        {
            get { return mConfigurationFile; }
            set { mConfigurationFile = ( value == null ? String.Empty : value ); }
        }

        #endregion

    }

}
