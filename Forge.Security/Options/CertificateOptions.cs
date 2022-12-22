using System;
using System.Security.Cryptography.X509Certificates;

namespace Forge.Security.Options
{

    /// <summary>
    /// Options for certificate source
    /// </summary>
    public class CertificateOptions : ICertificateOptions
    {

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
