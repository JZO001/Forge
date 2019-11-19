/* *********************************************************************
 * Date: 24 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Net.Synapse.NetworkServices;

namespace Forge.Net.TerraGraf
{

    /// <summary>
    /// Represents the synapse network factory for TerraGraf
    /// </summary>
    public class TerraGrafNetworkFactory : MBRBase, INetworkFactory
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TerraGrafNetworkFactory" /> class.
        /// </summary>
        public TerraGrafNetworkFactory()
        {
            NetworkManager.Instance.Start();
        }

        /// <summary>
        /// Creates the TCP listener.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns>
        /// TcpListener implementation
        /// </returns>
        public ITcpListener CreateTcpListener(int port)
        {
            return new TcpListener(port);
        }

        /// <summary>
        /// Creates the TCP listener.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>
        /// TcpListener implementation
        /// </returns>
        public ITcpListener CreateTcpListener(Synapse.AddressEndPoint endPoint)
        {
            if (endPoint == null)
            {
                ThrowHelper.ThrowArgumentNullException("endPoint");
            }
            return new TcpListener(endPoint.Port);
        }

        /// <summary>
        /// Creates the TCP client.
        /// </summary>
        /// <returns>
        /// TcpClient implementation
        /// </returns>
        public ITcpClient CreateTcpClient()
        {
            return new TcpClient();
        }

        /// <summary>
        /// Creates the UDP client.
        /// </summary>
        /// <returns>
        /// UdpClient implementation
        /// </returns>
        public IUdpClient CreateUdpClient()
        {
            return new UdpClient();
        }

        /// <summary>
        /// Creates the UDP client.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns>
        /// UdpClient implementation
        /// </returns>
        public IUdpClient CreateUdpClient(int port)
        {
            return new UdpClient(port);
        }

    }

}
