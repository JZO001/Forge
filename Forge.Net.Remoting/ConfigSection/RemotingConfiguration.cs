/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Security.Permissions;

namespace Forge.Net.Remoting.ConfigSection
{

    /// <summary>
    /// Configuration access helper class for remoting
    /// </summary>
#if NETFRAMEWORK
    [SecurityPermission(SecurityAction.Demand, Unrestricted = true)]
#endif
    public class RemotingConfiguration : Forge.Configuration.Shared.SharedConfigSettings<RemotingSection, RemotingConfiguration>
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes the <see cref="RemotingConfiguration"/> class.
        /// </summary>
        static RemotingConfiguration()
        {
            LOG_PREFIX = "REMOTING_CONFIGURATION";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemotingConfiguration"/> class.
        /// </summary>
        public RemotingConfiguration()
            : base()
        {
        }

        #endregion

    }

}
