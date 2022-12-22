/* *********************************************************************
 * Date: 11 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Security.Permissions;

namespace Forge.Net.Services.ConfigSection
{

    /// <summary>
    /// Configuration access helper class for Network Services
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2143:TransparentMethodsShouldNotDemandFxCopRule")]
#if NETFRAMEWORK
    [SecurityPermission(SecurityAction.Demand, Unrestricted = true)]
#endif
    public class NetworkServiceConfiguration : Forge.Configuration.Shared.SharedConfigSettings<NetworkServiceSection, NetworkServiceConfiguration>
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes the <see cref="NetworkServiceConfiguration"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static NetworkServiceConfiguration()
        {
            LOG_PREFIX = "NETWORK_SERVICES_CONFIGURATION";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkServiceConfiguration"/> class.
        /// </summary>
        public NetworkServiceConfiguration()
            : base()
        {
        }

        #endregion

    }

}
