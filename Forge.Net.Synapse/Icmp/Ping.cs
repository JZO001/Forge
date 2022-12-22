/* *********************************************************************
 * Date: 10 Oct 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Forge.Invoker;
using Forge.Legacy;
using Forge.Shared;
using Forge.Threading;
using Forge.Threading.Tasking;

namespace Forge.Net.Synapse.Icmp
{

#if NET40

    /// <summary>
    /// Delegate for async execution
    /// </summary>
    /// <param name="host">The host.</param>
    /// <param name="dataLength">Length of the data.</param>
    /// <param name="pingCount">The ping count.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="waitTimeBetweenAttempts">The wait time between attempts.</param>
    internal delegate void ExecuteDelegate(string host, int dataLength, int pingCount, int timeout, int waitTimeBetweenAttempts);

#endif

    /// <summary>
    /// ICMP Ping implementation
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public sealed class Ping : MBRBase
    {

        #region Field(s)

        /// <summary>
        /// Default packet size for icmp
        /// </summary>
        public static readonly int DEFAULT_PACKET_SIZE = 32;

        /// <summary>
        /// Default attempt number on an execution
        /// </summary>
        public static readonly int DEFAULT_PING_COUNTER = 4;

        /// <summary>
        /// Default timeout for a ping attempt
        /// </summary>
        public static readonly int DEFAULT_PING_TIMEOUT = 5000;

        /// <summary>
        /// Wait time between attempts
        /// </summary>
        public static readonly int DEFAULT_WAITTIME_ATTEMPTS = 1000;

        private static readonly int MAX_PACKET_SIZE = 65536;

        private static readonly int SOCKET_ERROR = -1;

        private static readonly byte ICMP_ECHO = 8;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mAveragePingTime = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IPAddress mRemoteIpAddress = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mMaximumPingTime = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mMinimumPingTime = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mPacketLost = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double mPacketLostPercent = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mPacketReceived = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mPacketSent = 0;

        private int mPingLoop = 0;

#if NET40
        private ExecuteDelegate mExecutionDelegate = null;
#endif
        private System.Action<string, int, int, int, int> mExecutionActionDelegate = null;
        private int mAsyncActiveExecutionCount = 0;
        private AutoResetEvent mAsyncActiveExecutionEvent = null;

        private readonly object LOCK_EXECUTE = new object();

        /// <summary>
        /// Occurs when [event ping result].
        /// </summary>
        public event EventHandler<PingResultEventArgs> EventPingResult;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="Ping"/> class.
        /// </summary>
        public Ping()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the average ping time.
        /// </summary>
        /// <value>
        /// The average ping time.
        /// </value>
        [DebuggerHidden]
        public int AveragePingTime
        {
            get { return mAveragePingTime; }
        }

        /// <summary>
        /// Gets the remote ip address.
        /// </summary>
        /// <value>
        /// The remote ip address.
        /// </value>
        [DebuggerHidden]
        public IPAddress RemoteIpAddress
        {
            get { return mRemoteIpAddress; }
        }

        /// <summary>
        /// Gets the maximum ping time.
        /// </summary>
        /// <value>
        /// The maximum ping time.
        /// </value>
        [DebuggerHidden]
        public int MaximumPingTime
        {
            get { return mMaximumPingTime; }
        }

        /// <summary>
        /// Gets the minimum ping time.
        /// </summary>
        /// <value>
        /// The minimum ping time.
        /// </value>
        [DebuggerHidden]
        public int MinimumPingTime
        {
            get { return mMinimumPingTime; }
        }

        /// <summary>
        /// Gets the packet lost.
        /// </summary>
        /// <value>
        /// The packet lost.
        /// </value>
        [DebuggerHidden]
        public int PacketLost
        {
            get { return mPacketLost; }
        }

        /// <summary>
        /// Gets the packet lost percent.
        /// </summary>
        /// <value>
        /// The packet lost percent.
        /// </value>
        [DebuggerHidden]
        public double PacketLostPercent
        {
            get { return mPacketLostPercent; }
        }

        /// <summary>
        /// Gets the packet received.
        /// </summary>
        /// <value>
        /// The packet received.
        /// </value>
        [DebuggerHidden]
        public int PacketReceived
        {
            get { return mPacketReceived; }
        }

        /// <summary>
        /// Gets the packet sent.
        /// </summary>
        /// <value>
        /// The packet sent.
        /// </value>
        [DebuggerHidden]
        public int PacketSent
        {
            get { return mPacketSent; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is finished.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is finished; otherwise, <c>false</c>.
        /// </value>
        public bool IsFinished { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [sync event raiser].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [sync event raiser]; otherwise, <c>false</c>.
        /// </value>
        [Obsolete]
        public bool SyncEventRaiser { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [UI invoke on events].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [UI invoke on events]; otherwise, <c>false</c>.
        /// </value>
        [Obsolete]
        public bool UIInvocationOnEvents { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [parallel invocation on events].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [parallel invocation on events]; otherwise, <c>false</c>.
        /// </value>
        [Obsolete]
        public bool ParallelInvocationOnEvents { get; set; }

        #endregion

        #region Public method(s)

#if NET40

        /// <summary>
        /// Begins the execute.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginExecute(string host, AsyncCallback callback, object state)
        {
            return BeginExecute(host, DEFAULT_PACKET_SIZE, DEFAULT_PING_COUNTER, DEFAULT_PING_TIMEOUT, DEFAULT_WAITTIME_ATTEMPTS, callback, state);
        }

        /// <summary>
        /// Begins the execute.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="dataLength">Length of the data.</param>
        /// <param name="pingCount">The ping count.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="waitTimeBetweenAttempts">The wait time between attempts.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginExecute(string host, int dataLength, int pingCount, int timeout, int waitTimeBetweenAttempts, AsyncCallback callback, object state)
        {
            Interlocked.Increment(ref mAsyncActiveExecutionCount);
            ExecuteDelegate d = new ExecuteDelegate(Execute);
            if (mAsyncActiveExecutionEvent == null)
            {
                lock (LOCK_EXECUTE)
                {
                    if (mAsyncActiveExecutionEvent == null)
                    {
                        mAsyncActiveExecutionEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveExecutionEvent.WaitOne();
            mExecutionDelegate = d;
            return d.BeginInvoke(host, dataLength, pingCount, timeout, waitTimeBetweenAttempts, callback, state);
        }

        /// <summary>
        /// Ends the execute.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        public void EndExecute(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (mExecutionDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndAccept called multiple times.", "asyncResult");
            }
            try
            {
                mExecutionDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                mExecutionDelegate = null;
                mAsyncActiveExecutionEvent.Set();
                CloseAsyncActiveExecutionEvent(Interlocked.Decrement(ref mAsyncActiveExecutionCount));
            }
        }

#endif

        /// <summary>
        /// Begins the execute.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginExecute(string host, ReturnCallback callback, object state)
        {
            return BeginExecute(host, DEFAULT_PACKET_SIZE, DEFAULT_PING_COUNTER, DEFAULT_PING_TIMEOUT, DEFAULT_WAITTIME_ATTEMPTS, callback, state);
        }

        /// <summary>
        /// Begins the execute.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="dataLength">Length of the data.</param>
        /// <param name="pingCount">The ping count.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="waitTimeBetweenAttempts">The wait time between attempts.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginExecute(string host, int dataLength, int pingCount, int timeout, int waitTimeBetweenAttempts, ReturnCallback callback, object state)
        {
            Interlocked.Increment(ref mAsyncActiveExecutionCount);
            System.Action<string, int, int, int, int> d = new System.Action<string, int, int, int, int>(Execute);
            if (mAsyncActiveExecutionEvent == null)
            {
                lock (LOCK_EXECUTE)
                {
                    if (mAsyncActiveExecutionEvent == null)
                    {
                        mAsyncActiveExecutionEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveExecutionEvent.WaitOne();
            mExecutionActionDelegate = d;
            return d.BeginInvoke(host, dataLength, pingCount, timeout, waitTimeBetweenAttempts, callback, state);
        }

        /// <summary>
        /// Ends the execute.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        public void EndExecute(ITaskResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (mExecutionActionDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndAccept called multiple times.", "asyncResult");
            }
            try
            {
                mExecutionActionDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                mExecutionActionDelegate = null;
                mAsyncActiveExecutionEvent.Set();
                CloseAsyncActiveExecutionEvent(Interlocked.Decrement(ref mAsyncActiveExecutionCount));
            }
        }

#if NETCOREAPP3_1_OR_GREATER

        /// <summary>
        /// Pings the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        public async Task ExecuteAsync(string host)
        {
            await Task.Run(() => Execute(host));
        }

        /// <summary>
        /// Pings the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="dataLength">Length of the data.</param>
        /// <param name="pingCount">The ping count.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="waitTimeBetweenAttempts">The wait time between attempts.</param>
        public async Task ExecuteAsync(string host, int dataLength, int pingCount, int timeout, int waitTimeBetweenAttempts)
        {
            await Task.Run(() => Execute(host, dataLength, pingCount, timeout, waitTimeBetweenAttempts));
        }

#endif

        /// <summary>
        /// Pings the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        public void Execute(string host)
        {
            Execute(host, DEFAULT_PACKET_SIZE, DEFAULT_PING_COUNTER, DEFAULT_PING_TIMEOUT, DEFAULT_WAITTIME_ATTEMPTS);
        }

        /// <summary>
        /// Pings the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="dataLength">Length of the data.</param>
        /// <param name="pingCount">The ping count.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="waitTimeBetweenAttempts">The wait time between attempts.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times"), MethodImpl(MethodImplOptions.Synchronized)]
        public void Execute(string host, int dataLength, int pingCount, int timeout, int waitTimeBetweenAttempts)
        {
            if (string.IsNullOrEmpty(host))
            {
                ThrowHelper.ThrowArgumentNullException("host");
            }
            if (dataLength < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("dataLength");
            }
            if (pingCount < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("pingCount");
            }
            if (timeout < 1)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("timeout");
            }

            IsFinished = false;
            IPHostEntry hostByName = null;
            int bytesReceived = 0;
            int tickCount = 0;
            int pingStop = 0;
            mPingLoop = 0;

            try
            {
                hostByName = Dns.GetHostEntry(host);
            }
            catch (Exception)
            {
                IsFinished = true;
                OnPingResult(PingResultEnum.HostNotFoundError);
                OnPingResult(PingResultEnum.Finished);
                return;
            }

            IPEndPoint remoteEP = new IPEndPoint(hostByName.AddressList[0], 0);
            mRemoteIpAddress = remoteEP.Address;
            EndPoint localPoint = new IPEndPoint(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0], 0);

            IcmpPacket packet = new IcmpPacket();
            packet.Type = ICMP_ECHO;
            packet.SubCode = 0;
            packet.CheckSum = Convert.ToUInt16(0);
            packet.Identifier = Convert.ToUInt16(45);
            packet.SequenceNumber = Convert.ToUInt16(0);
            packet.Data = new byte[dataLength];

            for (int i = 0; i <= dataLength - 1; i++)
            {
                packet.Data[i] = 35; // #
            }

            int packetSize = dataLength + 8;
            byte[] buffer = new byte[packetSize];
            int res = CreatePacket(packet, buffer, packetSize, dataLength);
            if (res == SOCKET_ERROR)
            {
                IsFinished = true;
                OnPingResult(PingResultEnum.BadPacketError);
                OnPingResult(PingResultEnum.Finished);
            }
            else
            {
                ushort[] checksumBuffer = new ushort[(Convert.ToInt32(Math.Ceiling((double)(Convert.ToDouble(res) / 2.0))) - 1) + 1];
                int index = 0;
                if (checksumBuffer.Length > 0)
                {
                    for (int i = 0; i <= checksumBuffer.Length - 1; i++)
                    {
                        checksumBuffer[i] = BitConverter.ToUInt16(buffer, index);
                        index += 2;
                    }
                }

                packet.CheckSum = CheckSum(checksumBuffer);

                byte[] dataBuffer = new byte[packetSize + 1];
                if (CreatePacket(packet, dataBuffer, packetSize, dataLength) == SOCKET_ERROR)
                {
                    IsFinished = true;
                    OnPingResult(PingResultEnum.BadPacketError);
                    OnPingResult(PingResultEnum.Finished);
                }
                else
                {
                    long totalTime = 0L;
                    mPacketSent = 0;
                    mPacketReceived = 0;
                    mMinimumPingTime = int.MaxValue;
                    mMaximumPingTime = int.MinValue;

                    do
                    {
                        bool receivedFlag = false;
                        using (Socket pingSocket = new Socket(mRemoteIpAddress.AddressFamily, SocketType.Raw, ProtocolType.Icmp))
                        {
                            pingSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeout);
                            pingSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, timeout);

                            byte[] receiveBuffer = new byte[MAX_PACKET_SIZE];
                            tickCount = Environment.TickCount;
                            mPacketSent++;

                            if (pingSocket.SendTo(dataBuffer, packetSize, SocketFlags.None, remoteEP) == SOCKET_ERROR)
                            {
                                OnPingResult(PingResultEnum.SocketError);
                            }
                            else
                            {
                                bytesReceived = 0;

                                while (!receivedFlag)
                                {
                                    try
                                    {
                                        bytesReceived = pingSocket.ReceiveFrom(receiveBuffer, MAX_PACKET_SIZE, SocketFlags.None, ref localPoint);
                                    }
                                    catch (Exception)
                                    {
                                        OnPingResult(PingResultEnum.RequestTimeout);
                                        receivedFlag = false;
                                        break;
                                    }
                                    if (bytesReceived == -1)
                                    {
                                        OnPingResult(PingResultEnum.NoResponse);
                                        receivedFlag = false;
                                        break;
                                    }
                                    if (bytesReceived > 0)
                                    {
                                        receivedFlag = true;
                                        pingStop = Environment.TickCount - tickCount;
                                        if (pingStop > timeout)
                                        {
                                            OnPingResult(PingResultEnum.RequestTimeout);
                                            receivedFlag = false;
                                            Thread.Sleep(DEFAULT_WAITTIME_ATTEMPTS);
                                        }
                                        else
                                        {
                                            OnPingResult(bytesReceived - 28, pingStop);
                                        }
                                        break;
                                    }
                                }
                                if (receivedFlag)
                                {
                                    mPacketReceived++;
                                    totalTime += pingStop;
                                    if (pingStop > mMaximumPingTime)
                                    {
                                        mMaximumPingTime = pingStop;
                                    }
                                    if (pingStop < mMinimumPingTime)
                                    {
                                        mMinimumPingTime = pingStop;
                                    }
                                }
                            }

                            mPingLoop++;

                            pingSocket.Shutdown(SocketShutdown.Both);
                            pingSocket.Close();

                            receiveBuffer = null;

                            if (receivedFlag & (mPingLoop < pingCount))
                            {
                                Thread.Sleep(DEFAULT_WAITTIME_ATTEMPTS);
                            }
                        }
                    }
                    while (mPingLoop < pingCount);

                    mPacketLost = mPacketSent - mPacketReceived;
                    if (mPacketReceived == 0)
                    {
                        mPacketLostPercent = 0.0;
                        mMinimumPingTime = 0;
                        mMaximumPingTime = 0;
                        mAveragePingTime = 0;
                    }
                    else
                    {
                        mPacketLostPercent = (((double)(mPacketSent - mPacketReceived)) / ((double)mPacketSent)) * 100.0;
                        mAveragePingTime = Convert.ToInt32((double)(((double)totalTime) / ((double)mPacketSent)));
                    }

                    buffer = null;
                    checksumBuffer = null;
                    dataBuffer = null;
                    packet.Data = null;
                    packet = null;

                    IsFinished = true;
                    OnPingResult(PingResultEnum.Finished);
                }
            }
        }

        /// <summary>
        /// Aborts the current operation(s).
        /// </summary>
        public void Abort()
        {
            mPingLoop = int.MaxValue - 1;
        }

        #endregion

        #region Private method(s)

        private ushort CheckSum(ushort[] buffer)
        {
            int ic = 0;

            for (int i = 0; i <= buffer.Length - 1; i++)
            {
                ic += Convert.ToInt32(buffer[i]);
            }

            ic = (ic >> 16) + (ic & 65535);
            ic += (ic >> 16);

            return Convert.ToUInt16(65535 - ic);
        }

        private int CreatePacket(IcmpPacket packet, byte[] buffer, int packetSize, int pingData)
        {
            int result = 0;

            int destinationIndex = 0;
            byte[] sourceArray = new byte[] { packet.Type };
            byte[] buffer3 = new byte[] { packet.SubCode };
            byte[] bytes = BitConverter.GetBytes(packet.CheckSum);
            byte[] buffer5 = BitConverter.GetBytes(packet.Identifier);
            byte[] buffer6 = BitConverter.GetBytes(packet.SequenceNumber);

            Array.Copy(sourceArray, 0, buffer, destinationIndex, sourceArray.Length);
            destinationIndex += sourceArray.Length;

            Array.Copy(buffer3, 0, buffer, destinationIndex, buffer3.Length);
            destinationIndex += buffer3.Length;

            Array.Copy(bytes, 0, buffer, destinationIndex, bytes.Length);
            destinationIndex += bytes.Length;

            Array.Copy(buffer5, 0, buffer, destinationIndex, buffer5.Length);
            destinationIndex += buffer5.Length;

            Array.Copy(buffer6, 0, buffer, destinationIndex, buffer6.Length);
            destinationIndex += buffer6.Length;

            Array.Copy(packet.Data, 0, buffer, destinationIndex, pingData);
            destinationIndex += pingData;

            if (destinationIndex != packetSize)
            {
                result = SOCKET_ERROR;
            }
            else
            {
                result = destinationIndex;
            }

            sourceArray = null;
            buffer3 = null;
            bytes = null;
            buffer5 = null;
            buffer6 = null;

            return result;
        }

        private void OnPingResult(PingResultEnum result)
        {
            Executor.Invoke(EventPingResult, this, new PingResultEventArgs(result));
        }

        private void OnPingResult(int receivedBytes, int responseTime)
        {
            Executor.Invoke(EventPingResult, this, new PingResultEventArgs(mRemoteIpAddress, receivedBytes, responseTime));
        }

        private void CloseAsyncActiveExecutionEvent(int asyncActiveCount)
        {
            if ((mAsyncActiveExecutionEvent != null) && (asyncActiveCount == 0))
            {
                mAsyncActiveExecutionEvent.Dispose();
                mAsyncActiveExecutionEvent = null;
            }
        }

        #endregion

        #region Nested type(s)

        private class IcmpPacket
        {

            internal ushort CheckSum;

            internal byte[] Data;

            internal ushort Identifier;

            internal ushort SequenceNumber;

            internal byte SubCode;

            internal byte Type;

        }

        #endregion

    }

}
