/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Forge.Invoker;
using Forge.Shared;
using NHibernate.Mapping.Attributes;

namespace Forge.ORM.NHibernateExtension.Model.Distributed
{

    /// <summary>
    /// Version tracker for entities
    /// </summary>
    [Serializable]
    [Component]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("[{GetType()}, VersionDeviceId = {DeviceId}, VersionSeqNumber = {SeqNumber}]")]
    public class EntityVersion : ICloneable, IComparable<EntityVersion>, IEquatable<EntityVersion>, IComparable, INotifyPropertyChanging, INotifyPropertyChanged
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Property(Column = "versionDeviceId", NotNull = true)]
        private long versionDeviceId;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Property(Column = "versionSeqNumber", NotNull = true)]
        private long versionSeqNumber = 1;

        [NonSerialized]
        private PropertyChangingEventHandler propertyChangingDelegate;

        [NonSerialized]
        private PropertyChangedEventHandler propertyChangedDelegate;

        /// <summary>
        /// Occurs when a property value is changing.
        /// </summary>
        public virtual event PropertyChangingEventHandler PropertyChanging
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                propertyChangingDelegate = (PropertyChangingEventHandler)Delegate.Combine(propertyChangingDelegate, value);
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                propertyChangingDelegate = (PropertyChangingEventHandler)Delegate.Remove(propertyChangingDelegate, value);
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                propertyChangedDelegate = (PropertyChangedEventHandler)Delegate.Combine(propertyChangedDelegate, value);
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                propertyChangedDelegate = (PropertyChangedEventHandler)Delegate.Remove(propertyChangedDelegate, value);
            }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityVersion"/> class.
        /// </summary>
        protected EntityVersion()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityVersion"/> class.
        /// </summary>
        /// <param name="deviceId">The device id.</param>
        public EntityVersion(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                ThrowHelper.ThrowArgumentNullException("deviceId");
            }
            versionDeviceId = HashGeneratorHelper.GetSHA256BasedValue(deviceId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityVersion"/> class.
        /// </summary>
        /// <param name="deviceId">The device id.</param>
        public EntityVersion(long deviceId)
        {
            versionDeviceId = deviceId;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the device id.
        /// </summary>
        /// <value>
        /// The device id.
        /// </value>
        [DebuggerHidden]
        public virtual long DeviceId
        {
            get { return versionDeviceId; }
            set
            {
                OnPropertyChanging("DeviceId");
                versionDeviceId = value;
                OnPropertyChanged("DeviceId");
            }
        }

        /// <summary>
        /// Gets or sets the seq number.
        /// </summary>
        /// <value>
        /// The seq number.
        /// </value>
        [DebuggerHidden]
        public virtual long SeqNumber
        {
            get { return versionSeqNumber; }
            set
            {
                OnPropertyChanging("SeqNumber");
                versionSeqNumber = value;
                OnPropertyChanged("SeqNumber");
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Increase the seq number.
        /// </summary>
        public void IncSeqNumber()
        {
            OnPropertyChanging("SeqNumber");
            if (versionSeqNumber == Int64.MaxValue)
            {
                versionSeqNumber = 0;
            }
            versionSeqNumber++;
            OnPropertyChanged("SeqNumber");
        }

        /// <summary>
        /// Decs the seq number.
        /// </summary>
        public void DecSeqNumber()
        {
            OnPropertyChanging("SeqNumber");
            if (versionSeqNumber == 0)
            {
                versionSeqNumber = long.MaxValue;
            }
            versionSeqNumber--;
            OnPropertyChanged("SeqNumber");
        }

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public virtual int CompareTo(EntityVersion other)
        {
            if (other == null)
            {
                ThrowHelper.ThrowArgumentNullException("other");
            }

            int ret = versionDeviceId.CompareTo(other.versionDeviceId);
            if (ret == 0)
            {
                ret = versionSeqNumber.CompareTo(other.versionSeqNumber);
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
            if (!(obj is EntityVersion))
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(obj, typeof(EntityVersion));
            }
            return CompareTo((EntityVersion)obj);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            EntityVersion cloned = (EntityVersion)GetType().GetConstructor(BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, new Type[] { }, null).Invoke(new object[] { });
            cloned.versionDeviceId = versionDeviceId;
            cloned.versionSeqNumber = versionSeqNumber;
            return cloned;
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

            EntityVersion other = (EntityVersion)obj;
            return other.versionDeviceId == versionDeviceId && other.versionSeqNumber == versionSeqNumber;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public virtual bool Equals(EntityVersion other)
        {
            return Equals((object)other);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}", versionDeviceId, versionSeqNumber);
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Called when [property changing].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanging(string propertyName)
        {
            Executor.Invoke(propertyChangingDelegate, this, new PropertyChangingEventArgs(propertyName));
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            Executor.Invoke(propertyChangedDelegate, this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }

}
