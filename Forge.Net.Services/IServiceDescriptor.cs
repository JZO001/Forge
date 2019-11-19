/* *********************************************************************
 * Date: 20 Dec 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Collections.Generic;
using Forge.Configuration;

namespace Forge.Net.Services
{

    /// <summary>
    /// Represents a service descriptor factory
    /// </summary>
    public interface IServiceDescriptor
    {

        /// <summary>
        /// Creates the service property.
        /// </summary>
        /// <returns>Properties of the service</returns>
        Dictionary<string, PropertyItem> CreateServiceProperty();

    }

}
