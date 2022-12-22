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
    /// Simple file descriptor
    /// </summary>
    [Serializable]
    public class SimpleFileDescriptor : FileDescriptorBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleFileDescriptor"/> class.
        /// </summary>
        /// <param name="fileInfo">The fi.</param>
        public SimpleFileDescriptor(FileInfo fileInfo)
            : base(Guid.NewGuid().ToString(), DescriptorTypeEnum.Custom, fileInfo)
        {
        }

    }

}
