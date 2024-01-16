using System.Diagnostics;

namespace Forge.Net.Synapse.Options
{

    /// <summary>Options for SSL client stream factory</summary>
    public class SslClientStreamFactoryOptions : StreamFactoryOptions
    {

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mServerNameOnCertificate = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mSkipSslPolicyErrors = false;

        /// <summary>Initializes a new instance of the <see cref="SslClientStreamFactoryOptions" /> class.</summary>
        public SslClientStreamFactoryOptions()
        {
        }

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

    }

}
