using System;
using System.Collections.Generic;
using System.Diagnostics;
using Forge.Testing.Entities.Enums;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities
{

    /// <summary>
    /// Represents an enumerator type
    /// </summary>
    [Serializable]
    [Class(Table = "EnumeratorTypes")]
    public class EnumeratorType : EntityBase
    {

        #region Field(s)

        /// <summary>
        /// Represents the type of the enumerator
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Property(NotNull = true)]
        private SystemEnumeratorTypeEnum systemEnumeratorType = SystemEnumeratorTypeEnum.NotDefinied;

        /// <summary>
        /// Represents that the flag attribute is presents on this type of enumerator
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Property(NotNull = true)]
        private bool flags = false;

        /// <summary>
        /// Represents the items of the enumerator type
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Set(0, Name = "items", Generic = true, Lazy = CollectionLazy.True, Cascade = "none")]
        [Key(1)]
        [Column(2, Name = "enumType_restId")]
        [Column(3, Name = "enumType_deviceId")]
        [Column(4, Name = "enumType_id")]
        [OneToMany(5, NotFound = NotFoundMode.Exception, ClassType = typeof(EnumeratorItem))]
        private ISet<EnumeratorItem> items = new HashSet<EnumeratorItem>();

        [ManyToOne(0, Name = "guest", ClassType = typeof(Guest), Cascade = "none")]
        [Column(1, Name = "guest_restId")]
        [Column(2, Name = "guest_deviceId")]
        [Column(3, Name = "guest_id")]
        private Guest guest = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumeratorType"/> class.
        /// </summary>
        public EnumeratorType()
            : base()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the type of the system enumerator.
        /// </summary>
        /// <value>
        /// The type of the system enumerator.
        /// </value>
        [DebuggerHidden]
        public virtual SystemEnumeratorTypeEnum SystemEnumeratorType
        {
            get { return systemEnumeratorType; }
            set
            {
                OnPropertyChanging("systemEnumeratorType");
                systemEnumeratorType = value;
                OnPropertyChanged("systemEnumeratorType");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="EnumeratorType"/> is flags.
        /// </summary>
        /// <value>
        ///   <c>true</c> if flags; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public virtual bool Flags
        {
            get { return flags; }
            set
            {
                OnPropertyChanging("flags");
                flags = value;
                OnPropertyChanged("flags");
            }
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [DebuggerHidden]
        public virtual ISet<EnumeratorItem> Items
        {
            get { return new HashSet<EnumeratorItem>(items); }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }

                foreach (EnumeratorItem item in value)
                {
                    if (!item.EnumeratorType.Equals(this))
                    {
                        throw new InvalidOperationException("One of the provided item in the collection belongs to an other type of enumerator.");
                    }
                }

                OnPropertyChanging("items");
                items = new HashSet<EnumeratorItem>(value);
                OnPropertyChanged("items");
            }
        }

        /// <summary>
        /// Gets or sets the guest.
        /// </summary>
        /// <value>
        /// The guest.
        /// </value>
        [DebuggerHidden]
        public virtual Guest Guest
        {
            get { return guest; }
            set
            {
                OnPropertyChanging("guest");
                guest = value;
                OnPropertyChanged("guest");
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            bool result = base.Equals(obj);

            if (result)
            {
                EnumeratorType other = (EnumeratorType)obj;
                if (this.SystemEnumeratorType != other.SystemEnumeratorType)
                {
                    result = false;
                }
                else if (this.Flags != other.Flags)
                {
                    result = false;
                }
            }

            return result;
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

        #endregion

    }

}
