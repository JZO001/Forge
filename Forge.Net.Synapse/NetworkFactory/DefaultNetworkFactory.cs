/* *********************************************************************
 * Date: 07 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Net;
using System.Net.Sockets;
using Forge.Legacy;
using Forge.Net.Synapse.NetworkServices;
using Forge.Net.Synapse.NetworkServices.Defaults;

namespace Forge.Net.Synapse.NetworkFactory
{

    /// <summary>
    /// Represents the default network factory for .NET low level sockets
    /// </summary>
    public class DefaultNetworkFactory : MBRBase, IDefaultNetworkFactory
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultNetworkFactory"/> class.
        /// </summary>
        public DefaultNetworkFactory()
        {
        }

        /// <summary>
        /// Creates the TCP client.
        /// </summary>
        /// <returns>TcpClient implementation</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public ITcpClient CreateTcpClient()
        {
            return new TcpClientWrapper(new TcpClient());
        }

        /// <summary>
        /// Creates the TCP listener.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns>TcpClient implementation</returns>
        public ITcpListener CreateTcpListener(int port)
        {
            return new TcpListenerWrapper(new TcpListener(IPAddress.Any, port));
        }

        /// <summary>
        /// Creates the TCP listener.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>TcpClient implementation</returns>
        public ITcpListener CreateTcpListener(AddressEndPoint endPoint)
        {
            return new TcpListenerWrapper(new TcpListener(Dns.GetHostAddresses(endPoint.Host)[0], endPoint.Port));
        }

        /// <summary>
        /// Creates the UDP client.
        /// </summary>
        /// <returns>UdpClient implementation</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public IUdpClient CreateUdpClient()
        {
            return new UdpClientWrapper(new UdpClient());
        }

        /// <summary>
        /// Creates the UDP client.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns>UdpClient implementation</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public IUdpClient CreateUdpClient(int port)
        {
            return new UdpClientWrapper(new UdpClient(port));
        }

    }

}
