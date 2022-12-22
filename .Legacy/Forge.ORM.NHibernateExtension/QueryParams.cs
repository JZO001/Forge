/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using Forge.ORM.NHibernateExtension.Criterias;
using Forge.ORM.NHibernateExtension.Model;
using NHibernate;
using NHibernate.Criterion;

namespace Forge.ORM.NHibernateExtension
{

    /// <summary>
    /// Represents the base class for query parameters
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("[{GetType()}, MaxResults = {MaxResults}]")]
    public abstract class QueryParamsBase : ICloneable
    {

        #region Field(s)

        /// <summary>
        /// The default value of max results
        /// </summary>
        protected static readonly int DEFAULT_MAX_RESULTS = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Type mEntityType = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mMaxResults = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mTimeout = System.Threading.Timeout.Infinite;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParams" /> class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        protected QueryParamsBase(Type entityType)
            : this(entityType, DEFAULT_MAX_RESULTS)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParams" /> class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="maxResults">The max results.</param>
        protected QueryParamsBase(Type entityType, int maxResults)
        {
            if (entityType == null)
            {
                ThrowHelper.ThrowArgumentNullException("entityType");
            }
            if (maxResults < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("maxResults");
            }
            if (!typeof(EntityBaseWithoutId).IsAssignableFrom(entityType))
            {
                ThrowHelper.ThrowArgumentException("Provided type is not assignable from EntityBaseWithoutId.");
            }

            this.mEntityType = entityType;
            this.mMaxResults = maxResults;
            this.Timeout = System.Threading.Timeout.Infinite;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the id.
        /// </summary>
        [DebuggerHidden]
        public int Id { get { return GetHashCode(); } }

        /// <summary>
        /// Gets or sets the max results.
        /// </summary>
        /// <value>
        /// The max results.
        /// </value>
        [DebuggerHidden]
        public int MaxResults
        {
            get { return mMaxResults; }
            set
            {
                if (value < 0)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }
                this.mMaxResults = value;
            }
        }

        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        [DebuggerHidden]
        public virtual Type EntityType
        {
            get { return mEntityType; }
        }

        /// <summary>
        /// Gets or sets the timeout of the underlying ADO.NET query.
        /// </summary>
        /// <value>
        /// The timeout.
        /// </value>
        [DebuggerHidden]
        public int Timeout
        {
            get { return mTimeout; }
            set
            {
                if (value < System.Threading.Timeout.Infinite)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }
                this.mTimeout = value;
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Gets the detached criteria.
        /// </summary>
        /// <returns></returns>
        public abstract DetachedCriteria GetDetachedCriteria();

        /// <summary>
        /// Prepares the criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public virtual void PrepareCriteria(ICriteria criteria)
        {
            if (criteria == null)
            {
                ThrowHelper.ThrowArgumentNullException("criteria");
            }

            criteria.SetTimeout(mTimeout);

            if (mMaxResults > 0)
            {
                criteria.SetMaxResults(mMaxResults);
            }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            QueryParamsBase cloned = (QueryParamsBase)this.GetType().GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, new Type[] { typeof(Type) }, null).Invoke(new object[] { this.mEntityType });
            cloned.mMaxResults = this.mMaxResults;
            return cloned;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(QueryParamsBase other)
        {
            return Equals((object)other);
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
            if (ReferenceEquals(this, obj)) return true;
            if (!obj.GetType().Equals(GetType())) return false;

            QueryParamsBase other = (QueryParamsBase)obj;
            return other.mMaxResults == this.mMaxResults;
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
    /// Parameters for queries
    /// <example>
    /// <code>
    /// QueryParams&lt;EnumeratorItem&gt; qp = null;
    /// Criteria c = null;
    /// IList&lt;EnumeratorItem&gt; resultItem1 = null;
    ///
    /// c = new GroupCriteria(
    ///         new ArithmeticCriteria("value", 0L),
    ///         new ArithmeticCriteria("id.systemId", 1L),
    ///         new ArithmeticCriteria("enumeratorType.guest.id.systemId", 1L),
    ///         new ArithmeticCriteria("enumeratorType.guest.name", "G1"),
    ///         new ArithmeticCriteria("enumeratorType.guest.name", "G1"),
    ///         new ArithmeticCriteria("enumeratorType.systemEnumeratorType", SystemEnumeratorTypeEnum.PaymentModes));
    ///
    /// qp = new QueryParams&lt;EnumeratorItem&gt;(c);
    /// resultItem1 = uow.Query&lt;EnumeratorItem&gt;(qp);
    /// </code>
    /// </example>
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("[{GetType()}, MaxResults = {MaxResults}, OrderBys = {OrderBys.Count}]")]
    public class QueryParams : QueryParamsBase, IEquatable<QueryParams>
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Criteria mCriteria = AllCriteria.Instance;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<OrderBy> mOrderBys = new List<OrderBy>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DetachedCriteria mDetachedCriteria = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParams"/> class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        public QueryParams(Type entityType)
            : this(entityType, AllCriteria.Instance, (ICollection<OrderBy>)null, DEFAULT_MAX_RESULTS)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParams"/> class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="criteria">The criteria.</param>
        public QueryParams(Type entityType, Criteria criteria)
            : this(entityType, criteria, (ICollection<OrderBy>)null, DEFAULT_MAX_RESULTS)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParams"/> class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="maxResults">The max results.</param>
        public QueryParams(Type entityType, Criteria criteria, int maxResults)
            : this(entityType, criteria, (ICollection<OrderBy>)null, maxResults)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParams"/> class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="orderBy">The order by.</param>
        public QueryParams(Type entityType, Criteria criteria, OrderBy orderBy)
            : this(entityType, criteria, orderBy, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParams"/> class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="maxResults">The max results.</param>
        public QueryParams(Type entityType, Criteria criteria, OrderBy orderBy, int maxResults)
            : base(entityType, maxResults)
        {
            if (criteria == null)
            {
                ThrowHelper.ThrowArgumentNullException("criteria");
            }

            this.mCriteria = criteria;
            if (orderBy != null)
            {
                this.mOrderBys.Add(orderBy);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParams"/> class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="orderBys">The order bys.</param>
        public QueryParams(Type entityType, Criteria criteria, ICollection<OrderBy> orderBys)
            : this(entityType, criteria, orderBys, DEFAULT_MAX_RESULTS)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParams"/> class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="orderBys">The order bys.</param>
        /// <param name="maxResults">The max results.</param>
        public QueryParams(Type entityType, Criteria criteria, ICollection<OrderBy> orderBys, int maxResults)
            : base(entityType, maxResults)
        {
            if (criteria == null)
            {
                ThrowHelper.ThrowArgumentNullException("criteria");
            }

            this.mCriteria = criteria;
            if (orderBys != null)
            {
                this.mOrderBys.AddRange(orderBys);
            }
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
        public Criteria Criteria
        {
            get { return mCriteria; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }
                this.mCriteria = value;
            }
        }

        /// <summary>
        /// Gets the order bys.
        /// </summary>
        [DebuggerHidden]
        public List<OrderBy> OrderBys
        {
            get { return mOrderBys; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Gets the detached criteria.
        /// </summary>
        /// <returns></returns>
        public override DetachedCriteria GetDetachedCriteria()
        {
            if (mDetachedCriteria == null)
            {
                mDetachedCriteria = DetachedCriteria.For(this.EntityType, "this");
                ((Criteria)this.mCriteria.Clone()).BuildCriteria(mDetachedCriteria);
            }
            return mDetachedCriteria;
        }

        /// <summary>
        /// Prepares the criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public override void PrepareCriteria(ICriteria criteria)
        {
            base.PrepareCriteria(criteria);

            if (mOrderBys.Count > 0)
            {
                foreach (OrderBy orderBy in mOrderBys)
                {
                    criteria.AddOrder(new Order(orderBy.FieldName, orderBy.OrderMode == OrderModeEnum.Asc ? true : false));
                }
            }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            QueryParams clone = (QueryParams)base.Clone();
            clone.Criteria = this.Criteria;
            clone.mOrderBys = new List<OrderBy>(this.mOrderBys);
            return clone;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(QueryParams other)
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

            QueryParams other = (QueryParams)obj;
            bool result = base.Equals(obj) && other.mCriteria.Equals(this.mCriteria);
            if (result)
            {
                if (other.mOrderBys.Count == this.mOrderBys.Count)
                {
                    for (int i = 0; i < this.mOrderBys.Count; i++)
                    {
                        if (!other.mOrderBys[i].Equals(this.mOrderBys[i]))
                        {
                            result = false;
                            break;
                        }
                    }
                }
                else
                {
                    result = false;
                }
            }
            return result;
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
    /// Parameters for queries (generic)
    /// <example>
    /// <code>
    /// QueryParams&lt;EnumeratorItem&gt; qp = null;
    /// Criteria c = null;
    /// c = new GroupCriteria(GroupCriteriaLogicEnum.Or,
    /// new GroupCriteria(
    ///     new ArithmeticCriteria("value", 0L),
    ///     new ArithmeticCriteria("id.systemId", 1L),
    ///     new ArithmeticCriteria("enumeratorType.guest.id.systemId", 1L),
    ///     new ArithmeticCriteria("enumeratorType.guest.name", "G1"),
    ///     new ArithmeticCriteria("enumeratorType.guest.name", "G1"),
    ///     new ArithmeticCriteria("enumeratorType.systemEnumeratorType", SystemEnumeratorTypeEnum.PaymentModes)),
    /// new GroupCriteria(
    ///     new ArithmeticCriteria("value", 0L),
    ///     new ArithmeticCriteria("id.systemId", 1L),
    ///     new ArithmeticCriteria("enumeratorType.guest.id.systemId", 1L),
    ///     new ArithmeticCriteria("enumeratorType.guest.name", "G1"),
    ///     new ArithmeticCriteria("enumeratorType.guest.name", "G1"),
    ///     new ArithmeticCriteria("enumeratorType.systemEnumeratorType", SystemEnumeratorTypeEnum.PaymentModes))
    /// );
    /// qp = new QueryParams&lt;EnumeratorItem&gt;(c);
    /// IList&lt;EnumeratorItem&gt; resultItem2 = uow.Query&lt;EnumeratorItem&gt;(qp);
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="TEntity">The type of the class.</typeparam>
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("[{GetType()}, MaxResults = {MaxResults}, OrderBys = {OrderBys.Count}]")]
    public class QueryParams<TEntity> : QueryParams, IEquatable<QueryParams<TEntity>> where TEntity : EntityBaseWithoutId
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParams{TEntity}"/> class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        protected QueryParams(Type entityType)
            : base(entityType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParams{TEntity}"/> class.
        /// </summary>
        public QueryParams()
            : base(typeof(TEntity))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParams{TEntity}"/> class.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public QueryParams(Criteria criteria)
            : base(typeof(TEntity), criteria)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParams{TEntity}"/> class.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="maxResults">The max results.</param>
        public QueryParams(Criteria criteria, int maxResults)
            : base(typeof(TEntity), criteria, (ICollection<OrderBy>)null, maxResults)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParams{TEntity}"/> class.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="orderBy">The order by.</param>
        public QueryParams(Criteria criteria, OrderBy orderBy)
            : base(typeof(TEntity), criteria, orderBy, DEFAULT_MAX_RESULTS)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParams{TEntity}"/> class.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="maxResults">The max results.</param>
        public QueryParams(Criteria criteria, OrderBy orderBy, int maxResults)
            : base(typeof(TEntity), criteria, orderBy, maxResults)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParams{TEntity}"/> class.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="orderBys">The order bys.</param>
        public QueryParams(Criteria criteria, ICollection<OrderBy> orderBys)
            : base(typeof(TEntity), criteria, orderBys, DEFAULT_MAX_RESULTS)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParams{TEntity}"/> class.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="orderBys">The order bys.</param>
        /// <param name="maxResults">The max results.</param>
        public QueryParams(Criteria criteria, ICollection<OrderBy> orderBys, int maxResults)
            : base(typeof(TEntity), criteria, orderBys, maxResults)
        {
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(QueryParams<TEntity> other)
        {
            return Equals((object)other);
        }

        #endregion

    }

}
