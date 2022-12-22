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
using System.Text;
using Forge.ORM.NHibernateExtension.Model;
using Forge.Shared;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace Forge.ORM.NHibernateExtension.Criterias
{

    /// <summary>
    /// Criteria base class
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("[{GetType()}, FieldName = '{FieldName}', Negation = {Negation}]")]
    public abstract class Criteria : IReflectionCriteria, IEquatable<Criteria>
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mFieldName = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mNegation = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Criteria mParent = null;

        /// <summary>
        /// Cached criterion
        /// </summary>
        [NonSerialized]
        protected ICriterion mCriterion = null;

        [NonSerialized]
        private Dictionary<string, AssociationEntry> mAssociations = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="Criteria"/> class.
        /// </summary>
        protected Criteria()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Criteria"/> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        protected Criteria(string fieldName)
            : this(fieldName, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Criteria" /> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="negation">if set to <c>true</c> [disjunction].</param>
        protected Criteria(string fieldName, bool negation)
        {
            if (String.IsNullOrEmpty(fieldName))
            {
                ThrowHelper.ThrowArgumentNullException("fieldName");
            }
            mFieldName = fieldName;
            mNegation = negation;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public virtual Criteria Parent
        {
            get { return mParent; }
            set
            {
                mParent = value;
                Reset();
            }
        }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public virtual string FieldName
        {
            get { return mFieldName; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }
                mFieldName = value;
                Reset();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Criteria" /> is negation.
        /// </summary>
        /// <value>
        ///   <c>true</c> if negation; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Negation
        {
            get { return mNegation; }
            set
            {
                mNegation = value;
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
        public abstract bool ResultForEntity(EntityBaseWithoutId entity);

        /// <summary>
        /// Builds the criterion.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public virtual void BuildCriteria(DetachedCriteria criteria)
        {
            BuildCriteria(criteria, null);
        }

        /// <summary>
        /// Builds the criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="dependencyCriterion">The dependency criterion.</param>
        public virtual void BuildCriteria(DetachedCriteria criteria, Junction dependencyCriterion)
        {
            if (criteria == null)
            {
                ThrowHelper.ThrowArgumentNullException("criteria");
            }

            if (FieldName.Contains(".") && !FieldName.ToLower().StartsWith("id."))
            {
                CreateDetachedCriteria(criteria, FieldName, dependencyCriterion);
            }
            else
            {
                string fieldName = string.Empty;
                AssociationEntry e = null;
                if (!FieldName.ToLower().Contains("id."))
                {
                    Dictionary<string, AssociationEntry> ascs = GetAssociations();
                    if (ascs.ContainsKey(FieldName))
                    {
                        e = ascs[FieldName];
                    }
                    else
                    {
                        e = new AssociationEntry(FieldName, FieldName, string.Format("p{0}", Stopwatch.GetTimestamp().ToString()));
                        ascs[e.Key] = e;
                    }
                    fieldName = string.Format("{0}.{1}", criteria.Alias, e.Association);
                }
                else
                {
                    fieldName = string.IsNullOrEmpty(criteria.Alias) ? FieldName : string.Format("{0}.{1}", criteria.Alias, FieldName);
                }

                if (mCriterion == null)
                {
                    mCriterion = BuildCriterion(fieldName);
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

            Criteria other = (Criteria)obj;
            return other.mFieldName == mFieldName && other.mNegation.Equals(mNegation);
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public virtual bool Equals(Criteria other)
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
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            Criteria cloned = (Criteria)GetType().GetConstructor(BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, new Type[] { }, null).Invoke(new object[] { });
            cloned.mFieldName = mFieldName;
            cloned.mNegation = mNegation;
            return cloned;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return mCriterion == null ? "(Criteria has not been built)" : mCriterion.ToString();
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Resets this instance.
        /// </summary>
        protected virtual void Reset()
        {
            mCriterion = null;
            GetAssociations().Clear();
            //if (this.mParent != null) mParent.Reset();
        }

        /// <summary>
        /// Gets the associations.
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, AssociationEntry> GetAssociations()
        {
            Dictionary<string, AssociationEntry> result = null;

            if (Parent == null)
            {
                if (mAssociations == null)
                {
                    mAssociations = new Dictionary<string, AssociationEntry>();
                }
                result = mAssociations;
            }
            else
            {
                result = Parent.GetAssociations();
            }

            return result;
        }

        /// <summary>
        /// Builds the criterion.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        protected abstract ICriterion BuildCriterion(string fieldName);

        /// <summary>
        /// Creates the detached criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="property">The property.</param>
        /// <param name="dependencyCriterion">The dependency criterion.</param>
        protected void CreateDetachedCriteria(DetachedCriteria criteria, string property, Junction dependencyCriterion)
        {
            if (criteria == null)
            {
                ThrowHelper.ThrowArgumentNullException("criteria");
            }
            if (string.IsNullOrEmpty(property))
            {
                ThrowHelper.ThrowArgumentNullException("property");
            }

            List<string> propertyList = new List<string>(property.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries));
            if (propertyList.Count > 1)
            {
                CreateNextPropertyLevel(criteria, propertyList, 0, dependencyCriterion);
                propertyList.Clear();
            }
        }

        #endregion

        #region Private method(s)

        private void CreateNextPropertyLevel(DetachedCriteria criteria, List<string> propertyList, int listPointer, Junction dependencyCriterion)
        {
            Dictionary<string, AssociationEntry> ascs = GetAssociations();

            string associationKey = GetAssociationKey(propertyList, listPointer);
            AssociationEntry e = null;
            if (ascs.ContainsKey(associationKey))
            {
                e = ascs[associationKey];
                if (listPointer == propertyList.Count - 1)
                {
                    if (mCriterion == null)
                    {
                        mCriterion = BuildCriterion(e.Association);
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
                else
                {
                    DetachedCriteria subCriteria = e.Criteria;
                    if (subCriteria == null)
                    {
                        subCriteria = criteria.CreateCriteria(e.Association, e.Alias, JoinType.InnerJoin);
                        e.Criteria = subCriteria;
                    }
                    listPointer++;
                    CreateNextPropertyLevel(subCriteria, propertyList, listPointer, dependencyCriterion);
                }
            }
            else
            {
                bool isThisId = associationKey.ToLower().EndsWith(".id");
                if (listPointer == 0)
                {
                    e = new AssociationEntry(propertyList[0], propertyList[0], string.Format("p{0}{1}", Stopwatch.GetTimestamp().ToString(), listPointer.ToString()));
                }
                else
                {
                    string parentAssociation = GetAssociationKey(propertyList, listPointer - 1);
                    AssociationEntry parentEntry = ascs[parentAssociation]; // léteznie kell, máskülönben ide sem juthattam volna
                    e = new AssociationEntry(associationKey, string.Format("{0}.{1}", parentEntry.Alias, propertyList[listPointer]), string.Format("p{0}{1}", Stopwatch.GetTimestamp().ToString(), listPointer.ToString()));
                }
                if (!isThisId)
                {
                    // az id asszociációkat nem mentjük le
                    ascs[e.Key] = e;
                }

                if (listPointer == propertyList.Count - 1 || isThisId)
                {
                    if (mCriterion == null)
                    {
                        mCriterion = BuildCriterion((isThisId && listPointer < propertyList.Count - 1) ? string.Format("{0}.{1}", e.Association, propertyList[listPointer + 1]) : e.Association);
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
                else
                {
                    DetachedCriteria subCriteria = e.Criteria;
                    if (subCriteria == null)
                    {
                        subCriteria = criteria.CreateCriteria(e.Association, e.Alias, JoinType.InnerJoin);
                        e.Criteria = subCriteria;
                    }
                    listPointer++;
                    CreateNextPropertyLevel(subCriteria, propertyList, listPointer, dependencyCriterion);
                }
            }
        }

        private static string GetAssociationKey(List<string> propertyList, int toIndex)
        {
            StringBuilder sb = new StringBuilder();

            bool appendPoint = false;
            for (int i = 0; i <= toIndex; i++)
            {
                if (appendPoint)
                {
                    sb.Append(".");
                }
                else
                {
                    appendPoint = true;
                }
                sb.Append(propertyList[i]);
            }

            return sb.ToString();
        }

        #endregion

    }

}
