/* *********************************************************************
 * Date: 09 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Security.Permissions;

namespace Forge.UpdateFramework.ConfigSection
{

    /// <summary>
    /// Configuration access helper class for Update Framework
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Unrestricted = true)]
    public class UpdateFrameworkConfiguration : Forge.Configuration.Shared.SharedConfigSettings<UpdateFrameworkSection, UpdateFrameworkConfiguration>
    {

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="UpdateFrameworkConfiguration"/> class.
        /// </summary>
        static UpdateFrameworkConfiguration()
        {
            LOG_PREFIX = "UPDATE_CONFIGURATION";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateFrameworkConfiguration"/> class.
        /// </summary>
        public UpdateFrameworkConfiguration()
            : base()
        {
        }

        #endregion

    }

}
