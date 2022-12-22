/* *********************************************************************
 * Date: 21 Nov 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Net.Services.Locators;
using Forge.TimeServer.Contracts;

namespace Forge.TimeServer.Client
{

    /// <summary>
    /// Finds the time server service on the network
    /// </summary>
    public sealed class TimeServerServiceLocator : RemoteServiceLocator<ITimeServer, TimeServerClientImpl, TimeServerServiceLocator>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeServerServiceLocator"/> class.
        /// </summary>
        public TimeServerServiceLocator()
            : base(Consts.SERVICE_ID)
        {
        }

    }

}
