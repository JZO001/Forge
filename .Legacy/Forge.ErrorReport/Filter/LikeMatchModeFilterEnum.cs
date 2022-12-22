/* *********************************************************************
 * Date: 24 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.ErrorReport.Filter
{

    /// <summary>
    /// Matching mode for like filter
    /// </summary>
    [Serializable]
    public enum LikeMatchModeFilterEnum
    {
        /// <summary>
        /// The provided string is exactly match with cell value
        /// </summary>
        Exact = 0,
        /// <summary>
        /// The cell value starts with the provided string
        /// </summary>
        Start,
        /// <summary>
        /// The cell value ends with the provided string
        /// </summary>
        End,
        /// <summary>
        /// The cell value contains the provided string
        /// </summary>
        Anywhere
    }

}
