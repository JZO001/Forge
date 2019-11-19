/* *********************************************************************
 * Date: 07 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Configuration.Shared;

namespace Forge.Net.TerraGraf.ConfigSection
{

    /// <summary>
    /// Configuration Section for TerraGraf
    /// </summary>
    [Serializable]
    public sealed class TerraGrafSection : ConfigurationSectionStandard
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TerraGrafSection"/> class.
        /// </summary>
        public TerraGrafSection() : base()
        {
        }

    }

}
