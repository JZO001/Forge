/* *********************************************************************
 * Date: 6 Sep 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Collections;

namespace Forge.Net.Services.Locators
{

    /// <summary>
    /// Represents the list of service providers of an event
    /// </summary>
    [Serializable]
    public class ServiceProvidersChangedEventArgs : EventArgs
    {

        private readonly ListSpecialized<ServiceProvider> mServiceProviders = new ListSpecialized<ServiceProvider>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProvidersChangedEventArgs"/> class.
        /// </summary>
        /// <param name="serviceProviders">The service providers.</param>
        public ServiceProvidersChangedEventArgs(ListSpecialized<ServiceProvider> serviceProviders)
        {
            this.mServiceProviders.AddRange(serviceProviders);
        }

        /// <summary>
        /// Gets the service providers.
        /// </summary>
        /// <value>
        /// The service providers.
        /// </value>
        public ListSpecialized<ServiceProvider> ServiceProviders
        {
            get
            {
                return new ListSpecialized<ServiceProvider>(mServiceProviders);
            }
        }

    }

}
