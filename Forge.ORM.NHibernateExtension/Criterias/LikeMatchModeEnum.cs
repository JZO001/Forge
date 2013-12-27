/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.ORM.NHibernateExtension.Criterias
{

    /// <summary>
    /// Matching mode for like criteria
    /// </summary>
    [Serializable]
    public enum LikeMatchModeEnum : int
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
