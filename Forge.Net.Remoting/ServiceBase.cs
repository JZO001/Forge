/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Forge.Collections;
using Forge.Net.Remoting.Channels;
using Forge.Net.Remoting.Messaging;
using Forge.Net.Remoting.Proxy;
using Forge.Reflection;

namespace Forge.Net.Remoting
{

    /// <summary>
    /// Base class for services. Provide an implementation to send reply back to the caller manually.
    /// </summary>
    public abstract class ServiceBase : MBRBase
    {

        #region Field(s)

        /*
         * Szerver oldalon szükség van:
         * Channel - amin keresztül az üzenet jött. CallContext-nek kell tartalmaznia.
         * Session - a kapcsolat azonosítója, amin keresztül érkezett az üzenet, mert a válasz ezen keresztül megy majd vissza. CallContext-nek kell tartalmaznia.
         * ProxyId - ezzel beazonosítható, hogy kinek megy a válaszüzenet a túloldalon. Így talál vissza a hívó proxy-ra. CallContext-nek kell tartalmaznia.
         */

        /// <summary>
        /// Reply error message template
        /// </summary>
        protected static readonly String AUTO_SEND_REPLY_ERROR_MSG = "Unable to send response automatically. Reason: {0}";

        /// <summary>
        /// Singleton container
        /// </summary>
        protected static readonly Dictionary<Type, Object> mSingletonContainer = new Dictionary<Type, Object>(); // implType and instance

        /// <summary>
        /// Contrant and instances storage
        /// </summary>
        protected static readonly ListSpecialized<ContractAndInstanceStruct> mContractAndInstancePerSessionAndChannel = new ListSpecialized<ContractAndInstanceStruct>();

        /// <summary>
        /// Call contexts
        /// </summary>
        protected static readonly Dictionary<Thread, CallContextForReply> mCallContextForReply = new Dictionary<Thread, CallContextForReply>();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBase"/> class.
        /// </summary>
        protected ServiceBase()
        {
            ChannelServices.RegisterChannelEvent += new EventHandler<ChannelRegistrationEventArgs>(ChannelRegisterEventHandler);
            ChannelServices.UnregisterChannelEvent += new EventHandler<ChannelRegistrationEventArgs>(ChannelUnregisteredEventHandler);
            foreach (Channel channel in ChannelServices.RegisteredChannels)
            {
                channel.ReceiveMessage += new EventHandler<ReceiveMessageEventArgs>(ChannelReceiveMessageEventHandler);
            }
        }

        #endregion

        #region Public static method(s)

        /// <summary>
        /// Sends the response manually.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.InvalidOperationException">There is no waiting response context found.</exception>
        public static void SendResponseManually(Object value)
        {
            CallContextForReply cc = null;
            lock (mCallContextForReply)
            {
                if (mCallContextForReply.ContainsKey(Thread.CurrentThread))
                {
                    cc = mCallContextForReply[Thread.CurrentThread];
                }
                else
                {
                    throw new InvalidOperationException("There is no waiting response context found.");
                }
            }
            try
            {
                SendResponse(cc.Channel, cc.SessionId, cc.Message, cc.ReturnType, value, null, cc.ReturnTimeout);
            }
            finally
            {
                // sikeres küldés esetén törlünk, nehogy mégegyszer valaki elküldje
                lock (mCallContextForReply)
                {
                    mCallContextForReply.Remove(Thread.CurrentThread);
                }
            }
        }

        /// <summary>
        /// Sends the response manually.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="timeout">The timeout.</param>
        /// <exception cref="System.InvalidOperationException">There is no waiting response context found.</exception>
        public static void SendResponseManually(Object value, long timeout)
        {
            CallContextForReply cc = null;
            lock (mCallContextForReply)
            {
                if (mCallContextForReply.ContainsKey(Thread.CurrentThread))
                {
                    cc = mCallContextForReply[Thread.CurrentThread];
                }
                else
                {
                    throw new InvalidOperationException("There is no waiting response context found.");
                }
            }
            try
            {
                SendResponse(cc.Channel, cc.SessionId, cc.Message, cc.ReturnType, value, null, timeout);
            }
            finally
            {
                // sikeres küldés esetén törlünk, nehogy mégegyszer valaki elküldje
                lock (mCallContextForReply)
                {
                    mCallContextForReply.Remove(Thread.CurrentThread);
                }
            }
        }

