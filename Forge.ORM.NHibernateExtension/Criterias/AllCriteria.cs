/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Forge.ORM.NHibernateExtension.Model;
using Forge.Shared;
using NHibernate.Criterion;

namespace Forge.ORM.NHibernateExtension.Criterias
{

    /// <summary>
    /// Default criteria for query initializations. These criteria means ALL.
    /// <example>
    /// <code>
    ///     Criteria criteria = AllCriteria.Instance;
    ///     IListSpecialized&lt;CustomEntities&gt; resultList = uow.Query&lt;CustomEntities&gt;(criteria);
    ///
    ///     foreach (CustomEntities e in resultList)
    ///     {
    ///         Assert.IsTrue(criteria.ResultForEntity(e));
    ///         Assert.AreEqual(resultList.Count, 2);
    ///     }
    /// </code>
    /// </example>
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("[{GetType()}]")]
    public sealed class AllCriteria : Criteria
    {

        #region Field(s)

        private static AllCriteria mSingleton = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Prevents a default instance of the <see cref="AllCriteria"/> class from being created.
        /// </summary>
        private AllCriteria()
        {
            mCriterion = Expression.Sql(" 1=1 ");
        }

        #endregion

        #region Public properties

        /// <summary>
        /// NOT USED
        /// Gets or sets a value indicating whether this <see cref="Criteria" /> is negation.
        /// </summary>
        /// <value>
        ///   <c>true</c> if negation; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="System.NotImplementedException">
        /// </exception>
        [DebuggerHidden]
        public override bool Negation
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Gets the instance of the AllCriteria.
        /// </summary>
        /// <returns></returns>
        public static AllCriteria Instance
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (mSingleton == null)
                {
                    mSingleton = new AllCriteria();
                }
                return mSingleton;
            }
        }

        /// <summary>
        /// Examine criteria match on the provided entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public override bool ResultForEntity(EntityBaseWithoutId entity)
        {
            return true;
        }

        /// <summary>
        /// Builds the criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="dependencyCriterion">The dependency criterion.</param>
        public override void BuildCriteria(DetachedCriteria criteria, Junction dependencyCriterion)
        {
            if (criteria == null)
            {
                ThrowHelper.ThrowArgumentNullException("criteria");
            }
            if (dependencyCriterion != null)
            {
                dependencyCriterion.Add(mCriterion);
            }
            else
            {
                criteria.Add(mCriterion);
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return ReferenceEquals(this, obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        [DebuggerHidden]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        [DebuggerHidden]
        public override object Clone()
        {
            return this;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return mCriterion.ToString();
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Resets this instance.
        /// </summary>
        protected override void Reset()
        {
        }

        /// <summary>
        /// Builds the criterion.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        protected override ICriterion BuildCriterion(string fieldName)
        {
            return mCriterion;
        }

        #endregion

    }

}
