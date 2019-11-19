/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Configuration.Shared;

namespace Forge.Net.Services.ConfigSection
{

    /// <summary>
    /// Configuration section for Network Services
    /// </summary>
    [Serializable]
    public class NetworkServiceSection : ConfigurationSectionStandard
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkServiceSection"/> class.
        /// </summary>
        public NetworkServiceSection() : base()
        {
        }

    }

}
