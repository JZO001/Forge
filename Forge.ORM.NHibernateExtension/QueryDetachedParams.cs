/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Forge.ORM.NHibernateExtension.Model;
using Forge.Shared;
using NHibernate.Criterion;

namespace Forge.ORM.NHibernateExtension
{

    /// <summary>
    /// Parameters for query which uses NHibernate DetachedCriteria
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("[{GetType()}, MaxResults = {MaxResults}]")]
    public class QueryDetachedParams : QueryParamsBase, IEquatable<QueryDetachedParams>
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DetachedCriteria mCriteria = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDetachedParams"/> class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        public QueryDetachedParams(Type entityType)
            : this(entityType, DetachedCriteria.For(entityType), DEFAULT_MAX_RESULTS)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDetachedParams"/> class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="criteria">The criteria.</param>
        public QueryDetachedParams(Type entityType, DetachedCriteria criteria)
            : this(entityType, criteria, DEFAULT_MAX_RESULTS)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDetachedParams" /> class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="maxResults">The max results.</param>
        public QueryDetachedParams(Type entityType, DetachedCriteria criteria, int maxResults)
            : base(entityType, maxResults)
        {
            if (criteria == null)
            {
                ThrowHelper.ThrowArgumentNullException("criteria");
            }

            mCriteria = criteria;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the criteria.
        /// </summary>
        /// <value>
        /// The criteria.
        /// </value>
        [DebuggerHidden]
        public DetachedCriteria Criteria
        {
            get { return mCriteria; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }
                mCriteria = value;
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Gets the detached criteria.
        /// </summary>
        /// <returns></returns>
        public override DetachedCriteria GetDetachedCriteria()
        {
            return mCriteria;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            QueryDetachedParams clone = (QueryDetachedParams)base.Clone();
            clone.Criteria = Criteria;
            return clone;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(QueryDetachedParams other)
        {
            return Equals((object)other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!obj.GetType().Equals(GetType())) return false;

            QueryDetachedParams other = (QueryDetachedParams)obj;
            return base.Equals(obj) && other.mCriteria.Equals(mCriteria);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

    }

    /// <summary>
    /// Parameters for query which uses NHibernate DetachedCriteria (generic)
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("[{GetType()}, MaxResults = {MaxResults}]")]
    public class QueryDetachedParams<TEntity> : QueryDetachedParams, IEquatable<QueryDetachedParams<TEntity>> where TEntity : EntityBaseWithoutId
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDetachedParams{TEntity}"/> class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        protected QueryDetachedParams(Type entityType)
            : base(entityType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDetachedParams{TEntity}"/> class.
        /// </summary>
        public QueryDetachedParams()
            : base(typeof(TEntity))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDetachedParams{TEntity}"/> class.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public QueryDetachedParams(DetachedCriteria criteria)
            : base(typeof(TEntity), criteria, DEFAULT_MAX_RESULTS)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDetachedParams{TEntity}"/> class.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="maxResults">The max results.</param>
        public QueryDetachedParams(DetachedCriteria criteria, int maxResults)
            : base(typeof(TEntity), criteria, maxResults)
        {
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(QueryDetachedParams<TEntity> other)
        {
            return Equals((object)other);
        }

        #endregion

    }

}
