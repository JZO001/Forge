/* *********************************************************************
 * Date: 16 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.IO;
using Forge.Configuration.Check;

namespace Forge.UpdateFramework
{

    /// <summary>
    /// Validates an application configuration file
    /// </summary>
    [Serializable]
    public class AppConfigDescriptor : FileDescriptorBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigDescriptor"/> class.
        /// </summary>
        /// <param name="fileInfo">The file info.</param>
        public AppConfigDescriptor(FileInfo fileInfo)
            : base(Guid.NewGuid().ToString(), DescriptorTypeEnum.Configuration, fileInfo)
        {
            this.IsValid = ConfigurationValidator.ValidateConfiguration(fileInfo.FullName);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid { get; private set; }

    }

}
