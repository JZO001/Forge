/* *********************************************************************
 * Date: 6 Sep 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Net.Services.Locators
{

    /// <summary>
    /// Represents the prefered service provider event argument
    /// </summary>
    [Serializable]
    public class PreferedServiceProviderChangedEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PreferedServiceProviderChangedEventArgs" /> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public PreferedServiceProviderChangedEventArgs(ServiceProvider serviceProvider)
        {
            this.PreferedServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the prefered service provider.
        /// </summary>
        /// <value>
        /// The prefered service provider.
        /// </value>
        public ServiceProvider PreferedServiceProvider { get; private set; }

    }

}
