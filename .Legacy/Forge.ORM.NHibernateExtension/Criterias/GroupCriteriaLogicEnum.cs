/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.ORM.NHibernateExtension.Criterias
{

    /// <summary>
    /// Assign logic for group criteria
    /// </summary>
    [Serializable]
    public enum GroupCriteriaLogicEnum : int
    {
        /// <summary>
        /// And expression
        /// </summary>
        And = 1,
        /// <summary>
        /// Or expression
        /// </summary>
        Or
    }

}
