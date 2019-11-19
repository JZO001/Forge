/* *********************************************************************
 * Date: 12 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Net.Remoting.Channels;

namespace Forge.Net.Remoting.Service
{

    /// <summary>
    /// Represent a service factory to administrate contract, channel and an implementation type
    /// </summary>
    public interface IServiceFactory : IDisposable
    {

        /// <summary>
        /// Gets the service contract.
        /// </summary>
        /// <value>
        /// The service contract.
        /// </value>
        Type ServiceContract { get; }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        Channel Channel { get; }

        /// <summary>
        /// Gets the type of the implementation.
        /// </summary>
        /// <value>
        /// The type of the implementation.
        /// </value>
        Type ImplementationType { get; }

        /// <summary>
        /// Opens this instance.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes this instance.
        /// </summary>
        void Close();

    }

}
