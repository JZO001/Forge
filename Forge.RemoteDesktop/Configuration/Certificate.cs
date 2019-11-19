/* *********************************************************************
 * Date: 9 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Forge.Configuration.Shared;
using Forge.RemoteDesktop.ConfigSection;
using Forge.Security;

namespace Forge.RemoteDesktop.Configuration
{

    /// <summary>
    /// Load certificate from configuration
    /// </summary>
    public static class Certificate
    {

        #region Field(s)
        
        private static X509Certificate2 mCertificate = null;

        #endregion

        #region Constructor(s)
        
        /// <summary>
        /// Initializes the <see cref="Certificate"/> class.
        /// </summary>
        static Certificate()
        {
            RemoteDesktopConfiguration.SectionHandler.OnConfigurationChanged += new System.EventHandler<System.EventArgs>(SectionHandler_OnConfigurationChanged);
        }

        #endregion

        #region Public method(s)
        
        /// <summary>
        /// Gets the X509 certificate.
        /// </summary>
        /// <value>
        /// The X509 certificate.
        /// </value>
        public static X509Certificate2 X509Certificate
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (mCertificate == null)
                {
                    CertificateConfigurationSource source = new CertificateConfigurationSource();
                    source.Initialize(ConfigurationAccessHelper.GetCategoryPropertyByPath(RemoteDesktopConfiguration.Settings.CategoryPropertyItems, "Certificate"));
                    mCertificate = source.Certificate;
                }
                return mCertificate;
            }
        }

        #endregion

        #region Private method(s)
        
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void SectionHandler_OnConfigurationChanged(object sender, System.EventArgs e)
        {
            mCertificate = null;
        } 

        #endregion

    }

}
