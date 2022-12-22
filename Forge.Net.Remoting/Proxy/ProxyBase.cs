/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Threading;
using Forge.Legacy;
using Forge.Net.Remoting.Channels;
using Forge.Net.Remoting.Messaging;
using Forge.Shared;

namespace Forge.Net.Remoting.Proxy
{

    /// <summary>
    /// Base class for Proxies
    /// </summary>
    public abstract class ProxyBase : MBRBase, IRemoteContract, IDisposable
    {

        #region Field(s)

        /// <summary>
        /// Represents the constant which identifiy the proxy identifier in the call context
        /// </summary>
        public static readonly string PROXY_ID = "PROXY_ID";

        private static long mProxyIdAllocator = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long mProxyId = 0;

        /// <summary>
        /// The channel to used
        /// </summary>
        protected readonly Channel mChannel = null;

        /// <summary>
        /// The session identifier
        /// </summary>
        protected string mSessionId = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mDisposed = false;

        /*
         * Szerver oldalon szükség van:
         * Channel - amin keresztül az üzenet jött. CallContext-nek kell tartalmaznia.
         * Session - a kapcsolat azonosítója, amin keresztül érkezett az üzenet, mert a válasz ezen keresztül megy majd vissza. CallContext-nek kell tartalmaznia.
         * ProxyId - ezzel beazonosítható, hogy kinek megy a válaszüzenet a túloldalon. Így talál vissza a hívó proxy-ra. CallContext-nek kell tartalmaznia.
         */

        /*
         * Kliens oldalon szükség van:
         * Channel - amin kiküldöm az üzenetet
         * Session - a kapcsolat azonosítója, ahová megy az üzenet
         * ProxyId - ezzel vagyok beazonosítható, ha jön a válaszüzenet. Így talál vissza hozzám.
         */

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyBase"/> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="sessionId">The session id.</param>
        protected ProxyBase(Channel channel, string sessionId)
        {
            if (channel == null)
            {
                ThrowHelper.ThrowArgumentNullException("channel");
            }
            if (string.IsNullOrEmpty("sessionId"))
            {
                ThrowHelper.ThrowArgumentNullException("sessionId");
            }
            mChannel = channel;
            mSessionId = sessionId;
            mProxyId = Interlocked.Increment(ref mProxyIdAllocator);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="ProxyBase"/> is reclaimed by garbage collection.
        /// </summary>
        ~ProxyBase()
        {
            Dispose(false);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        public Channel Channel
        {
            get
            {
                return mChannel;
            }
        }

        /// <summary>
        /// Gets the proxy id.
        /// </summary>
        /// <value>
        /// The proxy id.
        /// </value>
        public long ProxyId
        {
            get { return mProxyId; }
        }

        /// <summary>
        /// Gets the session id.
        /// </summary>
        /// <value>
        /// The session id.
        /// </value>
        public string SessionId
        {
            get { return mSessionId; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed
        {
            get { return mDisposed; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Sends the response manually.
        /// </summary>
        /// <param name="value">The value.</param>
        protected void SendResponseManually(object value)
        {
            ServiceBase.SendResponseManually(value);
        }

        /// <summary>
        /// Sends the response manually.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="returnTimeout">The return timeout.</param>
        protected void SendResponseManually(object value, long returnTimeout)
        {
            ServiceBase.SendResponseManually(value, returnTimeout);
        }

        /// <summary>
        /// Gets the timeout by method.
        /// </summary>
        /// <param name="serviceContract">The service contract.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <param name="timeoutType">Type of the timeout.</param>
        /// <returns></returns>
        protected long GetTimeoutByMethod(Type serviceContract, string methodName, MethodParameter[] parameterTypes, MethodTimeoutEnum timeoutType)
        {
            return ServiceBase.GetTimeoutByMethod(serviceContract, methodName, parameterTypes, timeoutType);
        }

        /// <summary>
        /// Does the dispose check.
        /// </summary>
        protected void DoDisposeCheck()
        {
            if (mDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!mDisposed)
            {
                ServiceBase.UnregisterProxy(this);
                mDisposed = true;
                if (!mChannel.IsSessionReusable)
                {
                    mChannel.Disconnect(mSessionId);
                }
            }
        }

        #endregion

    }

}
