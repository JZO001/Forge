/* *********************************************************************
 * Date: 08 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Net.Remoting
{

    /// <summary>
    /// Defines how well-known objects are activated.
    /// </summary>
    [Serializable]
    public enum WellKnownObjectModeEnum
    {
        /// <summary>
        /// Every incoming message is serviced by the same object instance. Instance never released.
        /// </summary>
        Singleton = 0,
        
        /// <summary>
        /// Every incoming message is serviced by a new object instance. After the response sent, instance will be dropped.
        /// </summary>
        SingleCall,

        /// <summary>
        /// Every incoming message is serviced by the same object instance per network session. When the session closed, instance will be dropped.
        /// </summary>
        PerSession
    }

}
