using Forge.Net.Synapse.Options;

namespace Forge.Net.Synapse.NetworkServices
{

    /// <summary>
    /// Factory for client side SSL streams
    /// </summary>
    public interface ISslClientStreamFactory : IClientStreamFactory
    {

        /// <summary>
        /// Gets or sets the server name on certificate.
        /// </summary>
        /// <value>
        /// The server name on certificate.
        /// </value>
        string ServerNameOnCertificate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [skip SSL policy errors].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [skip SSL policy errors]; otherwise, <c>false</c>.
        /// </value>
        bool SkipSslPolicyErrors { get; set; }

        /// <summary>
        /// Initializes the specified config item.
        /// </summary>
        /// <param name="options">The options.</param>
        void Initialize(SslClientStreamFactoryOptions options);

    }

}
