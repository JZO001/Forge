/* *********************************************************************
 * Date: 07 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Threading.Tasks;
using Forge.Configuration;
using Forge.Configuration.Shared;
using Forge.Net.Synapse.Options;

namespace Forge.Net.Synapse.NetworkServices
{

    /// <summary>
    /// Common base for stream factory interface
    /// </summary>
    public interface IStreamFactory
    {

        #region Public properties

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        bool IsInitialized { get; }

        /// <summary>
        /// Gets or sets the size of the receive buffer.
        /// </summary>
        /// <value>
        /// The size of the receive buffer.
        /// </value>
        int ReceiveBufferSize { get; set; }

        /// <summary>
        /// Gets or sets the size of the send buffer.
        /// </summary>
        /// <value>
        /// The size of the send buffer.
        /// </value>
        int SendBufferSize { get; set; }

        /// <summary>
        /// Gets or sets a value of no delay.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [no delay]; otherwise, <c>false</c>.
        /// </value>
        bool NoDelay { get; set; }

        /// <summary>
        /// Gets or sets the receive timeout.
        /// </summary>
        /// <value>
        /// The receive timeout.
        /// </value>
        int ReceiveTimeout { get; set; }

        /// <summary>
        /// Gets or sets the send timeout.
        /// </summary>
        /// <value>
        /// The send timeout.
        /// </value>
        int SendTimeout { get; set; }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes the specified config item.
        /// </summary>
        /// <param name="configItem">The config item.</param>
        void Initialize(IPropertyItem configItem);

        /// <summary>
        /// Initializes the specified config item.
        /// </summary>
        /// <param name="options">The pptions.</param>
        void Initialize(StreamFactoryOptions options);

        /// <summary>
        /// Creates the network stream.
        /// </summary>
        /// <param name="tcpClient">The TCP client.</param>
        /// <returns>Network Stream instance</returns>
        NetworkStream CreateNetworkStream(ITcpClient tcpClient);

#if NETCOREAPP3_1_OR_GREATER
        /// <summary>
        /// Creates the network stream asynhronously.
        /// </summary>
        /// <param name="tcpClient">The TCP client.</param>
        /// <returns>Network Stream instance</returns>
        Task<NetworkStream> CreateNetworkStreamAsync(ITcpClient tcpClient);
#endif

        #endregion

    }

}
