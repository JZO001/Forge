/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Shared;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace Forge.ORM.NHibernateExtension.Model
{

    /// <summary>
    /// Base entity for inherited entity types with identifier
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("[{GetType()}, Id = '{Id}', Deleted = {Deleted}]")]
    public abstract class EntityBaseGenericId<TID> : EntityBaseWithoutId where TID : IComparable
    {

        #region Field(s)

        private bool mHashcodeCreated = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mHashcode = 0;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityBaseGenericId{TID}"/> class.
        /// </summary>
        protected EntityBaseGenericId()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [DebuggerHidden]
        public abstract TID Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is saved.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is saved; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public override bool IsSaved
        {
            get { return Id != null && base.IsSaved; }
            set { base.IsSaved = value; }
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
            if (Id == null)
            {
                sb.Append("(unsaved)");
            }
            else
            {
                sb.Append(Id.ToString());
            }
            sb.Append(", Created: ");
            sb.Append(EntityCreationTime.ToString());
            sb.Append(", Modified: ");
            sb.Append(EntityModificationTime.ToString());
            sb.Append(", Deleted: ");
            sb.Append(Deleted.ToString());
            return sb.ToString();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override int GetHashCode()
        {
            if (!mHashcodeCreated)
            {
                if (Id == null)
                {
                    mHashcode = base.GetHashCode();
                }
                else
                {
                    mHashcode = 9 * Id.GetHashCode();
                    mHashcode = 7 ^ mHashcode + EntityCreationTime.GetHashCode();
                }
                mHashcodeCreated = true;
            }
            return mHashcode;
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

            EntityBaseGenericId<TID> other = (EntityBaseGenericId<TID>)obj;
            if (!base.Equals(obj))
            {
                return false;
            }
            if (Id == null || !Id.Equals(other.Id))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="o">The other.</param>
        /// <returns></returns>
        public override int CompareTo(EntityBaseWithoutId o)
        {
            if (o == null) ThrowHelper.ThrowArgumentNullException("o");
            if (!(o is EntityBaseGenericId<TID>))
            {
                ThrowHelper.ThrowArgumentException("o");
            }

            EntityBaseGenericId<TID> obj = (EntityBaseGenericId<TID>)o;
            int result = 0;
            if (Id == null)
            {
                result = EntityCreationTime.CompareTo(obj.EntityCreationTime);
            }
            else
            {
                result = Id.CompareTo(obj.Id);
            }
            return result; // default is equals. This provides the functionality as keep the original order of the treesets
        }

        #endregion

    }

}
