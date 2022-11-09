/* *********************************************************************
 * Date: 9 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Forge.Configuration;
using Forge.Configuration.Shared;

namespace Forge.Security
{

    /// <summary>
    /// Represents a certificate store configuration
    /// </summary>
    public class CertificateConfigurationSource : MBRBase, IInitializable
    {

        #region Field(s)

        private X509Certificate2 mCertificate = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mInitialized = false;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool IsInitialized
        {
            get { return mInitialized; }
            protected set { mInitialized = value; }
        }

        /// <summary>
        /// Gets or sets the certificate.
        /// </summary>
        /// <value>
        /// The certificate.
        /// </value>
        [DebuggerHidden]
        public X509Certificate2 Certificate
        {
            get { return mCertificate; }
            protected set { mCertificate = value; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes the specified config item.
        /// </summary>
        /// <param name="configItem">The config item.</param>
        /// <exception cref="InvalidConfigurationValueException">
        /// CertificateSource value is invalid.
        /// or
        /// CertificateSource value is invalid.
        /// or
        /// CertificateFile value is invalid.
        /// or
        /// Password value is invalid.
        /// or
        /// Subject value is invalid.
        /// or
        /// CertificateSource value is invalid.
        /// </exception>
        /// <exception cref="InvalidConfigurationException">
        /// Invalid certificate.
        /// or
        /// Failed to find certificate.
        /// or
        /// Failed to find certificate.
        /// </exception>
        public virtual void Initialize(CategoryPropertyItem configItem)
        {
            if (configItem == null)
            {
                ThrowHelper.ThrowArgumentNullException("configItem");
            }
            if (!mInitialized)
            {
                string certSource = ConfigurationAccessHelper.GetValueByPath(configItem.PropertyItems, "CertificateSource");
                if (string.IsNullOrEmpty(certSource))
                {
                    throw new InvalidConfigurationValueException("CertificateSource value is invalid.");
                }
                certSource = certSource.Trim().ToLower();
                if (string.IsNullOrEmpty(certSource))
                {
                    throw new InvalidConfigurationValueException("CertificateSource value is invalid.");
                }
                if ("file".Equals(certSource))
                {
                    string certFile = ConfigurationAccessHelper.GetValueByPath(configItem.PropertyItems, "CertificateFile");
                    if (string.IsNullOrEmpty(certFile))
                    {
                        throw new InvalidConfigurationValueException("CertificateFile value is invalid.");
                    }
                    FileInfo fi = new FileInfo(certFile);
                    if (!fi.Exists)
                    {
                        throw new InvalidConfigurationException(string.Format("Certificate file not found: {0}", fi.FullName));
                    }

                    string passwordString = ConfigurationAccessHelper.GetValueByPath(configItem.PropertyItems, "Password");
                    if (string.IsNullOrEmpty(passwordString))
                    {
                        throw new InvalidConfigurationValueException("Password value is invalid.");
                    }

                    try
                    {
                        mCertificate = new X509Certificate2(fi.FullName, passwordString);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidConfigurationException("Invalid certificate.", ex);
                    }
                }
                else if ("store".Equals(certSource))
                {
                    StoreName storeName = StoreName.My;
                    StoreLocation storeLocation = StoreLocation.CurrentUser;

                    string storeNameStr = ConfigurationAccessHelper.GetValueByPath(configItem.PropertyItems, "StoreName");
                    if (!string.IsNullOrEmpty(storeNameStr))
                    {
                        storeName = (StoreName)Enum.Parse(typeof(StoreName), storeNameStr);
                    }

                    string storeLocationStr = ConfigurationAccessHelper.GetValueByPath(configItem.PropertyItems, "StoreLocation");
                    if (!string.IsNullOrEmpty(storeLocationStr))
                    {
                        storeLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), storeLocationStr);
                    }

                    string subject = ConfigurationAccessHelper.GetValueByPath(configItem.PropertyItems, "Subject");
                    if (string.IsNullOrEmpty(subject))
                    {
                        throw new InvalidConfigurationValueException("Subject value is invalid.");
                    }

                    X509Store store = new X509Store(storeName, storeLocation);
                    store.Open(OpenFlags.ReadOnly);
                    try
                    {
                        foreach (X509Certificate2 c in store.Certificates)
                        {
                            if (c.Subject.Equals(subject))
                            {
                                mCertificate = c;
                                break;
                            }
                        }
                        if (mCertificate == null)
                        {
                            throw new InvalidConfigurationException("Failed to find certificate.");
                        }
                    }
                    catch (InvalidConfigurationException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidConfigurationException("Failed to find certificate.", ex);
                    }
                    finally
                    {
                        store.Close();
                    }
                }
                else if ("frombase64".Equals(certSource))
                {
                    string certBase64 = ConfigurationAccessHelper.GetValueByPath(configItem.PropertyItems, "CertificateBase64");
                    if (string.IsNullOrEmpty(certBase64))
                    {
                        throw new InvalidConfigurationValueException("CertificateBase64 value is invalid.");
                    }

                    byte[] data = Convert.FromBase64String(certBase64);

                    string passwordString = ConfigurationAccessHelper.GetValueByPath(configItem.PropertyItems, "Password");
                    if (string.IsNullOrEmpty(passwordString))
                    {
                        throw new InvalidConfigurationValueException("Password value is invalid.");
                    }

                    try
                    {
                        mCertificate = new X509Certificate2(data, passwordString);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidConfigurationException("Invalid certificate.", ex);
                    }
                }
                else if ("generatenew".Equals(certSource))
                {
                    string subject = ConfigurationAccessHelper.GetValueByPath(configItem.PropertyItems, "Subject");
                    if (string.IsNullOrEmpty(subject))
                    {
                        throw new InvalidConfigurationValueException("Subject value is invalid.");
                    }

                    string passwordString = ConfigurationAccessHelper.GetValueByPath(configItem.PropertyItems, "Password");
                    if (string.IsNullOrEmpty(passwordString))
                    {
                        passwordString = Guid.NewGuid().ToString();
                    }

                    DateTime dtCertStart = DateTime.UtcNow.AddDays(-1);
                    string certStartDate = ConfigurationAccessHelper.GetValueByPath(configItem.PropertyItems, "CertValidStartDate");
                    if (!string.IsNullOrEmpty(certStartDate))
                    {
                        DateTime.TryParse(certStartDate, out dtCertStart);
                    }

                    DateTime dtCertExpired = DateTime.MaxValue;
                    string certExpiry = ConfigurationAccessHelper.GetValueByPath(configItem.PropertyItems, "CertExpiryDate");
                    if (!string.IsNullOrEmpty(certExpiry))
                    {
                        DateTime.TryParse(certExpiry, out dtCertExpired);
                    }

                    try
                    {
                        byte[] data = CertificateFactory.CreateSelfSignCertificatePfx(subject, dtCertStart, dtCertExpired, passwordString);
                        mCertificate = new X509Certificate2(data, passwordString);
#if NET40 || NETSTANDARD2_1
                        mCertificate.FriendlyName = "Generated Certificate";
#endif
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidConfigurationException("Invalid certificate.", ex);
                    }
                }
                else
                {
                    throw new InvalidConfigurationValueException("CertificateSource value is invalid.");
                }

                this.mInitialized = true;
            }
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Does the initialize check.
        /// </summary>
        protected void DoInitializeCheck()
        {
            if (!mInitialized)
            {
                throw new InitializationException();
            }
        }

        #endregion

    }

}
