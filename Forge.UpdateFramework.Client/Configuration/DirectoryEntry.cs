/* *********************************************************************
 * Date: 15 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.IO;
using Forge.Configuration;
using Forge.Configuration.Shared;
using Forge.Shared;

namespace Forge.UpdateFramework.Client.Configuration
{

    /// <summary>
    /// Represents a directory entry
    /// </summary>
    [Serializable]
    public sealed class DirectoryEntry
    {

        #region Field(s)

        private static readonly string EXCLUDE_SUBFOLDERS = "IncludeSubFolders";

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryEntry"/> class.
        /// </summary>
        /// <param name="absolutePath">The absolute path.</param>
        /// <param name="excludeSubFolders">if set to <c>true</c> [exclude sub folders].</param>
        public DirectoryEntry(string absolutePath, bool excludeSubFolders)
        {
            if (!PathHelper.IsAbsolutePath(absolutePath))
            {
                ThrowHelper.ThrowArgumentException(string.Format("Provided path is not absolute: '{0}'.", absolutePath), "absolutePath");
            }

            this.FolderName = PathHelper.CutoffBackslashFromPathEnd(absolutePath);
            this.IncludeSubFolders = excludeSubFolders;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryEntry"/> class.
        /// </summary>
        /// <param name="codeBaseFolder">The code base folder.</param>
        /// <param name="item">The item.</param>
        public DirectoryEntry(string codeBaseFolder, IPropertyItem item)
        {
            if (string.IsNullOrEmpty(codeBaseFolder))
            {
                ThrowHelper.ThrowArgumentNullException("codeBaseFolder");
            }
            if (item == null)
            {
                ThrowHelper.ThrowArgumentNullException("item");
            }
            if (!PathHelper.IsAbsolutePath(codeBaseFolder))
            {
                ThrowHelper.ThrowArgumentException(string.Format("Provided path is not absolute: '{0}'.", codeBaseFolder), "codeBaseFolder");
            }

            if (PathHelper.IsAbsolutePath(item.Id))
            {
                this.FolderName = PathHelper.CutoffBackslashFromPathEnd(item.Id.ToLower().Trim());
            }
            else
            {
                this.FolderName = Path.Combine(codeBaseFolder, item.Id).ToLower().Trim();
            }

            bool subFolder = true;
            ConfigurationAccessHelper.ParseBooleanValue(item, EXCLUDE_SUBFOLDERS, ref subFolder);
            this.IncludeSubFolders = subFolder;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the name of the folder.
        /// </summary>
        /// <value>
        /// The name of the folder.
        /// </value>
        public string FolderName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [exclude sub folders].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [exclude sub folders]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeSubFolders { get; private set; }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return FolderName.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!obj.GetType().Equals(GetType())) return false;

            DirectoryEntry other = (DirectoryEntry)obj;
            return other.IncludeSubFolders.Equals(this.IncludeSubFolders) && other.FolderName.Equals(this.FolderName);
        }

        #endregion

    }

}
