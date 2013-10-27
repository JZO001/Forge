/* *********************************************************************
 * Date: 02 May 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Security.Permissions;

namespace Forge.Persistence.StorageProviders.ConfigSection
{

    /// <summary>
    /// Configuration access helper class for storages
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Unrestricted = true)]
    public class StorageConfiguration : Forge.Configuration.Shared.SharedConfigSettings<StorageSection, StorageConfiguration>
    {

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="StorageConfiguration"/> class.
        /// </summary>
        static StorageConfiguration()
        {
            LOG_PREFIX = "HIBERNATE_STORAGE_PROVIDER";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageConfiguration"/> class.
        /// </summary>
        public StorageConfiguration()
            : base()
        {
        }

        #endregion

    }

}
