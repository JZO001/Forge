/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Forge.Invoker;
using Forge.Shared;
using NHibernate.Mapping.Attributes;

namespace Forge.ORM.NHibernateExtension.Model
{

    /// <summary>
    /// Base entity for inherited entity types
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("[{GetType()}, Deleted = {Deleted}]")]
    public abstract class EntityBaseWithoutId : ICloneable, IComparable<EntityBaseWithoutId>, IComparable, INotifyPropertyChanging, INotifyPropertyChanged, IEquatable<EntityBaseWithoutId>
    {

        #region Field(s)

        //private static readonly ILog LOGGER = LogManager.GetLogger<EntityBaseWithoutId>();

        [EntityFieldDescription("Represents the creation time (UTC) of the entity")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Property(NotNull = true)]
        private DateTime entityCreationTime = DateTime.UtcNow;

        [EntityFieldDescription("Represents the last modification time (UTC) of the entity")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Property(NotNull = true)]
        private DateTime entityModificationTime = DateTime.UtcNow;

        [EntityFieldDescription("Represents the entity is logically deleted or not")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Property(Column = "isDeleted", NotNull = true)]
        private bool deleted = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isSaved = true;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isChanged = true;

        [NonSerialized]
        private PropertyChangingEventHandler propertyChangingDelegate;

        [NonSerialized]
        private PropertyChangedEventHandler propertyChangedDelegate;

        /// <summary>
        /// Occurs when a property value is changing.
        /// </summary>
        public virtual event PropertyChangingEventHandler PropertyChanging
        {
            add
            {
                lock (typeof(EntityBaseWithoutId))
                {
                    propertyChangingDelegate = (PropertyChangingEventHandler)Delegate.Combine(propertyChangingDelegate, value);
                }
            }
            remove
            {
                lock (typeof(EntityBaseWithoutId))
                {
                    propertyChangingDelegate = (PropertyChangingEventHandler)Delegate.Remove(propertyChangingDelegate, value);
                }
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (typeof(EntityBaseWithoutId))
                {
                    propertyChangedDelegate = (PropertyChangedEventHandler)Delegate.Combine(propertyChangedDelegate, value);
                }
            }
            remove
            {
                lock (typeof(EntityBaseWithoutId))
                {
                    propertyChangedDelegate = (PropertyChangedEventHandler)Delegate.Remove(propertyChangedDelegate, value);
                }
            }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityBaseWithoutId"/> class.
        /// </summary>
        protected EntityBaseWithoutId()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets a value indicating whether [deleted].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [deleted]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public virtual bool Deleted
        {
            get { return deleted; }
            set
            {
                OnPropertyChanging("Deleted");
                deleted = value;
                OnPropertyChanged("Deleted");
            }
        }

        /// <summary>
        /// Gets the entity creation time.
        /// </summary>
        [DebuggerHidden]
        public virtual DateTime EntityCreationTime
        {
            get { return entityCreationTime; }
            //set { entityCreationTime = value; }
        }

        /// <summary>
        /// Gets or sets the entity modification time.
        /// </summary>
        /// <value>
        /// The entity modification time.
        /// </value>
        [DebuggerHidden]
        public virtual DateTime EntityModificationTime
        {
            get { return entityModificationTime; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }
                OnPropertyChanging("EntityModificationTime");
                entityModificationTime = value;
                OnPropertyChanged("EntityModificationTime");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is saved.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is saved; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public virtual bool IsSaved
        {
            get { return isSaved; }
            set { isSaved = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the content of this instance is changed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is changed; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public virtual bool IsChanged
        {
            get { return isChanged; }
            set { isChanged = value; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(GetType().Name);
            sb.Append(", Created: ");
            sb.Append(entityCreationTime.ToString());
            sb.Append(", Modified: ");
            sb.Append(entityModificationTime.ToString());
            sb.Append(", Deleted: ");
            sb.Append(deleted.ToString());
            return sb.ToString();
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

            EntityBaseWithoutId other = (EntityBaseWithoutId)obj;
            if (entityCreationTime != other.entityCreationTime && (entityCreationTime == null || !entityCreationTime.Equals(other.entityCreationTime)))
            {
                return false;
            }
            if (entityModificationTime != other.entityModificationTime && (entityModificationTime == null || !entityModificationTime.Equals(other.entityModificationTime)))
            {
                return false;
            }
            if (deleted != other.deleted)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public virtual bool Equals(EntityBaseWithoutId other)
        {
            return Equals((object)other);
        }

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="o">The other.</param>
        /// <returns></returns>
        public virtual int CompareTo(EntityBaseWithoutId o)
        {
            return EntityCreationTime.CompareTo(o.EntityCreationTime);
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
            if (!(obj is EntityBaseWithoutId))
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(obj, typeof(EntityBaseWithoutId));
            }
            return CompareTo((EntityBaseWithoutId)obj);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            EntityBaseWithoutId cloned = (EntityBaseWithoutId)GetType().GetConstructor(Type.EmptyTypes).Invoke(null);

            // I do not clone id and version. This is harm the rules of cloning, but I need to protected the system.
            // if you want to create a perfect clone, set the id and version values as well
            //        result.id = (EntityId) this.id.clone();
            //        result.version = (EntityVersion) this.version.clone();
            cloned.entityCreationTime = entityCreationTime;
            cloned.entityModificationTime = entityModificationTime;
            cloned.deleted = deleted;
            cloned.isSaved = isSaved;

            InternalClone(GetType(), cloned);

            return cloned;
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Called when [property changing].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanging(string propertyName)
        {
            Executor.Invoke(propertyChangingDelegate, this, new PropertyChangingEventArgs(propertyName));
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            isChanged = true;
            Executor.Invoke(propertyChangedDelegate, this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Clone the entire entity automatically
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="newEntity">The new entity.</param>
        protected virtual void InternalClone(Type type, EntityBaseWithoutId newEntity)
        {
            if (!type.Equals(typeof(EntityBaseWithoutId)))
            {
                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (!field.IsStatic && !field.IsLiteral && !field.IsInitOnly)
                    {
                        object fieldValue = field.GetValue(this);
                        if (fieldValue == null)
                        {
                            // simple null value
                            field.SetValue(newEntity, null);
                        }
                        else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition().Equals(typeof(System.Collections.Generic.ISet<>)))
                        {
                            // generic HashSet (.NET 4)
                            Type genericType = field.FieldType.GetGenericArguments()[0];
                            Type hashSetType = typeof(System.Collections.Generic.HashSet<>).MakeGenericType(genericType);
                            object hashSet = hashSetType.GetConstructor(Type.EmptyTypes).Invoke(null);
                            hashSet.GetType().GetMethod("UnionWith").Invoke(hashSet, new object[] { fieldValue });
                            field.SetValue(newEntity, hashSet);
                        }
                        //else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition().Equals(typeof(Iesi.Collections.Generic.ISet<int>)))
                        //{
                        //    // generic HashedSet (Iesi)
                        //    Type genericType = field.FieldType.GetGenericArguments()[0];
                        //    Type hashSetType = typeof(Iesi.Collections.Generic.HashedSet<>).MakeGenericType(genericType);
                        //    object hashset = hashSetType.GetConstructor(Type.EmptyTypes).Invoke(null);
                        //    hashset.GetType().GetMethod("AddAll").Invoke(hashset, new object[] { fieldValue });
                        //    field.SetValue(newEntity, hashset);
                        //}
                        else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition().Equals(typeof(System.Collections.Generic.IList<int>)))
                        {
                            // generic List (.NET)
                            Type genericType = field.FieldType.GetGenericArguments()[0];
                            Type hashSetType = typeof(System.Collections.Generic.List<>).MakeGenericType(genericType);
                            object hashset = hashSetType.GetConstructor(Type.EmptyTypes).Invoke(null);
                            hashset.GetType().GetMethod("AddRange").Invoke(hashset, new object[] { fieldValue });
                            field.SetValue(newEntity, hashset);
                        }
                        else if (field.FieldType.Equals(typeof(System.Collections.IList)))
                        {
                            // non-generic list (.NET)
                            field.SetValue(newEntity, new System.Collections.ArrayList((System.Collections.ICollection)fieldValue));
                        }
                        //else if (field.FieldType.Equals(typeof(Iesi.Collections.ISet)))
                        //{
                        //    // non-generic HashedSet (Iesi)
                        //    field.SetValue(newEntity, new Iesi.Collections.HashedSet((System.Collections.ICollection)fieldValue));
                        //}
                        else
                        {
                            // value type or entity
                            field.SetValue(newEntity, fieldValue);
                        }
                    }
                }
                InternalClone(type.BaseType, newEntity);
            }
        }

        #endregion

    }

}
