/* *********************************************************************
 * Date: 20 Feb 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Configuration;

namespace Forge.Configuration.Shared.Interfaces
{

    /// <summary>
    /// Configuration settings handler interface for local configuration handler classes
    /// </summary>
    /// <typeparam name="SectionType">The type of the ection type.</typeparam>
    public interface IConfigurationSettingsHandler<SectionType> : IDisposable, IConfigurationSettingsBase where SectionType : System.Configuration.ConfigurationSection
    {

        /// <summary>
        /// Gets the settings local.
        /// </summary>
        /// <value>
        /// The settings local.
        /// </value>
        SectionType SettingsLocal { get; }

        /// <summary>
        /// Gets the last known good settings local.
        /// </summary>
        /// <value>
        /// The last known good settings local.
        /// </value>
        SectionType LastKnownGoodSettingsLocal { get; }

        /// <summary>
        /// Gets or sets the default configuration file.
        /// </summary>
        /// <value>
        /// The default configuration file.
        /// </value>
        String DefaultConfigurationFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use local configuration].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use local configuration]; otherwise, <c>false</c>.
        /// </value>
        bool UseLocalConfiguration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [restart on external changes].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [restart on external changes]; otherwise, <c>false</c>.
        /// </value>
        bool RestartOnExternalChanges { get; set; }

        /// <summary>
        /// Refreshes the instance.
        /// </summary>
        void RefreshInstance();

        /// <summary>
        /// Saves the instance.
        /// </summary>
        /// <param name="mode">The mode.</param>
        void SaveInstance(ConfigurationSaveMode mode);

    }

}
