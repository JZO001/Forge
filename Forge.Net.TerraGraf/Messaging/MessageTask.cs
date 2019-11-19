/* *********************************************************************
 * Date: 09 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Threading;
using Forge.Net.TerraGraf.Contexts;
using Forge.Net.TerraGraf.NetworkPeers;

namespace Forge.Net.TerraGraf.Messaging
{

    /// <summary>
    /// Represents a task for message sending
    /// </summary>
    internal sealed class MessageTask : MBRBase, IDisposable, IEquatable<MessageTask>
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TerraGrafMessageBase mMessage = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mSuccess = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private EventWaitHandle mSentEvent = null;

        private Stopwatch mWatch = null;

        private readonly object mLockObject = new object();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageTask"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        internal MessageTask(TerraGrafMessageBase message)
        {
            if (message == null)
            {
                ThrowHelper.ThrowArgumentNullException("message");
            }
            this.mMessage = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageTask"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sentEvent">The sent event.</param>
        internal MessageTask(TerraGrafMessageBase message, EventWaitHandle sentEvent)
            : this(message)
        {
            this.mSentEvent = sentEvent;
            if (!string.IsNullOrEmpty(message.TargetId) && NetworkManager.Instance.InternalLocalhost.Id.Equals(message.SenderId))
            {
                // saját TCP és nem Broadcast UDP üzeneteknél azonnal indul az időmérés
                mWatch = Stopwatch.StartNew();
            }
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [DebuggerHidden]
        internal TerraGrafMessageBase Message
        {
            get { return mMessage; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is success.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is success; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        internal bool IsSuccess
        {
            get { return mSuccess; }
            set { mSuccess = value; }
        }

        /// <summary>
        /// Gets the reply time.
        /// </summary>
        /// <value>
        /// The reply time.
        /// </value>
        internal long ReplyTime
        {
            get { return mWatch == null ? Timeout.Infinite : mWatch.ElapsedMilliseconds; }
        }

        /// <summary>
        /// Gets the sent event.
        /// </summary>
        /// <value>
        /// The sent event.
        /// </value>
        internal EventWaitHandle SentEvent
        {
            get { return mSentEvent; }
        }

        /// <summary>
        /// Gets the time watch.
        /// </summary>
        /// <value>
        /// The time watch.
        /// </value>
        internal Stopwatch TimeWatch
        {
            get { return mWatch; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [message subscribed to acknowledge].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [message subscribed to acknowledge]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        internal bool MessageSubscribedToAcknowledge
        {
            get;
            set;
        }

        #endregion

        #region Method(s)

        /// <summary>
        /// Waits for sent event, if the message is TCP and sent by me.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        internal void WaitForSentEvent(int timeout)
        {
            if (mMessage.MessageType == MessageTypeEnum.Tcp && NetworkManager.Instance.InternalLocalhost.Id.Equals(mMessage.SenderId))
            {
                // csak a saját TCP üzeneteimre lehet várakozni
                lock (mLockObject)
                {
                    if (mWatch == null)
                    {
                        mWatch = Stopwatch.StartNew();
                    }
                    if (mSentEvent == null)
                    {
                        mSentEvent = new ManualResetEvent(false);
                    }
                }
                if (mSentEvent.WaitOne(timeout) && IsSuccess)
                {
                    // replytime feljegyzése
                    NetworkPeerRemote networkPeer = (NetworkPeerRemote)NetworkPeerContext.GetNetworkPeerById(mMessage.TargetId);
                    networkPeer.Session.ReplyTime = ReplyTime;
                }
            }
            else
            {
                IsSuccess = true;
            }
        }

        /// <summary>
        /// Raises the sent event finished.
        /// </summary>
        internal void RaiseSentEventFinished()
        {
            lock (mLockObject)
            {
                if (mWatch != null && mWatch.IsRunning)
                {
                    mWatch.Stop();
                }
                if (mSentEvent != null)
                {
                    try
                    {
                        mSentEvent.Set();
                    }
                    catch (Exception) { }
                }
            }
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(MessageTask other)
        {
            return Equals((object)other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!obj.GetType().Equals(GetType())) return false;

            MessageTask other = (MessageTask)obj;
            return other.mMessage == mMessage && other.mSuccess == mSuccess;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
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
                }
                if (mSentEvent != null)
                {
                    mSentEvent.Dispose();
                    mSentEvent = null;
                }
            }
            GC.SuppressFinalize(this);
        }

        #endregion

    }

}
