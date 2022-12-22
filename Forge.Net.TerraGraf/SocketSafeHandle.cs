/* *********************************************************************
 * Date: 05 Jun 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Legacy;
using Forge.Shared;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Forge.Net.TerraGraf
{

    /// <summary>
    /// Implementation of a safe socket handler
    /// </summary>
    internal sealed class SocketSafeHandle : MBRBase, ISocketSafeHandle
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Socket mSocket = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketSafeHandle"/> class.
        /// </summary>
        /// <param name="socket">The socket.</param>
        internal SocketSafeHandle(Socket socket)
        {
            if (socket == null)
            {
                ThrowHelper.ThrowArgumentNullException("socket");
            }
            mSocket = socket;
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Detect the status of the socket
        /// </summary>
        /// <param name="microSeconds">The micro seconds.</param>
        /// <param name="selectMode">The select mode.</param>
        /// <returns>
        /// True, if the conditions match with select mode
        /// </returns>
        public bool Pool(int microSeconds, SelectMode selectMode)
        {
            return mSocket.Pool(microSeconds, selectMode);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            mSocket.Close();
        }

        /// <summary>
        /// Close the socket and wait for the specified time.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public void Close(int timeout)
        {
            mSocket.Close(timeout);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the address family.
        /// </summary>
        /// <value>
        /// The address family.
        /// </value>
        public AddressFamily AddressFamily
        {
            get { return mSocket.AddressFamily; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ISocketSafeHandle" /> is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        public bool Connected
        {
            get { return mSocket.Connected; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable broadcast].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable broadcast]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableBroadcast
        {
            get { return mSocket.EnableBroadcast; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is bound.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is bound; otherwise, <c>false</c>.
        /// </value>
        public bool IsBound
        {
            get { return mSocket.IsBound; }
        }

        /// <summary>
        /// Gets the local end point.
        /// </summary>
        /// <value>
        /// The local end point.
        /// </value>
        public EndPoint LocalEndPoint
        {
            get { return mSocket.LocalEndPoint; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [no delay].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [no delay]; otherwise, <c>false</c>.
        /// </value>
        public bool NoDelay
        {
            get { return mSocket.NoDelay; }
        }

        /// <summary>
        /// Gets or sets the size of the receive buffer.
        /// </summary>
        /// <value>
        /// The size of the receive buffer.
        /// </value>
        public int ReceiveBufferSize
        {
            get { return mSocket.ReceiveBufferSize; }
        }

        /// <summary>
        /// Gets or sets the receive timeout.
        /// </summary>
        /// <value>
        /// The receive timeout.
        /// </value>
        public int ReceiveTimeout
        {
            get { return mSocket.ReceiveTimeout; }
        }

        /// <summary>
        /// Gets or sets the remote end point.
        /// </summary>
        /// <value>
        /// The remote end point.
        /// </value>
        public EndPoint RemoteEndPoint
        {
            get { return mSocket.RemoteEndPoint; }
        }

        /// <summary>
        /// Gets or sets the size of the send buffer.
        /// </summary>
        /// <value>
        /// The size of the send buffer.
        /// </value>
        public int SendBufferSize
        {
            get { return mSocket.SendBufferSize; }
        }

        /// <summary>
        /// Gets or sets the send timeout.
        /// </summary>
        /// <value>
        /// The send timeout.
        /// </value>
        public int SendTimeout
        {
            get { return mSocket.SendTimeout; }
        }

        /// <summary>
        /// Gets the type of the protocol.
        /// </summary>
        /// <value>
        /// The type of the protocol.
        /// </value>
        public ProtocolType ProtocolType
        {
            get { return mSocket.ProtocolType; }
        }

        /// <summary>
        /// Gets the type of the socket.
        /// </summary>
        /// <value>
        /// The type of the socket.
        /// </value>
        public SocketType SocketType
        {
            get { return mSocket.SocketType; }
        }

        /// <summary>
        /// Gets or sets the TTL.
        /// </summary>
        /// <value>
        /// The TTL.
        /// </value>
        public short Ttl
        {
            get { return mSocket.Ttl; }
        }

        #endregion

    }

}
