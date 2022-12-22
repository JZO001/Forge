/* *********************************************************************
 * Date: 06 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Net.Remoting;

namespace Forge.RemoteDesktop.Contracts
{

    /// <summary>
    /// Represents the communication contract of the remote desktop system
    /// </summary>
    [ServiceContract(WellKnownObjectMode = WellKnownObjectModeEnum.PerSession)]
    public interface IRemoteDesktop : IRemoteDesktopClient, IRemoteDesktopService
    {
    }

}
