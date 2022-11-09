/* *********************************************************************
 * Date: 12 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Net.Services.Locators;
using Forge.UpdateFramework.Client.Proxy;
using Forge.UpdateFramework.Contracts;

namespace Forge.UpdateFramework.Client
{

    /// <summary>
    /// Finds the Update Framework Service on the TerraGraf network and creates proxies
    /// </summary>
    internal sealed class UpdateServiceLocator : RemoteServiceLocator<IUpdateService, UpdateServiceClientImpl, UpdateServiceLocator>
    {

        #region Constructor(s)
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateServiceLocator"/> class.
        /// </summary>
        public UpdateServiceLocator()
            : base(Consts.UPDATE_SERVICE_ID)
        {
        } 

        #endregion

    }

}
