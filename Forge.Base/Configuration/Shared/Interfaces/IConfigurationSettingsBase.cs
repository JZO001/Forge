/* *********************************************************************
 * Date: 20 Feb 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Configuration.Shared.Interfaces
{

    /// <summary>
    /// Interface for configuration settings handler event interface for local configuration handler classes
    /// </summary>
    public interface IConfigurationSettingsBase
    {

        /// <summary>
        /// Occurs when [on configuration changed].
        /// </summary>
        event EventHandler<EventArgs> OnConfigurationChanged;

    }

}
