/* *********************************************************************
 * Date: 24 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Configuration;

namespace Forge.ErrorReport.Filter
{

    /// <summary>
    /// Represents an error report filter
    /// </summary>
    public interface IErrorReportFilter : IInitializable
    {

        /// <summary>
        /// Filters the specified package.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <returns>True, if the filter criterias matches, otherwise False.</returns>
        bool Filter(ReportPackage package);

    }

}
