/* *********************************************************************
 * Date: 08 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Net.Remoting
{

    /// <summary>
    /// Represents the reliablility of a call
    /// </summary>
    [Serializable]
    public enum MessageTypeEnum
    {
        /// <summary>
        /// Send message which does not wait for a response. Use this type with void methods. This type of message has acknowledge.
        /// </summary>
        Datagram = 0,

        /// <summary>
        /// Send message which does not wait for a response. Use this type with void methods. This type of message has no acknowledge,
        /// which means there is no guarantee the message will be delivered to the server.
        /// </summary>
        DatagramOneway,

        /// <summary>
        /// Request message for remote method which has return value.
        /// </summary>
        Request,

        /// <summary>
        /// Response of a request which trnasfer the return value of the method.
        /// </summary>
        Response,

        /// <summary>
        /// For reliable communication the datagram and the response will be acknowledged.
        /// </summary>
        Acknowledge
    }

}
