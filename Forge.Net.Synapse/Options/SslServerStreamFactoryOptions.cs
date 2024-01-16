using Forge.Security.Options;
using Forge.Shared;
using System;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Forge.Net.Synapse.Options
{

    /// <summary>Options for SSL server stream factory</summary>
    public class SslServerStreamFactoryOptions : StreamFactoryOptions, ICertificateOptions
    {

        /// <summary>
        /// Represents the used certificate
        /// </summary>
        private X509Certificate mServerCertificate = null;

        /// <summary>Initializes a new instance of the <see cref="SslServerStreamFactoryOptions" /> class.</summary>
        public SslServerStreamFactoryOptions()
        {
        }

        /// <summary>Gets or sets the server certificate.</summary>
        /// <value>The server certificate.</value>
        public X509Certificate ServerCertificate
        {
            get { return mServerCertificate; }
            set { mServerCertificate = value; }
        }

        /// <summary>
        /// Gets or sets the protocol.
        /// </summary>
        /// <value>
        /// The protocol.
        /// </value>
        public SslProtocols Protocol { get; set; }

        /// <summary>Gets or sets a value indicating whether this instance is client certificate required.</summary>
        /// <value>
        ///   <c>true</c> if this instance is client certificate required; otherwise, <c>false</c>.</value>
        public bool IsClientCertificateRequired { get; set; }

        /// <summary>Gets or sets a value indicating whether [check certificate revocation].</summary>
        /// <value>
        ///   <c>true</c> if [check certificate revocation]; otherwise, <c>false</c>.</value>
        public bool CheckCertificateRevocation { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether [leave inner stream open].</summary>
        /// <value>
        ///   <c>true</c> if [leave inner stream open]; otherwise, <c>false</c>.</value>
        public bool LeaveInnerStreamOpen { get; set; }

        /// <summary>
        /// Gets or sets the certificate source.
        /// 'file': read certificate from a file. Password maybe need to open it.
        /// 'store': read certificate from a windows store. StoreName, StoreLocation and Subject are neccessary to fill out.
        /// 'frombase64': parse certificate from a base64 string. Password is neccessary.
        /// 'generatenew': generate a new self-signed certificate. Subject, password and optionally CertValidStartDate and CertExpiryDate properties required.
        /// </summary>
        /// <value>The certificate source.</value>
        public string CertificateSource { get; set; } = "generatenew";

        /// <summary>Gets or sets the password for the certificate.</summary>
        /// <value>The password.</value>
        public string Password { get; set; } = Guid.NewGuid().ToString();

        /// <summary>Gets or sets the cert valid start date.</summary>
        /// <value>The cert valid start date.</value>
        public DateTime CertValidStartDate { get; set; } = DateTime.Now.AddDays(-1);

        /// <summary>Gets or sets the cert expiry date.</summary>
        /// <value>The cert expiry date.</value>
        public DateTime CertExpiryDate { get; set; } = DateTime.MaxValue;

        /// <summary>Gets or sets the name of the store.</summary>
        /// <value>The name of the store.</value>
        public StoreName StoreName { get; set; } = StoreName.My;

        /// <summary>Gets or sets the store location.</summary>
        /// <value>The store location.</value>
        public StoreLocation StoreLocation { get; set; } = StoreLocation.CurrentUser;

        /// <summary>Gets or sets the subject.</summary>
        /// <value>The subject.</value>
        public string Subject { get; set; } = "ForgeNetRemoting";

        /// <summary>Gets or sets the certificate file.</summary>
        /// <value>The certificate file.</value>
        public string CertificateFile { get; set; } = "DefaultCert.pfx";

        /// <summary>Gets or sets the certificate base64.</summary>
        /// <value>The certificate base64.</value>
        public string CertificateBase64 { get; set; }

    }

}
