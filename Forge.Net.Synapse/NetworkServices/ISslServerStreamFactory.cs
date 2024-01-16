using Forge.Net.Synapse.Options;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Forge.Net.Synapse.NetworkServices
{

    /// <summary>
    /// Factory for server side SSL streams
    /// </summary>
    public interface ISslServerStreamFactory : IServerStreamFactory
    {

        /// <summary>Gets or sets the server certificate.</summary>
        /// <value>The server certificate.</value>
        X509Certificate ServerCertificate { get; set; }

        /// <summary>
        /// Gets or sets the protocol.
        /// </summary>
        /// <value>
        /// The protocol.
        /// </value>
        SslProtocols Protocol { get; set; }

        /// <summary>Gets or sets a value indicating whether this instance is client certificate required.</summary>
        /// <value>
        ///   <c>true</c> if this instance is client certificate required; otherwise, <c>false</c>.</value>
        bool IsClientCertificateRequired { get; set; }

        /// <summary>Gets or sets a value indicating whether [check certificate revocation].</summary>
        /// <value>
        ///   <c>true</c> if [check certificate revocation]; otherwise, <c>false</c>.</value>
        bool CheckCertificateRevocation { get; set; }

        /// <summary>Gets or sets a value indicating whether [leave inner stream open].</summary>
        /// <value>
        ///   <c>true</c> if [leave inner stream open]; otherwise, <c>false</c>.</value>
        bool LeaveInnerStreamOpen { get; set; }

        /// <summary>
        /// Initializes the specified config item.
        /// </summary>
        /// <param name="options">The options.</param>
        void Initialize(SslServerStreamFactoryOptions options);

    }

}
