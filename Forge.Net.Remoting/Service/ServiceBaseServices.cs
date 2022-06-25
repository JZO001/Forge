/* *********************************************************************
 * Date: 12 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Forge.Collections;
using Forge.Configuration.Shared;
using Forge.EventRaiser;
using Forge.Logging;
using Forge.Net.Remoting.Channels;
using Forge.Net.Remoting.ConfigSection;
using Forge.Net.Remoting.Messaging;
using Forge.Net.Remoting.Proxy;
using Forge.Net.Remoting.Validators;
using Forge.Reflection;

namespace Forge.Net.Remoting.Service
{

    /// <summary>
    /// Manage pre-defined service definitions
    /// </summary>
    public sealed class ServiceBaseServices : ServiceBase
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(ServiceBaseServices));

        private static readonly Dictionary<Type, ContractServiceSideDescriptor> mContractDescriptors = new Dictionary<Type, ContractServiceSideDescriptor>();

        private static readonly Dictionary<Type, WellKnownObjectModeEnum> mContractObjectModesCache = new Dictionary<Type, WellKnownObjectModeEnum>();

        private static readonly ServiceBaseServices mSingletonInstance = new ServiceBaseServices();

        private static bool mInitialized = false;

        /// <summary>
        /// Occurs when [event initialization].
        /// </summary>
        public static event EventHandler<ServiceInitializationStateEventArgs> EventInitialization;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Prevents a default instance of the <see cref="ServiceBaseServices"/> class from being created.
        /// </summary>
        private ServiceBaseServices()
            : base()
        {
            foreach (Channel channel in ChannelServices.RegisteredChannels)
            {
                channel.SessionStateChange += new EventHandler<SessionStateEventArgs>(ChannelSessionStateChangeEventHandler);
            }
        }

        #endregion

        #region Public static properties

        /// <summary>
        /// Gets a value indicating whether [is initialized].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is initialized]; otherwise, <c>false</c>.
        /// </value>
        public static bool IsInitialized
        {
            get { return mInitialized; }
        }

        #endregion

        #region Public static method(s)

        /// <summary>
        /// Initialize ServiceBase services. It also initialize channel services too.
        /// </summary>
        /// <exception cref="Forge.Configuration.Shared.InvalidConfigurationException">
        /// Contract type not definied. Empty item found in configuration.
        /// or
        /// or
        /// </exception>
        /// <exception cref="Forge.Configuration.Shared.InvalidConfigurationValueException">
        /// </exception>
        /// <exception cref="InvalidProxyImplementationException">
        /// </exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Initialize()
        {
            if (!mInitialized)
            {
                Raiser.CallDelegatorBySync(EventInitialization, new object[] { null, new ServiceInitializationStateEventArgs(ServiceInitializationStateEnum.Before) });
                ChannelServices.Initialize();
                if (LOGGER.IsInfoEnabled) LOGGER.Info("Initializing ServiceBase services.");

                CategoryPropertyItem pi = ConfigurationAccessHelper.GetCategoryPropertyByPath(RemotingConfiguration.Settings.CategoryPropertyItems, "Services");
                if (pi != null)
                {
                    IEnumerator<CategoryPropertyItem> iterator = pi.GetEnumerator();
                    try
                    {
                        while (iterator.MoveNext())
                        {
                            pi = iterator.Current;
                            if (string.IsNullOrEmpty(pi.Id))
                            {
                                throw new InvalidConfigurationException("Contract type not definied. Empty item found in configuration.");
                            }
                            Type contractType = null;
                            Type defaultImplementationType = null;

                            try
                            {
                                contractType = TypeHelper.GetTypeFromString(pi.Id);
                                if (ContractDescriptors.ContainsKey(contractType))
                                {
                                    throw new InvalidConfigurationException(String.Format("Duplicated contract type configuration found in services. Contract: {0}", contractType.FullName));
                                }
                                ContractValidator.ValidateContractIntegrity(contractType);
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidConfigurationValueException(String.Format("Unable to resolve contract type: {0}", pi.Id), ex);
                            }

                            if (!string.IsNullOrEmpty(pi.EntryValue))
                            {
                                try
                                {
                                    defaultImplementationType = TypeHelper.GetTypeFromString(pi.EntryValue);
                                    if (!contractType.IsAssignableFrom(defaultImplementationType))
                                    {
                                        throw new InvalidProxyImplementationException(String.Format("Provided default implementation type '{0}' does not implement contract interface '{1}'.", defaultImplementationType.FullName, contractType.FullName));
                                    }
                                    ImplementationValidator.ValidateImplementationIntegrity(defaultImplementationType);
                                }
                                catch (Exception ex)
                                {
                                    throw new InvalidConfigurationValueException(String.Format("Unable to resolve implementation type: {0}", pi.EntryValue), ex);
                                }
                            }

                            ContractServiceSideDescriptor descriptor = new ContractServiceSideDescriptor(contractType, defaultImplementationType);

                            IEnumerator<CategoryPropertyItem> channelIterator = pi.GetEnumerator();
                            while (channelIterator.MoveNext())
                            {
                                CategoryPropertyItem channelImplementationItem = channelIterator.Current;
                                if (string.IsNullOrEmpty(channelImplementationItem.Id))
                                {
                                    throw new InvalidConfigurationValueException(String.Format("Channel identifier is missing from a configuration item of the contract '{0}'", pi.Id));
                                }
                                if (string.IsNullOrEmpty(channelImplementationItem.EntryValue))
                                {
                                    throw new InvalidConfigurationValueException(String.Format("Implementation type is missing from a configuration item of the contract '{0}'", pi.Id));
                                }
                                if (!ChannelServices.IsChannelRegistered(channelImplementationItem.Id))
                                {
                                    throw new InvalidConfigurationValueException(String.Format("Unregistered channel provided '{0}' in configuration section of the contract: {1}.", channelImplementationItem.Id, pi.Id));
                                }
                                Type type = null;
                                try
                                {
                                    type = TypeHelper.GetTypeFromString(channelImplementationItem.EntryValue);
                                    if (!contractType.IsAssignableFrom(type))
                                    {
                                        throw new InvalidProxyImplementationException(String.Format("Provided implementation type '{0}' does not implement contract interface '{1}'.", type.FullName, contractType.FullName));
                                    }
                                    ImplementationValidator.ValidateImplementationIntegrity(type);
                                }
                                catch (Exception ex)
                                {
                                    throw new InvalidConfigurationValueException(String.Format("Unable to resolve non-default implementation type: {0} for contract: {1} for the channel: {2}", channelImplementationItem.EntryValue, pi.Id, channelImplementationItem.Id), ex);
                                }
                                if (descriptor.ImplementationPerChannel.ContainsKey(channelImplementationItem.Id))
                                {
                                    throw new InvalidConfigurationException(String.Format("Duplicated channel identifier at contract '{0}'.", pi.Id));
                                }
                                descriptor.ImplementationPerChannel.Add(channelImplementationItem.Id, type);
                            }

                            ContractDescriptors.Add(contractType, descriptor);
                        }
                        ChannelServices.StartListeningChannels();
                    }
                    catch (Exception)
                    {
                        ContractDescriptors.Clear();
                        throw;
                    }
                }

                mInitialized = true;
                if (LOGGER.IsInfoEnabled) LOGGER.Info("ServiceBase services successfully initialized.");
                Raiser.CallDelegatorBySync(EventInitialization, new object[] { null, new ServiceInitializationStateEventArgs(ServiceInitializationStateEnum.After) });
            }
        }

        /// <summary>
        /// Determines whether [is contract registered] [the specified contract type].
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        ///   <c>true</c> if [is contract registered] [the specified contract type]; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static bool IsContractRegistered(Type contractType)
        {
            return mContractDescriptors.ContainsKey(contractType);
        }

        /// <summary>
        /// Registers the contract.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="defaultImplementationType">Default type of the implementation.</param>
        public static void RegisterContract(Type contractType, Type defaultImplementationType)
        {
            DoInitializeCheck();
            if (contractType == null)
            {
                ThrowHelper.ThrowArgumentNullException("contractType");
            }

            ContractValidator.ValidateContractIntegrity(contractType);

            if (defaultImplementationType != null)
            {
                ImplementationValidator.ValidateImplementationIntegrity(defaultImplementationType);
            }

            lock (mContractDescriptors)
            {
                if (mContractDescriptors.ContainsKey(contractType))
                {
                    ThrowHelper.ThrowArgumentException("Contract type has already registered.");
                }
                mContractDescriptors.Add(contractType, new ContractServiceSideDescriptor(contractType, defaultImplementationType));
            }
        }

        /// <summary>
        /// Unregister contract.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        public static void UnregisterContract(Type contractType)
        {
            DoInitializeCheck();
            if (contractType == null)
            {
                ThrowHelper.ThrowArgumentNullException("contractType");
            }

            lock (mContractDescriptors)
            {
                if (mContractDescriptors.ContainsKey(contractType))
                {
                    mContractDescriptors.Remove(contractType);
                }
            }
        }

        /// <summary>
        /// Gets the contract default.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns></returns>
        public static ContractServiceSideDescriptor GetContractDefault(Type contractType)
        {
            DoInitializeCheck();
            if (contractType == null)
            {
                ThrowHelper.ThrowArgumentNullException("contractType");
            }

            ContractServiceSideDescriptor result = null;

            lock (mContractDescriptors)
            {
                if (mContractDescriptors.ContainsKey(contractType))
                {
                    result = mContractDescriptors[contractType];
                }
            }

            return result;
        }

        /// <summary>
        /// Changes the contract defaults.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="defaultImplementationType">Default type of the implementation.</param>
        public static void ChangeContractDefaults(Type contractType, Type defaultImplementationType)
        {
            DoInitializeCheck();
            if (contractType == null)
            {
                ThrowHelper.ThrowArgumentNullException("contractType");
            }

            lock (mContractDescriptors)
            {
                if (mContractDescriptors.ContainsKey(contractType))
                {
                    mContractDescriptors.Remove(contractType);
                    mContractDescriptors.Add(contractType, new ContractServiceSideDescriptor(contractType, defaultImplementationType));
                }
                else
                {
                    RegisterContract(contractType, defaultImplementationType);
                }
            }
        }

        /// <summary>
        /// Registers the implementation for channel.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="channelId">The channel id.</param>
        /// <param name="defaultImplementationType">Default type of the implementation.</param>
        public static void RegisterImplementationForChannel(Type contractType, string channelId, Type defaultImplementationType)
        {
            DoInitializeCheck();
            if (contractType == null)
            {
                ThrowHelper.ThrowArgumentNullException("contractType");
            }
            if (!ChannelServices.IsChannelRegistered(channelId))
            {
                ThrowHelper.ThrowArgumentException("Default channel identifier has not found.");
            }

            lock (mContractDescriptors)
            {
                if (mContractDescriptors.ContainsKey(contractType))
                {
                    ContractServiceSideDescriptor descriptor = mContractDescriptors[contractType];
                    if (defaultImplementationType == null)
                    {
                        if (descriptor.ImplementationPerChannel.ContainsKey(channelId))
                        {
                            descriptor.ImplementationPerChannel.Remove(channelId);
                        }
                    }
                    else
                    {
                        ImplementationValidator.ValidateImplementationIntegrity(defaultImplementationType);
                        descriptor.ImplementationPerChannel[channelId] = defaultImplementationType;
                    }
                }
                else
                {
                    ThrowHelper.ThrowArgumentException("Contract type has not been registered.");
                }
            }
        }

        /// <summary>
        /// Unregister implementation for channel.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="channelId">The channel id.</param>
        public static void UnregisterImplementationForChannel(Type contractType, string channelId)
        {
            DoInitializeCheck();
            if (contractType == null)
            {
                ThrowHelper.ThrowArgumentNullException("contractType");
            }
            if (!ChannelServices.IsChannelRegistered(channelId))
            {
                ThrowHelper.ThrowArgumentException("Default channel identifier has not found.");
            }

            lock (mContractDescriptors)
            {
                if (mContractDescriptors.ContainsKey(contractType))
                {
                    ContractServiceSideDescriptor descriptor = mContractDescriptors[contractType];
                    if (descriptor.ImplementationPerChannel.ContainsKey(channelId))
                    {
                        descriptor.ImplementationPerChannel.Remove(channelId);
                    }
                }
                else
                {
                    ThrowHelper.ThrowArgumentException("Contract type has not been registered.");
                }
            }
        }

        #endregion

        #region Internal static method(s)

        /// <summary>
        /// Gets the contract descriptors.
        /// </summary>
        internal static Dictionary<Type, ContractServiceSideDescriptor> ContractDescriptors
        {
            get { return mContractDescriptors; }
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Channels the register event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Forge.Net.Remoting.Channels.ChannelRegistrationEventArgs"/> instance containing the event data.</param>
        protected override void ChannelRegisterEventHandler(object sender, ChannelRegistrationEventArgs e)
        {
            base.ChannelRegisterEventHandler(sender, e);
            e.Channel.SessionStateChange += new EventHandler<SessionStateEventArgs>(ChannelSessionStateChangeEventHandler);
        }

        /// <summary>
        /// Channels the unregistered event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Forge.Net.Remoting.Channels.ChannelRegistrationEventArgs"/> instance containing the event data.</param>
        protected override void ChannelUnregisteredEventHandler(object sender, ChannelRegistrationEventArgs e)
        {
            base.ChannelUnregisteredEventHandler(sender, e);

            Channel channel = e.Channel;

            // leiratkozom az eseményről
            channel.SessionStateChange -= new EventHandler<SessionStateEventArgs>(ChannelSessionStateChangeEventHandler);

            // megszüntetem a channel-hez tartozó PerSession ProxyBase instance-okat
            lock (mContractAndInstancePerSessionAndChannel)
            {
                IEnumeratorSpecialized<ContractAndInstanceStruct> iterator = mContractAndInstancePerSessionAndChannel.GetEnumerator();
                while (iterator.MoveNext())
                {
                    ContractAndInstanceStruct s = iterator.Current;
                    if (s.Channel.Equals(channel))
                    {
                        iterator.Remove();
                        s.Instance.Dispose();
                    }
                }
            }

            // konfiguráció adminisztrációja, törlöm a csatornához köthető információkat
            List<ContractServiceSideDescriptor> list = new List<ContractServiceSideDescriptor>();
            lock (mContractDescriptors)
            {
                list.AddRange(mContractDescriptors.Values);
            }
            foreach (ContractServiceSideDescriptor ss in list)
            {
                lock (ss)
                {
                    ss.ImplementationPerChannel.Remove(channel.ChannelId);
                }
            }
            list.Clear();
        }

        /// <summary>
        /// Channels the receive message event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Forge.Net.Remoting.Channels.ReceiveMessageEventArgs"/> instance containing the event data.</param>
        protected override void ChannelReceiveMessageEventHandler(object sender, ReceiveMessageEventArgs e)
        {
            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("Executing service method. SessionId: '{0}', {1}", e.SessionId, e.Message.ToString()));

            Channel channel = (Channel)sender;
            String sessionId = e.SessionId;
            IMessage message = e.Message;

            // Request és response message type jöhet
            // Request mode
            // Response mode
            // Datagram mode - nincs válasz
            if (message.MessageType == MessageTypeEnum.Acknowledge || message.MessageType == MessageTypeEnum.Response)
            {
                // HIBA: nem megfelelő üzenet típus (implementációs hiba?)
                String errorMsg = String.Format("Invalid message type received: {0}. This may be an implementation error in the client proxy.", message.MessageType);
                if (LOGGER.IsErrorEnabled) LOGGER.Error(errorMsg);
            }
            else
            {
                RequestMessage rm = (RequestMessage)message;

                if (MessageInvokeModeEnum.RequestCallback.Equals(rm.MessageInvokeMode))
                {
                    return;
                }

                String contractName = rm.ContractName;
                Type contractType = null;
                try
                {
                    contractType = TypeHelper.GetTypeFromString(contractName);
                }
                catch (Exception ex)
                {
                    // HIBA: ez a típus nem szerepel a ClassPath-ban
                    try
                    {
                        SendResponse(channel, sessionId, rm, typeof(void), null, new MethodInvocationException(String.Format("Unable to resolve this type of contract '{0}'. Type has not found.", rm.ContractName), ex), channel.DefaultErrorResponseTimeout);
                    }
                    catch (Exception innerEx)
                    {
                        // válaszüzenet küldése sikertelen
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format(AUTO_SEND_REPLY_ERROR_MSG, innerEx.Message));
                    }
                    return;
                }
                ContractServiceSideDescriptor descriptor = null;
                lock (mContractDescriptors)
                {
                    if (mContractDescriptors.ContainsKey(contractType))
                    {
                        descriptor = mContractDescriptors[contractType];
                    }
                    else
                    {
                        // HIBA: ez a contract nincs nyilvántartva
                        try
                        {
                            SendResponse(channel, sessionId, rm, typeof(void), null, new MethodInvocationException(String.Format("This type of contract '{0}' has not been registered.", contractType.FullName)), channel.DefaultErrorResponseTimeout);
                        }
                        catch (Exception innerEx)
                        {
                            // válaszüzenet küldése sikertelen
                            if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format(AUTO_SEND_REPLY_ERROR_MSG, innerEx.Message));
                        }
                        return;
                    }
                }
                Type implType = null;
                lock (descriptor)
                {
                    if (descriptor.ImplementationPerChannel.ContainsKey(channel.ChannelId))
                    {
                        implType = descriptor.ImplementationPerChannel[channel.ChannelId];
                    }
                    else if (descriptor.DefaultImplementationType != null)
                    {
                        implType = descriptor.DefaultImplementationType;
                    }
                    else
                    {
                        // HIBA: a contract a megadott channel-hez nem definiált implementációs típust
                        try
                        {
                            SendResponse(channel, sessionId, rm, typeof(void), null, new MethodInvocationException(String.Format("Unable to find implementation for this type of contract '{0}' and channel id: '{1}'.", contractType.FullName, channel.ChannelId)), channel.DefaultErrorResponseTimeout);
                        }
                        catch (Exception innerEx)
                        {
                            // válaszüzenet küldése sikertelen
                            if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format(AUTO_SEND_REPLY_ERROR_MSG, innerEx.Message));
                        }
                        return;
                    }
                }
                Object instance = null;
                WellKnownObjectModeEnum objectMode = WellKnownObjectModeEnum.PerSession;
                ContractValidator.GetWellKnownObjectMode(contractType, out objectMode);
                if (objectMode == WellKnownObjectModeEnum.PerSession)
                {
                    // constructor keresése a Channel és String paraméterlistával, ha még ehhez a session-hez nem létezik instance
                    long proxyId = -1;
                    try
                    {
                        proxyId = (long)rm.Context[ProxyBase.PROXY_ID];
                    }
                    catch (Exception ex)
                    {
                        // HIBA: hiányzó vagy rossz formátumú ProxyId
                        try
                        {
                            SendResponse(channel, sessionId, rm, typeof(void), null, new MethodInvocationException("Unable to parse PROXY_ID from call context.", ex), channel.DefaultErrorResponseTimeout);
                        }
                        catch (Exception innerEx)
                        {
                            // válaszüzenet küldése sikertelen
                            if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format(AUTO_SEND_REPLY_ERROR_MSG, innerEx.Message));
                        }
                        return;
                    }
                    lock (mContractAndInstancePerSessionAndChannel)
                    {
                        foreach (ContractAndInstanceStruct s in mContractAndInstancePerSessionAndChannel)
                        {
                            if (s.Channel.Equals(channel) && s.ContractType.Equals(contractType) && s.SessionId.Equals(sessionId) && s.ImplType.Equals(implType) && s.ProxyId == proxyId)
                            {
                                instance = s.Instance;
                                break;
                            }
                        }
                        if (instance == null)
                        {
                            // még nem létezik a proxy példány, létre kell hozni
                            try
                            {
                                ConstructorInfo c = implType.GetConstructor(new Type[] { typeof(Channel), typeof(String) });
                                instance = c.Invoke(new object[] { channel, sessionId });
                                RegisterProxy(channel, contractType, implType, sessionId, proxyId, (ProxyBase)instance);
                            }
                            catch (Exception ex)
                            {
                                // HIBA: sikertelen az instance létrehozása
                                try
                                {
                                    SendResponse(channel, sessionId, rm, typeof(void), null, new MethodInvocationException(String.Format("Unable to instantiate type '{0}'. Public constructor is not accessible/found with parameter types: '{1}' and '{2}'.", implType.FullName, typeof(Channel).FullName, typeof(String).FullName), ex), channel.DefaultErrorResponseTimeout);
                                }
                                catch (Exception innerEx)
                                {
                                    // válaszüzenet küldése sikertelen
                                    if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format(AUTO_SEND_REPLY_ERROR_MSG, innerEx.Message));
                                }
                                return;
                            }
                        }
                    }
                }
                else if (objectMode == WellKnownObjectModeEnum.Singleton)
                {
                    // üres constructor kell, ha még nem létezik az instance
                    lock (mSingletonContainer)
                    {
                        if (mSingletonContainer.ContainsKey(implType))
                        {
                            instance = mSingletonContainer[implType];
                        }
                        else
                        {
                            try
                            {
                                // még nem volt létrehozva
                                instance = implType.GetConstructor(new Type[] { }).Invoke(null);
                                mSingletonContainer.Add(implType, instance);
                            }
                            catch (Exception ex)
                            {
                                // HIBA: sikertelen az instance létrehozása
                                try
                                {
                                    SendResponse(channel, sessionId, rm, typeof(void), null, new MethodInvocationException(String.Format("Unable to instantiate type '{0}'. Public empty constructor is not accessible/found.", implType.FullName), ex), channel.DefaultErrorResponseTimeout);
                                }
                                catch (Exception innerEx)
                                {
                                    // válaszüzenet küldése sikertelen
                                    if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format(AUTO_SEND_REPLY_ERROR_MSG, innerEx.Message));
                                }
                                return;
                            }
                        }
                    }
                }
                else
                {
                    // üres constructor kell, példányosítás mindig megtörténik
                    try
                    {
                        // még nem volt létrehozva
                        instance = implType.GetConstructor(new Type[] { }).Invoke(null);
                    }
                    catch (Exception ex)
                    {
                        // HIBA: sikertelen az instance létrehozása
                        try
                        {
                            SendResponse(channel, sessionId, rm, typeof(void), null, new MethodInvocationException(String.Format("Unable to instantiate type '{0}'. Public empty constructor is not accessible/found.", implType.FullName), ex), channel.DefaultErrorResponseTimeout);
                        }
                        catch (Exception innerEx)
                        {
                            // válaszüzenet küldése sikertelen
                            if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format(AUTO_SEND_REPLY_ERROR_MSG, innerEx.Message));
                        }
                        return;
                    }
                }

                // meg van az instance, lehet reflection-nel hívni a metódusára
                Type[] methodParamTypes = null;
                Object[] paramValues = null;
                if (rm.MethodParameters != null)
                {
                    methodParamTypes = new Type[rm.MethodParameters.Length];
                    paramValues = new Object[rm.MethodParameters.Length];
                    for (int i = 0; i < rm.MethodParameters.Length; i++)
                    {
                        try
                        {
                            methodParamTypes[i] = TypeHelper.GetTypeFromString(rm.MethodParameters[i].ClassName);
                        }
                        catch (Exception ex)
                        {
                            // HIBA: a paraméter egy típusa nem szerepel a Domainben, így nem feloldható, ismeretlen
                            try
                            {
                                SendResponse(channel, sessionId, rm, typeof(void), null, new MethodInvocationException(String.Format("Unable to resolve parameter type '{0}'. Type has not found.", rm.MethodParameters[i].ClassName), ex), channel.DefaultErrorResponseTimeout);
                            }
                            catch (Exception innerEx)
                            {
                                // válaszüzenet küldése sikertelen
                                if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format(AUTO_SEND_REPLY_ERROR_MSG, innerEx.Message));
                            }
                            return;
                        }
                        paramValues[i] = rm.MethodParameters[i].Value;
                    }
                }

                MethodInfo m = null;
                try
                {
                    m = FindMethod(implType, rm.MethodName, methodParamTypes);
                }
                catch (Exception ex)
                {
                    // HIBA: a metódus név és paraméterlista alapján nem található
                    try
                    {
                        SendResponse(channel, sessionId, rm, typeof(void), null, new MethodInvocationException(String.Format("Unable to find method '{0}' with parameter list: '{1}'.", rm.MethodName, methodParamTypes), ex), channel.DefaultErrorResponseTimeout);
                    }
                    catch (Exception innerEx)
                    {
                        // válaszüzenet küldése sikertelen
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format(AUTO_SEND_REPLY_ERROR_MSG, innerEx.Message));
                    }
                    return;
                }
                long returnTimeout = OperationContractAttribute.DEFAULT_METHOD_TIMEOUT;
                if (rm.MessageType == MessageTypeEnum.Request)
                {
                    MethodParameter[] mps = null;
                    if (m.GetParameters().Length > 0)
                    {
                        mps = new MethodParameter[m.GetParameters().Length];
                        for (int i = 0; i < m.GetParameters().Length; i++)
                        {
                            ParameterInfo pi = m.GetParameters()[i];
                            mps[i] = new MethodParameter(i, string.Format("{0}, {1}", pi.ParameterType.FullName, new AssemblyName(pi.ParameterType.Assembly.FullName).Name), null);
                        }
                    }
                    returnTimeout = ServiceBase.GetTimeoutByMethod(contractType, m.Name, mps, MethodTimeoutEnum.ReturnTimeout);
                    lock (mCallContextForReply)
                    {
                        mCallContextForReply.Add(Thread.CurrentThread, new CallContextForReply(channel, sessionId, rm, m.ReturnType, returnTimeout));
                    }
                }

                // visszatérési értéket fogadni
                Exception methodException = null;
                Object result = null;
                try
                {
                    result = m.Invoke(instance, paramValues);
                }
                catch (Exception ex)
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("Service method threw an exception. SessionId: '{0}', {1}", e.SessionId, e.Message.ToString()), ex);
                    methodException = ex;
                }

                // válaszüzenet küldése
                bool needToSendResponse = false;
                Type returnType = typeof(void);
                if (rm.MessageType == MessageTypeEnum.Request)
                {
                    lock (mCallContextForReply)
                    {
                        if (mCallContextForReply.ContainsKey(Thread.CurrentThread))
                        {
                            needToSendResponse = true;
                            returnType = mCallContextForReply[Thread.CurrentThread].ReturnType;
                        }
                    }
                }
                if (needToSendResponse)
                {
                    try
                    {
                        SendResponse(channel, sessionId, rm, returnType, result, methodException, returnTimeout);
                    }
                    catch (Exception ex)
                    {
                        // válaszüzenet küldése sikertelen
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format(AUTO_SEND_REPLY_ERROR_MSG, ex.Message));
                    }
                    finally
                    {
                        lock (mCallContextForReply)
                        {
                            mCallContextForReply.Remove(Thread.CurrentThread);
                        }
                    }
                }
                // saját tulajdonú instance, ami implementálja az IDisposable interface-t és SingleCall, akkor ráhívok a Dispose() metódusra
                if (objectMode == WellKnownObjectModeEnum.SingleCall && instance is IDisposable)
                {
                    ((IDisposable)instance).Dispose();
                }
            }
        }

        #endregion

        #region Private method(s)

        private static void DoInitializeCheck()
        {
            if (!mInitialized)
            {
                throw new InitializationException("Services has not been initialized.");
            }
        }

        private void ChannelSessionStateChangeEventHandler(object sender, SessionStateEventArgs e)
        {
            // PerSession módban a session-höz tartozó implementációkat meg kell szüntetni
            if (!e.IsConnected)
            {
                Channel channel = (Channel)sender;
                lock (mContractAndInstancePerSessionAndChannel)
                {
                    IEnumeratorSpecialized<ContractAndInstanceStruct> iterator = mContractAndInstancePerSessionAndChannel.GetEnumerator();
                    while (iterator.MoveNext())
                    {
                        ContractAndInstanceStruct s = iterator.Current;
                        if (s.Channel.Equals(channel) && s.SessionId.Equals(e.SessionId))
                        {
                            iterator.Remove();
                            s.Instance.Dispose();
                        }
                    }
                }
            }
        }

        #endregion

    }

}
