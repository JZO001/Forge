﻿/* *********************************************************************
 * Date: 11 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Forge.Collections;
using Forge.Configuration;
using Forge.Configuration.Shared;
using Forge.Management;
using Forge.Net.Remoting;
using Forge.Net.Remoting.Channels;
using Forge.Net.Remoting.Proxy;
using Forge.Net.Remoting.Sinks;
using Forge.Net.Services.ConfigSection;
using Forge.Net.Synapse;
using Forge.Net.Synapse.NetworkFactory;
using Forge.Net.TerraGraf;
using Forge.Net.TerraGraf.Contexts;
using Forge.Net.TerraGraf.NetworkPeers;
using log4net;

namespace Forge.Net.Services.Locators
{

    /// <summary>
    /// Base implementation for remote service locators
    /// </summary>
    /// <typeparam name="TIProxyType">The type of the I proxy type.</typeparam>
    /// <typeparam name="TProxyImplementationType">The type of the proxy impl type.</typeparam>
    /// <typeparam name="TLocatorType">The type of the locator type.</typeparam>
    public abstract class RemoteServiceLocator<TIProxyType, TProxyImplementationType, TLocatorType> : ManagerBase, IRemoteServiceLocator<TIProxyType>
        where TIProxyType : IRemoteContract
        where TProxyImplementationType : ProxyBase
        where TLocatorType : RemoteServiceLocator<TIProxyType, TProxyImplementationType, TLocatorType>
    {

        #region Field(s)

        private readonly ListSpecialized<ServiceProvider> mAvailableServiceProviders = new ListSpecialized<ServiceProvider>();

        /// <summary>
        /// The peers vs providers
        /// </summary>
        protected readonly Dictionary<INetworkPeer, ServiceProvider> mPeersVsProviders = new Dictionary<INetworkPeer, ServiceProvider>();

        /// <summary>
        /// Logger
        /// </summary>
        protected static readonly ILog LOGGER = LogManager.GetLogger(typeof(TLocatorType));

        /// <summary>
        /// Log prefix for log messages
        /// </summary>
        protected readonly string LOG_PREFIX = typeof(TLocatorType).Name;

        private ServiceProvider mPreferedServiceProvider = null;

        /// <summary>
        /// Occurs when [event service state changed].
        /// </summary>
        public event EventHandler<ServiceStateChangedEventArgs> EventServiceStateChanged;

        /// <summary>
        /// Occurs when [event available service providers changed].
        /// </summary>
        public event EventHandler<ServiceProvidersChangedEventArgs> EventAvailableServiceProvidersChanged;

        /// <summary>
        /// Occurs when [event prefered service provider changed].
        /// </summary>
        public event EventHandler<PreferedServiceProviderChangedEventArgs> EventPreferedServiceProviderChanged;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteServiceLocator&lt;TIProxyType, TProxyImplType, TLocatorType&gt;"/> class.
        /// </summary>
        /// <param name="locatorId">The locator id.</param>
        protected RemoteServiceLocator(string locatorId)
        {
            if (string.IsNullOrEmpty(locatorId))
            {
                ThrowHelper.ThrowArgumentNullException("locatorId");
            }

            this.Id = locatorId;
            this.ServiceState = ServiceStateEnum.Unavailable;
            this.ChannelId = string.Empty;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets the channel id.
        /// </summary>
        /// <value>
        /// The channel id.
        /// </value>
        public string ChannelId { get; protected set; }

        /// <summary>
        /// Gets the state of the service.
        /// </summary>
        /// <value>
        /// The state of the service.
        /// </value>
        public ServiceStateEnum ServiceState
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the available service providers.
        /// </summary>
        /// <value>
        /// The available service providers.
        /// </value>
        public ListSpecialized<ServiceProvider> AvailableServiceProviders
        {
            get
            {
                lock (mAvailableServiceProviders)
                {
                    return new ListSpecialized<ServiceProvider>(mAvailableServiceProviders);
                }
            }
            protected set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }

                lock (mAvailableServiceProviders)
                {
                    mAvailableServiceProviders.Clear();
                    mAvailableServiceProviders.AddRange(value);
                    OnAvailableServiceProviderChanged(mAvailableServiceProviders);
                }
            }
        }

        /// <summary>
        /// Gets or sets the prefered service provider.
        /// </summary>
        /// <value>
        /// The prefered service provider.
        /// </value>
        public ServiceProvider PreferedServiceProvider 
        {
            get { return mPreferedServiceProvider; } 
            protected set
            {
                mPreferedServiceProvider = value;
                OnPreferedServiceProviderChanged(value);
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns>
        /// Manager State
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override ManagerStateEnum Start()
        {
            if (this.ManagerState != ManagerStateEnum.Started)
            {
                if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("{0}, initializing locator...", LOG_PREFIX));

                OnStart(ManagerEventStateEnum.Before);

                try
                {
                    Forge.Net.TerraGraf.NetworkManager.Instance.Start();
                    Forge.Net.Remoting.Channels.ChannelServices.Initialize();
                    Forge.Net.Remoting.Proxy.ProxyServices.Initialize();

                    // check channel
                    this.ChannelId = ConfigurationAccessHelper.GetValueByPath(NetworkServiceConfiguration.Settings.CategoryPropertyItems, string.Format("Locators/{0}", Id));
                    if (string.IsNullOrEmpty(this.ChannelId))
                    {
                        this.ChannelId = Id;
                    }

                    Channel channel = LookUpChannel();

                    if (!ProxyServices.IsContractRegistered(typeof(TIProxyType)))
                    {
                        ProxyServices.RegisterContract(typeof(TIProxyType), channel.ChannelId, typeof(TProxyImplementationType));
                    }

                    // init TerraGraf and find the service
                    Forge.Net.TerraGraf.NetworkManager.Instance.NetworkPeerContextChanged += new EventHandler<NetworkPeerContextEventArgs>(NetworkPeerContextChangedEventHandler);
                    Forge.Net.TerraGraf.NetworkManager.Instance.NetworkPeerDiscovered += new EventHandler<NetworkPeerChangedEventArgs>(NetworkPeerDiscoveredEventHandler);
                    Forge.Net.TerraGraf.NetworkManager.Instance.NetworkPeerUnaccessible += new EventHandler<NetworkPeerChangedEventArgs>(NetworkPeerUnaccessibleEventHandler);
                    Forge.Net.TerraGraf.NetworkManager.Instance.NetworkPeerContextChanged += new EventHandler<NetworkPeerContextEventArgs>(NetworkPeerContextChangedEventHandler);

                    FindService();

                    this.ManagerState = ManagerStateEnum.Started;

                    if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("{0}, locator successfully initialized.", LOG_PREFIX));
                }
                catch (Exception)
                {
                    this.ManagerState = ManagerStateEnum.Fault;
                    throw;
                }
                finally
                {
                    OnStart(ManagerEventStateEnum.After);
                }
            }
            return this.ManagerState;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <returns>
        /// Manager State
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override ManagerStateEnum Stop()
        {
            if (this.ManagerState == ManagerStateEnum.Started)
            {
                OnStop(ManagerEventStateEnum.Before);
                Forge.Net.TerraGraf.NetworkManager.Instance.NetworkPeerContextChanged -= new EventHandler<NetworkPeerContextEventArgs>(NetworkPeerContextChangedEventHandler);
                Forge.Net.TerraGraf.NetworkManager.Instance.NetworkPeerDiscovered -= new EventHandler<NetworkPeerChangedEventArgs>(NetworkPeerDiscoveredEventHandler);
                Forge.Net.TerraGraf.NetworkManager.Instance.NetworkPeerUnaccessible -= new EventHandler<NetworkPeerChangedEventArgs>(NetworkPeerUnaccessibleEventHandler);
                Forge.Net.TerraGraf.NetworkManager.Instance.NetworkPeerContextChanged -= new EventHandler<NetworkPeerContextEventArgs>(NetworkPeerContextChangedEventHandler);
                this.ManagerState = ManagerStateEnum.Stopped;
                OnStop(ManagerEventStateEnum.After);
            }
            return this.ManagerState;
        }

        /// <summary>
        /// Gets the proxy.
        /// </summary>
        /// <returns>Proxy instance</returns>
        /// <exception cref="Forge.InitializationException"></exception>
        /// <exception cref="ServiceNotAvailableException"></exception>
        public virtual TIProxyType GetProxy()
        {
            if (this.ManagerState != ManagerStateEnum.Started)
            {
                throw new InitializationException();
            }

            ServiceProvider provider = this.PreferedServiceProvider;
            if (provider == null)
            {
                throw new ServiceNotAvailableException();
            }

            ProxyFactory<TIProxyType> factory = new ProxyFactory<TIProxyType>(this.ChannelId, provider.RemoteEndPoint);
            return factory.CreateProxy();
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Called when [service state changed].
        /// </summary>
        /// <param name="state">The state.</param>
        protected virtual void OnServiceStateChanged(ServiceStateEnum state)
        {
            this.ServiceState = state;
            RaiseEvent(EventServiceStateChanged, this, new ServiceStateChangedEventArgs(state));
        }

        /// <summary>
        /// Called when [available service provider changed].
        /// </summary>
        /// <param name="serviceProviders">The service providers.</param>
        protected virtual void OnAvailableServiceProviderChanged(ListSpecialized<ServiceProvider> serviceProviders)
        {
            RaiseEvent(EventAvailableServiceProvidersChanged, this, new ServiceProvidersChangedEventArgs(serviceProviders));
        }

        /// <summary>
        /// Called when [prefered service provider changed].
        /// </summary>
        /// <param name="provider">The provider.</param>
        protected virtual void OnPreferedServiceProviderChanged(ServiceProvider provider)
        {
            RaiseEvent(EventPreferedServiceProviderChanged, this, new PreferedServiceProviderChangedEventArgs(provider));
        }

        /// <summary>
        /// Looks up channel.
        /// </summary>
        /// <returns></returns>
        protected virtual Channel LookUpChannel()
        {
            Channel channel = ChannelServices.GetChannelById(this.ChannelId);
            if (channel == null)
            {
                // regisztrálás
                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("{0}, locator cannot find channel with id '{1}'. Create default channel.", LOG_PREFIX, this.ChannelId));
                BinaryMessageSink sink = new BinaryMessageSink(true, 1024);
                List<IMessageSink> sinks = new List<IMessageSink>();
                sinks.Add(sink);

                TerraGrafNetworkFactory networkFactory = new TerraGrafNetworkFactory();

                DefaultServerStreamFactory serverStreamFactory = new DefaultServerStreamFactory();
                DefaultClientStreamFactory clientStreamFactory = new DefaultClientStreamFactory();

                channel = new TCPChannel(this.ChannelId, sinks, sinks, networkFactory, serverStreamFactory, clientStreamFactory);

                ChannelServices.RegisterChannel(channel);
            }

            return channel;
        }

        /// <summary>
        /// Finds the service providers.
        /// </summary>
        protected virtual void FindService()
        {
            ListSpecialized<ServiceProvider> providers = new ListSpecialized<ServiceProvider>();

            foreach (NetworkContext nc in Forge.Net.TerraGraf.Contexts.NetworkContext.KnownNetworkContexts)
            {
                foreach (INetworkPeerRemote peer in nc.KnownNetworkPeers)
                {
                    if (peer.Distance > 0)
                    {
                        ServiceProvider provider = CheckServiceAvailable(peer);
                        if (provider != null && !providers.Contains(provider))
                        {
                            providers.Add(provider);
                        }
                    }
                }
            }

            ServiceProvider localhost = CheckServiceAvailable(Forge.Net.TerraGraf.NetworkManager.Instance.Localhost);
            if (localhost != null && !providers.Contains(localhost))
            {
                providers.Add(localhost);
            }

            if (providers.Count > 0)
            {
                // vannak kiszolgálók
                providers.Sort();
                ServiceProvider provider = providers[0];
                if (this.PreferedServiceProvider == null || !provider.RemoteEndPoint.Equals(this.PreferedServiceProvider.RemoteEndPoint))
                {
                    if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("{0}, selected service host is '{1}' on port {2}.", LOG_PREFIX, provider.RemoteEndPoint.Host, provider.RemoteEndPoint.Port.ToString()));
                    this.PreferedServiceProvider = provider;
                }

                this.AvailableServiceProviders = providers;

                if (this.ServiceState == ServiceStateEnum.Unavailable)
                {
                    OnServiceStateChanged(ServiceStateEnum.Available);
                }
            }
            else
            {
                if (this.PreferedServiceProvider != null)
                {
                    this.PreferedServiceProvider = null;
                }
                if (this.AvailableServiceProviders.Count == 0)
                {
                    // eddig sem voltak kiszolgálók
                    if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("{0}, service still unavailable.", LOG_PREFIX));
                }
                else
                {
                    // most már nincs kiszolgáló
                    if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("{0}, service unavailable.", LOG_PREFIX));
                    this.AvailableServiceProviders = providers;
                    OnServiceStateChanged(ServiceStateEnum.Unavailable);
                }
            }

            providers.Clear();
        }

        /// <summary>
        /// Checks the service available.
        /// </summary>
        /// <param name="peer">The peer.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Int64.TryParse(System.String,System.Int64@)")]
        protected virtual ServiceProvider CheckServiceAvailable(INetworkPeer peer)
        {
            if (peer == null) ThrowHelper.ThrowArgumentNullException("peer");

            ServiceProvider result = null;

            NetworkPeerDataContext context = peer.PeerContext;

            if (context != null && context.PropertyItems.ContainsKey(Id))
            {
                PropertyItem root = context.PropertyItems[Id];
                string contextData = root.Value;
                if (!string.IsNullOrEmpty(contextData))
                {
                    string[] values = contextData.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length > 0)
                    {
                        long priority = 0;
                        int port = 0;
                        if (int.TryParse(values[0], out port) && port >= 0 && port <= 65535)
                        {
                            if (values.Length > 1)
                            {
                                long.TryParse(values[1], out priority);
                            }
                            Dictionary<string, PropertyItem> properties = null;
                            if (root.PropertyItems.Count > 0)
                            {
                                properties = new Dictionary<string, PropertyItem>(root.PropertyItems);
                            }
                            if (mPeersVsProviders.ContainsKey(peer))
                            {
                                result = mPeersVsProviders[peer];
                                result.Priority = priority;
                                result.Properties = properties;
                                result.RemoteEndPoint = new AddressEndPoint(peer.Id, port);
                            }
                            else
                            {
                                result = new ServiceProvider(peer, new AddressEndPoint(peer.Id, port), priority, properties);
                                mPeersVsProviders.Add(peer, result);
                            }
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        #region Private method(s)

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void NetworkPeerContextChangedEventHandler(object sender, NetworkPeerContextEventArgs e)
        {
            if (this.ManagerState == ManagerStateEnum.Started)
            {
                FindService();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void NetworkPeerDiscoveredEventHandler(object sender, NetworkPeerChangedEventArgs e)
        {
            if (this.ManagerState == ManagerStateEnum.Started)
            {
                FindService();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void NetworkPeerUnaccessibleEventHandler(object sender, NetworkPeerChangedEventArgs e)
        {
            if (this.ManagerState == ManagerStateEnum.Started)
            {
                FindService();
            }
        }

        #endregion

    }

}
