/* *********************************************************************
 * Date: 09 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Net.TerraGraf.Messaging
{

    /// <summary>
    /// Enums for messages
    /// </summary>
    [Serializable]
    internal enum MessageCodeEnum : int
    {
        /// <summary>
        /// Undefinied message
        /// </summary>
        Undefinied = 0,

        /// <summary>
        /// Message for UDP broadcasting
        /// </summary>
        UdpBroadcast = 1,

        /// <summary>
        /// Acknowledge message for all TCP message
        /// </summary>
        Acknowledge = 2,

        /// <summary>
        /// Message for send user raw data
        /// </summary>
        SocketRawData = 3,

        /// <summary>
        /// Message for open a socket connection request
        /// </summary>
        SocketOpenRequest = 4,

        /// <summary>
        /// Message for open a socket connection response
        /// </summary>
        SocketOpenResponse = 5,

        /// <summary>
        /// Message for close a socket connection
        /// </summary>
        SocketClose = 6,

        /// <summary>
        /// Message when a new connection established and sending a graf descriptor message
        /// </summary>
        Negotiation = 7,

        /// <summary>
        /// TerraGraf information message
        /// </summary>
        TerraGrafInformation = 8,

        /// <summary>
        /// Update information for a network peer
        /// </summary>
        TerraGrafPeerUpdate = 9,

        /// <summary>
        /// Low level Tcp message acknowledge
        /// </summary>
        LowLevelTcpAcknowledge = 10

    }

}
