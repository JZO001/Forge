/* *********************************************************************
 * Date: 9 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Configuration.Shared;

namespace Forge.ErrorReport.ConfigSection
{

    /// <summary>
    /// Configuration section for remote desktop
    /// </summary>
    [Serializable]
    public class ErrorReportSection : ConfigurationSectionStandard
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReportSection"/> class.
        /// </summary>
        public ErrorReportSection()
            : base()
        {
        }

    }

}
