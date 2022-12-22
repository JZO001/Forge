/* *********************************************************************
 * Date: 22 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Configuration;
using Forge.ErrorReport.Filter;

namespace Forge.ErrorReport.Sink
{

    /// <summary>
    /// Represents a report package processor
    /// </summary>
    public interface IErrorReportPackageSink : IInitializable
    {

        /// <summary>
        /// Gets the sink unique identifier.
        /// </summary>
        /// <value>
        /// The sink unique identifier.
        /// </value>
        string SinkId { get; }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        IErrorReportFilter Filter { get; set; }

        /// <summary>
        /// Processes the report package.
        /// </summary>
        /// <param name="package">The package.</param>
        void ProcessReportPackage(ReportPackage package);

        /// <summary>
        /// Closes the sink.
        /// </summary>
        void Close();

    }

}
