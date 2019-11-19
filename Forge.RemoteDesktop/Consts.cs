/* *********************************************************************
 * Date: 9 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

namespace Forge.RemoteDesktop
{

    /// <summary>
    /// Represents the constants used by the remote desktop
    /// </summary>
    public class Consts
    {

        /// <summary>
        /// Identifier of the service and locator
        /// </summary>
        public const string SERVICE_ID = "RemoteDesktopApplication";

        /// <summary>
        /// The minimal desktop image clip size
        /// </summary>
        public const int MINIMAL_CLIP_SIZE = 10;

        /// <summary>
        /// The default desktop image clip size
        /// </summary>
        public const int DEFAULT_DESKTOP_IMAGE_CLIP_SIZE = 100;

        /// <summary>
        /// The default value for clients per service threads
        /// </summary>
        public const int DEFAULT_CLIENTS_PER_SERVICE_THREADS = 3;

        /// <summary>
        /// The minimal value for clients per service threads
        /// </summary>
        public const int MINIMAL_CLIENTS_PER_SERVICE_THREADS = 1;

        /// <summary>
        /// The default maximum number of failed logins before blacklist
        /// </summary>
        public const int DEFAULT_MAXIMUM_FAILED_LOGIN_ATTEMPT = 3;

        /// <summary>
        /// The default value in minutes for a blacklisted client
        /// </summary>
        public const int DEFAULT_BLACKLIST_TIMEOUT_IN_MINUTES = 5;

        /// <summary>
        /// The default image clip quality
        /// </summary>
        public const int DEFAULT_IMAGE_CLIP_QUALITY = 90;

        /// <summary>
        /// The default mouse move send interval
        /// </summary>
        public const int DEFAULT_MOUSE_MOVE_SEND_INTERVAL = 100;

    }

}
