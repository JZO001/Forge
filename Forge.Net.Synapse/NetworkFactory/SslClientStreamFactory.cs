/* *********************************************************************
 * Date: 14 Jun 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.ComponentModel;
using System;
using System.Diagnostics;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Forge.Configuration.Shared;
using Forge.Net.Synapse.NetworkServices;
using Forge.Net.Synapse.Options;
using Forge.Shared;
using Forge.Configuration;
using Forge.Logging.Abstraction;

namespace Forge.Net.Synapse.NetworkFactory
{

    /// <summary>
    /// SSL Client Stream Factory
    /// </summary>
    public class SslClientStreamFactory : StreamFactoryBase, ISslClientStreamFactory
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger<SslClientStreamFactory>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mServerNameOnCertificate = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mSkipSslPolicyErrors = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="SslClientStreamFactory"/> class.
        /// </summary>
        public SslClientStreamFactory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SslClientStreamFactory"/> class.
        /// </summary>
        /// <param name="serverNameOnCertificate">The server name on certificate.</param>
        public SslClientStreamFactory(string serverNameOnCertificate)
        {
            if (string.IsNullOrEmpty(serverNameOnCertificate))
            {
                ThrowHelper.ThrowArgumentNullException("serverNameOnCertificate");
            }
            mServerNameOnCertificate = serverNameOnCertificate;
            IsInitialized = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SslClientStreamFactory"/> class.
        /// </summary>
        /// <param name="serverNameOnCertificate">The server name on certificate.</param>
        /// <param name="receiveBufferSize">Size of the receive buffer.</param>
        /// <param name="sendBufferSize">Size of the send buffer.</param>
        /// <param name="skipSslPolicyErrors">if set to <c>true</c> [skip SSL policy errors].</param>
        public SslClientStreamFactory(string serverNameOnCertificate, int receiveBufferSize, int sendBufferSize, bool skipSslPolicyErrors)
            : this(serverNameOnCertificate)
        {
            ReceiveBufferSize = receiveBufferSize;
            SendBufferSize = sendBufferSize;
            mSkipSslPolicyErrors = skipSslPolicyErrors;
            IsInitialized = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SslClientStreamFactory"/> class.
        /// </summary>
        /// <param name="serverNameOnCertificate">The server name on certificate.</param>
        /// <param name="skipSslPolicyErrors">if set to <c>true</c> [skip SSL policy errors].</param>
        public SslClientStreamFactory(string serverNameOnCertificate, bool skipSslPolicyErrors)
            : this(serverNameOnCertificate)
        {
            mSkipSslPolicyErrors = skipSslPolicyErrors;
            IsInitialized = true;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the server name on certificate.
        /// </summary>
        /// <value>
        /// The server name on certificate.
        /// </value>
        [DebuggerHidden]
        public string ServerNameOnCertificate
        {
            get { return mServerNameOnCertificate; }
            set { mServerNameOnCertificate = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [skip SSL policy errors].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [skip SSL policy errors]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool SkipSslPolicyErrors
        {
            get { return mSkipSslPolicyErrors; }
            set { mSkipSslPolicyErrors = value; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes the specified config item.
        /// </summary>
        /// <param name="configItem">The config item.</param>
        /// <exception cref="Forge.Configuration.Shared.InvalidConfigurationValueException">ServerNameOnCertificate value is invalid.</exception>
        public override void Initialize(IPropertyItem configItem)
        {
            if (configItem == null)
            {
                ThrowHelper.ThrowArgumentNullException("configItem");
            }
            if (!IsInitialized)
            {
                mServerNameOnCertificate = ConfigurationAccessHelper.GetValueByPath(configItem, "ServerNameOnCertificate");
                if (string.IsNullOrEmpty(mServerNameOnCertificate))
                {
                    throw new InvalidConfigurationValueException("ServerNameOnCertificate value is invalid.");
                }

                string skipSslPolicy = ConfigurationAccessHelper.GetValueByPath(configItem, "SkipSslPolicyErrors");
                if (!string.IsNullOrEmpty(skipSslPolicy))
                {
                    bool value = false;
                    if (bool.TryParse(skipSslPolicy, out value))
                    {
                        mSkipSslPolicyErrors = value;
                    }
                }

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
            if (options is SslClientStreamFactoryOptions)
            {
                Initialize((SslClientStreamFactoryOptions)options);
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Initializes the specified config item.
        /// </summary>
        /// <param name="options">The options.</param>
        public void Initialize(SslClientStreamFactoryOptions options)
        {
            base.Initialize(options);
            ServerNameOnCertificate = options.ServerNameOnCertificate;
            SkipSslPolicyErrors = options.SkipSslPolicyErrors;
            IsInitialized = true;
        }

        /// <summary>
        /// Creates the network stream.
        /// </summary>
        /// <param name="tcpClient"></param>
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

            SslStream sslStream = new SslStream(tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
            sslStream.AuthenticateAsClient(mServerNameOnCertificate);

            return new NetworkStream(sslStream, tcpClient.Client);
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Validates the server certificate.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="sslPolicyErrors">The SSL policy errors.</param>
        /// <returns></returns>
        protected virtual bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (mSkipSslPolicyErrors || sslPolicyErrors == SslPolicyErrors.None)
                return true;

            if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("SSL_CLIENT_STREAM_FACTORY, certificate error: {0}", sslPolicyErrors));

            return false;
        }

        #endregion

    }

}
