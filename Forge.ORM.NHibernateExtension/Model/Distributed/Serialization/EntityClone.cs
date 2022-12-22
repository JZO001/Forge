/* *********************************************************************
 * Date: 26 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Forge.ORM.NHibernateExtension.Model.Distributed.Serialization
{

    /// <summary>
    /// Represent a remote entity with type, id and field values. This class used by the infrastructure.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("[{GetType()}, Id = '{EntityId}', EntityType = '{EntityType}', Fields = {Fields.Count}]")]
    public class EntityClone : EntityProxy
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<string, object> mFields = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Version mEntityTypeVersion = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityClone" /> class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="entityId">The entity id.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="entityTypeVersion">The entity type version.</param>
        public EntityClone(Type entityType, EntityId entityId, Dictionary<string, object> fields, Version entityTypeVersion)
            : base(entityType, entityId)
        {
            if (fields == null)
            {
                ThrowHelper.ThrowArgumentNullException("fields");
            }
            if (entityTypeVersion == null)
            {
                ThrowHelper.ThrowArgumentNullException("entityTypeVersion");
            }

            mFields = fields;
            mEntityTypeVersion = entityTypeVersion;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the fields.
        /// </summary>
        [DebuggerHidden]
        public Dictionary<string, object> Fields
        {
            get { return mFields; }
        }

        /// <summary>
        /// Gets the entity type version.
        /// </summary>
        /// <value>
        /// The entity type version.
        /// </value>
        [DebuggerHidden]
        public Version EntityTypeVersion
        {
            get { return mEntityTypeVersion; }
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
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!obj.GetType().Equals(GetType())) return false;

            bool result = base.Equals(obj);
            if (result)
            {
                EntityClone other = (EntityClone)obj;
                if (mFields != other.mFields && (mFields == null || !mFields.Equals(other.mFields)))
                {
                    result = false;
                }
                else if (mEntityTypeVersion != other.mEntityTypeVersion && (mEntityTypeVersion == null || !mEntityTypeVersion.Equals(other.mEntityTypeVersion)))
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
            int hash = base.GetHashCode();
            hash = 29 * hash + (mFields != null ? mFields.GetHashCode() : 0);
            hash = 3 * hash + (mEntityTypeVersion != null ? mEntityTypeVersion.GetHashCode() : 0);
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
            return string.Format("{0}, Field size: {1}. Type version: {2}", base.ToString(), mFields.Count, mEntityTypeVersion == null ? "<null>" : mEntityTypeVersion.ToString());
        }

        #endregion

    }

}
