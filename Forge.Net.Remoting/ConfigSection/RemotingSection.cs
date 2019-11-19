/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Configuration.Shared;

namespace Forge.Net.Remoting.ConfigSection
{

    /// <summary>
    /// Configuration section for remoting
    /// </summary>
    [Serializable]
    public class RemotingSection : ConfigurationSectionStandard
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RemotingSection"/> class.
        /// </summary>
        public RemotingSection() : base()
        {
        }

    }

}