        /// <summary>
        /// Checks the proxy registered.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <exception cref="Forge.Net.Remoting.ProxyNotRegisteredException"></exception>
        public static void CheckProxyRegistered(ProxyBase proxy)
        {
            if (proxy == null)
            {
                ThrowHelper.ThrowArgumentNullException("proxy");
            }

            bool result = false;

            lock (mContractAndInstancePerSessionAndChannel)
            {
                foreach (ContractAndInstanceStruct s in mContractAndInstancePerSessionAndChannel)
                {
                    if (s.Instance.Equals(proxy))
                    {
                        result = true;
                        break;
                    }
                }
            }

            if (!result)
            {
                throw new ProxyNotRegisteredException();
            }
        }

        /// <summary>
        /// Gets the peer proxy id.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>Identifier of the proxy</returns>
        /// <exception cref="Forge.Net.Remoting.ProxyNotRegisteredException"></exception>
        public static long GetPeerProxyId(ProxyBase proxy)
        {
            if (proxy == null)
            {
                ThrowHelper.ThrowArgumentNullException("proxy");
            }

            long result = -1;

            lock (mContractAndInstancePerSessionAndChannel)
            {
                foreach (ContractAndInstanceStruct s in mContractAndInstancePerSessionAndChannel)
                {
                    if (s.Instance.Equals(proxy))
                    {
                        result = s.ProxyId;
                        break;
                    }
                }
            }

            if (result == -1)
            {
                throw new ProxyNotRegisteredException();
            }

            return result;
        }

        /// <summary>
        /// Finds the method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="methodParams">The method params.</param>
        /// <returns>MedhotInfo</returns>
        /// <exception cref="System.MissingMethodException">
        /// </exception>
        public static MethodInfo FindMethod(Type type, String methodName, Type[] methodParams)
        {
            if (type == null)
            {
                ThrowHelper.ThrowArgumentNullException("type");
            }
            if (string.IsNullOrEmpty(methodName))
            {
                ThrowHelper.ThrowArgumentNullException("methodName");
            }

            MethodInfo m = FindMethodInner(type, methodName, methodParams);

            if (m == null)
            {
                throw new MissingMethodException(string.Format("Unable to find method on type '{0}'. Method name: '{1}'. Number of parameters: {2}", type.FullName, methodName, methodParams == null ? "0" : methodParams.Length.ToString()));
            }

            return m;
        }

        /// <summary>
        /// Gets the timeout by method.
        /// </summary>
        /// <param name="serviceContract">The service contract.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <param name="timeoutType">Type of the timeout.</param>
        /// <returns>Timeout value</returns>
        /// <exception cref="Forge.Net.Remoting.InvalidProxyImplementationException">
        /// </exception>
        public static long GetTimeoutByMethod(Type serviceContract, String methodName, MethodParameter[] parameterTypes, MethodTimeoutEnum timeoutType)
        {
            long result = 0;
            Type[] pts = null;
            if (parameterTypes != null && parameterTypes.Length > 0)
            {
                pts = new Type[parameterTypes.Length];
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    try
                    {
                        pts[i] = TypeHelper.GetTypeFromString(parameterTypes[i].ClassName);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidProxyImplementationException(String.Format("Provided parameter type '{0}' not resolved. This may be a proxy implementation error.", parameterTypes[i].ClassName), ex);
                    }
                }
            }
            try
            {
                MethodInfo m = ServiceBase.FindMethod(serviceContract, methodName, pts);
                OperationContractAttribute oc = TypeHelper.GetAttribute<OperationContractAttribute>(m);
                if (oc == null)
                {
                    // ez a metódus nem tartalmaz annotációt, ami csak akkor lehetséges, ha a proxy hibásan van generálva
                    throw new InvalidProxyImplementationException(String.Format("Provided method '{0}' found, but it has not got {1} annotation definition. This may be a proxy implementation error.", methodName, typeof(OperationContractAttribute).FullName));
                }
                else
                {
                    result = timeoutType == MethodTimeoutEnum.CallTimeout ? oc.CallTimeout : oc.ReturnTimeout;
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                if (pts != null)
                {
                    foreach (Type c in pts)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(", ");
                        }
                        sb.Append(c.Name);
                    }
                    sb.Insert(0, String.Format(" Parameter types: ", pts.ToString()));
                }
                throw new InvalidProxyImplementationException(String.Format("Unable to find method name '{0}' with parameter types.{1}", methodName, sb.ToString()), ex);
            }

