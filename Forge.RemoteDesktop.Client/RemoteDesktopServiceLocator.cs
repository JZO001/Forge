/* *********************************************************************
 * Date: 9 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Net.Services.Locators;
using Forge.RemoteDesktop.Contracts;

namespace Forge.RemoteDesktop.Client
{

    /// <summary>
    /// Finds the Remote Desktop Services on the TerraGraf network and creates proxies
    /// </summary>
    public sealed class RemoteDesktopServiceLocator : RemoteServiceLocator<IRemoteDesktop, RemoteDesktopClientImpl, RemoteDesktopServiceLocator>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDesktopServiceLocator"/> class.
        /// </summary>
        public RemoteDesktopServiceLocator()
            : base(Consts.SERVICE_ID)
        {
        }

    }

}
