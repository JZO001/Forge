/* *********************************************************************
 * Date: 12 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Forge.Legacy;
using Forge.Net.Remoting.Channels;
using Forge.Net.Remoting.Validators;
using Forge.Shared;
using Forge.Threading;

namespace Forge.Net.Remoting.Service
{

    /// <summary>
    /// Represent a service factory implementation to administrate contract, channel and an implementation type.
    /// You can assign a contract, an implementation and a channel to receive remote calls from clients.
    /// </summary>
    /// <typeparam name="TContract">The type of the contract.</typeparam>
    public sealed class ServiceFactory<TContract> : MBRBase, IServiceFactory where TContract : IRemoteContract
    {

        #region Field(s)

        private static readonly List<IServiceFactory> mFactories = new List<IServiceFactory>();

        private static readonly DeadlockSafeLock mFactoryLock = new DeadlockSafeLock("ServiceFactory");

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Channel mChannel = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Type mImplementationType = null;

        private bool mControlServiceContract = false;

        private bool mControlChannel = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes the <see cref="ServiceFactory&lt;TContract&gt;"/> class.
        /// </summary>
        static ServiceFactory()
        {
            ServiceBaseServices.Initialize();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="ServiceFactory&lt;TContract&gt;"/> class from being created.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        private ServiceFactory(string channelId, Type implementationType)
        {
            if (string.IsNullOrEmpty(channelId))
            {
                ThrowHelper.ThrowArgumentNullException("channelId");
            }
            if (implementationType == null)
            {
                ThrowHelper.ThrowArgumentNullException("implementationType");
            }

            if (!ServiceContract.IsAssignableFrom(implementationType))
            {
                throw new ArgumentException("Provided implementation does not implements the contract interface.", "implementationType");
            }

            ContractValidator.ValidateContractIntegrity(ServiceContract);
            ImplementationValidator.ValidateImplementationIntegrity(implementationType);

            mChannel = ChannelServices.GetChannelById(channelId);
            if (mChannel == null)
            {
                throw new ChannelNotFoundException(channelId);
            }

            mImplementationType = implementationType;

            // adatstruktúra adminisztrációja
            // egy factory csak azt adhatja hozzá, ami nincs és csak azt veheti el, ami még nem volt.
            ContractServiceSideDescriptor descriptor = null;
            Dictionary<Type, ContractServiceSideDescriptor> contractDescriptors = ServiceBaseServices.ContractDescriptors;
            lock (contractDescriptors)
            {
                if (contractDescriptors.ContainsKey(ServiceContract))
                {
                    // ismert contract
                    descriptor = contractDescriptors[ServiceContract];
                }
                else
                {
                    // ilyen contract még nincs
                    mControlServiceContract = true;
                    mControlChannel = true;
                }
            }
            if (descriptor != null)
            {
                lock (descriptor)
                {
                    if (!descriptor.ImplementationPerChannel.ContainsKey(channelId))
                    {
                        mControlChannel = true;
                    }
                    else
                    {
                        if (descriptor.ImplementationPerChannel.ContainsKey(channelId))
                        {
                            // ehhez a csatornához és contracthoz már egy másik implementáció van rendelve
                            Type currentImplType = descriptor.ImplementationPerChannel[channelId];
                            if (!currentImplType.Equals(implementationType))
                            {
                                throw new ArgumentException(string.Format("Unable to register provided implementation type: '{0}'. An other implementation type '{1}' has already definied for channel '{2}' and contract '{3}'.", implementationType.FullName, currentImplType.FullName, channelId, ServiceContract.FullName));
                            }
                        }
                    }
                }
            }
            ChannelServices.UnregisterChannelEvent += new EventHandler<ChannelRegistrationEventArgs>(ChannelUnregisteredEventHandler);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="ServiceFactory&lt;TContract&gt;"/> is reclaimed by garbage collection.
        /// </summary>
        ~ServiceFactory()
        {
            Dispose(false);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the service contract.
        /// </summary>
        /// <value>
        /// The service contract.
        /// </value>
        [DebuggerHidden]
        public Type ServiceContract
        {
            get { return typeof(TContract); }
        }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        [DebuggerHidden]
        public Channel Channel
        {
            get { return mChannel; }
        }

        /// <summary>
        /// Gets the type of the implementation.
        /// </summary>
        /// <value>
        /// The type of the implementation.
        /// </value>
        [DebuggerHidden]
        public Type ImplementationType
        {
            get { return mImplementationType; }
        }

        #endregion

        #region Public static method(s)

        /// <summary>
        /// Get a service factory to create and receive calls from a client.
        /// </summary>
        /// <typeparam name="TServiceContract">The type of the contract.</typeparam>
        /// <param name="channelId">The channel id.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <returns>Service factory</returns>
        /// <exception cref="System.ArgumentNullException">
        /// channelId
        /// or
        /// implementationType
        /// </exception>
        /// <exception cref="ChannelNotFoundException"></exception>
        public static IServiceFactory GetServiceFactory<TServiceContract>(string channelId, Type implementationType) where TServiceContract : IRemoteContract
        {
            if (string.IsNullOrEmpty(channelId))
            {
                throw new ArgumentNullException("channelId");
            }
            if (implementationType == null)
            {
                throw new ArgumentNullException("implementationType");
            }

            Channel channel = ChannelServices.GetChannelById(channelId);
            if (channel == null)
            {
                throw new ChannelNotFoundException(channelId);
            }

            IServiceFactory result = null;

            mFactoryLock.Lock();
            try
            {
                if (mFactories.Count > 0)
                {
                    foreach (ServiceFactory<IRemoteContract> factory in mFactories)
                    {
                        if (factory.ImplementationType.Equals(implementationType) && factory.ServiceContract.Equals(typeof(TServiceContract)) &&
                            factory.Channel.Equals(channel))
                        {
                            result = factory;
                            break;
                        }
                    }
                }
                if (result == null)
                {
                    result = new ServiceFactory<TServiceContract>(channelId, implementationType);
                    mFactories.Add(result);
                }
            }
            finally
            {
                mFactoryLock.Unlock();
            }

            return result;
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Opens this instance.
        /// </summary>
        public void Open()
        {
            bool registerContract = false;
            ContractServiceSideDescriptor descriptor = null;
            Dictionary<Type, ContractServiceSideDescriptor> contractDescriptors = ServiceBaseServices.ContractDescriptors;
            lock (contractDescriptors)
            {
                if (contractDescriptors.ContainsKey(ServiceContract))
                {
                    descriptor = contractDescriptors[ServiceContract];
                }
                else
                {
                    descriptor = new ContractServiceSideDescriptor(ServiceContract, null);
                    registerContract = true;
                }
            }
            if (!mControlServiceContract)
            {
                lock (descriptor)
                {
                    if (mControlChannel)
                    {
                        if (!descriptor.ImplementationPerChannel.ContainsKey(mChannel.ChannelId))
                        {
                            descriptor.ImplementationPerChannel.Add(mChannel.ChannelId, mImplementationType);
                        }
                    }
                }
            }
            if (registerContract)
            {
                lock (contractDescriptors)
                {
                    if (!contractDescriptors.ContainsKey(ServiceContract))
                    {
                        contractDescriptors.Add(ServiceContract, descriptor);
                    }
                }
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            ContractServiceSideDescriptor descriptor = null;
            Dictionary<Type, ContractServiceSideDescriptor> contractDescriptors = ServiceBaseServices.ContractDescriptors;
            lock (contractDescriptors)
            {
                if (mControlServiceContract)
                {
                    contractDescriptors.Remove(ServiceContract);
                }
                else if (contractDescriptors.ContainsKey(ServiceContract) && mControlChannel)
                {
                    descriptor = contractDescriptors[ServiceContract]; // csak akkor szedem elő, ha a channel kontrol is az Én felelősségem
                }
            }
            if (descriptor != null)
            {
                lock (descriptor)
                {
                    descriptor.ImplementationPerChannel.Remove(mChannel.ChannelId);
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private method(s)

        private void ChannelUnregisteredEventHandler(object sender, ChannelRegistrationEventArgs e)
        {
            if (e.Channel.Equals(this.mChannel))
            {
                Close();
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
                mFactoryLock.Lock();
                try
                {
                    mFactories.Remove(this);
                }
                finally
                {
                    mFactoryLock.Unlock();
                }
            }
            ChannelServices.UnregisterChannelEvent -= new EventHandler<ChannelRegistrationEventArgs>(ChannelUnregisteredEventHandler);
        }

        #endregion

    }

}
