/* *********************************************************************
 * Date: 08 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Shared;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Forge.Net.Synapse
{

    /// <summary>
    /// Represent a host and a port
    /// </summary>
    [Serializable]
    public class AddressEndPoint : EndPoint, IEquatable<AddressEndPoint>
    {

        #region Field(s)

        /// <summary>
        /// Represents that the id of a network peer does not matter
        /// </summary>
        public static readonly string Any = IPAddress.Any.ToString();

        /// <summary>
        /// Represents that the id of a network peer does not matter
        /// </summary>
        public static readonly string IPv6Any = IPAddress.IPv6Any.ToString();

        /// <summary>
        /// Represents the broadcast address
        /// </summary>
        public static readonly string Broadcast = IPAddress.Broadcast.ToString();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private AddressFamily mFamily = AddressFamily.Unspecified;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mHost = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mPort = 0;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressEndPoint"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public AddressEndPoint(string host, int port)
            : this(host, port, AddressFamily.Unspecified)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressEndPoint"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="family">The family.</param>
        public AddressEndPoint(string host, int port, AddressFamily family)
        {
            if (string.IsNullOrEmpty(host))
            {
                ThrowHelper.ThrowArgumentNullException("host");
            }
            if (!ValidateTcpPort(port))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("port");
            }

            mHost = host;
            mPort = port;
            mFamily = family;
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Parses the specified host and port.
        /// </summary>
        /// <param name="hostAndPort">The host and port.</param>
        /// <returns>AddressEndPoint</returns>
        public static AddressEndPoint Parse(string hostAndPort)
        {
            if (string.IsNullOrEmpty(hostAndPort))
            {
                ThrowHelper.ThrowArgumentNullException("hostAndPort");
            }

            AddressEndPoint result = null;

            if (hostAndPort.IndexOf(":") > -1)
            {
                string[] data = hostAndPort.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length > 2)
                {
                    throw new FormatException();
                }
                result = new AddressEndPoint(data[0], int.Parse(data[1]));
            }
            else
            {
                result = new AddressEndPoint(hostAndPort, 0);
            }

            return result;
        }

        /// <summary>
        /// Parses the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>AddressEndPoint</returns>
        public static AddressEndPoint Parse(IPEndPoint endPoint)
        {
            if (endPoint == null)
            {
                ThrowHelper.ThrowArgumentNullException("endPoint");
            }
            return Parse(endPoint.ToString());
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

            AddressEndPoint other = (AddressEndPoint)obj;
            return other.mFamily == mFamily && other.mHost.Equals(mHost, StringComparison.InvariantCultureIgnoreCase) && other.mPort == mPort;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>True, if the other instance is equals with this.</returns>
        public bool Equals(AddressEndPoint other)
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

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Host: {0}, port: {1}", Host, Port);
        }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Validates the TCP port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns></returns>
        public static bool ValidateTcpPort(int port)
        {
            return ((port >= 0) && (port <= 0xffff));
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the address family to which the endpoint belongs.
        /// </summary>
        /// <returns>One of the <see cref="T:System.Net.Sockets.AddressFamily" /> values.</returns>
        ///   <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        ///   </PermissionSet>
        /// <exception cref="T:System.NotImplementedException">Any attempt is made to get or set the property when the property is not overridden in a descendant class.</exception>
        [DebuggerHidden]
        public override AddressFamily AddressFamily
        {
            get { return mFamily; }
        }

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        [DebuggerHidden]
        public string Host
        {
            get { return mHost; }
        }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        [DebuggerHidden]
        public int Port
        {
            get { return mPort; }
        }

        #endregion

    }

}
