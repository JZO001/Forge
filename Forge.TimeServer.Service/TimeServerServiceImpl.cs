/* *********************************************************************
 * Date: 21 Nov 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.TimeServer.Service
{

    /// <summary>
    /// Time Server singleton implementation
    /// </summary>
    public class TimeServerServiceImpl : Forge.MBRBase, Forge.TimeServer.Contracts.ITimeServer
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeServerServiceImpl"/> class.
        /// </summary>
        public TimeServerServiceImpl()
        {
        }

        /// <summary>
        /// Gets the UTC date time.
        /// </summary>
        /// <returns></returns>
        public long GetUTCDateTime()
        {
            return DateTime.UtcNow.Ticks;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

    }

}
