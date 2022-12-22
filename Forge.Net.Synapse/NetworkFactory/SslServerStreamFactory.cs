/* *********************************************************************
 * Date: 14 Jun 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Forge.Configuration;
using Forge.Configuration.Shared;
using Forge.Net.Synapse.NetworkServices;
using Forge.Net.Synapse.Options;
using Forge.Security;
using Forge.Shared;

namespace Forge.Net.Synapse.NetworkFactory
{

    /// <summary>
    /// SSL Server Stream Factory
    /// </summary>
    public class SslServerStreamFactory : StreamFactoryBase, ISslServerStreamFactory
    {

        #region Field(s)

        /// <summary>
        /// Represents the used certificate
        /// </summary>
        protected X509Certificate mServerCertificate = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="SslServerStreamFactory"/> class.
        /// </summary>
        public SslServerStreamFactory()
        {
#if NETCOREAPP3_1_OR_GREATER
            Protocol = SslProtocols.Tls13;
#else
            Protocol = SslProtocols.Default;
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
            mServerCertificate = certificate;
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

        /// <summary>Gets or sets the server certificate.</summary>
        /// <value>The server certificate.</value>
        [DebuggerHidden]
        public X509Certificate ServerCertificate
        {
            get { return mServerCertificate; }
            set
            {
                if (value == null) ThrowHelper.ThrowArgumentNullException("value");
                mServerCertificate = value;
            }
        }

        /// <summary>
        /// Gets or sets the protocol.
        /// </summary>
        /// <value>
        /// The protocol.
        /// </value>
        [DebuggerHidden]
        public SslProtocols Protocol { get; set; }

        /// <summary>Gets or sets a value indicating whether this instance is client certificate required.</summary>
        /// <value>
        ///   <c>true</c> if this instance is client certificate required; otherwise, <c>false</c>.</value>
        [DebuggerHidden]
        public bool IsClientCertificateRequired { get; set; }

        /// <summary>Gets or sets a value indicating whether [check certificate revocation].</summary>
        /// <value>
        ///   <c>true</c> if [check certificate revocation]; otherwise, <c>false</c>.</value>
        [DebuggerHidden]
        public bool CheckCertificateRevocation { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether [leave inner stream open].</summary>
        /// <value>
        ///   <c>true</c> if [leave inner stream open]; otherwise, <c>false</c>.</value>
        [DebuggerHidden]
        public bool LeaveInnerStreamOpen { get; set; }

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
        public override void Initialize(IPropertyItem configItem)
        {
            if (configItem == null)
            {
                ThrowHelper.ThrowArgumentNullException("configItem");
            }
            if (!IsInitialized)
            {
                CertificateConfigurationSource certSource = new CertificateConfigurationSource();
                certSource.Initialize(configItem);
                mServerCertificate = certSource.Certificate;

                IPropertyItem pi = ConfigurationAccessHelper.GetPropertyByPath(configItem, "Protocol");
                if (pi != null)
                {
                    try
                    {
                        Protocol = (SslProtocols)Enum.Parse(typeof(SslProtocols), pi.Value, true);
                    }
                    catch (Exception)
                    {
                    }
                }

                IsClientCertificateRequired = ParseBooleanValue(configItem, "IsClientCertificateRequired");
                pi = ConfigurationAccessHelper.GetPropertyByPath(configItem, "CheckCertificateRevocation");
                if (pi != null)
                {
                    CheckCertificateRevocation = ParseBooleanValue(configItem, "CheckCertificateRevocation");
                }
                LeaveInnerStreamOpen = ParseBooleanValue(configItem, "LeaveInnerStreamOpen");

                base.Initialize(configItem);

                IsInitialized = true;
            }
        }

        /// <summary>No not use this method</summary>
        /// <param name="options">The options.</param>
        [Browsable(false)]
        [Obsolete]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        public override void Initialize(StreamFactoryOptions options)
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
        {
            if (options is SslServerStreamFactoryOptions)
            {
                Initialize((SslServerStreamFactoryOptions)options);
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Initializes the specified config item.
        /// </summary>
        /// <param name="options">The options.</param>
        public void Initialize(SslServerStreamFactoryOptions options)
        {
            base.Initialize(options);
            ServerCertificate = options.ServerCertificate;
            Protocol = options.Protocol;
            IsClientCertificateRequired = options.IsClientCertificateRequired;
            CheckCertificateRevocation = options.CheckCertificateRevocation;
            LeaveInnerStreamOpen = options.LeaveInnerStreamOpen;

            CertificateConfigurationSource certSource = new CertificateConfigurationSource();
            certSource.Initialize(options);
            mServerCertificate = certSource.Certificate;

            IsInitialized = true;
        }

        /// <summary>
        /// Creates the stream.
        /// </summary>
        /// <param name="tcpClient">The TCP client.</param>
        /// <returns>Network Stream instance</returns>
        public override NetworkStream CreateNetworkStream(ITcpClient tcpClient)
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

            SslStream sslStream = new SslStream(tcpClient.GetStream(), LeaveInnerStreamOpen);
            sslStream.AuthenticateAsServer(mServerCertificate, IsClientCertificateRequired, Protocol, CheckCertificateRevocation);

            return new NetworkStream(sslStream, tcpClient.Client);
        }

        #endregion

    }

}
