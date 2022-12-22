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
    /// Simple server stream factory
    /// </summary>
    public class DefaultServerStreamFactory : StreamFactoryBase, IServerStreamFactory
    {

        #region Constructor(s)

        /// <summary>
        /// Prevents a default instance of the <see cref="DefaultServerStreamFactory"/> class from being created.
        /// </summary>
        public DefaultServerStreamFactory()
        {
            IsInitialized = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultServerStreamFactory"/> class.
        /// </summary>
        /// <param name="receiveBufferSize">Size of the receive buffer.</param>
        /// <param name="sendBufferSize">Size of the send buffer.</param>
        public DefaultServerStreamFactory(int receiveBufferSize, int sendBufferSize)
            : base(receiveBufferSize, sendBufferSize)
        {
            IsInitialized = true;
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Creates the stream.
        /// </summary>
        /// <param name="tcpClient">Tcp Client service</param>
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
