/* *********************************************************************
 * Date: 26 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

namespace Forge.WindowsServiceControl
{

    /// <summary>
    /// Service Config Info Levels
    /// </summary>
    internal enum ServiceConfig2InfoLevelEnum : int
    {

        /// <summary>
        /// The lpBuffer parameter is a pointer to a SERVICE_DESCRIPTION structure
        /// </summary>
        SERVICE_CONFIG_DESCRIPTION = 0x00000001,

        /// <summary>
        /// The lpBuffer parameter is a pointer to a SERVICE_FAILURE_ACTIONS structure
        /// </summary>
        SERVICE_CONFIG_FAILURE_ACTIONS = 0x00000002

    }

}
