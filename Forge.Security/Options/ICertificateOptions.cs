using System;
using System.Security.Cryptography.X509Certificates;

namespace Forge.Security.Options
{

    /// <summary>
    /// Options for certificate source
    /// </summary>
    public interface ICertificateOptions
    {

        /// <summary>
        /// Gets or sets the certificate source.
        /// 'file': read certificate from a file. Password maybe need to open it.
        /// 'store': read certificate from a windows store. StoreName, StoreLocation and Subject are neccessary to fill out.
        /// 'frombase64': parse certificate from a base64 string. Password is neccessary.
        /// 'generatenew': generate a new self-signed certificate. Subject, password and optionally CertValidStartDate and CertExpiryDate properties required.
        /// </summary>
        /// <value>The certificate source.</value>
        string CertificateSource { get; set; }

        /// <summary>Gets or sets the password for the certificate.</summary>
        /// <value>The password.</value>
        string Password { get; set; }

        /// <summary>Gets or sets the cert valid start date.</summary>
        /// <value>The cert valid start date.</value>
        DateTime CertValidStartDate { get; set; }

        /// <summary>Gets or sets the cert expiry date.</summary>
        /// <value>The cert expiry date.</value>
        DateTime CertExpiryDate { get; set; }

        /// <summary>Gets or sets the name of the store.</summary>
        /// <value>The name of the store.</value>
        StoreName StoreName { get; set; }

        /// <summary>Gets or sets the store location.</summary>
        /// <value>The store location.</value>
        StoreLocation StoreLocation { get; set; }

        /// <summary>Gets or sets the subject.</summary>
        /// <value>The subject.</value>
        string Subject { get; set; }

        /// <summary>Gets or sets the certificate file.</summary>
        /// <value>The certificate file.</value>
        string CertificateFile { get; set; }

        /// <summary>Gets or sets the certificate base64.</summary>
        /// <value>The certificate base64.</value>
        string CertificateBase64 { get; set; }

    }

}
