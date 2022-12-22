/* *********************************************************************
 * Date: 15 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Net.Remoting;

namespace Forge.ErrorReport.Contracts
{

    /// <summary>
    /// Represents the error report contract
    /// </summary>
    [ServiceContract(WellKnownObjectMode = WellKnownObjectModeEnum.Singleton)]
    public interface IErrorReportSendContract : IRemoteContract
    {

        /// <summary>
        /// Sends the error report.
        /// </summary>
        /// <param name="package">The package.</param>
        [OperationContract(AllowParallelExecution = true, CallTimeout = 600000, IsOneWay = true, IsReliable = true)]
        void SendErrorReport(ReportPackage package);

    }

}
