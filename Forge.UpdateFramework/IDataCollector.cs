/* *********************************************************************
 * Date: 15 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Collections.Generic;
using Forge.Configuration;

namespace Forge.UpdateFramework
{

    /// <summary>
    /// Represents a data collector for the client side updater
    /// </summary>
    public interface IDataCollector : IInitializable
    {

        /// <summary>
        /// Collects the specified update data.
        /// </summary>
        /// <param name="updateData">The update data.</param>
        void Collect(Dictionary<string, DescriptorBase> updateData);

    }

}
