/* *********************************************************************
 * Date: 10 Oct 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

namespace Forge.Net.Synapse.Icmp
{

    /// <summary>
    /// Ping operation result types
    /// </summary>
    public enum PingResultEnum
    {

        /// <summary>
        /// State of a normal ping operation
        /// </summary>
        Result = 0,

        /// <summary>
        /// Occurs when the ping task finished
        /// </summary>
        Finished,

        /// <summary>
        /// Occurs when the remote host did not found
        /// </summary>
        HostNotFoundError,

        /// <summary>
        /// Occurs when the icmp packet creation failed
        /// </summary>
        BadPacketError,

        /// <summary>
        /// Occurs when the icmp packet failed to send  
        /// </summary>
        SocketError,

        /// <summary>
        /// Occurs when the icmp request timed out
        /// </summary>
        RequestTimeout,

        /// <summary>
        /// Occurs when the remote side did not respond
        /// </summary>
        NoResponse
    }

}
