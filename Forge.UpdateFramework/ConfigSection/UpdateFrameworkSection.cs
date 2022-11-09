/* *********************************************************************
 * Date: 09 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Configuration.Shared;

namespace Forge.UpdateFramework.ConfigSection
{

    /// <summary>
    /// Configuration Section for Update Framework
    /// </summary>
    [Serializable]
    public class UpdateFrameworkSection : ConfigurationSectionStandard
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateFrameworkSection"/> class.
        /// </summary>
        public UpdateFrameworkSection()
            : base()
        {
        }

    }

}
