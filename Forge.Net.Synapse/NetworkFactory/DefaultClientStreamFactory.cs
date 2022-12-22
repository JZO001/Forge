/* *********************************************************************
 * Date: 07 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Net.Synapse.NetworkServices;
using Forge.Shared;

namespace Forge.Net.Synapse.NetworkFactory
{

    /// <summary>
    /// Client side stream factory
    /// </summary>
    public class DefaultClientStreamFactory : StreamFactoryBase, IClientStreamFactory
    {

        #region Constructor(s)

        /// <summary>
        /// Prevents a default instance of the <see cref="DefaultClientStreamFactory"/> class from being created.
        /// </summary>
        public DefaultClientStreamFactory()
        {
            IsInitialized = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultClientStreamFactory"/> class.
        /// </summary>
        /// <param name="receiveBufferSize">Size of the receive buffer.</param>
        /// <param name="sendBufferSize">Size of the send buffer.</param>
        public DefaultClientStreamFactory(int receiveBufferSize, int sendBufferSize)
            : base(receiveBufferSize, sendBufferSize)
        {
            IsInitialized = true;
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Creates the network stream.
        /// </summary>
        /// <param name="tcpClient">The TcpClient service</param>
        /// <returns>Network Stream instance</returns>
        public override NetworkStream CreateNetworkStream(ITcpClient tcpClient)
        {
            if (tcpClient == null)
            {
                ThrowHelper.ThrowArgumentNullException("tcpClient");
            }
            tcpClient.Client.ReceiveBufferSize = ReceiveBufferSize;
            tcpClient.Client.SendBufferSize = SendBufferSize;
            tcpClient.Client.NoDelay = NoDelay;
            tcpClient.Client.ReceiveTimeout = ReceiveTimeout;
            tcpClient.Client.SendTimeout = SendTimeout;
            return tcpClient.GetStream();
        }

        #endregion

    }

}
