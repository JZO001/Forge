/* *********************************************************************
 * Date: 12 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.UpdateFramework.Contracts;

namespace Forge.UpdateFramework.Service
{

    /// <summary>
    /// Update service implementation proxy
    /// </summary>
    public class UpdateServiceServiceImpl : Forge.UpdateFramework.Service.UpdateServiceAbstractServiceProxy
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateServiceServiceImpl"/> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="sessionId">The session id.</param>
        public UpdateServiceServiceImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        /// <summary>
        /// Sends the client content description.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public override UpdateResponseArgs SendClientContentDescription(UpdateRequestArgs args)
        {
            throw new System.NotImplementedException();
        }

    }

}
