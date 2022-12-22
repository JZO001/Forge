/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Forge.ORM.NHibernateExtension.Model;
using Forge.Reflection;
using Forge.Shared;
using NHibernate.Criterion;

namespace Forge.ORM.NHibernateExtension.Criterias
{

    /// <summary>
    /// Null criteria
    /// <example>
    /// <code>
    /// Criteria criteria = new NullCriteria("name");
    /// IListSpecialized&lt;CustomEntities&gt; resultList = uow.Query&lt;CustomEntities&gt;(criteria);
    ///
    /// foreach (CustomEntities e in resultList)
    /// {
    ///     Assert.IsTrue(criteria.ResultForEntity(e));
    ///     Assert.AreEqual(resultList.Count, 1);
    ///     Assert.IsNull(e.Name);
    /// }
    /// </code>
    /// </example>
    /// </summary>
    [Serializable]
    [DataContract]
    [DebuggerDisplay("[{GetType()}, FieldName = {FieldName}, Negation = {Negation}]")]
    public sealed class NullCriteria : Criteria
    {

        #region Constructor(s)

        /// <summary>
        /// Prevents a default instance of the <see cref="NullCriteria"/> class from being created.
        /// </summary>
        private NullCriteria()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullCriteria"/> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        public NullCriteria(string fieldName)
            : this(fieldName, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullCriteria"/> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="negation">if set to <c>true</c> [negation].</param>
        public NullCriteria(string fieldName, bool negation)
            : base(fieldName, negation)
        {
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Examine criteria match on the provided entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public override bool ResultForEntity(EntityBaseWithoutId entity)
        {
            if (entity == null)
            {
                ThrowHelper.ThrowArgumentNullException("entity");
            }

            ExtractObjectData of = ExtractObjectData.Create(FieldName);
            return Negation ? of.GetValue(entity) != null : of.GetValue(entity) == null;
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Builds the criterion.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        protected override ICriterion BuildCriterion(string fieldName)
        {
            return Negation ? Restrictions.Not(Restrictions.IsNull(fieldName)) : Restrictions.IsNull(fieldName);
        }

        #endregion

    }

}
