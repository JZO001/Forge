/* *********************************************************************
 * Date: 07 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Threading;
using Forge.Net.Synapse.NetworkServices;

namespace Forge.Net.Synapse
{

    /// <summary>
    /// Network stream
    /// </summary>
    public class NetworkStream : Stream
    {

        #region Field(s)

        //private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(NetworkStream));

        private static long mGlobalIdentifierCounter = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly long mId = 0;
        private SslStream mSslStream = null;
        private Stream mInnerStream = null;
        private ISocket mSocket = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkStream"/> class.
        /// </summary>
        /// <param name="socket">The socket.</param>
        public NetworkStream(ISocket socket)
        {
            if (socket == null)
            {
                ThrowHelper.ThrowArgumentNullException("socket");
            }
            this.mSocket = socket;
            this.mId = Interlocked.Increment(ref mGlobalIdentifierCounter);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkStream"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="socket">The socket.</param>
        public NetworkStream(System.Net.Sockets.NetworkStream stream, ISocket socket)
            : this(socket)
        {
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }
            this.mInnerStream = stream;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkStream"/> class.
        /// </summary>
        /// <param name="sslStream">The SSL stream.</param>
        /// <param name="socket">The socket.</param>
        public NetworkStream(SslStream sslStream, ISocket socket)
            : this(socket)
        {
            if (sslStream == null)
            {
                ThrowHelper.ThrowArgumentNullException("sslStream");
            }
            this.mInnerStream = sslStream;
            this.mSslStream = sslStream;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="NetworkStream"/> is reclaimed by garbage collection.
        /// </summary>
        ~NetworkStream()
        {
            Dispose(false);
        }

        #endregion

        #region Public member(s)

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public long Id
        {
            get { return mId; }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <returns>true if the stream supports reading; otherwise, false.</returns>
        public override bool CanRead
        {
            get { return this.mInnerStream == null ? true : this.mInnerStream.CanRead; }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <returns>true if the stream supports seeking; otherwise, false.</returns>
        public override bool CanSeek
        {
            get { return this.mInnerStream == null ? false : this.mInnerStream.CanSeek; }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <returns>true if the stream supports writing; otherwise, false.</returns>
        public override bool CanWrite
        {
            get { return this.mInnerStream == null ? true : this.mInnerStream.CanWrite; }
        }

        /// <summary>
        /// When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void Flush()
        {
            if (this.mInnerStream != null)
            {
                this.mInnerStream.Flush();
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <returns>A long value representing the length of the stream in bytes.</returns>
        /// <exception cref="T:System.NotSupportedException">A class derived from Stream does not support seeking.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override long Length
        {
            get { return this.mInnerStream != null ? this.mInnerStream.Length : this.mSocket.Available; }
        }

        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <returns>The current position within the stream.</returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support seeking.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override long Position
        {
            get { return this.mInnerStream != null ? this.mInnerStream.Position : 0; }
            set
            {
                if (this.mInnerStream != null)
                {
                    this.mInnerStream.Position = value;
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is larger than the buffer length.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="buffer" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support reading.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            if ((offset < 0) || (offset > buffer.Length))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("offset");
            }
            if ((count < 0) || (count > (buffer.Length - offset)))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("count");
            }
            if (count == 0)
            {
                return 0;
            }

            int result = 0;
            if (this.mInnerStream == null)
            {
                result = this.mSocket.Receive(buffer, offset, count);
                //if (!(this.mSocket is Forge.Net.Synapse.NetworkFactory.SocketWrapper))
                //{
                //    LOGGER.Debug(string.Format("NETWORK_STREAM, {0} byte(s) read from socket {1}.", count.ToString(), mSocket.GetHashCode().ToString()));
                //}
            }
            else
            {
                result = this.mInnerStream.Read(buffer, offset, count);
            }
            return result;
        }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to obtain the new position.</param>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support seeking, such as if the stream is constructed from a pipe or console output.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (this.mInnerStream == null)
            {
                return 0;
            }
            return this.mInnerStream.Seek(offset, origin);
        }

        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override void SetLength(long value)
        {
            if (this.mInnerStream != null)
            {
                this.mInnerStream.SetLength(value);
            }
        }

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count" /> bytes from <paramref name="buffer" /> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is greater than the buffer length.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="buffer" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support writing.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            if (count == 0)
            {
                return;
            }
            if ((offset < 0) || (offset > buffer.Length))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("offset");
            }
            if ((count < 0) || (count > (buffer.Length - offset)))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("size");
            }

            if (this.mInnerStream == null)
            {
                this.mSocket.Send(buffer, offset, count);
            }
            else
            {
                this.mInnerStream.Write(buffer, offset, count);
            }
        }

        /// <summary>
        /// Gets or sets the size of the send buffer of the underlying socket.
        /// </summary>
        /// <value>
        /// The size of the send buffer.
        /// </value>
        public int SendBufferSize
        {
            get { return this.mSocket.SendBufferSize; }
            set { this.mSocket.SendBufferSize = value; }
        }

        /// <summary>
        /// Gets or sets the size of the receive buffer of the underlying socket.
        /// </summary>
        /// <value>
        /// The size of the receive buffer.
        /// </value>
        public int ReceiveBufferSize
        {
            get { return this.mSocket.ReceiveBufferSize; }
            set { this.mSocket.ReceiveBufferSize = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [no delay].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [no delay]; otherwise, <c>false</c>.
        /// </value>
        public bool NoDelay
        {
            get { return this.mSocket.NoDelay; }
            set { this.mSocket.NoDelay = value; }
        }

#if IS_WINDOWS

        /// <summary>
        /// Sets the keep alive values.
        /// </summary>
        /// <param name="state">if set to <c>true</c> [state].</param>
        /// <param name="keepAliveTime">The keep alive time.</param>
        /// <param name="keepAliveInterval">The keep alive interval.</param>
        /// <returns></returns>
        public int SetKeepAliveValues(bool state, int keepAliveTime, int keepAliveInterval)
        {
            return this.mSocket.SetKeepAliveValues(state, keepAliveInterval, keepAliveInterval);
        }

#endif

        /// <summary>
        /// Gets a value indicating whether this <see cref="NetworkStream"/> is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        public bool Connected
        {
            get { return this.mSocket.Connected; }
        }

        /// <summary>
        /// Gets or sets the receive timeout.
        /// </summary>
        /// <value>
        /// The receive timeout.
        /// </value>
        public int ReceiveTimeout
        {
            get { return this.mSocket.ReceiveTimeout; }
            set { this.mSocket.ReceiveTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the send timeout.
        /// </summary>
        /// <value>
        /// The send timeout.
        /// </value>
        public int SendTimeout
        {
            get { return this.mSocket.SendTimeout; }
            set { this.mSocket.SendTimeout = value; }
        }

        /// <summary>
        /// Gets the local end point.
        /// </summary>
        /// <value>
        /// The local end point.
        /// </value>
        public AddressEndPoint LocalEndPoint
        {
            get { return this.mSocket.LocalEndPoint; }
        }

        /// <summary>
        /// Gets the remote end point.
        /// </summary>
        /// <value>
        /// The remote end point.
        /// </value>
        public AddressEndPoint RemoteEndPoint
        {
            get { return this.mSocket.RemoteEndPoint; }
        }

        /// <summary>
        /// Gets or sets the TimeToLive value of the underlying socket.
        /// </summary>
        /// <value>
        /// The TTL.
        /// </value>
        public short Ttl
        {
            get { return this.mSocket.Ttl; }
            set { this.mSocket.Ttl = value; }
        }

        /// <summary>
        /// Gets the cipher algorithm.
        /// </summary>
        /// <value>
        /// The cipher algorithm.
        /// </value>
        public CipherAlgorithmType CipherAlgorithm
        {
            get
            {
                if (this.mSslStream == null)
                {
                    return CipherAlgorithmType.None;
                }
                return this.mSslStream.CipherAlgorithm;
            }
        }

        /// <summary>
        /// Gets the cipher strength.
        /// </summary>
        /// <value>
        /// The cipher strength.
        /// </value>
        public int CipherStrength
        {
            get
            {
                if (this.mSslStream == null)
                {
                    return 0;
                }
                return this.mSslStream.CipherStrength;
            }
        }

        /// <summary>
        /// Gets the hash algorithm.
        /// </summary>
        /// <value>
        /// The hash algorithm.
        /// </value>
        public HashAlgorithmType HashAlgorithm
        {
            get
            {
                if (this.mSslStream == null)
                {
                    return HashAlgorithmType.None;
                }
                return this.mSslStream.HashAlgorithm;
            }
        }

        /// <summary>
        /// Gets the hash strength.
        /// </summary>
        /// <value>
        /// The hash strength.
        /// </value>
        public int HashStrength
        {
            get
            {
                if (this.mSslStream == null)
                {
                    return 0;
                }
                return this.mSslStream.HashStrength;
            }
        }

        /// <summary>
        /// Gets the SSL protocol.
        /// </summary>
        /// <value>
        /// The SSL protocol.
        /// </value>
        public SslProtocols SslProtocol
        {
            get
            {
                if (this.mSslStream == null)
                {
                    return SslProtocols.None;
                }
                return this.mSslStream.SslProtocol;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is authenticated.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is authenticated; otherwise, <c>false</c>.
        /// </value>
        public bool IsAuthenticated
        {
            get
            {
                if (this.mSslStream == null)
                {
                    return false;
                }
                return this.mSslStream.IsAuthenticated;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is encrypted.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is encrypted; otherwise, <c>false</c>.
        /// </value>
        public bool IsEncrypted
        {
            get
            {
                if (this.mSslStream == null)
                {
                    return false;
                }
                return this.mSslStream.IsEncrypted;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is mutually authenticated.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is mutually authenticated; otherwise, <c>false</c>.
        /// </value>
        public bool IsMutuallyAuthenticated
        {
            get
            {
                if (this.mSslStream == null)
                {
                    return false;
                }
                return this.mSslStream.IsMutuallyAuthenticated;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is signed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is signed; otherwise, <c>false</c>.
        /// </value>
        public bool IsSigned
        {
            get
            {
                if (this.mSslStream == null)
                {
                    return false;
                }
                return this.mSslStream.IsSigned;
            }
        }

        /// <summary>
        /// Gets the local certificate.
        /// </summary>
        /// <value>
        /// The local certificate.
        /// </value>
        public System.Security.Cryptography.X509Certificates.X509Certificate LocalCertificate
        {
            get
            {
                if (this.mSslStream == null)
                {
                    return null;
                }
                return this.mSslStream.LocalCertificate;
            }
        }

        /// <summary>
        /// Gets the remote certificate.
        /// </summary>
        /// <value>
        /// The remote certificate.
        /// </value>
        public System.Security.Cryptography.X509Certificates.X509Certificate RemoteCertificate
        {
            get
            {
                if (this.mSslStream == null)
                {
                    return null;
                }
                return this.mSslStream.RemoteCertificate;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (mSocket == null)
            {
                return base.GetHashCode();
            }
            return mSocket.GetHashCode();
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.IO.Stream"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (this.mInnerStream != null)
                {
                    this.mInnerStream.Dispose();
                }
                else
                {
                    this.mSocket.Dispose();
                }
            }
        }

        #endregion

    }

}
