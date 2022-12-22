/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Forge.Shared;
using NHibernate.Mapping.Attributes;

namespace Forge.ORM.NHibernateExtension.Model.Distributed
{

    /// <summary>
    /// Composite identifier for entities
    /// </summary>
    [Serializable]
    [Component]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("[{GetType()}, SystemId = {SystemId}, DeviceId = {DeviceId}, Id = {Id}]")]
    public class EntityId : ICloneable, IComparable<EntityId>, IComparable, IEquatable<EntityId>
    {

        #region Field(s)

        /// <summary>
        /// Separator for ToString() method
        /// </summary>
        public static readonly char ID_SEPARATOR = ':';

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Property(NotNull = true)]
        private long systemId = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Property(NotNull = true)]
        private long deviceId = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Property(NotNull = true)]
        private long id = 0;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityId"/> class.
        /// </summary>
        protected EntityId()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityId"/> class.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        public EntityId(string entityId)
        {
            if (string.IsNullOrEmpty(entityId))
            {
                ThrowHelper.ThrowArgumentNullException("entityId");
            }
            string[] elements = entityId.Split(ID_SEPARATOR);
            if (elements.Length != 3 || elements[0].Length == 0 || elements[1].Length == 0 || elements[2].Length == 0)
            {
                throw new ArgumentException("Invalid entityId argument provided.");
            }
            systemId = long.Parse(elements[0]);
            deviceId = long.Parse(elements[1]);
            id = long.Parse(elements[2]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityId"/> class.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        public EntityId(EntityId entityId)
        {
            if (entityId == null)
            {
                ThrowHelper.ThrowArgumentNullException("entityId");
            }
            systemId = entityId.systemId;
            deviceId = entityId.deviceId;
            id = entityId.id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityId"/> class.
        /// </summary>
        /// <param name="systemId">The system id.</param>
        /// <param name="deviceId">The device id.</param>
        /// <param name="id">The id.</param>
        public EntityId(long systemId, string deviceId, long id)
            : this(systemId, HashGeneratorHelper.GetSHA256BasedValue(deviceId), id)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityId"/> class.
        /// </summary>
        /// <param name="systemId">The system id.</param>
        /// <param name="deviceId">The device id.</param>
        /// <param name="id">The id.</param>
        public EntityId(long systemId, long deviceId, long id)
        {
            this.systemId = systemId;
            this.deviceId = deviceId;
            this.id = id;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the system id.
        /// </summary>
        /// <value>
        /// The system id.
        /// </value>
        [DebuggerHidden]
        public virtual long SystemId
        {
            get { return systemId; }
            //set { systemId = value; }
        }

        /// <summary>
        /// Gets the device id.
        /// </summary>
        /// <value>
        /// The device id.
        /// </value>
        [DebuggerHidden]
        public virtual long DeviceId
        {
            get { return deviceId; }
            //set { deviceId = value; }
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [DebuggerHidden]
        public virtual long Id
        {
            get { return id; }
            //set { id = value; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Parses the specified entity id.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <returns>The parsed EntityId instance.</returns>
        public static EntityId Parse(string entityId)
        {
            return new EntityId(entityId);
        }

        /// <summary>
        /// Try to parse the provided value.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="id">The id.</param>
        /// <returns>True, if the parse was successful, otherwise False.</returns>
        public static bool TryParse(string entityId, out EntityId id)
        {
            bool result = true;
            id = null;

            try
            {
                id = new EntityId(entityId);
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public virtual int CompareTo(EntityId other)
        {
            if (other == null)
            {
                ThrowHelper.ThrowArgumentNullException("other");
            }
            int ret = systemId.CompareTo(other.systemId);
            if (ret == 0)
            {
                ret = deviceId.CompareTo(other.deviceId);
            }
            if (ret == 0)
            {
                ret = id.CompareTo(other.id);
            }
            return ret;
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
            if (!(obj is EntityId))
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(obj, typeof(EntityId));
            }
            return CompareTo((EntityId)obj);
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

            EntityId other = (EntityId)obj;
            return other.SystemId == SystemId && other.DeviceId == DeviceId && other.Id == Id;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public virtual bool Equals(EntityId other)
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
            int hash = 3;
            hash = 43 * hash + SystemId.GetHashCode();
            hash = 43 ^ hash + DeviceId.GetHashCode();
            hash = 43 ^ hash + Id.GetHashCode();
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
            return string.Format("{0}{1}{2}{3}{4}", SystemId, ID_SEPARATOR, DeviceId, ID_SEPARATOR, Id);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            return GetType().GetConstructor(new Type[] { typeof(long), typeof(long), typeof(long) }).Invoke(new object[] { systemId, deviceId, id });
        }

        #endregion

    }

}
