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
    /// Like criteria. This expression is not case sensite.
    /// <example>
    /// <code>
    /// Criteria LikeAnyWhereCriteria = new LikeCriteria("name", "Cash2", LikeMatchModeEnum.Anywhere);
    /// IListSpecialized&lt;EnumeratorItem&gt; resultList = uow.Query&lt;EnumeratorItem&gt;(LikeAnyWhereCriteria);
    /// foreach (EnumeratorItem e in resultList)
    /// {
    ///     Assert.IsTrue(LikeAnyWhereCriteria.ResultForEntity(e));
    ///     Assert.AreEqual(resultList.Count, 0);
    ///     Assert.AreNotEqual(e.Name, "Cash2");
    /// }
    /// </code>
    /// </example>
    /// </summary>
    [Serializable]
    [DataContract]
    [DebuggerDisplay("[{GetType()}, FieldName = {FieldName}, Value = '{Value}', MatchMode = {MatchMode}, Negation = {Negation}]")]
    public sealed class LikeCriteria : Criteria
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mValue = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private LikeMatchModeEnum mMatchMode = LikeMatchModeEnum.Anywhere;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Prevents a default instance of the <see cref="LikeCriteria"/> class from being created.
        /// </summary>
        private LikeCriteria()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LikeCriteria"/> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        public LikeCriteria(string fieldName, string value)
            : this(fieldName, value, LikeMatchModeEnum.Anywhere)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LikeCriteria"/> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        /// <param name="matchMode">The match mode.</param>
        public LikeCriteria(string fieldName, string value, LikeMatchModeEnum matchMode)
            : base(fieldName)
        {
            if (value == null)
            {
                ThrowHelper.ThrowArgumentNullException("value");
            }
            mValue = value;
            mMatchMode = matchMode;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value
        {
            get { return mValue; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }
                mValue = value;
                Reset();
            }
        }

        /// <summary>
        /// Gets or sets the match mode.
        /// </summary>
        /// <value>
        /// The match mode.
        /// </value>
        public LikeMatchModeEnum MatchMode
        {
            get { return mMatchMode; }
            set
            {
                mMatchMode = value;
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
            String entityValue = ef.GetValue(entity).ToString();
            if (entityValue != null)
            {
                switch (MatchMode)
                {
                    case LikeMatchModeEnum.Exact:
                        result = entityValue.ToLower().Equals(Value.ToLower());
                        break;

                    case LikeMatchModeEnum.Start:
                        result = entityValue.ToLower().StartsWith(Value.ToLower());
                        break;

                    case LikeMatchModeEnum.End:
                        result = entityValue.ToLower().EndsWith(Value.ToLower());
                        break;

                    case LikeMatchModeEnum.Anywhere:
                        result = entityValue.ToLower().Contains(Value.ToLower());
                        break;

                    default:
                        throw new NotImplementedException();
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
            LikeCriteria cloned = (LikeCriteria)base.Clone();
            cloned.mValue = mValue;
            cloned.mMatchMode = mMatchMode;
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

            switch (MatchMode)
            {
                case LikeMatchModeEnum.Exact:
                    result = Restrictions.Like(fieldName, Value, global::NHibernate.Criterion.MatchMode.Exact);
                    break;

                case LikeMatchModeEnum.Start:
                    result = Restrictions.Like(fieldName, Value, global::NHibernate.Criterion.MatchMode.Start);
                    break;

                case LikeMatchModeEnum.End:
                    result = Restrictions.Like(fieldName, Value, global::NHibernate.Criterion.MatchMode.End);
                    break;

                case LikeMatchModeEnum.Anywhere:
                    result = Restrictions.Like(fieldName, Value, global::NHibernate.Criterion.MatchMode.Anywhere);
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
