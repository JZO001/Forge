/* *********************************************************************
 * Date: 24 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.ErrorReport.Filter
{

    /// <summary>
    /// Operands for arithemtic filter
    /// </summary>
    [Serializable]
    public enum ArithmeticFilterOperandEnum
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
