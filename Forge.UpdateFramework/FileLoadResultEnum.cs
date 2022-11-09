/* *********************************************************************
 * Date: 16 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.UpdateFramework
{

    /// <summary>
    /// File load result
    /// </summary>
    [Serializable]
    public enum FileLoadResultEnum
    {
        /// <summary>
        /// File successfully loaded
        /// </summary>
        Success = 0,

        /// <summary>
        /// Unable to access file for security reasons
        /// </summary>
        SecurityError,

        /// <summary>
        /// File not found
        /// </summary>
        FileNotFound,

        /// <summary>
        /// File format is not corrent
        /// </summary>
        BadFormat,

        /// <summary>
        /// Occurs when an unspecified error occured
        /// </summary>
        UnspecifiedError

    }

}
