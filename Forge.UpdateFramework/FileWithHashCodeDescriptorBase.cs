/* *********************************************************************
 * Date: 16 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;

namespace Forge.UpdateFramework
{

    /// <summary>
    /// Represents a file descriptor which makes hash calculation from the file
    /// </summary>
    [Serializable]
    public abstract class FileWithHashCodeDescriptorBase : FileDescriptorBase
    {

        #region Constructor(s)
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FileWithHashCodeDescriptorBase"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="descriptorType">Type of the descriptor.</param>
        /// <param name="fileInfo">The fi.</param>
        protected FileWithHashCodeDescriptorBase(string id, DescriptorTypeEnum descriptorType, FileInfo fileInfo)
            : base(id, descriptorType, fileInfo)
        {
            try
            {
                using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (SHA256Managed sha = new SHA256Managed())
                    {
                        this.HashCode = sha.ComputeHash(fs);
                    }
                }
            }
            catch (SecurityException)
            {
                this.FileLoadResult = FileLoadResultEnum.SecurityError;
            }
            catch (FileNotFoundException)
            {
                this.FileLoadResult = FileLoadResultEnum.FileNotFound;
            }
            catch (Exception)
            {
                this.FileLoadResult = FileLoadResultEnum.UnspecifiedError;
            }
        } 

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        public byte[] HashCode { get; private set; }

        #endregion

    }

}