            return result;
        }

        #endregion

        #region Internal static method(s)

        /// <summary>
        /// Registers the proxy.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="implType">Type of the impl.</param>
        /// <param name="sessionId">The session id.</param>
        /// <param name="proxyId">The proxy id.</param>
        /// <param name="instance">The instance.</param>
        internal static void RegisterProxy(Channel channel, Type contractType, Type implType, String sessionId, long proxyId, ProxyBase instance)
        {
            lock (mContractAndInstancePerSessionAndChannel)
            {
                mContractAndInstancePerSessionAndChannel.Add(new ContractAndInstanceStruct(channel, contractType, implType, sessionId, proxyId, instance));
            }
        }

        /// <summary>
        /// Unregisters the proxy.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        internal static void UnregisterProxy(ProxyBase proxy)
        {
            if (proxy == null)
            {
                ThrowHelper.ThrowArgumentNullException("proxy");
            }
            lock (mContractAndInstancePerSessionAndChannel)
            {
                IEnumeratorSpecialized<ContractAndInstanceStruct> iterator = mContractAndInstancePerSessionAndChannel.GetEnumerator();
                while (iterator.MoveNext())
                {
                    ContractAndInstanceStruct s = iterator.Current;
                    if (s.Instance.Equals(proxy))
                    {
                        iterator.Remove();
                        break;
                    }
                }
            }
        }

        private static MethodInfo FindMethodInner(Type type, String methodName, Type[] methodParams)
        {
            MethodInfo m = null;

            if (methodParams == null)
            {
                m = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            }
            else
            {
                m = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance, null, methodParams, null);
            }
            if (m == null)
            {
                if (type.IsInterface)
                {
                    if (type.GetInterfaces().Length > 0)
                    {
                        foreach (Type iType in type.GetInterfaces())
                        {
                            m = FindMethodInner(iType, methodName, methodParams);
                            if (m != null)
                            {
                                break;
                            }
                        }
                    }
                }
                else if (type.BaseType == null || type.BaseType.Equals(typeof(Object)))
                {
                    //throw new MissingMethodException();
                }
                else
                {
                    m = FindMethodInner(type.BaseType, methodName, methodParams);
                }
            }

            return m;
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Sends the response.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="sessionId">The session id.</param>
        /// <param name="rm">The rm.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="result">The result.</param>
        /// <param name="methodException">The method exception.</param>
        /// <param name="timeout">The timeout.</param>
        protected static void SendResponse(Channel channel, String sessionId, RequestMessage rm, Type returnType, Object result, Exception methodException, long timeout)
        {
            if (rm.MessageType == MessageTypeEnum.Request)
            {
                ResponseMessage response = new ResponseMessage(rm.CorrelationId, new MethodParameter(0, string.Format("{0}, {1}", returnType.FullName, new AssemblyName(returnType.Assembly.FullName).Name), result), methodException);
                response.Context.Add(ProxyBase.PROXY_ID, rm.Context[ProxyBase.PROXY_ID]);
                channel.SendMessage(sessionId, response, timeout);
            }
        }

        /// <summary>
        /// Channels the register event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Forge.Net.Remoting.Channels.ChannelRegistrationEventArgs"/> instance containing the event data.</param>
        protected virtual void ChannelRegisterEventHandler(object sender, ChannelRegistrationEventArgs e)
        {
            Channel channel = e.Channel;
            channel.ReceiveMessage += new EventHandler<ReceiveMessageEventArgs>(ChannelReceiveMessageEventHandler);
        }

        /// <summary>
        /// Channels the unregistered event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Forge.Net.Remoting.Channels.ChannelRegistrationEventArgs"/> instance containing the event data.</param>
        protected virtual void ChannelUnregisteredEventHandler(object sender, ChannelRegistrationEventArgs e)
        {
            Channel channel = e.Channel;
            channel.ReceiveMessage -= new EventHandler<ReceiveMessageEventArgs>(ChannelReceiveMessageEventHandler);
        }

        /// <summary>
        /// Channels the receive message event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Forge.Net.Remoting.Channels.ReceiveMessageEventArgs"/> instance containing the event data.</param>
        protected abstract void ChannelReceiveMessageEventHandler(object sender, ReceiveMessageEventArgs e);

        #endregion

        #region Nested classes

        /// <summary>
        /// Contract and instances
        /// </summary>
        protected sealed class ContractAndInstanceStruct
        {

            #region Field(s)

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Channel mChannel = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Type mContractType = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Type mImplType = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private String mSessionId = string.Empty;

            /*
             * Szerver oldalon azt mondja meg, hogy a kliens proxy-nak mi az azonosítója, akinek a hívására létrejött.
             * Így tudjuk megszólítani ClientSide metódus hívással.
             * 
             * Kliens oldalon pedig a szerver oldali hívások így találnak rá az instance-ra.
             */
            private long mProxyId = 0;

            private ProxyBase mInstance;

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="ContractAndInstanceStruct"/> class.
            /// </summary>
            /// <param name="channel">The channel.</param>
            /// <param name="contractType">Type of the contract.</param>
            /// <param name="implType">Type of the impl.</param>
            /// <param name="sessionId">The session id.</param>
            /// <param name="proxyId">The proxy id.</param>
            /// <param name="instance">The instance.</param>
            public ContractAndInstanceStruct(Channel channel, Type contractType, Type implType, String sessionId, long proxyId,
                ProxyBase instance)
            {
                if (channel == null)
                {
                    ThrowHelper.ThrowArgumentNullException("channel");
                }
                if (contractType == null)
                {
                    ThrowHelper.ThrowArgumentNullException("contractType");
                }
                if (implType == null)
                {
                    ThrowHelper.ThrowArgumentNullException("implType");
                }
                if (string.IsNullOrEmpty(sessionId))
                {
                    ThrowHelper.ThrowArgumentNullException("sessionId");
                }
                if (instance == null)
                {
                    ThrowHelper.ThrowArgumentNullException("instance");
                }
                if (!typeof(ProxyBase).IsAssignableFrom(implType))
                {
                    ThrowHelper.ThrowArgumentException("Type of implementation is not assignable from ProxyBase.");
                }

                this.mChannel = channel;
                this.mContractType = contractType;
                this.mImplType = implType;
                this.mSessionId = sessionId;
                this.mProxyId = proxyId;
                this.mInstance = instance;
            }

            #endregion

            #region Public properties

            /// <summary>
            /// Gets the channel.
            /// </summary>
            [DebuggerHidden]
            public Channel Channel
            {
                get { return mChannel; }
            }

            /// <summary>
            /// Gets the type of the contract.
            /// </summary>
            /// <value>
            /// The type of the contract.
            /// </value>
            [DebuggerHidden]
            public Type ContractType
            {
                get { return mContractType; }
            }

            /// <summary>
            /// Gets the type of the impl.
            /// </summary>
            /// <value>
            /// The type of the impl.
            /// </value>
            [DebuggerHidden]
            public Type ImplType
            {
                get { return mImplType; }
            }

            /// <summary>
            /// Gets the session id.
            /// </summary>
            [DebuggerHidden]
            public String SessionId
            {
                get { return mSessionId; }
            }

            /// <summary>
            /// Gets the instance.
            /// </summary>
            [DebuggerHidden]
            public ProxyBase Instance
            {
                get { return mInstance; }
            }

            /// <summary>
            /// Gets the proxy id.
            /// </summary>
            [DebuggerHidden]
            public long ProxyId
            {
                get { return mProxyId; }
            }

            #endregion

        }

        /// <summary>
        /// Call context reply
        /// </summary>
        protected sealed class CallContextForReply
        {

            #region Field(s)

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Channel mChannel = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private String mSessionId = string.Empty;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private RequestMessage mMessage = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Type mReturnType = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private long mReturnTimeout = OperationContractAttribute.DEFAULT_METHOD_TIMEOUT;

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="CallContextForReply"/> class.
            /// </summary>
            /// <param name="channel">The channel.</param>
            /// <param name="sessionId">The session id.</param>
            /// <param name="message">The message.</param>
            /// <param name="returnType">Type of the return.</param>
            /// <param name="returnTimeout">The return timeout.</param>
            public CallContextForReply(Channel channel, String sessionId, RequestMessage message, Type returnType, long returnTimeout)
            {
                if (channel == null)
                {
                    ThrowHelper.ThrowArgumentNullException("channel");
                }
                if (string.IsNullOrEmpty(sessionId))
                {
                    ThrowHelper.ThrowArgumentNullException("sessionId");
                }
                if (message == null)
                {
                    ThrowHelper.ThrowArgumentNullException("message");
                }
                if (returnType == null)
                {
                    ThrowHelper.ThrowArgumentNullException("returnType");
                }
                this.mChannel = channel;
                this.mSessionId = sessionId;
                this.mMessage = message;
                this.mReturnType = returnType;
                this.mReturnTimeout = returnTimeout;
            }

            #endregion

            #region Public properties

            /// <summary>
            /// Gets the channel.
            /// </summary>
            [DebuggerHidden]
            public Channel Channel
            {
                get { return mChannel; }
            }

            /// <summary>
            /// Gets the session id.
            /// </summary>
            [DebuggerHidden]
            public String SessionId
            {
                get { return mSessionId; }
            }

            /// <summary>
            /// Gets the message.
            /// </summary>
            [DebuggerHidden]
            public RequestMessage Message
            {
                get { return mMessage; }
            }

            /// <summary>
            /// Gets the type of the return.
            /// </summary>
            /// <value>
            /// The type of the return.
            /// </value>
            [DebuggerHidden]
            public Type ReturnType
            {
                get { return mReturnType; }
            }

            /// <summary>
            /// Gets the return timeout.
            /// </summary>
            [DebuggerHidden]
            public long ReturnTimeout
            {
                get { return mReturnTimeout; }
            }

            #endregion

        }

        #endregion

    }

}
