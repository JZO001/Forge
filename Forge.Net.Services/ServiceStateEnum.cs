/* *********************************************************************
 * Date: 11 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

namespace Forge.Net.Services
{

    /// <summary>
    /// Represents the state of a service
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public enum ServiceStateEnum
    {
        /// <summary>
        /// The service is unavailable
        /// </summary>
        Unavailable = 0,

        /// <summary>
        /// The service is available
        /// </summary>
        Available

    }

}
