/* *********************************************************************
 * Date: 16 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.IO;

namespace Forge.UpdateFramework
{

    /// <summary>
    /// Simple file descriptor which creates hash code from file data
    /// </summary>
    [Serializable]
    public class SimpleFileHashDescriptor : FileWithHashCodeDescriptorBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleFileHashDescriptor"/> class.
        /// </summary>
        /// <param name="fileInfo">The fi.</param>
        public SimpleFileHashDescriptor(FileInfo fileInfo)
            : base(Guid.NewGuid().ToString(), DescriptorTypeEnum.Custom, fileInfo)
        {
        }

    }

}
