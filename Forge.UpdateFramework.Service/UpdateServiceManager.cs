/* *********************************************************************
 * Date: 12 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Net.Remoting.Channels;
using Forge.Net.Services.Services;
using Forge.UpdateFramework.Contracts;

namespace Forge.UpdateFramework.Service
{

    /// <summary>
    /// Update service implementation
    /// </summary>
    public sealed class UpdateServiceManager : RemoteServiceBase<IUpdateService, UpdateServiceServiceImpl, UpdateServiceManager>
    {

        #region Constructor(s)
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateServiceManager"/> class.
        /// </summary>
        public UpdateServiceManager()
            : base(Consts.UPDATE_SERVICE_ID)
        {
        } 

        #endregion

        protected override Net.Remoting.Channels.Channel LookUpChannel()
        {
            Channel channel = base.LookUpChannel();

            // TODO: register remote admin contract if needs...

            return channel;
        }

    }

}
