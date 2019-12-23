/* *********************************************************************
 * Date: 11 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Forge.Configuration;
using Forge.Configuration.Shared;
using Forge.Logging;
using Forge.Management;
using Forge.Net.Remoting;
using Forge.Net.Remoting.Channels;
using Forge.Net.Remoting.Service;
using Forge.Net.Remoting.Sinks;
using Forge.Net.Services.ConfigSection;
using Forge.Net.Synapse;
using Forge.Net.Synapse.NetworkFactory;
using Forge.Net.TerraGraf;
using Forge.Net.TerraGraf.Contexts;

namespace Forge.Net.Services.Services
{

    /// <summary>
    /// Base implementation for remote services
    /// </summary>
    /// <typeparam name="TIServiceProxyType">The type of the I service proxy type.</typeparam>
    /// <typeparam name="TServiceProxyImplementationType">The type of the service proxy impl type.</typeparam>
    /// <typeparam name="TServiceType">The type of the service type.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class RemoteServiceBase<TIServiceProxyType, TServiceProxyImplementationType, TServiceType> : ManagerBase, IRemoteService
        where TIServiceProxyType : IRemoteContract
        where TServiceProxyImplementationType : IRemoteContract
        where TServiceType : RemoteServiceBase<TIServiceProxyType, TServiceProxyImplementationType, TServiceType>, new()
    {

        #region Field(s)

        private static TServiceType mSingletonInstance = default(TServiceType);

        /// <summary>
        /// Logger
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        protected readonly ILog LOGGER = LogManager.GetLogger(typeof(TServiceType));

        /// <summary>
        /// Log prefix for log messages
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        protected readonly string LOG_PREFIX = typeof(TServiceType).Name;

        /// <summary>
        /// Priority of the service
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        protected long mPriority = 0;

        /// <summary>
        /// The service descriptor
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        protected IServiceDescriptor mServiceDescriptor = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the RemoteServiceBase class.
        /// </summary>
        /// <param name="serviceId">The service id.</param>
        protected RemoteServiceBase(string serviceId)
        {
            if (string.IsNullOrEmpty(serviceId))
            {
                ThrowHelper.ThrowArgumentNullException("serviceId");
            }

            this.Id = serviceId;
            this.ChannelId = string.Empty;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static TServiceType Instance
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (mSingletonInstance == null)
                {
                    if (Forge.Net.TerraGraf.NetworkManager.Instance.ManagerState != ManagerStateEnum.Started)
                    {
                        Forge.Net.TerraGraf.NetworkManager.Instance.Start();
                    }
                    mSingletonInstance = new TServiceType();
                }
                return mSingletonInstance;
            }
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the priority of this service instance.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        /// <exception cref="ServiceNotAvailableException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        public long Priority
        {
            get { return mPriority; }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (this.ManagerState != ManagerStateEnum.Started)
                {
                    throw new ServiceNotAvailableException();
                }
                if (value < 0)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }
                RegisterToPeerContext(LookUpChannel(), value, mServiceDescriptor);
                this.mPriority = value;
            }
        }

        /// <summary>
        /// Gets the service descriptor.
        /// </summary>
        /// <value>
        /// The service descriptor.
        /// </value>
        /// <exception cref="ServiceNotAvailableException"></exception>
        public IServiceDescriptor ServiceDescriptor
        {
            get { return mServiceDescriptor; }
            set
            {
                if (this.ManagerState != ManagerStateEnum.Started)
                {
                    throw new ServiceNotAvailableException();
                }
                this.mServiceDescriptor = value;
                RegisterToPeerContext(LookUpChannel(), mPriority, mServiceDescriptor);
            }
        }

        #endregion

        #region Protected properties

        /// <summary>
        /// Gets or sets the channel id.
        /// </summary>
        /// <value>
        /// The channel id.
        /// </value>
        protected string ChannelId { get; set; }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns>
        /// Manager State
        /// </returns>
        public override ManagerStateEnum Start()
        {
            return Start(0);
        }

        /// <summary>
        /// Starts this instance with the specified priority.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <returns>
        /// Manager State
        /// </returns>
        public virtual ManagerStateEnum Start(long priority)
        {
            return Start(priority, mServiceDescriptor);
        }

        /// <summary>
        /// Starts this instance with the specified priority.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="serviceDescriptor">The service descriptor.</param>
        /// <returns>
        /// Manager State
        /// </returns>
        /// <exception cref="InvalidConfigurationException"></exception>
        /// <exception cref="InvalidConfigurationException"></exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual ManagerStateEnum Start(long priority, IServiceDescriptor serviceDescriptor)
        {
            if (this.ManagerState != ManagerStateEnum.Started)
            {
                if (priority < 0)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("priority");
                }

                if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("{0}, initializing service...", LOG_PREFIX));

                OnStart(ManagerEventStateEnum.Before);

                try
                {
                    ChannelServices.Initialize();

                    this.ChannelId = ConfigurationAccessHelper.GetValueByPath(NetworkServiceConfiguration.Settings.CategoryPropertyItems, string.Format("Services/{0}", Id));
                    if (string.IsNullOrEmpty(this.ChannelId))
                    {
                        this.ChannelId = Id;
                    }

                    Channel channel = LookUpChannel();

                    if (channel.ServerEndpoints.Count == 0)
                    {
                        throw new InvalidConfigurationException(string.Format("Channel '{0}' has not got listening server endpoint(s).", channel.ChannelId));
                    }

                    ServiceBaseServices.Initialize();
                    if (!ServiceBaseServices.IsContractRegistered(typeof(TIServiceProxyType)))
                    {
                        ServiceBaseServices.RegisterContract(typeof(TIServiceProxyType), typeof(TServiceProxyImplementationType));
                    }

                    this.mPriority = priority;
                    this.mServiceDescriptor = serviceDescriptor;

                    RegisterToPeerContext(channel, priority, serviceDescriptor);

                    this.ManagerState = ManagerStateEnum.Started;

                    if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("{0}, service successfully initialized.", LOG_PREFIX));
                }
                catch (Exception)
                {
                    this.ManagerState = ManagerStateEnum.Fault;
                    throw;
                }

                OnStart(ManagerEventStateEnum.After);
            }
            return this.ManagerState;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <returns>
        /// Manager State
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override ManagerStateEnum Stop()
        {
            if (this.ManagerState == ManagerStateEnum.Started)
            {
                if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("{0}, service stopping...", LOG_PREFIX));

                OnStop(ManagerEventStateEnum.Before);

                Forge.Net.TerraGraf.NetworkManager.Instance.Localhost.PeerContextLock.Lock();
                try
                {
                    NetworkPeerDataContext context = Forge.Net.TerraGraf.NetworkManager.Instance.Localhost.PeerContext;
                    if (context != null)
                    {
                        context.PropertyItems.Remove(Id);
                        Forge.Net.TerraGraf.NetworkManager.Instance.Localhost.PeerContext = context;
                    }
                }
                finally
                {
                    Forge.Net.TerraGraf.NetworkManager.Instance.Localhost.PeerContextLock.Unlock();
                }

                if (ServiceBaseServices.IsContractRegistered(typeof(TIServiceProxyType)))
                {
                    ServiceBaseServices.UnregisterContract(typeof(TIServiceProxyType));
                }

                this.ManagerState = ManagerStateEnum.Stopped;

                if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("{0}, service successfully stopped.", LOG_PREFIX));

                OnStop(ManagerEventStateEnum.After);
            }
            return this.ManagerState;
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Looks up channel.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "LookUp")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        protected virtual Channel LookUpChannel()
        {
            Channel channel = ChannelServices.GetChannelById(this.ChannelId);
            if (channel == null)
            {
                // regisztrálás
                BinaryMessageSink sink = new BinaryMessageSink(true, 1024);
                List<IMessageSink> sinks = new List<IMessageSink>();
                sinks.Add(sink);

                AddressEndPoint serverEndPoint = new AddressEndPoint("127.0.0.1", 0);
                List<AddressEndPoint> serverData = new List<AddressEndPoint>();
                serverData.Add(serverEndPoint);

                TerraGrafNetworkFactory networkFactory = new TerraGrafNetworkFactory();

                DefaultServerStreamFactory serverStreamFactory = new DefaultServerStreamFactory();
                DefaultClientStreamFactory clientStreamFactory = new DefaultClientStreamFactory();

                channel = new TCPChannel(this.ChannelId, sinks, sinks, serverData, networkFactory, serverStreamFactory, clientStreamFactory);
                channel.StartListening();

                ChannelServices.RegisterChannel(channel);
            }

            return channel;
        }

        /// <summary>
        /// Registers to peer context.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="serviceDescriptor">The service descriptor.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int64.ToString")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
        protected virtual void RegisterToPeerContext(Channel channel, long priority, IServiceDescriptor serviceDescriptor)
        {
            if (channel == null) ThrowHelper.ThrowArgumentNullException("channel");

            Forge.Net.TerraGraf.NetworkManager.Instance.Localhost.PeerContextLock.Lock();
            try
            {
                NetworkPeerDataContext context = Forge.Net.TerraGraf.NetworkManager.Instance.Localhost.PeerContext;
                if (context == null)
                {
                    context = new NetworkPeerDataContext();
                }

                PropertyItem root = new PropertyItem(Id, string.Format("{0};{1}", channel.ServerEndpoints.ToList<AddressEndPoint>()[0].Port.ToString(), priority.ToString()));
                if (serviceDescriptor != null)
                {
                    Dictionary<string, PropertyItem> items = serviceDescriptor.CreateServiceProperty();
                    if (items != null)
                    {
                        foreach (KeyValuePair<string, PropertyItem> kv in items)
                        {
                            root.PropertyItems.Add(kv.Key, kv.Value);
                        }
                    }
                }
                context.PropertyItems[Id] = root;
                Forge.Net.TerraGraf.NetworkManager.Instance.Localhost.PeerContext = context;
            }
            finally
            {
                Forge.Net.TerraGraf.NetworkManager.Instance.Localhost.PeerContextLock.Unlock();
            }
        }

        #endregion

    }

}
