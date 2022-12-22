/* *********************************************************************
 * Date: 26 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

namespace Forge.WindowsServiceControl
{

    /// <summary>
    /// Service Action Types
    /// </summary>
    internal enum ServiceActionTypeEnum : uint
    {

        /// <summary>
        /// No action
        /// </summary>
        SC_ACTION_NONE = 0x00000000,

        /// <summary>
        /// Restart the service
        /// </summary>
        SC_ACTION_RESTART = 0x00000001,

        /// <summary>
        /// Reboot the computer
        /// </summary>
        SC_ACTION_REBOOT = 0x00000002,

        /// <summary>
        /// Run a command
        /// </summary>
        SC_ACTION_RUN_COMMAND = 0x00000003

    }

}
