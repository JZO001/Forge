/* *********************************************************************
 * Date: 09 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Net.Remoting;

namespace Forge.UpdateFramework.Contracts
{

    /// <summary>
    /// Update service communication interface
    /// </summary>
    [ServiceContract(WellKnownObjectMode = WellKnownObjectModeEnum.PerSession)]
    public interface IUpdateService : IRemoteContract
    {

        /// <summary>
        /// Service orders the client to execute the update search process
        /// </summary>
        [OperationContract(Direction = OperationDirectionEnum.ClientSide, IsOneWay = true)]
        void CheckUpdatePush();

        /// <summary>
        /// Sends the client content description.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        [OperationContract(CallTimeout = 600000, ReturnTimeout = 600000)]
        UpdateResponseArgs SendClientContentDescription(UpdateRequestArgs args);

    }

}
