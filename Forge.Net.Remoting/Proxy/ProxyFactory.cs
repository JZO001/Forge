/* *********************************************************************
 * Date: 12 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Forge.Legacy;
using Forge.Net.Remoting.Channels;
using Forge.Net.Remoting.Validators;
using Forge.Net.Synapse;
using Forge.Shared;
using Forge.Threading;
using Forge.Threading.Tasking;

namespace Forge.Net.Remoting.Proxy
{

#if NET40
    /// <summary>
    /// Delegate for create a proxy asynchronously
    /// </summary>
    /// <typeparam name="TContract">The type of the contract.</typeparam>
    /// <returns>The contract</returns>
    internal delegate TContract CreateProxyDelegate<TContract>() where TContract : IRemoteContract;
#endif

    /// <summary>
    /// Helps create new proxy to the generic provided service interface
    /// </summary>
    /// <typeparam name="TContract">The type of the contract.</typeparam>
    public sealed class ProxyFactory<TContract> : MBRBase where TContract : IRemoteContract
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Type mContractInterface = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Type mRealProxyType = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private AddressEndPoint mRemoteEndPoint = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Channel mChannel = null;

        private int mAsyncActiveCreateProxyCount = 0;

        private AutoResetEvent mAsyncActiveCreateProxyEvent = null;

#if NET40
        private CreateProxyDelegate<TContract> mCreateProxyDelegateOld = null;
#endif
        private System.Func<TContract> mCreateProxyDelegate;

        //private Semaphore mSemaphoreListener = null;

        //private int mClosedLevel = 0;

        //private bool mListening = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes the <see cref="ProxyFactory&lt;TContract&gt;"/> class.
        /// </summary>
        static ProxyFactory()
        {
            ProxyServices.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyFactory&lt;TContract&gt;"/> class.
        /// </summary>
        public ProxyFactory()
        {
            // default channel and proxy implementation for the contract
            if (ProxyServices.ContractDescriptors.ContainsKey(ContractInterface))
            {
                ContractClientSideDescriptor descriptor = ProxyServices.ContractDescriptors[ContractInterface];
                if (!string.IsNullOrEmpty(descriptor.DefaultChannelId) && descriptor.DefaultProxyType != null)
                {
                    ValidateConstructorParameters(ContractInterface, descriptor.DefaultProxyType, descriptor.DefaultChannelId);
                }
                else
                {
                    // nincs default konfiguráció ehhez a contract-hoz
                    throw new InitializationException(string.Format("Default configuration not found for this contract type: {0}", ContractInterface.FullName));
                }
            }
            else
            {
                // nincs ilyen contract definiálva
                throw new InitializationException(string.Format("No configuration exist for this type of contract: {0}", ContractInterface.FullName));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyFactory&lt;TContract&gt;"/> class.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        public ProxyFactory(string channelId)
        {
            if (string.IsNullOrEmpty(channelId))
            {
                ThrowHelper.ThrowArgumentNullException("channelId");
            }
            if (ProxyServices.ContractDescriptors.ContainsKey(ContractInterface))
            {
                ContractClientSideDescriptor descriptor = ProxyServices.ContractDescriptors[ContractInterface];
                foreach (KeyValuePair<string, Type> entry in descriptor.ImplementationPerChannel)
                {
                    if (entry.Key.Equals(channelId))
                    {
                        ValidateConstructorParameters(ContractInterface, entry.Value, channelId);
                        break;
                    }
                }
                // itt csak azt vizsgálom, hogy a for ciklus megtalálta-e a channel-t és a hozzá tartozó proxy type-ot.
                if (mChannel == null)
                {
                    // nem találtam, megnézem, hogy a default jó-e
                    if (!string.IsNullOrEmpty(descriptor.DefaultChannelId) && descriptor.DefaultProxyType != null && descriptor.DefaultChannelId.Equals(channelId))
                    {
                        // jó lesz a default
                        ValidateConstructorParameters(ContractInterface, descriptor.DefaultProxyType, descriptor.DefaultChannelId);
                    }
                    else
                    {
                        // hiba, ehhez a channel-hez nincs proxy definiálva
                        throw new InitializationException(string.Format("Proxy type definition not found for this channel '{0}' and contract '{1}'.", channelId, ContractInterface.FullName));
                    }
                }
            }
            else
            {
                // nincs ilyen contract definiálva
                throw new InitializationException(string.Format("No configuration exist for this type of contract: {0}", ContractInterface.FullName));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyFactory&lt;TContract&gt;"/> class.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="realProxyType">Type of the real proxy.</param>
        public ProxyFactory(string channelId, Type realProxyType)
        {
            if (string.IsNullOrEmpty(channelId))
            {
                ThrowHelper.ThrowArgumentNullException("channelId");
            }
            if (!typeof(ProxyBase).IsAssignableFrom(realProxyType))
            {
                throw new InitializationException(string.Format("Provided real proxy type '{0}' does not inherits from {1} type.", realProxyType.FullName, typeof(ProxyBase).FullName));
            }

            ValidateConstructorParameters(ContractInterface, realProxyType, channelId);
            if (mChannel.DefaultConnectionData == null)
            {
                ThrowHelper.ThrowArgumentException("No default connection data specified in the provided channel.", "channel.getDefaultConnectionData");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyFactory&lt;TContract&gt;"/> class.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="realProxyType">Type of the real proxy.</param>
        /// <param name="remoteEp">The remote ep.</param>
        public ProxyFactory(string channelId, Type realProxyType, AddressEndPoint remoteEp)
        {
            if (string.IsNullOrEmpty(channelId))
            {
                ThrowHelper.ThrowArgumentNullException("channelId");
            }
            if (!typeof(ProxyBase).IsAssignableFrom(realProxyType))
            {
                throw new InitializationException(string.Format("Provided real proxy type '{0}' does not inherits from {1} type.", realProxyType.FullName, typeof(ProxyBase).FullName));
            }
            ValidateConstructorParameters(ContractInterface, realProxyType, channelId, remoteEp);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyFactory&lt;TContract&gt;"/> class.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="remoteEp">The remote ep.</param>
        public ProxyFactory(string channelId, AddressEndPoint remoteEp)
        {
            if (string.IsNullOrEmpty(channelId))
            {
                ThrowHelper.ThrowArgumentNullException("channelId");
            }
            if (ProxyServices.ContractDescriptors.ContainsKey(ContractInterface))
            {
                ContractClientSideDescriptor descriptor = ProxyServices.ContractDescriptors[ContractInterface];
                foreach (KeyValuePair<string, Type> entry in descriptor.ImplementationPerChannel)
                {
                    if (entry.Key.Equals(channelId))
                    {
                        ValidateConstructorParameters(ContractInterface, entry.Value, channelId, remoteEp);
                        break;
                    }
                }
                // itt csak azt vizsgálom, hogy a for ciklus megtalálta-e a channel-t és a hozzá tartozó proxy type-ot.
                if (mChannel == null)
                {
                    // nem találtam, megnézem, hogy a default jó-e
                    if (!string.IsNullOrEmpty(descriptor.DefaultChannelId) && descriptor.DefaultProxyType != null && descriptor.DefaultChannelId.Equals(channelId))
                    {
                        // jó lesz a default
                        ValidateConstructorParameters(ContractInterface, descriptor.DefaultProxyType, descriptor.DefaultChannelId, remoteEp);
                    }
                    else
                    {
                        // hiba, ehhez a channel-hez nincs proxy definiálva
                        throw new InitializationException(string.Format("Proxy type definition not found for this channel '{0}' and contract '{1}'.", channelId, ContractInterface.FullName));
                    }
                }
            }
            else
            {
                // nincs ilyen contract definiálva
                throw new InitializationException(string.Format("No configuration exist for this type of contract: {0}", ContractInterface.FullName));
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the contract interface.
        /// </summary>
        /// <value>
        /// The contract interface.
        /// </value>
        [DebuggerHidden]
        public Type ContractInterface
        {
            get { return typeof(TContract); }
        }

        /// <summary>
        /// Gets the type of the real proxy.
        /// </summary>
        /// <value>
        /// The type of the real proxy.
        /// </value>
        [DebuggerHidden]
        public Type RealProxyType
        {
            get { return mRealProxyType; }
        }

        /// <summary>
        /// Gets the remote end point.
        /// </summary>
        [DebuggerHidden]
        public AddressEndPoint RemoteEndPoint
        {
            get { return mRemoteEndPoint; }
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
        /// Gets or sets a value indicating whether [exclusive address use].
        /// </summary>
        /// <value>
        /// <c>true</c>  if [exclusive address use]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool ExclusiveAddressUse
        {
            get;
            set;
        }

        #endregion

        #region Public method(s)

#if NET40
        /// <summary>
        /// Begins the create proxy.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginCreateProxy(AsyncCallback callback, object state)
        {
            Interlocked.Increment(ref mAsyncActiveCreateProxyCount);
            CreateProxyDelegate<TContract> d = new CreateProxyDelegate<TContract>(CreateProxy);
            if (mAsyncActiveCreateProxyEvent == null)
            {
                lock (this)
                {
                    if (mAsyncActiveCreateProxyEvent == null)
                    {
                        mAsyncActiveCreateProxyEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveCreateProxyEvent.WaitOne();
            mCreateProxyDelegateOld = d;
            return d.BeginInvoke(callback, state);
        }

        /// <summary>
        /// Ends the create proxy.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Contract</returns>
        public TContract EndCreateProxy(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (mCreateProxyDelegateOld == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndCreateProxy called multiple times.", "asyncResult");
            }

            try
            {
                return mCreateProxyDelegateOld.EndInvoke(asyncResult);
            }
            finally
            {
                mCreateProxyDelegateOld = null;
                mAsyncActiveCreateProxyEvent.Set();
                CloseAsyncActiveCreateProxyEvent(Interlocked.Decrement(ref mAsyncActiveCreateProxyCount));
            }
        }
#endif

        /// <summary>
        /// Begins the create proxy.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginCreateProxy(ReturnCallback callback, object state)
        {
            Interlocked.Increment(ref mAsyncActiveCreateProxyCount);
            System.Func<TContract> d = new System.Func<TContract>(CreateProxy);
            if (mAsyncActiveCreateProxyEvent == null)
            {
                lock (this)
                {
                    if (mAsyncActiveCreateProxyEvent == null)
                    {
                        mAsyncActiveCreateProxyEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveCreateProxyEvent.WaitOne();
            mCreateProxyDelegate = d;
            return d.BeginInvoke(callback, state);
        }

        /// <summary>
        /// Ends the create proxy.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Contract</returns>
        public TContract EndCreateProxy(ITaskResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (mCreateProxyDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndCreateProxy called multiple times.", "asyncResult");
            }

            try
            {
                return mCreateProxyDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                mCreateProxyDelegate = null;
                mAsyncActiveCreateProxyEvent.Set();
                CloseAsyncActiveCreateProxyEvent(Interlocked.Decrement(ref mAsyncActiveCreateProxyCount));
            }
        }

        /// <summary>
        /// Creates the proxy.
        /// </summary>
        /// <returns>Contract</returns>
        public TContract CreateProxy()
        {
            ConstructorInfo c = mRealProxyType.GetConstructor(new Type[] { typeof(Channel), typeof(string) });
            ProxyBase proxy = (ProxyBase)c.Invoke(new object[] { mChannel, mChannel.ConnectTo(mRemoteEndPoint) });
            WellKnownObjectModeEnum mode = WellKnownObjectModeEnum.PerSession;
            if (ContractValidator.GetWellKnownObjectMode(mContractInterface, out mode) && mode == WellKnownObjectModeEnum.PerSession)
            {
                ServiceBase.RegisterProxy(mChannel, mContractInterface, proxy.GetType(), proxy.SessionId, proxy.ProxyId, proxy);
            }
            return (TContract)(IRemoteContract)proxy;
        }

        #endregion

        #region Private method(s)

        private void ValidateConstructorParameters(Type contractInterface, Type realProxyType, string channelId)
        {
            if (string.IsNullOrEmpty(channelId))
            {
                ThrowHelper.ThrowArgumentNullException("channelId");
            }
            if (!ChannelServices.IsChannelRegistered(channelId))
            {
                throw new ChannelNotFoundException(channelId);
            }
            Channel c = ChannelServices.GetChannelById(channelId);
            ValidateConstructorParameters(contractInterface, realProxyType, channelId, c.DefaultConnectionData);
        }

        private void ValidateConstructorParameters(Type contractInterface, Type realProxyType, string channelId, AddressEndPoint remoteEp)
        {
            if (contractInterface == null)
            {
                ThrowHelper.ThrowArgumentNullException("contractInterface");
            }
            if (realProxyType == null)
            {
                ThrowHelper.ThrowArgumentNullException("realProxyType");
            }
            if (string.IsNullOrEmpty(channelId))
            {
                ThrowHelper.ThrowArgumentNullException("channelId");
            }
            if (!contractInterface.IsAssignableFrom(realProxyType))
            {
                ThrowHelper.ThrowArgumentException("Provided client proxy does not implements the provided contract interface.", "realProxyType");
            }
            if (remoteEp == null)
            {
                ThrowHelper.ThrowArgumentNullException("remoteEp");
            }
            if (!ChannelServices.IsChannelRegistered(channelId))
            {
                throw new ChannelNotFoundException(channelId);
            }

            ContractValidator.ValidateContractIntegrity(contractInterface);
            ImplementationValidator.ValidateProxyIntegration(realProxyType);

            mContractInterface = contractInterface;
            mRealProxyType = realProxyType;
            mChannel = ChannelServices.GetChannelById(channelId);
            mRemoteEndPoint = remoteEp;
        }

        private void CloseAsyncActiveCreateProxyEvent(int asyncActiveCount)
        {
            if ((mAsyncActiveCreateProxyEvent != null) && (asyncActiveCount == 0))
            {
                mAsyncActiveCreateProxyEvent.Close();
                mAsyncActiveCreateProxyEvent = null;
            }
        }

        #endregion

    }

}
