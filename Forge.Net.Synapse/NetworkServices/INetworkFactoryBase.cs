/* *********************************************************************
 * Date: 08 Jun 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

namespace Forge.Net.Synapse.NetworkServices
{

    /// <summary>
    /// Represents the service methods for a Network factory
    /// </summary>
    public interface INetworkFactoryBase
    {

        /// <summary>
        /// Creates the TCP listener.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns>TcpListener implementation</returns>
        ITcpListener CreateTcpListener(int port);

        /// <summary>
        /// Creates the TCP listener.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>TcpListener implementation</returns>
        ITcpListener CreateTcpListener(AddressEndPoint endPoint);

        /// <summary>
        /// Creates the TCP client.
        /// </summary>
        /// <returns>TcpClient implementation</returns>
        ITcpClient CreateTcpClient();

        /// <summary>
        /// Creates the UDP client.
        /// </summary>
        /// <returns>UdpClient implementation</returns>
        IUdpClient CreateUdpClient();

        /// <summary>
        /// Creates the UDP client.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns>UdpClient implementation</returns>
        IUdpClient CreateUdpClient(int port);

    }

}
