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
    /// Between criteria
    /// <example>
    /// <code>
    /// Criteria betweenCriteria = new BetweenCriteria("value", 1L, 3L);
    ///
    /// IListSpecialized&lt;EnumeratorItem&gt; resultList = uow.Query&lt;EnumeratorItem&gt;(betweenCriteria);
    /// foreach (EnumeratorItem e in resultList)
    /// {
    ///     Assert.IsTrue(betweenCriteria.ResultForEntity(e));
    ///     Assert.AreEqual(resultList.Count, 6);
    /// }
    /// </code>
    /// </example>
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("[{GetType()}, FieldName = {FieldName}, LoValue = '{HiValue}', HiValue = '{HiValue}', Negation = {Negation}]")]
    public sealed class BetweenCriteria : Criteria
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IComparable mHiValue = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IComparable mLoValue = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Prevents a default instance of the <see cref="BetweenCriteria"/> class from being created.
        /// </summary>
        private BetweenCriteria()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BetweenCriteria" /> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="loValue">The lo value.</param>
        /// <param name="hiValue">The hi value.</param>
        public BetweenCriteria(string fieldName, IComparable loValue, IComparable hiValue)
            : this(fieldName, loValue, hiValue, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BetweenCriteria" /> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="loValue">The lo value.</param>
        /// <param name="hiValue">The hi value.</param>
        /// <param name="disjunction">if set to <c>true</c> [disjunction].</param>
        public BetweenCriteria(string fieldName, IComparable loValue, IComparable hiValue, bool disjunction)
            : base(fieldName)
        {
            if (hiValue == null)
            {
                ThrowHelper.ThrowArgumentNullException("hiValue");
            }
            if (loValue == null)
            {
                ThrowHelper.ThrowArgumentNullException("loValue");
            }
            mHiValue = hiValue;
            mLoValue = loValue;
            Negation = disjunction;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the hi value.
        /// </summary>
        /// <value>
        /// The hi value.
        /// </value>
        public IComparable HiValue
        {
            get { return mHiValue; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }
                mHiValue = value;
                Reset();
            }
        }

        /// <summary>
        /// Gets or sets the lo value.
        /// </summary>
        /// <value>
        /// The lo value.
        /// </value>
        public IComparable LoValue
        {
            get { return mLoValue; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }
                mLoValue = value;
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

            ExtractObjectData ef = ExtractObjectData.Create(FieldName);
            object fieldValue = ef.GetValue(entity);
            bool result = HiValue.CompareTo(fieldValue) >= 0 && LoValue.CompareTo(fieldValue) <= 0; // az értéknek a két érték közé kell esnie
            if (Negation)
            {
                // az értéknek nem szabad a két érték közé kell esnie
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
            BetweenCriteria cloned = (BetweenCriteria)base.Clone();
            cloned.mHiValue = mHiValue;
            cloned.mLoValue = mLoValue;
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
            ICriterion result = Restrictions.Between(fieldName, LoValue, HiValue);
            if (Negation)
            {
                result = Restrictions.Not(result);
            }
            return result;
        }

        #endregion

    }

}
