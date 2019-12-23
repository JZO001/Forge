/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Logging;
using System;
using System.IO;

namespace Forge.Net.Remoting.Channels
{

    /// <summary>
    /// Self remover file stream
    /// </summary>
    internal class ReadOnlySelfRemoverFileStream : FileStream
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(ReadOnlySelfRemoverFileStream));

        private string mFileName = string.Empty;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySelfRemoverFileStream"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileMode">The file mode.</param>
        internal ReadOnlySelfRemoverFileStream(string fileName, FileMode fileMode)
            : base(fileName, fileMode, fileMode == FileMode.Open ? FileAccess.Read : FileAccess.Write, FileShare.Read)
        {
            this.mFileName = fileName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySelfRemoverFileStream"/> class.
        /// </summary>
        /// <param name="fileInfo">The file info.</param>
        /// <param name="fileMode">The file mode.</param>
        internal ReadOnlySelfRemoverFileStream(FileInfo fileInfo, FileMode fileMode)
            : base(fileInfo.FullName, fileMode, fileMode == FileMode.Open ? FileAccess.Read : FileAccess.Write, FileShare.Read)
        {
            this.mFileName = fileInfo.FullName;
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.IO.FileStream"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                try
                {
                    new FileInfo(mFileName).Delete();
                }
                catch (Exception ex)
                {
                    if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("Failed to remove temporary file: {0}", mFileName), ex);
                }
            }
        }

        #endregion

    }

}
