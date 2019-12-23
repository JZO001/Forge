/* *********************************************************************
 * Date: 9 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Security.Permissions;

namespace Forge.ErrorReport.ConfigSection
{

    /// <summary>
    /// Configuration access helper class for remote desktop
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Unrestricted = true)]
    public class ErrorReportConfiguration : Forge.Configuration.Shared.SharedConfigSettings<ErrorReportSection, ErrorReportConfiguration>
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes the <see cref="ErrorReportConfiguration"/> class.
        /// </summary>
        static ErrorReportConfiguration()
        {
            LOG_PREFIX = "ERROR_REPORT_CONFIGURATION";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReportConfiguration"/> class.
        /// </summary>
        public ErrorReportConfiguration()
            : base()
        {
        }

        #endregion

    }

}
