/* *********************************************************************
 * Date: 08 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Net.Remoting
{

    /// <summary>
    /// Specifies the direction of the remote procedure call.
    /// </summary>
    [Serializable]
    public enum OperationDirectionEnum
    {
        /// <summary>
        /// Client can call the operation on the server.
        /// </summary>
        ServerSide = 0,

        /// <summary>
        /// Server can call the operation on the client.
        /// </summary>
        ClientSide
    }

}
