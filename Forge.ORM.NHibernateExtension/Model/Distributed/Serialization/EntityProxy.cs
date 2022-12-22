/* *********************************************************************
 * Date: 26 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Shared;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Forge.ORM.NHibernateExtension.Model.Distributed.Serialization
{

    /// <summary>
    /// Represent a remote entity reference with type and id. This class used by the infrastructure.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("[{GetType()}, Id = '{EntityId}', EntityType = '{EntityType}']")]
    public class EntityProxy : IComparable<EntityProxy>, IComparable, IEquatable<EntityProxy>
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Type mEntityType = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly EntityId mEntityId = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityProxy"/> class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="entityId">The entity id.</param>
        public EntityProxy(Type entityType, EntityId entityId)
        {
            if (entityType == null)
            {
                ThrowHelper.ThrowArgumentNullException("entityType");
            }
            if (entityId == null)
            {
                ThrowHelper.ThrowArgumentNullException("entityId");
            }
            mEntityId = entityId;
            mEntityType = entityType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityProxy"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public EntityProxy(EntityBase entity)
        {
            if (entity == null)
            {
                ThrowHelper.ThrowArgumentNullException("entity");
            }
            if (entity.Id == null)
            {
                ThrowHelper.ThrowArgumentException("Missing id from entity.", "entity");
            }
            mEntityId = (EntityId)entity.Id.Clone();
            mEntityType = entity.GetType();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        [DebuggerHidden]
        public Type EntityType
        {
            get { return mEntityType; }
        }

        /// <summary>
        /// Gets the entity id.
        /// </summary>
        [DebuggerHidden]
        public EntityId EntityId
        {
            get { return mEntityId; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public virtual int CompareTo(EntityProxy o)
        {
            if (o == null)
            {
                ThrowHelper.ThrowArgumentNullException("o");
            }
            int result = mEntityType.AssemblyQualifiedName.CompareTo(o.EntityType.AssemblyQualifiedName);
            if (result == 0)
            {
                result = mEntityId.CompareTo(o.EntityId);
            }
            return result;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="obj"/> is not the same type as this instance. </exception>
        public virtual int CompareTo(object obj)
        {
            if (obj == null)
            {
                ThrowHelper.ThrowArgumentNullException("obj");
            }
            if (!(obj is EntityProxy))
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(obj, typeof(EntityProxy));
            }
            return CompareTo(obj as EntityProxy);
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

            EntityProxy other = (EntityProxy)obj;
            return other.mEntityType.Equals(mEntityType) && other.mEntityId.Equals(mEntityId);
        }

        /// <summary>
        /// Equalses the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public virtual bool Equals(EntityProxy o)
        {
            return Equals((object)o);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int hash = 7;
            hash = 47 * hash + (mEntityType != null ? mEntityType.GetHashCode() : 0);
            hash = 47 * hash + (mEntityId != null ? mEntityId.GetHashCode() : 0);
            return hash;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}@{1}, {2} -> {3}", GetType().Name, GetHashCode().ToString(), mEntityType.FullName, mEntityId.ToString());
        }

        #endregion

    }

}
