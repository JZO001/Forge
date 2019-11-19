/* *********************************************************************
 * Date: 9 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Security.Permissions;

namespace Forge.RemoteDesktop.ConfigSection
{

    /// <summary>
    /// Configuration access helper class for remote desktop
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Unrestricted = true)]
    public class RemoteDesktopConfiguration : Forge.Configuration.Shared.SharedConfigSettings<RemoteDesktopSection, RemoteDesktopConfiguration>
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes the <see cref="RemoteDesktopConfiguration"/> class.
        /// </summary>
        static RemoteDesktopConfiguration()
        {
            LOG_PREFIX = "REMOTE_DESKTOP_CONFIGURATION";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDesktopConfiguration"/> class.
        /// </summary>
        public RemoteDesktopConfiguration()
            : base()
        {
        }

        #endregion

    }

}
