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
    /// Arithmetic criteria for comparable types
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
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("[{GetType()}, FieldName = {FieldName}, Value = '{Value}', Operand = {Operand}, Negation = {Negation}]")]
    public sealed class ArithmeticCriteria : Criteria
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IComparable mValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ArithmeticOperandEnum mOperand = ArithmeticOperandEnum.Equal;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Prevents a default instance of the <see cref="ArithmeticCriteria"/> class from being created.
        /// </summary>
        private ArithmeticCriteria()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArithmeticCriteria"/> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        public ArithmeticCriteria(string fieldName, IComparable value)
            : this(fieldName, value, ArithmeticOperandEnum.Equal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArithmeticCriteria"/> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        /// <param name="operand">The operand.</param>
        public ArithmeticCriteria(string fieldName, IComparable value, ArithmeticOperandEnum operand)
            : base(fieldName)
        {
            if (value == null)
            {
                ThrowHelper.ThrowArgumentNullException("value");
            }

            Type valueType = value.GetType();
            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition().Equals(typeof(EntityBaseGenericId<>)))
            {
                dynamic dynValue = value;
                if (dynValue.Id == null)
                {
                    ThrowHelper.ThrowArgumentException(String.Format("Provided entity has not got identifier. Entity type: '{0}'.", valueType.FullName), "value");
                }
                if (fieldName.ToLower().Equals("id"))
                {
                    FieldName = fieldName;
                }
                else
                {
                    FieldName = fieldName.ToLower().EndsWith(".id") ? fieldName : String.Format("{0}.id", fieldName);
                }
                mValue = dynValue.Id;
            }
            else
            {
                mValue = value;
            }
            mOperand = operand;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public IComparable Value
        {
            get { return mValue; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }
                Type valueType = value.GetType();
                if (valueType.IsGenericType && valueType.GetGenericTypeDefinition().Equals(typeof(EntityBaseGenericId<>)))
                {
                    dynamic dynValue = value;
                    if (dynValue.Id == null)
                    {
                        ThrowHelper.ThrowArgumentException(String.Format("Provided entity has not got identifier. Entity type: '{0}'.", valueType.FullName), "value");
                    }
                    mValue = dynValue.Id;
                }
                else
                {
                    mValue = value;
                }
                Reset();
            }
        }

        /// <summary>
        /// Gets or sets the operand.
        /// </summary>
        /// <value>
        /// The operand.
        /// </value>
        public ArithmeticOperandEnum Operand
        {
            get { return mOperand; }
            set
            {
                mOperand = value;
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
            ExtractObjectData ef = ExtractObjectData.Create(FieldName);
            object fieldValue = ef.GetValue(entity);

            switch (Operand)
            {
                case ArithmeticOperandEnum.Equal:
                    result = Value.CompareTo(fieldValue) == 0;
                    break;

                case ArithmeticOperandEnum.NotEqual:
                    result = Value.CompareTo(fieldValue) != 0;
                    break;

                case ArithmeticOperandEnum.Greater:
                    result = Value.CompareTo(fieldValue) < 0;
                    break;

                case ArithmeticOperandEnum.Lower:
                    result = Value.CompareTo(fieldValue) > 0;
                    break;

                case ArithmeticOperandEnum.GreaterOrEqual:
                    result = Value.CompareTo(fieldValue) <= 0;
                    break;

                case ArithmeticOperandEnum.LowerOrEqual:
                    result = Value.CompareTo(fieldValue) >= 0;
                    break;

                default:
                    throw new NotImplementedException();
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
            ArithmeticCriteria cloned = (ArithmeticCriteria)base.Clone();
            cloned.mOperand = mOperand;
            cloned.mValue = mValue;
            return cloned;
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Builds the criterion.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override ICriterion BuildCriterion(string fieldName)
        {
            ICriterion result = null;

            switch (Operand)
            {
                case ArithmeticOperandEnum.Equal:
                    result = Restrictions.Eq(fieldName, Value);
                    break;

                case ArithmeticOperandEnum.NotEqual:
                    result = Restrictions.Not(Restrictions.Eq(fieldName, Value));
                    break;

                case ArithmeticOperandEnum.Greater:
                    result = new SimpleExpression(fieldName, Value, " > ");
                    break;

                case ArithmeticOperandEnum.Lower:
                    result = new SimpleExpression(fieldName, Value, " < ");
                    break;

                case ArithmeticOperandEnum.GreaterOrEqual:
                    result = Restrictions.Ge(fieldName, Value);
                    break;

                case ArithmeticOperandEnum.LowerOrEqual:
                    result = Restrictions.Le(fieldName, Value);
                    break;

                default:
                    throw new NotImplementedException();
            }

            if (Negation)
            {
                result = Restrictions.Not(result);
            }

            return result;
        }

        #endregion

    }

}
