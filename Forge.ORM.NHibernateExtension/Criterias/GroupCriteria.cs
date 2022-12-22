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

namespace Forge.ORM.NHibernateExtension.Criterias
{

    /// <summary>
    /// Group criterias into conjunctions or disjunctions (A and B and C..., A or B or C...)
    /// <example>
    /// <code>
    /// Criteria likeCriteria = new GroupCriteria(new LikeCriteria("name", "C", LikeMatchModeEnum.Anywhere),
    ///     new ArithmeticCriteria("enumeratorType.id.systemId", 1L),
    ///     new ArithmeticCriteria("enumeratorType.systemEnumeratorType", SystemEnumeratorTypeEnum.PaymentModes));
    ///
    /// IListSpecialized&lt;EnumeratorItem&gt; resultList = uow.Query&lt;EnumeratorItem&gt;(likeCriteria);
    /// foreach (EnumeratorItem e in resultList)
    /// {
    ///     Assert.IsTrue(likeCriteria.ResultForEntity(e));
    /// }
    /// </code>
    /// </example>
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("[{GetType()}, Logic = {Logic}, Negation = {Negation}]")]
    public sealed class GroupCriteria : Criteria
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Criteria[] mCriterias = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private GroupCriteriaLogicEnum mLogic = GroupCriteriaLogicEnum.And;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Prevents a default instance of the <see cref="GroupCriteria"/> class from being created.
        /// </summary>
        private GroupCriteria()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupCriteria"/> class.
        /// </summary>
        /// <param name="criterias">The criterias.</param>
        public GroupCriteria(params Criteria[] criterias)
            : this(GroupCriteriaLogicEnum.And, criterias)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupCriteria"/> class.
        /// </summary>
        /// <param name="logic">The logic.</param>
        /// <param name="criterias">The criterias.</param>
        public GroupCriteria(GroupCriteriaLogicEnum logic, params Criteria[] criterias)
        {
            if (criterias == null || (criterias != null && criterias.Length == 0))
            {
                ThrowHelper.ThrowArgumentNullException("criterias");
            }
            mLogic = logic;
            Criterias = criterias;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        [DebuggerHidden]
        public override string FieldName
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

        /// <summary>
        /// Gets or sets the criterias.
        /// </summary>
        /// <value>
        /// The criterias.
        /// </value>
        public Criteria[] Criterias
        {
            get { return mCriterias; }
            set
            {
                if (value == null || (value != null && value.Length == 0))
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }
                mCriterias = value;
                Reset();
                foreach (Criteria c in value)
                {
                    c.Parent = this;
                }
            }
        }

        /// <summary>
        /// Gets or sets the logic.
        /// </summary>
        /// <value>
        /// The logic.
        /// </value>
        public GroupCriteriaLogicEnum Logic
        {
            get { return mLogic; }
            set
            {
                mLogic = value;
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

            bool result = true;
            if (Logic == GroupCriteriaLogicEnum.Or)
            {
                result = false;
            }
            // shortcuts calculations
            foreach (Criteria c in Criterias)
            {
                if (Logic == GroupCriteriaLogicEnum.And)
                {
                    result = result && c.ResultForEntity(entity);
                    if (!result)
                    {
                        break;
                    }
                }
                else
                {
                    result = result || c.ResultForEntity(entity);
                    if (result)
                    {
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
        /// Builds the criterion.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="dependencyCriterion">The dependency criterion.</param>
        public override void BuildCriteria(DetachedCriteria criteria, Junction dependencyCriterion)
        {
            if (criteria == null)
            {
                ThrowHelper.ThrowArgumentNullException("criteria");
            }

            if (Criterias.Length == 1)
            {
                Criterias[0].BuildCriteria(criteria, dependencyCriterion);
            }
            else
            {
                if (mCriterion == null)
                {
                    mCriterion = BuildCriterion(string.Empty);
                }

                ICriterion localCriterion = mCriterion;
                if (Negation)
                {
                    localCriterion = Restrictions.Not(mCriterion);
                }

                if (dependencyCriterion != null)
                {
                    dependencyCriterion.Add(localCriterion);
                }
                else
                {
                    criteria.Add(localCriterion);
                }

                foreach (Criteria c in Criterias)
                {
                    c.BuildCriteria(criteria, (Junction)mCriterion);
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
            GroupCriteria cloned = (GroupCriteria)base.Clone();
            cloned.mLogic = mLogic;
            if (mCriterias != null)
            {
                Criteria[] clonedCriterias = new Criteria[mCriterias.Length];
                for (int i = 0; i < mCriterias.Length; i++)
                {
                    clonedCriterias[i] = mCriterias[i].Clone() as Criteria;
                    clonedCriterias[i].Parent = cloned;
                }
                cloned.mCriterias = clonedCriterias;
            }
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
            ICriterion result = null;

            if (Logic == GroupCriteriaLogicEnum.And)
            {
                result = Restrictions.Conjunction();
            }
            else
            {
                result = Restrictions.Disjunction();
            }

            return result;
        }

        #endregion

    }

}
