/* *********************************************************************
 * Date: 15 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.ErrorReport.Contracts;
using Forge.Net.Services.Locators;

namespace Forge.ErrorReport.Client
{

    /// <summary>
    /// Finds the Error Report Services on the TerraGraf network and creates proxies
    /// </summary>
    public sealed class ErrorReportServiceLocator : RemoteServiceLocator<IErrorReportSendContract, ErrorReportClientImpl, ErrorReportServiceLocator>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReportServiceLocator"/> class.
        /// </summary>
        public ErrorReportServiceLocator()
            : base(Consts.SERVICE_ID)
        {
        }

    }

}
