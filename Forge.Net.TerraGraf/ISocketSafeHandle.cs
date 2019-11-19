/* *********************************************************************
 * Date: 05 Jun 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Net;
using System.Net.Sockets;

namespace Forge.Net.TerraGraf
{

    /// <summary>
    /// Represents a safe socket handler which provides information about a socket.
    /// </summary>
    public interface ISocketSafeHandle
    {

        #region Public method(s)

        /// <summary>
        /// Detect the status of the socket
        /// </summary>
        /// <param name="microSeconds">The micro seconds.</param>
        /// <param name="selectMode">The select mode.</param>
        /// <returns>True, if the conditions match with select mode</returns>
        bool Pool(int microSeconds, SelectMode selectMode);

        /// <summary>
        /// Closes this instance.
        /// </summary>
        void Close();

        /// <summary>
        /// Close the socket and wait for the specified time.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        void Close(int timeout);

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the address family.
        /// </summary>
        /// <value>
        /// The address family.
        /// </value>
        AddressFamily AddressFamily { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ISocketSafeHandle"/> is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        bool Connected { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable broadcast].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable broadcast]; otherwise, <c>false</c>.
        /// </value>
        bool EnableBroadcast { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is bound.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is bound; otherwise, <c>false</c>.
        /// </value>
        bool IsBound { get; }

        /// <summary>
        /// Gets the local end point.
        /// </summary>
        /// <value>
        /// The local end point.
        /// </value>
        EndPoint LocalEndPoint { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [no delay].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [no delay]; otherwise, <c>false</c>.
        /// </value>
        bool NoDelay { get; }

        /// <summary>
        /// Gets or sets the size of the receive buffer.
        /// </summary>
        /// <value>
        /// The size of the receive buffer.
        /// </value>
        int ReceiveBufferSize { get; }

        /// <summary>
        /// Gets or sets the receive timeout.
        /// </summary>
        /// <value>
        /// The receive timeout.
        /// </value>
        int ReceiveTimeout { get; }

        /// <summary>
        /// Gets or sets the remote end point.
        /// </summary>
        /// <value>
        /// The remote end point.
        /// </value>
        EndPoint RemoteEndPoint { get; }

        /// <summary>
        /// Gets or sets the size of the send buffer.
        /// </summary>
        /// <value>
        /// The size of the send buffer.
        /// </value>
        int SendBufferSize { get; }

        /// <summary>
        /// Gets or sets the send timeout.
        /// </summary>
        /// <value>
        /// The send timeout.
        /// </value>
        int SendTimeout { get; }

        /// <summary>
        /// Gets the type of the protocol.
        /// </summary>
        /// <value>
        /// The type of the protocol.
        /// </value>
        ProtocolType ProtocolType { get; }

        /// <summary>
        /// Gets the type of the socket.
        /// </summary>
        /// <value>
        /// The type of the socket.
        /// </value>
        SocketType SocketType { get; }

        /// <summary>
        /// Gets or sets the TTL.
        /// </summary>
        /// <value>
        /// The TTL.
        /// </value>
        short Ttl { get; }

        #endregion

    }

}
