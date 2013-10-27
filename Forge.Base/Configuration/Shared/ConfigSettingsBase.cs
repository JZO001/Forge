/* *********************************************************************
 * Date: 20 Feb 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using Forge.EventRaiser;

namespace Forge.Configuration.Shared
{

    /// <summary>
    /// Represents the base container of the configuration sections
    /// </summary>
    public abstract class ConfigSettingsBase : MBRBase
    {

        #region Field(s)

        /// <summary>
        /// The known config sections
        /// </summary>
        protected readonly static List<ConfigSettingsBase> mKnownConfigSettings = new List<ConfigSettingsBase>();

        /// <summary>
        /// Occurs when [on configuration changed].
        /// </summary>
        public event EventHandler<EventArgs> OnConfigurationChanged;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the default configuration file.
        /// </summary>
        /// <value>
        /// The default configuration file.
        /// </value>
        public abstract string DefaultConfigurationFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [restart configuration external changes].
        /// </summary>
        /// <value>
        /// <c>true</c> if [restart configuration external changes]; otherwise, <c>false</c>.
        /// </value>
        public abstract bool RestartOnExternalChanges
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use local configuration].
        /// </summary>
        /// <value>
        /// <c>true</c> if [use local configuration]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool UseLocalConfiguration
        {
            get;
            set;
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Refreshes the instance.
        /// </summary>
        public abstract void RefreshInstance();

        /// <summary>
        /// Saves the instance.
        /// </summary>
        /// <param name="mode">The mode.</param>
        public abstract void SaveInstance(ConfigurationSaveMode mode);

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Creates the section.
        /// </summary>
        protected abstract void CreateSection();

        /// <summary>
        /// Validates this instance values manually.
        /// </summary>
        protected abstract void Validate();

        /// <summary>
        /// Raises the on configuration changed.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected virtual void RaiseOnConfigurationChanged(EventArgs e)
        {
            Raiser.CallDelegatorBySync(OnConfigurationChanged, new object[] { this, e }, false, false);
        }

        #endregion

    }

}
