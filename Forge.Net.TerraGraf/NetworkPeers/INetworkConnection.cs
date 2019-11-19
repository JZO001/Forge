/* *********************************************************************
 * Date: 21 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

namespace Forge.Net.TerraGraf.NetworkPeers
{

    /// <summary>
    /// Represent a public network connection
    /// </summary>
    public interface INetworkConnection
    {

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        long Id { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        bool IsConnected { get; }

        /// <summary>
        /// Closes this connection.
        /// </summary>
        void Close();

    }

}
