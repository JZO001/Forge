/* *********************************************************************
 * Date: 09 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Threading;
using Forge.Net.Synapse;
using Forge.Net.TerraGraf.Messaging;
using Forge.Net.TerraGraf.NetworkPeers;

namespace Forge.Net.TerraGraf.Connection
{

    /// <summary>
    /// Represents a connection task
    /// </summary>
    internal sealed class NetworkConnectionTask : MBRBase, IDisposable
    {

        #region Field(s)

        private static Int64 mTaskIdCounter = 0; // globális feladatazonosító

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long mTaskId = Interlocked.Increment(ref mTaskIdCounter); // feladatazonosító kiosztó

        private NetworkConnection mNetworkConnection = null; // hálózati kapcsolat osztálya

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mLocalRequest = false; // true, ha mi indítottuk a kapcsolódást, false, ha egy TCP szerveren esett be a kapcsolat

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mRemoteNetworkPeerId = string.Empty; // távoli peer azonosítója

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mRemoteNetworkContextName = string.Empty; // kézfogáskor a másik oldal által közölt network context név, ami kell a gráf üzenet szűréséhez

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mDisconnectOnConnectionDuplication = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long mReplyTime = 0;

        private ManualResetEvent mConnectionEvent = null;

        private Stopwatch mWatch = null;

        private readonly object mLockObject = new object();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkConnectionTask"/> class.
        /// </summary>
        /// <param name="networkStream">The network stream.</param>
        /// <param name="localRequest">if set to <c>true</c> [local request].</param>
        /// <param name="disconnectOnConnectionDuplication">if set to <c>true</c> [disconnect on connection duplication].</param>
        internal NetworkConnectionTask(NetworkStream networkStream, bool localRequest, bool disconnectOnConnectionDuplication)
        {
            if (networkStream == null)
            {
                ThrowHelper.ThrowArgumentNullException("networkStream");
            }
            this.mNetworkConnection = new NetworkConnection(this, networkStream);
            this.mLocalRequest = localRequest;
            this.mDisconnectOnConnectionDuplication = disconnectOnConnectionDuplication;
        }

        #endregion

        #region Public member(s)

        /// <summary>
        /// Gets the task id.
        /// </summary>
        /// <value>
        /// The task id.
        /// </value>
        [DebuggerHidden]
        internal long TaskId
        {
            get { return mTaskId; }
        }

        /// <summary>
        /// Gets the remote host peer.
        /// </summary>
        /// <value>
        /// The remote host peer.
        /// </value>
        internal NetworkPeerRemote RemoteHostPeer
        {
            get { return mNetworkConnection.OwnerSession == null ? null : mNetworkConnection.OwnerSession.RemotePeer; }
        }

        /// <summary>
        /// Gets a value indicating whether [local request].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [local request]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        internal bool LocalRequest
        {
            get { return mLocalRequest; }
        }

        /// <summary>
        /// Gets a value indicating whether [disconnect on connection duplication].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [disconnect on connection duplication]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        internal bool DisconnectOnConnectionDuplication
        {
            get { return mDisconnectOnConnectionDuplication; }
        }

        /// <summary>
        /// Waits for connection.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>True, if ok, False on timeout</returns>
        internal bool WaitForConnection(int timeout)
        {
            bool result = false;
            lock (mLockObject)
            {
                if (mWatch == null)
                {
                    mWatch = Stopwatch.StartNew();
                }
                if (mConnectionEvent == null)
                {
                    mConnectionEvent = new ManualResetEvent(false);
                }
            }
            if (result = !mConnectionEvent.WaitOne(timeout))
            {
                mNetworkConnection.InternalClose(); // időtúllépés
            }
            return result;
        }

        /// <summary>
        /// Raises the connection task finished.
        /// </summary>
        internal void RaiseConnectionTaskFinished()
        {
            lock (mLockObject)
            {
                if (mWatch != null && mWatch.IsRunning)
                {
                    mWatch.Stop();
                    mReplyTime = mWatch.ElapsedMilliseconds;
                }
                if (mConnectionEvent != null)
                {
                    try
                    {
                        mConnectionEvent.Set();
                    }
                    catch (Exception) { }
                }
            }
        }

        /// <summary>
        /// Gets the network connection.
        /// </summary>
        /// <value>
        /// The network connection.
        /// </value>
        [DebuggerHidden]
        internal NetworkConnection NetworkConnection
        {
            get { return mNetworkConnection; }
        }

        /// <summary>
        /// Gets or sets the remote network peer id.
        /// </summary>
        /// <value>
        /// The remote network peer id.
        /// </value>
        [DebuggerHidden]
        internal string RemoteNetworkPeerId
        {
            get { return mRemoteNetworkPeerId; }
            set { mRemoteNetworkPeerId = value; }
        }

        /// <summary>
        /// Gets or sets the name of the remote network context.
        /// </summary>
        /// <value>
        /// The name of the remote network context.
        /// </value>
        [DebuggerHidden]
        internal string RemoteNetworkContextName
        {
            get { return mRemoteNetworkContextName; }
            set { mRemoteNetworkContextName = value; }
        }

        /// <summary>
        /// Gets or sets the reply time.
        /// </summary>
        /// <value>
        /// The reply time.
        /// </value>
        [DebuggerHidden]
        internal long ReplyTime
        {
            get { return mReplyTime; }
        }

        /// <summary>
        /// Gets or sets the information message.
        /// </summary>
        /// <value>
        /// The information message.
        /// </value>
        [DebuggerHidden]
        internal TerraGrafInformationMessage InformationMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Stops the watch.
        /// </summary>
        internal void StopWatch()
        {
            lock (mLockObject)
            {
                if (mWatch != null && mWatch.IsRunning)
                {
                    mWatch.Stop();
                    mReplyTime = mWatch.ElapsedMilliseconds;
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            lock (mLockObject)
            {
                if (mWatch != null && mWatch.IsRunning)
                {
                    mWatch.Stop();
                    mReplyTime = mWatch.ElapsedMilliseconds;
                }
                if (mConnectionEvent != null)
                {
                    mConnectionEvent.Dispose();
                    mConnectionEvent = null;
                }
            }
            this.mNetworkConnection = null;
            GC.SuppressFinalize(this);
        }

        #endregion

    }

}
