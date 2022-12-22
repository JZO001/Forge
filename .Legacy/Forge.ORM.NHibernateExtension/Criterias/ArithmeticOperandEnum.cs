/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.ORM.NHibernateExtension.Criterias
{

    /// <summary>
    /// Operands for arithemtic criteria
    /// </summary>
    [Serializable]
    public enum ArithmeticOperandEnum : int
    {
        /// <summary>
        /// Equal
        /// </summary>
        Equal = 0,
        /// <summary>
        /// NotEqual
        /// </summary>
        NotEqual,
        /// <summary>
        /// Greater
        /// </summary>
        Greater,
        /// <summary>
        /// Lower
        /// </summary>
        Lower,
        /// <summary>
        /// Greater or equal
        /// </summary>
        GreaterOrEqual,
        /// <summary>
        /// Lower or equal
        /// </summary>
        LowerOrEqual
    }

}
