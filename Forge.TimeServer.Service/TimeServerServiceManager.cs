/* *********************************************************************
 * Date: 21 Nov 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Net.Services.Services;
using Forge.TimeServer.Contracts;

namespace Forge.TimeServer.Service
{

    /// <summary>
    /// Time Server Service Manager
    /// </summary>
    public sealed class TimeServerServiceManager : RemoteServiceBase<ITimeServer, TimeServerServiceImpl, TimeServerServiceManager>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeServerServiceManager"/> class.
        /// </summary>
        public TimeServerServiceManager()
            : base(Consts.SERVICE_ID)
        {
        }

    }

}
