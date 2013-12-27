/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.ORM.NHibernateExtension.Criterias
{

    /// <summary>
    /// Sort mode for OrderBy
    /// </summary>
    [Serializable]
    public enum OrderModeEnum : int
    {
        /// <summary>
        /// Sort by ascending
        /// </summary>
        Asc = 1,
        /// <summary>
        /// Sort by descending
        /// </summary>
        Desc
    }

}
