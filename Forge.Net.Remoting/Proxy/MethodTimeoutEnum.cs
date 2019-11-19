/* *********************************************************************
 * Date: 20 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Net.Remoting.Proxy
{

    /// <summary>
    /// Represent the type of the method timeout
    /// </summary>
    [Serializable]
    public enum MethodTimeoutEnum
    {
        /// <summary>
        /// Remote method call timeout
        /// </summary>
        CallTimeout = 0,
        
        /// <summary>
        /// Return timeout
        /// </summary>
        ReturnTimeout
    }

}
