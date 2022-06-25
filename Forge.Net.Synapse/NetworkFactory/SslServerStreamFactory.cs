/* *********************************************************************
 * Date: 14 Jun 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Forge.Configuration.Shared;
using Forge.Net.Synapse.NetworkServices;
using Forge.Security;

namespace Forge.Net.Synapse.NetworkFactory
{

    /// <summary>
    /// SSL Server Stream Factory
    /// </summary>
    public class SslServerStreamFactory : StreamFactoryBase, IServerStreamFactory
    {

        #region Field(s)

        /// <summary>
        /// Represents the used certificate
        /// </summary>
        protected X509Certificate mCertificate = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="SslServerStreamFactory"/> class.
        /// </summary>
        public SslServerStreamFactory()
        {
#if NETCOREAPP3_1_OR_GREATER
            this.Protocol = SslProtocols.Tls13;
#else
            this.Protocol = SslProtocols.Default;
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SslServerStreamFactory"/> class.
        /// </summary>
        /// <param name="certificate">The certificate.</param>
        public SslServerStreamFactory(X509Certificate certificate) : this()
        {
            if (certificate == null)
            {
                ThrowHelper.ThrowArgumentNullException("certificate");
            }
            this.mCertificate = certificate;
            IsInitialized = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SslServerStreamFactory"/> class.
        /// </summary>
        /// <param name="certificate">The certificate.</param>
        /// <param name="receiveBufferSize">Size of the receive buffer.</param>
        /// <param name="sendBufferSize">Size of the send buffer.</param>
        public SslServerStreamFactory(X509Certificate certificate, int receiveBufferSize, int sendBufferSize)
            : this(certificate)
        {
            ReceiveBufferSize = receiveBufferSize;
            SendBufferSize = sendBufferSize;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the protocol.
        /// </summary>
        /// <value>
        /// The protocol.
        /// </value>
        public SslProtocols Protocol { get; set; }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes the specified config item.
        /// </summary>
        /// <param name="configItem">The config item.</param>
        /// <exception cref="Forge.Configuration.Shared.InvalidConfigurationValueException">CertificateSource value is invalid.
        /// or
        /// CertificateSource value is invalid.
        /// or
        /// CertificateFile value is invalid.
        /// or
        /// Password value is invalid.
        /// or
        /// Subject value is invalid.
        /// or
        /// CertificateSource value is invalid.</exception>
        /// <exception cref="Forge.Configuration.Shared.InvalidConfigurationException">Invalid certificate.
        /// or
        /// Failed to find certificate.
        /// or
        /// Failed to find certificate.</exception>
        public override void Initialize(CategoryPropertyItem configItem)
        {
            if (configItem == null)
            {
                ThrowHelper.ThrowArgumentNullException("configItem");
            }
            if (!IsInitialized)
            {
                CertificateConfigurationSource certSource = new CertificateConfigurationSource();
                certSource.Initialize(configItem);
                mCertificate = certSource.Certificate;

                CategoryPropertyItem pi = ConfigurationAccessHelper.GetCategoryPropertyByPath(configItem.PropertyItems, "Protocol");
                if (pi != null)
                {
                    try
                    {
                        Protocol = (SslProtocols)Enum.Parse(typeof(SslProtocols), pi.EntryValue, true);
                    }
                    catch (Exception)
                    {
                    }
                }

                base.Initialize(configItem);

                IsInitialized = true;
            }
        }

        /// <summary>
        /// Creates the stream.
        /// </summary>
        /// <param name="tcpClient">The TCP client.</param>
        /// <returns>Network Stream instance</returns>
        public NetworkStream CreateNetworkStream(ITcpClient tcpClient)
        {
            DoInitializeCheck();
            if (tcpClient == null)
            {
                ThrowHelper.ThrowArgumentNullException("tcpClient");
            }

            tcpClient.Client.ReceiveBufferSize = ReceiveBufferSize;
            tcpClient.Client.SendBufferSize = SendBufferSize;
            tcpClient.Client.NoDelay = NoDelay;
            tcpClient.Client.ReceiveTimeout = ReceiveTimeout;
            tcpClient.Client.SendTimeout = SendTimeout;

            SslStream sslStream = new SslStream(tcpClient.GetStream(), false);
            sslStream.AuthenticateAsServer(mCertificate, false, Protocol, true);

            return new NetworkStream(sslStream, tcpClient.Client);
        }

        #endregion

    }

}
