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
using Forge.Configuration.Shared;
using Forge.EventRaiser;
using Forge.Net.Remoting.Channels;
using Forge.Net.Remoting.ConfigSection;
using Forge.Net.Remoting.Messaging;
using Forge.Net.Remoting.Validators;
using Forge.Reflection;
using log4net;

namespace Forge.Net.Remoting.Proxy
{

    /// <summary>
    /// Manage pre-defined proxy definitions
    /// </summary>
    public sealed class ProxyServices : ServiceBase
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(ProxyServices));

        private static readonly Dictionary<Type, ContractClientSideDescriptor> mContractDescriptors = new Dictionary<Type, ContractClientSideDescriptor>();

        private static bool mInitialized = false;

        private static ProxyServices mSingletonInstance = null;

        /// <summary>
        /// Occurs when [event initialization].
        /// </summary>
        public static event EventHandler<ServiceInitializationStateEventArgs> EventInitialization;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Prevents a default instance of the <see cref="ProxyServices"/> class from being created.
        /// </summary>
        private ProxyServices()
            : base()
        {
        }

        #endregion

        #region Public static propertes

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
        /// Initializes this instance.
        /// </summary>
        /// <exception cref="Forge.Configuration.Shared.InvalidConfigurationException">
        /// Contract type not definied. Empty item found in configuration.
        /// or
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
                if (LOGGER.IsInfoEnabled) LOGGER.Info("Initializing client proxy services.");

                CategoryPropertyItem pi = ConfigurationAccessHelper.GetCategoryPropertyByPath(RemotingConfiguration.Settings.CategoryPropertyItems, "Clients");
                if (pi != null)
                {
                    try
                    {
                        IEnumerator<CategoryPropertyItem> iterator = pi.GetEnumerator();
                        while (iterator.MoveNext())
                        {
                            pi = iterator.Current;
                            if (string.IsNullOrEmpty(pi.Id))
                            {
                                throw new InvalidConfigurationException("Contract type not definied. Empty item found in configuration.");
                            }
                            Type contractType = null;
                            String defaultChannelId = null;
                            Type defaultProxyType = null;

                            try
                            {
                                contractType = TypeHelper.GetTypeFromString(pi.Id);
                                if (mContractDescriptors.ContainsKey(contractType))
                                {
                                    throw new InvalidConfigurationException(String.Format("Duplicated contract type configuration found in clients. Contract: {0}", contractType.FullName));
                                }
                                ContractValidator.ValidateContractIntegrity(contractType);
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidConfigurationValueException(String.Format("Unable to resolve contract type: {0}", pi.Id), ex);
                            }

                            {
                                CategoryPropertyItem defaultChannelIdPropertyItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(RemotingConfiguration.Settings.CategoryPropertyItems, String.Format("Clients/{0}/Defaults/DefaultChannelId", pi.Id));
                                if (defaultChannelIdPropertyItem != null)
                                {
                                    defaultChannelId = defaultChannelIdPropertyItem.EntryValue;
                                    if (string.Empty.Equals(defaultChannelId))
                                    {
                                        defaultChannelId = null;
                                    }
                                }
                            }

                            {
                                CategoryPropertyItem defaultProxyTypePropertyItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(RemotingConfiguration.Settings.CategoryPropertyItems, String.Format("Clients/{0}/Defaults/DefaultProxyType", pi.Id));
                                if (defaultProxyTypePropertyItem != null)
                                {
                                    if (defaultProxyTypePropertyItem.EntryValue != null && !string.Empty.Equals(defaultProxyTypePropertyItem.EntryValue))
                                    {
                                        try
                                        {
                                            defaultProxyType = TypeHelper.GetTypeFromString(defaultProxyTypePropertyItem.EntryValue);
                                            ImplementationValidator.ValidateProxyIntegration(defaultProxyType);
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new InvalidConfigurationValueException(String.Format("Unable to resolve proxy type: {0} for contract: {1}", defaultProxyTypePropertyItem.EntryValue, pi.Id), ex);
                                        }
                                    }
                                }
                            }
                            if (!(defaultChannelId == null && defaultProxyType == null))
                            {
                                if (defaultChannelId == null || defaultProxyType == null)
                                {
                                    throw new InvalidConfigurationException(String.Format("Both of 'DefaultChannelId' and 'DefaultProxyType' values are must be filled in configuration. Contract type: {0}", pi.Id));
                                }
                                if (!contractType.IsAssignableFrom(defaultProxyType))
                                {
                                    throw new InvalidProxyImplementationException(String.Format("Provided default proxy type '{0}' does not implement contract interface '{1}'.", defaultProxyType.FullName, contractType.FullName));
                                }
                            }

                            ContractClientSideDescriptor descriptor = new ContractClientSideDescriptor(contractType, defaultChannelId, defaultProxyType);

                            {
                                CategoryPropertyItem customDefinitionsPropertyItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(RemotingConfiguration.Settings.CategoryPropertyItems, String.Format("Clients/{0}/CustomDefinitions", pi.Id));
                                if (customDefinitionsPropertyItem != null)
                                {
                                    IEnumerator<CategoryPropertyItem> customDefinitionsIterator = customDefinitionsPropertyItem.GetEnumerator();
                                    while (customDefinitionsIterator.MoveNext())
                                    {
                                        CategoryPropertyItem channelProxyItem = customDefinitionsIterator.Current;
                                        if (string.IsNullOrEmpty(channelProxyItem.Id))
                                        {
                                            throw new InvalidConfigurationValueException(String.Format("Channel identifier is missing from a configuration item at the CustomDefinitions of the contract '{0}'", pi.Id));
                                        }
                                        if (string.IsNullOrEmpty(channelProxyItem.EntryValue))
                                        {
                                            throw new InvalidConfigurationValueException(String.Format("Proxy type is missing from a configuration item at the CustomDefinitions of the contract '{0}'", pi.Id));
                                        }
                                        if (!ChannelServices.IsChannelRegistered(channelProxyItem.Id))
                                        {
                                            throw new InvalidConfigurationValueException(String.Format("Unregistered channel provided '{0}' in CustomDefinitions configuration section of the contract: {1}.", channelProxyItem.Id, pi.Id));
                                        }
                                        Type type = null;
                                        try
                                        {
                                            type = TypeHelper.GetTypeFromString(channelProxyItem.EntryValue);
                                            if (!contractType.IsAssignableFrom(type))
                                            {
                                                throw new InvalidProxyImplementationException(String.Format("Provided proxy type '{0}' does not implement contract interface '{1}'.", type.FullName, contractType.FullName));
                                            }
                                            ImplementationValidator.ValidateProxyIntegration(type);
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new InvalidConfigurationValueException(String.Format("Unable to resolve non-default proxy type: {0} for contract: {1} for the channel: {2}", channelProxyItem.EntryValue, pi.Id, channelProxyItem.Id), ex);
                                        }
                                        if (descriptor.ImplementationPerChannel.ContainsKey(channelProxyItem.Id))
                                        {
                                            throw new InvalidConfigurationException(String.Format("Duplicated channel identifier in CustomDefinitions section at contract '{0}'.", pi.Id));
                                        }
                                        descriptor.ImplementationPerChannel.Add(channelProxyItem.Id, type);
                                    }
                                }
                            }
                            mContractDescriptors.Add(contractType, descriptor);
                        }
                        mSingletonInstance = new ProxyServices();
                        //if (mContractDescriptors.Count > 0)
                        //{
                        //    mSingletonInstance = new ProxyServices();
                        //}
                    }
                    catch (Exception ex)
                    {
                        mContractDescriptors.Clear();
                        throw ex;
                    }
                }
                //ChannelServices.startListeningChannels();

                mInitialized = true;
                if (LOGGER.IsInfoEnabled) LOGGER.Info("Client proxy services successfully initialized.");
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
        /// <exception cref="System.ArgumentNullException">contractType</exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static bool IsContractRegistered(Type contractType)
        {
            if (contractType == null)
            {
                throw new ArgumentNullException("contractType");
            }
            return mContractDescriptors.ContainsKey(contractType);
        }

        /// <summary>
        /// Registers the contract.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="defaultChannelId">The default channel id.</param>
        /// <param name="defaultProxyType">Default type of the proxy.</param>
        /// <exception cref="System.InvalidOperationException">Contract has already registered.</exception>
        public static void RegisterContract(Type contractType, string defaultChannelId, Type defaultProxyType)
        {
            DoInitializeCheck();
            if (contractType == null)
            {
                ThrowHelper.ThrowArgumentNullException("contractType");
            }

            ContractValidator.ValidateContractIntegrity(contractType);
            if (!ChannelServices.IsChannelRegistered(defaultChannelId))
            {
                ThrowHelper.ThrowArgumentException("Default channel identifier has not found.");
            }
            if (defaultProxyType != null)
            {
                ImplementationValidator.ValidateProxyIntegration(defaultProxyType);
            }
            lock (mContractDescriptors)
            {
                if (mContractDescriptors.ContainsKey(contractType))
                {
                    throw new InvalidOperationException("Contract has already registered.");
                }
                ContractClientSideDescriptor descriptor = new ContractClientSideDescriptor(contractType, defaultChannelId, defaultProxyType);
                mContractDescriptors.Add(contractType, descriptor);
            }
        }

        /// <summary>
        /// Gets the contract defaults.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>Client side contract descriptor</returns>
        public static ContractClientSideDescriptor GetContractDefaults(Type contractType)
        {
            DoInitializeCheck();
            if (contractType == null)
            {
                ThrowHelper.ThrowArgumentNullException("contractType");
            }

            ContractClientSideDescriptor result = null;

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
        /// <param name="defaultChannelId">The default channel id.</param>
        /// <param name="defaultProxyType">Default type of the proxy.</param>
        public static void ChangeContractDefaults(Type contractType, string defaultChannelId, Type defaultProxyType)
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
                    if (!ChannelServices.IsChannelRegistered(defaultChannelId))
                    {
                        ThrowHelper.ThrowArgumentException("Default channel identifier has not found.");
                    }
                    mContractDescriptors.Remove(contractType);
                    mContractDescriptors.Add(contractType, new ContractClientSideDescriptor(contractType, defaultChannelId, defaultProxyType));
                }
                else
                {
                    RegisterContract(contractType, defaultChannelId, defaultProxyType);
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
                    ContractClientSideDescriptor descriptor = mContractDescriptors[contractType];
                    if (defaultImplementationType == null)
                    {
                        if (descriptor.ImplementationPerChannel.ContainsKey(channelId))
                        {
                            descriptor.ImplementationPerChannel.Remove(channelId);
                        }
                    }
                    else
                    {
                        ImplementationValidator.ValidateProxyIntegration(defaultImplementationType);
                        if (descriptor.ImplementationPerChannel.ContainsKey(channelId))
                        {
                            descriptor.ImplementationPerChannel[channelId] = defaultImplementationType;
                        }
                        else
                        {
                            descriptor.ImplementationPerChannel.Add(channelId, defaultImplementationType);
                        }
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
        internal static Dictionary<Type, ContractClientSideDescriptor> ContractDescriptors
        {
            get { return mContractDescriptors; }
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Channels the receive message event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Forge.Net.Remoting.Channels.ReceiveMessageEventArgs"/> instance containing the event data.</param>
        protected override void ChannelReceiveMessageEventHandler(object sender, ReceiveMessageEventArgs e)
        {
            Channel channel = (Channel)sender;
            IMessage message = e.Message;
            String sessionId = e.SessionId;
            if (message.MessageType == MessageTypeEnum.Acknowledge || message.MessageType == MessageTypeEnum.Response)
            {
                // HIBA: nem megfelelő üzenet típus (implementációs hiba?)
                String errorMsg = String.Format("Invalid message type received: {0}. This may be an implementation error in the client proxy.", message.MessageType.ToString());
                if (LOGGER.IsErrorEnabled) LOGGER.Error(errorMsg);
            }
            else
            {
                RequestMessage rm = (RequestMessage)message;

                if (MessageInvokeModeEnum.RequestService.Equals(rm.MessageInvokeMode))
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
                {
                    WellKnownObjectModeEnum mode = WellKnownObjectModeEnum.PerSession;
                    if (ContractValidator.GetWellKnownObjectMode(contractType, out mode) && mode != WellKnownObjectModeEnum.PerSession)
                    {
                        // HIBA: nem PerSession a contract, amit meghívnak
                        try
                        {
                            SendResponse(channel, sessionId, rm, typeof(void), null, new MethodInvocationException(String.Format("Contract {0} must be configured to {1}.", typeof(WellKnownObjectModeEnum).Name, WellKnownObjectModeEnum.PerSession.ToString())), channel.DefaultErrorResponseTimeout);
                        }
                        catch (Exception innerEx)
                        {
                            // válaszüzenet küldése sikertelen
                            if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format(AUTO_SEND_REPLY_ERROR_MSG, innerEx.Message));
                        }
                        return;
                    }
                }
                lock (mContractDescriptors)
                {
                    if (!mContractDescriptors.ContainsKey(contractType))
                    {
                        // HIBA: ez a contract nincs nyilvántartva
                        try
                        {
                            SendResponse(channel, sessionId, rm, typeof(void), null, new MethodInvocationException(String.Format("This type of contract '{0}' has not been registered.", contractType.Name)), channel.DefaultErrorResponseTimeout);
                        }
                        catch (Exception innerEx)
                        {
                            // válaszüzenet küldése sikertelen
                            if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format(AUTO_SEND_REPLY_ERROR_MSG, innerEx.Message));
                        }
                        return;
                    }
                }
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
                ProxyBase proxy = null;
                lock (mContractAndInstancePerSessionAndChannel)
                {
                    foreach (ContractAndInstanceStruct s in mContractAndInstancePerSessionAndChannel)
                    {
                        if (s.Channel.Equals(channel) && s.ContractType.Equals(contractType) && s.SessionId.Equals(sessionId) && s.ProxyId == proxyId)
                        {
                            proxy = s.Instance;
                            break;
                        }
                    }
                    if (proxy == null)
                    {
                        // HIBA: nem létezik a hivatkozott proxy példány
                        try
                        {
                            SendResponse(channel, sessionId, rm, typeof(void), null, new MethodInvocationException(String.Format("Unable to find proxy on client side with ID: {0}", proxyId)), channel.DefaultErrorResponseTimeout);
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
                            // HIBA: a paraméter egy típusa nem szerepel a CLASSPATH-ban, így nem feloldható, ismeretlen
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
                    m = FindMethod(proxy.GetType(), rm.MethodName, methodParamTypes);
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
                    result = m.Invoke(proxy, paramValues);
                }
                catch (Exception ex)
                {
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

        #endregion

    }

}
