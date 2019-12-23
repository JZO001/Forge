/* *********************************************************************
 * Date: 21 Nov 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Net.Remoting;

namespace Forge.TimeServer.Contracts
{

    /// <summary>
    /// Represents the interface for Time Server service
    /// </summary>
    [ServiceContract(WellKnownObjectMode = WellKnownObjectModeEnum.Singleton)]
    public interface ITimeServer : IRemoteContract
    {

        /// <summary>
        /// Gets the UTC date time.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        long GetUTCDateTime();

    }

}
