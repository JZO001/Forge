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
    /// In Criteria
    /// <example>
    /// <code>
    /// Criteria criteria = new InCriteria("name", "Cash", "CreditCard", "Transfer");
    ///
    /// IListSpecialized&lt;EnumeratorItem&gt; resultList = uow.Query&lt;EnumeratorItem&gt;(criteria);
    /// </code>
    /// </example>
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("[{GetType()}, FieldName = {FieldName}, Negation = {Negation}]")]
    public sealed class InCriteria : Criteria
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IComparable[] mValues = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Prevents a default instance of the <see cref="InCriteria"/> class from being created.
        /// </summary>
        private InCriteria()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InCriteria"/> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="values">The values.</param>
        public InCriteria(string fieldName, params IComparable[] values)
            : this(fieldName, false, values)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InCriteria"/> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="disjunction">if set to <c>true</c> [all equals].</param>
        /// <param name="values">The values.</param>
        public InCriteria(string fieldName, bool disjunction, params IComparable[] values)
            : base(fieldName)
        {
            if (values == null || values.Length == 0)
            {
                ThrowHelper.ThrowArgumentNullException("values");
            }
            mValues = values;
            Negation = disjunction;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public IComparable[] Values
        {
            get { return mValues; }
            set
            {
                if (value == null || value.Length == 0)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }
                mValues = value;
                Reset();
            }
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

            bool result = false;

            ExtractObjectData of = ExtractObjectData.Create(FieldName);
            object entityValue = of.GetValue(entity);
            foreach (object o in Values)
            {
                if (o == null && entityValue == null)
                {
                    result = true;
                    break;
                }
                else if (entityValue != null)
                {
                    if (entityValue.Equals(o))
                    {
                        result = true;
                        break;
                    }
                }
            }

            if (Negation)
            {
                result = !result;
            }

            return result;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            InCriteria cloned = (InCriteria)base.Clone();
            cloned.mValues = mValues;
            return cloned;
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
            ICriterion result = Restrictions.In(fieldName, Values);
            if (Negation)
            {
                result = Restrictions.Not(result);
            }
            return result;
        }

        #endregion

    }

}
