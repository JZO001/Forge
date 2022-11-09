/* *********************************************************************
 * Date: 12 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

namespace Forge.UpdateFramework.Client.Proxy
{

    /// <summary>
    /// Update service client side proxy
    /// </summary>
    public class UpdateServiceClientImpl : Forge.UpdateFramework.Client.Proxy.UpdateServiceAbstractClientProxy
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateServiceClientImpl"/> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="sessionId">The session id.</param>
        public UpdateServiceClientImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        /// <summary>
        /// Service orders the client to execute the update search process
        /// </summary>
        public override void CheckUpdatePush()
        {
            
        }

    }

}
