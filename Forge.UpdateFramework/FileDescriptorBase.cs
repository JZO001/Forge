/* *********************************************************************
 * Date: 15 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Shared;
using System;
using System.IO;

namespace Forge.UpdateFramework
{

    /// <summary>
    /// Represents a descriptor which data comes from a file
    /// </summary>
    [Serializable]
    public abstract class FileDescriptorBase : DescriptorBase
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDescriptorBase"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="descriptorType">Type of the descriptor.</param>
        /// <param name="fileInfo">The fi.</param>
        protected FileDescriptorBase(string id, DescriptorTypeEnum descriptorType, FileInfo fileInfo)
            : base(id, descriptorType)
        {
            if (fileInfo == null)
            {
                ThrowHelper.ThrowArgumentNullException("fi");
            }

            if (PathHelper.CutoffBackslashFromPathEnd(fileInfo.FullName.ToLower()).StartsWith(PathHelper.CutoffBackslashFromPathEnd(new DirectoryInfo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase).FullName.ToLower())))
            {
                int len = PathHelper.CutoffBackslashFromPathEnd(new DirectoryInfo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase).FullName).Length;
                this.Location = fileInfo.DirectoryName.Substring(len, fileInfo.DirectoryName.Length - len);
            }
            else
            {
                this.Location = fileInfo.DirectoryName;
            }
            this.FileName = fileInfo.Name;
            this.FileLoadResult = FileLoadResultEnum.Success;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the location.
        /// </summary>
        public string Location { get; private set; }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets or sets the file load result.
        /// </summary>
        /// <value>
        /// The file load result.
        /// </value>
        public FileLoadResultEnum FileLoadResult { get; protected set; }

        #endregion

    }

}
