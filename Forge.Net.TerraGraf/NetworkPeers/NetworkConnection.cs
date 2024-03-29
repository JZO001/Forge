﻿/* *********************************************************************
 * Date: 08 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using Forge.Invoker;
using Forge.Legacy;
using Forge.Logging.Abstraction;
using Forge.Net.Synapse;
using Forge.Net.TerraGraf.Connection;
using Forge.Net.TerraGraf.Formatters;
using Forge.Net.TerraGraf.Messaging;
using Forge.Shared;

namespace Forge.Net.TerraGraf.NetworkPeers
{

    /// <summary>
    /// Represents a live network connection with its physical reply time
    /// </summary>
    //[Serializable]
    internal sealed class NetworkConnection : MBRBase, IEquatable<NetworkConnection>, INetworkConnection
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger<NetworkConnection>();

        private static readonly Forge.Threading.ThreadPool THREADPOOL = new Forge.Threading.ThreadPool("TerraGraf_Network_Send");

        private static readonly byte[] LOW_LEVEL_RESPONSE_BYTE = new byte[] { (byte)1 };

        //[NonSerialized]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NetworkStream mNetworkStream = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long mReplyTime = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NetworkPeerSession mOwnerSession = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NetworkConnectionTask mConnectionTask = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mConnected = true;

        private List<Queue<MessageTask>> mMessageQueues = new List<Queue<MessageTask>>();

        private List<MessageTask> mSentTCPMessages = new List<MessageTask>();

        private byte[] mReceiveBuffer = new byte[1];

        private readonly MessageFormatter<TerraGrafMessageBase> mMessageFormatter = new MessageFormatter<TerraGrafMessageBase>();

        private bool mInitialized = false;

        private long mTotalMessagesToSend = 0;

        private readonly object LOCK_OBJECT = new object();

        /// <summary>
        /// Occurs when [disconnect].
        /// </summary>
        internal event EventHandler<EventArgs> Disconnect;

        /// <summary>
        /// Occurs when [message arrived].
        /// </summary>
        internal event EventHandler<MessageArrivedEventArgs> MessageArrived;

        /// <summary>
        /// Occurs when [message send before].
        /// </summary>
        internal event EventHandler<MessageSendEventArgs> MessageSendBefore;

        /// <summary>
        /// Occurs when [message send after].
        /// </summary>
        internal event EventHandler<MessageSendEventArgs> MessageSendAfter;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkConnection"/> class.
        /// </summary>
        /// <param name="connectionTask">The connection task.</param>
        /// <param name="stream">The stream.</param>
        internal NetworkConnection(NetworkConnectionTask connectionTask, NetworkStream stream)
        {
            if (connectionTask == null)
            {
                ThrowHelper.ThrowArgumentNullException("connectionTask");
            }
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }
            mConnectionTask = connectionTask;
            mNetworkStream = stream;
            mReceiveBuffer = new byte[stream.ReceiveBufferSize];
            mMessageQueues.Add(new Queue<MessageTask>()); // Priority 1 (acknowledges)
            mMessageQueues.Add(new Queue<MessageTask>()); // Priority 2 (system messages)
            mMessageQueues.Add(new Queue<MessageTask>()); // Priority 3 (user level messages)
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public long Id
        {
            get { return mNetworkStream.Id; }
        }

        /// <summary>
        /// Gets the network stream.
        /// </summary>
        /// <value>
        /// The network stream.
        /// </value>
        [DebuggerHidden]
        internal NetworkStream NetworkStream
        {
            get { return mNetworkStream; }
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
            set { mReplyTime = value; }
        }

        /// <summary>
        /// Gets or sets the owner session.
        /// </summary>
        /// <value>
        /// The owner session.
        /// </value>
        [DebuggerHidden]
        internal NetworkPeerSession OwnerSession
        {
            get { return mOwnerSession; }
            set { mOwnerSession = value; }
        }

        /// <summary>
        /// Gets the connection task.
        /// </summary>
        /// <value>
        /// The connection task.
        /// </value>
        [DebuggerHidden]
        internal NetworkConnectionTask ConnectionTask
        {
            get { return mConnectionTask; }
            set { mConnectionTask = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool IsConnected
        {
            get { return mConnected; }
        }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Copies the content of the other network connection.
        /// </summary>
        /// <param name="other">The other.</param>
        internal void CopyOtherNetworkConnectionContent(NetworkConnection other)
        {
            if (other == null)
            {
                ThrowHelper.ThrowArgumentNullException("other");
            }
            lock (mSentTCPMessages)
            {
                if (mSentTCPMessages.Count > 0)
                {
                    // re-sending: sent, but not confirmed (ack) TCP messages
                    foreach (MessageTask task in mSentTCPMessages)
                    {
                        other.AddMessageTask(task);
                    }
                    mSentTCPMessages.Clear();
                }
                for (int i = 0; i < mMessageQueues.Count; i++)
                {
                    Queue<MessageTask> queue = mMessageQueues[i];
                    lock (queue)
                    {
                        while (queue.Count > 0)
                        {
                            MessageTask task = queue.Dequeue();
                            if (task.Message.MessageCode != MessageCodeEnum.LowLevelTcpAcknowledge)
                            {
                                other.AddMessageTask(task);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the finished to task failed.
        /// </summary>
        /// <param name="messagesToAcknowledge">The messages to acknowledge.</param>
        internal void SetFinishedToTaskFailed(Dictionary<long, MessageTask> messagesToAcknowledge)
        {
            for (int i = 0; i < mMessageQueues.Count; i++)
            {
                Queue<MessageTask> queue = mMessageQueues[i];
                lock (queue)
                {
                    while (queue.Count > 0)
                    {
                        MessageTask task = queue.Dequeue();
                        lock (messagesToAcknowledge)
                        {
                            // removing from the waiting items
                            if (messagesToAcknowledge.ContainsKey(task.Message.MessageId))
                            {
                                task.IsSuccess = false;
                                task.RaiseSentEventFinished();
                                messagesToAcknowledge.Remove(task.Message.MessageId);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void Initialize()
        {
            if (!mInitialized)
            {
                StartReceive();
                mInitialized = true;
            }
        }

        /// <summary>
        /// Adds the message task.
        /// </summary>
        /// <param name="messageTask">The message task.</param>
        internal void AddMessageTask(MessageTask messageTask)
        {
            if (messageTask == null)
            {
                ThrowHelper.ThrowArgumentNullException("messageTask");
            }

            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK_CONNECTION({0}), queuing a new message to send. {1}", mNetworkStream.Id, messageTask.Message.ToString()));

            Queue<MessageTask> queue = mMessageQueues[(int)messageTask.Message.Priority];
            lock (queue)
            {
                queue.Enqueue(messageTask);
            }
            Executor.Invoke(MessageSendBefore, this, new MessageSendEventArgs(messageTask, false));
            lock (LOCK_OBJECT)
            {
                mTotalMessagesToSend++;
                if (mTotalMessagesToSend == 1)
                {
                    THREADPOOL.QueueUserWorkItem(new WaitCallback(Send));
                }
            }
        }

        /// <summary>
        /// Internals the close.
        /// </summary>
        internal void InternalClose()
        {
            mConnected = false;
            mNetworkStream.Close();
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            mNetworkStream.Close();
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

            NetworkConnection other = (NetworkConnection)obj;
            return other.mNetworkStream.Equals(mNetworkStream);
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>True, if the other class is equals with this.</returns>
        public bool Equals(NetworkConnection other)
        {
            return Equals((object)other);
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

        #endregion

        #region Private method(s)

        private void StartReceive()
        {
            // ez dobhat kivételt
            if (mNetworkStream.Connected)
            {
                try
                {
                    mNetworkStream.BeginRead(mReceiveBuffer, 0, 1, new AsyncCallback(EndReceive), this);
                }
                catch (Exception)
                {
                    HandleDisconnection();
                }
            }
            else
            {
                HandleDisconnection();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Send(object state)
        {
            if (mConnected)
            {
                MessageTask task = null;
                Queue<MessageTask> selectedQueue = null;
                foreach (Queue<MessageTask> queue in mMessageQueues)
                {
                    lock (queue)
                    {
                        if (queue.Count > 0)
                        {
                            selectedQueue = queue;
                            task = queue.Peek();
                            break;
                        }
                    }
                }

                if (task != null)
                {
                    Stopwatch watch = Stopwatch.StartNew();
                    try
                    {
                        if (task.Message.MessageCode == MessageCodeEnum.LowLevelTcpAcknowledge)
                        {
                            // low-level feedback
                            mNetworkStream.Write(LOW_LEVEL_RESPONSE_BYTE, 0, LOW_LEVEL_RESPONSE_BYTE.Length);
                        }
                        else
                        {
                            if (task.Message.MessageType == MessageTypeEnum.Tcp)
                            {
                                // requests a low-level feedback about the TCP messages
                                lock (mSentTCPMessages)
                                {
                                    mSentTCPMessages.Add(task);
                                }
                            }
                            mMessageFormatter.Write(mNetworkStream, task.Message);
                        }
                        watch.Stop();
                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK_CONNNECTION, Id: {0}, Message sent. MessageId: {1}", mNetworkStream.Id.ToString(), task.Message.MessageId.ToString()));
                        mReplyTime = watch.ElapsedMilliseconds;
                        lock (selectedQueue)
                        {
                            selectedQueue.Dequeue();
                        }
                        // sending was successful
                        Executor.Invoke(MessageSendAfter, this, new MessageSendEventArgs(task, false));
                        lock (LOCK_OBJECT)
                        {
                            mTotalMessagesToSend--;
                            if (mTotalMessagesToSend > 0)
                            {
                                THREADPOOL.QueueUserWorkItem(new WaitCallback(Send));
                            }
                        }
                    }
                    catch (ObjectDisposedException ex)
                    {
                        // network error (closed stream)
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("Id: {0}, {1}, {2}", mNetworkStream.Id, ex.GetType().Name, ex.Message));
                        HandleDisconnection();
                    }
                    catch (IOException ex)
                    {
                        // network error
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("Id: {0}, {1}, {2}", mNetworkStream.Id, ex.GetType().Name, ex.Message));
                        mNetworkStream.Dispose();
                        HandleDisconnection();
                    }
                    catch (InvalidOperationException ex)
                    {
                        // network error
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("Id: {0}, {1}, {2}", mNetworkStream.Id, ex.GetType().Name, ex.Message));
                        mNetworkStream.Dispose();
                        HandleDisconnection();
                    }
                    catch (Exception ex)
                    {
                        // unknown error
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("Id: {0}, {1}, {2}", mNetworkStream.Id, ex.GetType().Name, ex.Message));
                        lock (selectedQueue)
                        {
                            selectedQueue.Dequeue();
                        }
                        mNetworkStream.Dispose();
                        HandleDisconnection();
                    }
                    finally
                    {
                        if (watch.IsRunning)
                        {
                            watch.Stop();
                        }
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void EndReceive(IAsyncResult result)
        {
            try
            {
                int len = mNetworkStream.EndRead(result);
                if (len > 0)
                {
                    if (mReceiveBuffer[0].Equals((byte)1))
                    {
                        // low-level feedback
                        lock (mSentTCPMessages)
                        {
                            if (mSentTCPMessages.Count > 0)
                            {
                                mSentTCPMessages.RemoveAt(0);
                            }
                        }
                        StartReceive();
                    }
                    else if (mReceiveBuffer[0].Equals((byte)0))
                    {
                        TerraGrafMessageBase message = mMessageFormatter.Read(mNetworkStream);
                        if (message.MessageType == MessageTypeEnum.Tcp)
                        {
                            // sending low-level feedback
                            lock (mSentTCPMessages)
                            {
                                if (mConnected)
                                {
                                    MessageLowLevelAcknowledge msg = new MessageLowLevelAcknowledge();
                                    AddMessageTask(new MessageTask(msg));
                                }
                            }
                        }
                        Executor.Invoke(MessageArrived, this, new MessageArrivedEventArgs(message));
                        StartReceive();
                    }
                    else
                    {
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK_CONNECTION: invalid start code in network stream. StreamId: {0}", mNetworkStream.Id));
                        mNetworkStream.Close();
                        HandleDisconnection();
                    }
                }
                else
                {
                    if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("Id: {0}, zero bytes received - socket disconnected.", mNetworkStream.Id));
                    HandleDisconnection();
                }
            }
            catch (Exception ex)
            {
                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("Id: {0}, {1}, {2}", mNetworkStream.Id, ex.GetType().Name, ex.Message));
                HandleDisconnection();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void HandleDisconnection()
        {
            if (mConnected)
            {
                mConnected = false;
                Executor.Invoke(Disconnect, this, EventArgs.Empty);
            }
        }

        #endregion

    }

}
