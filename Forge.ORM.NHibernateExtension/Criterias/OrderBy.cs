/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using Forge.ORM.NHibernateExtension.Model;
using Forge.Reflection;
using Forge.Shared;

namespace Forge.ORM.NHibernateExtension.Criterias
{

    /// <summary>
    /// OrderBy for ordering entities
    /// <example>
    /// <code>
    /// QueryParams&lt;Driver&gt; qp = new QueryParams&lt;Driver&gt;(
    ///     new GroupCriteria(
    ///         new ArithmeticCriteria("name", "B", ArithmeticOperandEnum.NotEqual),
    ///         new ArithmeticCriteria("orderMode", OrderModeEnum.Asc)
    ///     )
    /// );
    /// qp.MaxResults = 2;
    /// qp.MaxFetchDeepth = 3;
    /// qp.OrderBys.Add(new OrderBy("id.id", OrderModeEnum.Desc));
    /// IList&lt;Driver&gt; resultList = QueryHelper.Query&lt;Driver&gt;(session, qp);
    ///
    /// Assert.IsTrue(qp.Criteria.ResultForEntity(resultList[0]));
    /// </code>
    /// </example>
    /// </summary>
    [Serializable]
    [DataContract]
    [DebuggerDisplay("[{GetType()}, FieldName = '{FieldName}', OrderMode = {OrderMode}]")]
    public sealed class OrderBy : ICloneable, IComparer<EntityBaseWithoutId>, IEquatable<OrderBy>
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string mFieldName = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private OrderModeEnum mOrderMode = OrderModeEnum.Asc;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Prevents a default instance of the <see cref="OrderBy"/> class from being created.
        /// </summary>
        private OrderBy()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderBy"/> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        public OrderBy(string fieldName)
            : this(fieldName, OrderModeEnum.Asc)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderBy"/> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="orderMode">The order mode.</param>
        public OrderBy(string fieldName, OrderModeEnum orderMode)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                ThrowHelper.ThrowArgumentNullException("fieldName");
            }
            mFieldName = fieldName;
            mOrderMode = orderMode;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public string FieldName
        {
            get { return mFieldName; }
        }

        /// <summary>
        /// Gets the order mode.
        /// </summary>
        public OrderModeEnum OrderMode
        {
            get { return mOrderMode; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new OrderBy(FieldName, OrderMode);
        }

        /// <summary>
        /// Compares the specified left entity.
        /// </summary>
        /// <param name="leftEntity">The left entity.</param>
        /// <param name="rightEntity">The right entity.</param>
        /// <returns></returns>
        public int Compare(EntityBaseWithoutId leftEntity, EntityBaseWithoutId rightEntity)
        {
            if (leftEntity == null)
            {
                ThrowHelper.ThrowArgumentNullException("leftEntity");
            }
            if (rightEntity == null)
            {
                ThrowHelper.ThrowArgumentNullException("rightEntity");
            }

            int result = 0;

            ExtractObjectData of = ExtractObjectData.Create(FieldName);

            object entityValueLeft = of.GetValue(leftEntity);
            object entityValueRight = of.GetValue(rightEntity);

            if (entityValueLeft == null && entityValueRight == null)
            {
                // equals
            }
            else if (entityValueLeft == null)
            {
                result = -1;
            }
            else if (entityValueRight == null)
            {
                result = 1;
            }
            else
            {
                result = ((IComparable)entityValueLeft).CompareTo(entityValueRight);
            }

            if (OrderMode.Equals(OrderModeEnum.Desc))
            {
                // revert order
                result = result * -1;
            }

            return result;
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

            OrderBy ob = (OrderBy)obj;
            return ((ob.mFieldName == null ? mFieldName == null : ob.mFieldName.Equals(mFieldName))
                        && ob.mOrderMode.Equals(mOrderMode));
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(OrderBy other)
        {
            return Equals((object)other);
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

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, Field name: '{1}', Order: {2}", base.ToString(), FieldName, OrderMode);
        }

        #endregion

    }

}
