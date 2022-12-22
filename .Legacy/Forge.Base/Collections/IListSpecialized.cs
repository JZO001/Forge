/* *********************************************************************
 * Date: 19 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Collections.Generic;

namespace Forge.Collections
{

    /// <summary>
    /// Mutable list extension interface
    /// <example>
    /// <code>
    /// Criteria arithmeticCriteria = new ArithmeticCriteria("value", 3L, ArithmeticOperandEnum.Greater);
    ///
    /// IListSpecialized&lt;EnumeratorItem&gt; resultList = uow.Query&lt;EnumeratorItem&gt;(arithmeticCriteria);
    /// foreach (EnumeratorItem e in resultList)
    /// {
    ///     Assert.IsTrue(arithmeticCriteria.ResultForEntity(e));
    ///     Assert.AreEqual(resultList.Count, 1);
    ///     Assert.AreEqual(e.Value, 4L);
    /// }
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    public interface IListSpecialized<T> : IList<T>, ISubList<T>
    {
    }

}
