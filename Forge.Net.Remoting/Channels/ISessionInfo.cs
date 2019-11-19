/* *********************************************************************
 * Date: 12 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Net.Synapse;

namespace Forge.Net.Remoting.Channels
{

    /// <summary>
    /// Represents the network session information
    /// </summary>
    public interface ISessionInfo
    {

        /// <summary>
        /// Gets the session id.
        /// </summary>
        string SessionId { get; }

        /// <summary>
        /// Gets the remote end point.
        /// </summary>
        AddressEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// Gets the local end point.
        /// </summary>
        AddressEndPoint LocalEndPoint { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ISessionInfo"/> is reconnectable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if reconnectable; otherwise, <c>false</c>.
        /// </value>
        bool Reconnectable { get; }

    }

}
