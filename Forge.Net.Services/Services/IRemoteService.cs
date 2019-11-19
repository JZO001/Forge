/* *********************************************************************
 * Date: 11 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Management;

namespace Forge.Net.Services.Services
{

    /// <summary>
    /// Base services for a remote service
    /// </summary>
    public interface IRemoteService : IManager
    {

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        string Id { get; }

        /// <summary>
        /// Gets or sets the priority of this service instance.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        long Priority { get; set; }

        /// <summary>
        /// Gets the service descriptor.
        /// </summary>
        /// <value>
        /// The service descriptor.
        /// </value>
        IServiceDescriptor ServiceDescriptor { get; }

        /// <summary>
        /// Starts this instance with the specified priority.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <returns>Manager State</returns>
        ManagerStateEnum Start(long priority);

        /// <summary>
        /// Starts the specified priority.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="serviceDescriptor">The service descriptor.</param>
        /// <returns>Manager State</returns>
        ManagerStateEnum Start(long priority, IServiceDescriptor serviceDescriptor);

    }

}
