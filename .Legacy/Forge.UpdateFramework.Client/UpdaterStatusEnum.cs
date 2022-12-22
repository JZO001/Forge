/* *********************************************************************
 * Date: 15 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.UpdateFramework.Client
{

    /// <summary>
    /// States of the client updater
    /// </summary>
    [Serializable]
    public enum UpdaterStatusEnum
    {
        /// <summary>
        /// Update not found or the updater has no task
        /// </summary>
        UpdateNotFound = 0,

        /// <summary>
        /// Collecting update data and communicating with the update server
        /// </summary>
        CheckingUpdates,

        /// <summary>
        /// Waiting for user permission to download updates from the update server
        /// </summary>
        ReadyToDownloadUpdates,

        /// <summary>
        /// Downloading updates from the update server
        /// </summary>
        DownloadingUpdates,

        /// <summary>
        /// Waiting for user permission to install downloaded updates
        /// </summary>
        ReadyToInstallUpdates,

        /// <summary>
        /// Executing update procedure
        /// </summary>
        UpdateInProgess

    }

}
