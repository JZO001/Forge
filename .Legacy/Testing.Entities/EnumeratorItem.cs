using System;
using System.Diagnostics;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities
{

    /// <summary>
    /// Represents the item of an enumerator type
    /// </summary>
    [Serializable]
    [Class(Table = "EnumeratorItems")]
    public class EnumeratorItem : EntityBase
    {

        #region Field(s)

        /// <summary>
        /// Represents the type of this enumerator item
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [ManyToOne(0, Name = "enumeratorType", Cascade = "none")]
        [Column(1, Name = "enumType_restId")]
        [Column(2, Name = "enumType_deviceId")]
        [Column(3, Name = "enumType_id")]
        private EnumeratorType enumeratorType = null;

        /// <summary>
        /// Represents the name of the enumerator item
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Property(NotNull = true)]
        private string name = string.Empty;

        /// <summary>
        /// Represents the value of the enumerator item
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Property(NotNull = true)]
        private long value = 0;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumeratorItem"/> class.
        /// </summary>
        public EnumeratorItem()
            : base()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the type of the enumerator.
        /// </summary>
        /// <value>
        /// The type of the enumerator.
        /// </value>
        [DebuggerHidden]
        public virtual EnumeratorType EnumeratorType
        {
            get { return enumeratorType; }
            set
            {
                OnPropertyChanging("enumeratorType");
                enumeratorType = value;
                OnPropertyChanged("enumeratorType");
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DebuggerHidden]
        public virtual string Name
        {
            get { return name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }

                OnPropertyChanging("name");
                name = value;
                OnPropertyChanged("name");
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [DebuggerHidden]
        public virtual long Value
        {
            get { return this.value; }
            set
            {
                if (value < 0)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }

                OnPropertyChanging("value");
                this.value = value;
                OnPropertyChanged("value");
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
                EnumeratorItem other = (EnumeratorItem)obj;
                if (this.Name != other.Name)
                {
                    result = false;
                }
                else if (this.Value != other.Value)
                {
                    result = false;
                }
                else if (this.EnumeratorType != other.EnumeratorType && (this.EnumeratorType == null || !this.EnumeratorType.Equals(other.EnumeratorType)))
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
