/* *********************************************************************
 * Date: 9 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Configuration.Shared;

namespace Forge.RemoteDesktop.ConfigSection
{

    /// <summary>
    /// Configuration section for remote desktop
    /// </summary>
    [Serializable]
    public class RemoteDesktopSection : ConfigurationSectionStandard
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDesktopSection"/> class.
        /// </summary>
        public RemoteDesktopSection()
            : base()
        {
        }

    }

}
