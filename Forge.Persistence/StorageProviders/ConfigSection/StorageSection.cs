/* *********************************************************************
 * Date: 02 May 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Configuration.Shared;

namespace Forge.Persistence.StorageProviders.ConfigSection
{

    /// <summary>
    /// Configuration Section for storages
    /// </summary>
    [Serializable]
    public sealed class StorageSection : ConfigurationSectionStandard
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageSection"/> class.
        /// </summary>
        public StorageSection() : base()
        {
        }

    }

}
