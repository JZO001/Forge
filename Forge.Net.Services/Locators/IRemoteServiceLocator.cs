/* *********************************************************************
 * Date: 11 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Collections;
using Forge.Management;
using Forge.Net.Remoting;

namespace Forge.Net.Services.Locators
{

    /// <summary>
    /// Represents the base services of a service locator
    /// </summary>
    /// <typeparam name="TIProxyType">Generic proxy type.</typeparam>
    public interface IRemoteServiceLocator<TIProxyType> : IManager where TIProxyType : IRemoteContract
    {

        /// <summary>
        /// Occurs when [event service state changed].
        /// </summary>
        event EventHandler<ServiceStateChangedEventArgs> EventServiceStateChanged;

        /// <summary>
        /// Occurs when [event available service providers changed].
        /// </summary>
        event EventHandler<ServiceProvidersChangedEventArgs> EventAvailableServiceProvidersChanged;

        /// <summary>
        /// Occurs when [event prefered service provider changed].
        /// </summary>
        event EventHandler<PreferedServiceProviderChangedEventArgs> EventPreferedServiceProviderChanged;

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        string Id { get; }

        /// <summary>
        /// Gets the channel id.
        /// </summary>
        /// <value>
        /// The channel id.
        /// </value>
        string ChannelId { get; }

        /// <summary>
        /// Gets the state of the service.
        /// </summary>
        /// <value>
        /// The state of the service.
        /// </value>
        ServiceStateEnum ServiceState { get; }

        /// <summary>
        /// Gets the available service providers.
        /// </summary>
        /// <value>
        /// The available service providers.
        /// </value>
        ListSpecialized<ServiceProvider> AvailableServiceProviders { get; }

        /// <summary>
        /// Gets the prefered service provider.
        /// </summary>
        /// <value>
        /// The prefered service provider.
        /// </value>
        ServiceProvider PreferedServiceProvider { get; }

        /// <summary>
        /// Gets the proxy.
        /// </summary>
        /// <returns>Proxy instance</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        TIProxyType GetProxy();

    }

}
