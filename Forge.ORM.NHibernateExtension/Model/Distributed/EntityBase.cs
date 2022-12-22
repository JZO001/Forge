/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Forge.Logging.Abstraction;
using Forge.ORM.NHibernateExtension.Model.Distributed.Serialization;
using Forge.Reflection;
using Forge.Shared;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Mapping.Attributes;
using NHibernate.Proxy;

namespace Forge.ORM.NHibernateExtension.Model.Distributed
{

    /// <summary>
    /// Base entity for inherited entity types
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("[{GetType()}, Id = '{Id}', Version = '{Version}', Deleted = {Deleted}]")]
    public abstract class EntityBase : EntityBaseGenericId<EntityId>
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger<EntityBase>();

        [EntityFieldDescription("Represents the identifier of an entity")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [CompositeId(0, Name = "id", ClassType = typeof(EntityId))]
        [KeyProperty(1, Name = "systemId", Column = "systemId")]
        [KeyProperty(2, Name = "deviceId", Column = "deviceId")]
        [KeyProperty(3, Name = "id", Column = "id")]
        private EntityId id = null;

        [EntityFieldDescription("Represents the version of an entity")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [ComponentProperty]
        private EntityVersion version = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityBase"/> class.
        /// </summary>
        protected EntityBase()
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
        public override EntityId Id
        {
            get { return id; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }
                if (id != null && IsSaved)
                {
                    ThrowHelper.ThrowArgumentException("Unable to replace identifier of an existing entity.", "value");
                }

                OnPropertyChanging("Id");
                id = value;
                IsSaved = false; // ha beállítok id-t, az azt jelenti, hogy most lesz először mentve
                OnPropertyChanged("Id");
            }
        }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [DebuggerHidden]
        public virtual EntityVersion Version
        {
            get { return version; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }

                OnPropertyChanging("Version");
                version = value;
                IsSaved = false;
                OnPropertyChanged("Version");
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Creates the entity clone.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="EntityCloneException"></exception>
        public static EntityClone CreateEntityClone(EntityBase entity, IPersistenceContext context)
        {
            if (entity == null)
            {
                ThrowHelper.ThrowArgumentNullException("entity");
            }

            Dictionary<string, object> fields = new Dictionary<string, object>();
            try
            {
                Type entityType = entity.GetType();
                while (entityType != null)
                {
                    foreach (FieldInfo field in entityType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (!field.IsStatic && !field.IsLiteral && !field.IsInitOnly && !field.IsNotSerialized)
                        {
                            object fieldValue = field.GetValue(entity);
                            if (fieldValue == null)
                            {
                                fields[field.Name] = null;
                            }
                            else if (field.FieldType.FullName.StartsWith(typeof(System.Collections.Generic.ISet<>).FullName.Substring(0, typeof(System.Collections.Generic.ISet<>).FullName.IndexOf("`1") + 2)))
                            {
                                // generic HashSet (.NET 4)
                                // generic HashedSet (Iesi)
                                // non-generic HashedSet (Iesi)
                                HashSet<EntityProxy> newSet = new HashSet<EntityProxy>();
                                IEnumerator e = ((IEnumerable)fieldValue).GetEnumerator();
                                while (e.MoveNext())
                                {
                                    EntityBase item = (EntityBase)e.Current;
                                    CheckEntities(entity.GetType().FullName, field.Name, item);
                                    if (item is INHibernateProxy)
                                    {
                                        if (context == null)
                                        {
                                            throw new EntityCloneException(string.Format("Field '{0}' contains an NHibernate entity proxy type. Unsaved entity type: '{1}'. Parent entity type: '{2}'.", field.Name, fieldValue.GetType().Name, entity.GetType().Name));
                                        }
                                        item = (EntityBase)context.Unproxy(item);
                                    }
                                    newSet.Add(new EntityProxy(item));
                                }
                                fields[field.Name] = newSet;
                            }
                            else if ((field.FieldType.FullName.StartsWith(typeof(System.Collections.Generic.IList<>).FullName.Substring(0, typeof(System.Collections.Generic.IList<>).FullName.IndexOf("`1") + 2))) ||
                                (field.FieldType.Equals(typeof(System.Collections.IList))))
                            {
                                // generic List (.NET)
                                List<EntityProxy> newSet = new List<EntityProxy>();
                                IEnumerator e = ((IEnumerable)fieldValue).GetEnumerator();
                                while (e.MoveNext())
                                {
                                    EntityBase item = (EntityBase)e.Current;
                                    CheckEntities(entity.GetType().FullName, field.Name, item);
                                    if (item is INHibernateProxy)
                                    {
                                        if (context == null)
                                        {
                                            throw new EntityCloneException(string.Format("Field '{0}' contains an NHibernate entity proxy type. Unsaved entity type: '{1}'. Parent entity type: '{2}'.", field.Name, fieldValue.GetType().Name, entity.GetType().Name));
                                        }
                                        item = (EntityBase)context.Unproxy(item);
                                    }
                                    newSet.Add(new EntityProxy(item));
                                }
                                fields[field.Name] = newSet;
                            }
                            else if (fieldValue is EntityBase)
                            {
                                if (fieldValue != null && ((EntityBase)fieldValue).Id == null)
                                {
                                    throw new EntityCloneException(string.Format("Field '{0}' contains an unsaved entity. Unsaved entity type: '{1}'. Parent entity type: '{2}'.", field.Name, fieldValue.GetType().Name, entity.GetType().Name));
                                }
                                EntityBase item = (EntityBase)fieldValue;
                                if (item is INHibernateProxy)
                                {
                                    if (context == null)
                                    {
                                        throw new EntityCloneException(string.Format("Field '{0}' contains an NHibernate entity proxy type. Unsaved entity type: '{1}'. Parent entity type: '{2}'.", field.Name, fieldValue.GetType().Name, entity.GetType().Name));
                                    }
                                    item = (EntityBase)context.Unproxy(item);
                                }
                                fields[field.Name] = new EntityProxy(item);
                            }
                            else if (fieldValue is ICloneable)
                            {
                                fields[field.Name] = ((ICloneable)fieldValue).Clone();
                            }
                            else
                            {
                                fields[field.Name] = fieldValue;
                            }
                        }
                    }
                    entityType = entityType.BaseType;
                }
            }
            catch (EntityCloneException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EntityCloneException(string.Format("Unable to clone entity '{0}'.", entity.GetType().FullName), ex);
            }

            return new EntityClone(entity.GetType(), entity.Id, fields, entity.GetType().Assembly.GetName().Version);
        }

        /// <summary>
        /// Restores from entity clone.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="clone">The clone.</param>
        /// <param name="session">The session.</param>
        public static void RestoreFromEntityClone(EntityBase entity, EntityClone clone, ISession session)
        {
            RestoreFromEntityClone(entity, clone, true, null, session);
        }

        /// <summary>
        /// Restores from entity clone.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="clone">The clone.</param>
        /// <param name="throwsExceptionOnAnyFailure">if set to <c>true</c> [throws exception on any failure].</param>
        /// <param name="messageLog">The message log.</param>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        public static bool RestoreFromEntityClone(EntityBase entity, EntityClone clone, bool throwsExceptionOnAnyFailure, StringBuilder messageLog, ISession session)
        {
            if (entity == null)
            {
                ThrowHelper.ThrowArgumentNullException("entity");
            }
            if (clone == null)
            {
                ThrowHelper.ThrowArgumentNullException("clone");
            }
            if (session == null)
            {
                ThrowHelper.ThrowArgumentNullException("session");
            }

            if (messageLog == null)
            {
                messageLog = new StringBuilder();
            }

            bool result = true;

            foreach (KeyValuePair<string, object> entry in clone.Fields)
            {
                FieldInfo field = null;
                try
                {
                    field = TypeHelper.GetFieldByName(entity, entry.Key);

                    if (entry.Value == null)
                    {
                        field.SetValue(entity, null);
                    }
                    else if ((field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition().Equals(typeof(System.Collections.Generic.ISet<>))) ||
                        (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition().Equals(typeof(System.Collections.Generic.IList<>))) ||
                        (field.FieldType.Equals(typeof(System.Collections.IList))))
                    {
                        object enumerable = null;

                        if (field.FieldType.GetGenericTypeDefinition().Equals(typeof(System.Collections.Generic.ISet<>)))
                        {
                            // generic HashSet (.NET 4)
                            Type genericType = field.FieldType.GetGenericArguments()[0];
                            Type hashSetType = typeof(System.Collections.Generic.HashSet<>).MakeGenericType(genericType);
                            enumerable = hashSetType.GetConstructor(new Type[] { }).Invoke(null);
                        }
                        else if (field.FieldType.GetGenericTypeDefinition().Equals(typeof(System.Collections.Generic.IList<>)))
                        {
                            // generic List (.NET)
                            Type genericType = field.FieldType.GetGenericArguments()[0];
                            Type hashSetType = typeof(System.Collections.Generic.List<>).MakeGenericType(genericType);
                            enumerable = hashSetType.GetConstructor(new Type[] { }).Invoke(null);
                        }
                        else if (field.FieldType.Equals(typeof(System.Collections.IList)))
                        {
                            // non-generic list (.NET)
                            enumerable = new System.Collections.ArrayList();
                        }

                        MethodInfo mi = enumerable.GetType().GetMethod("Add");
                        IEnumerable proxySet = (IEnumerable)entry.Value;
                        foreach (EntityProxy item in proxySet)
                        {
                            EntityBase setItem = null;
                            try
                            {
                                setItem = (EntityBase)session.Get(item.EntityType, item.EntityId);
                            }
                            catch (Exception)
                            {
                            }
                            if (setItem == null)
                            {
                                string message = string.Format("Unable to find entity (Type: '{0}', Id: '{1}') which is an element of a Set. Parent entity type: '{2}', Field name of the set: '{3}'", item.EntityType.FullName, item.EntityId, entity.GetType().FullName, field.Name);
                                if (throwsExceptionOnAnyFailure)
                                {
                                    throw new EntityRestoreException(message);
                                }
                                else
                                {
                                    messageLog.Append(message);
                                    messageLog.Append(Environment.NewLine);
                                    result = false;
                                }
                            }
                            else
                            {
                                mi.Invoke(enumerable, new object[] { setItem });
                            }
                        }
                        field.SetValue(entity, enumerable);
                    }
                    else if (entry.Value is EntityProxy)
                    {
                        EntityProxy item = (EntityProxy)entry.Value;
                        EntityBase associatedEntity = null;
                        try
                        {
                            associatedEntity = (EntityBase)session.Get(item.EntityType, item.EntityId);
                        }
                        catch (Exception)
                        {
                        }
                        if (associatedEntity == null)
                        {
                            string message = string.Format("Unable to find an entity (Type: '{0}', Id: '{1}') which associated to its parent entity (Type: '{2}', Id: '{3}').", item.EntityType.FullName, item.EntityId, entity.GetType().FullName, entity.Id);
                            if (throwsExceptionOnAnyFailure)
                            {
                                throw new EntityRestoreException(message);
                            }
                            else
                            {
                                messageLog.Append(message);
                                messageLog.Append(Environment.NewLine);
                                result = false;
                            }
                        }
                        field.SetValue(entity, associatedEntity);
                    }
                    else if (entry.Value is ICloneable)
                    {
                        field.SetValue(entity, ((ICloneable)entry.Value).Clone());
                    }
                    else
                    {
                        field.SetValue(entity, entry.Value);
                    }
                }
                catch (MissingFieldException ex)
                {
                    // ez akkor történik, ha az eltérő entity verziók miatt nem található meg egyik másik field
                    // a rendszer tolerálja a hiányzó fieldeket, ha régi entitásból akarunk újabba menteni, de a megváltoztatott field típust nem
                    Version typeVersion = entity.GetType().Assembly.GetName().Version;
                    if (clone.EntityTypeVersion >= entity.GetType().Assembly.GetName().Version)
                    {
                        // azonos vagy újabb verzió
                        throw new EntityRestoreException(string.Format("Failed to restore entity. Entity restoration data belongs to version '{0}' which is newer than the current type version '{1}'. {2}", clone.EntityTypeVersion == null ? "<unknown>" : clone.EntityTypeVersion.ToString(), typeVersion.ToString(), clone.ToString()), ex);
                    }
                    else
                    {
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("Entity cannot be restored fully, field '{0}' not found. Entity restoration data belongs to version '{1}' which is newer than the current type version '{2}'.", entry.Key, clone.EntityTypeVersion == null ? "<unknown>" : clone.EntityTypeVersion.ToString(), typeVersion.ToString(), clone.ToString()));
                    }
                }
                catch (EntityRestoreException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new EntityRestoreException(string.Format("Failed to restore entity. {0}", clone.ToString()), ex);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(GetType().Name);
            //sb.Append("@");
            //sb.Append(GetHashCode().ToString());
            sb.Append(", Id: ");
            if (id == null)
            {
                sb.Append("(unsaved)");
            }
            else
            {
                sb.Append(id.ToString());
            }
            if (version != null)
            {
                sb.Append(", Version: ");
                sb.Append(version.ToString());
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
        /// Compares to.
        /// </summary>
        /// <param name="o">The other.</param>
        /// <returns></returns>
        public override int CompareTo(EntityBaseWithoutId o)
        {
            if (o == null) ThrowHelper.ThrowArgumentNullException("o");
            if (!(o is EntityBase))
            {
                ThrowHelper.ThrowArgumentException("o");
            }

            EntityBase obj = (EntityBase)o;
            int result = 0;
            if (id == null)
            {
                result = EntityCreationTime.CompareTo(obj.EntityCreationTime);
            }
            else
            {
                if (!id.SystemId.Equals(obj.Id.SystemId))
                {
                    result = id.SystemId.CompareTo(obj.Id.SystemId);
                }
                else if (!id.Id.Equals(obj.Id.Id))
                {
                    result = id.Id.CompareTo(obj.Id.Id);
                }
                else
                {
                    result = id.DeviceId.CompareTo(obj.Id.DeviceId);
                }
            }
            return result; // default is equals. This provides the functionality as keep the original order of the treesets
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
        public override int CompareTo(object obj)
        {
            if (obj == null)
            {
                ThrowHelper.ThrowArgumentNullException("obj");
            }
            if (!(obj is EntityBase))
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(obj, typeof(EntityBase));
            }
            return CompareTo((EntityBase)obj);
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Clone the entire entity automatically
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="newEntity">The new entity.</param>
        protected virtual void InternalClone(Type type, EntityBase newEntity)
        {
            if (!type.Equals(typeof(EntityBase)))
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
                        //else if (field.FieldType.Equals(typeof(System.Collections.IList)))
                        //{
                        //    // non-generic list (.NET)
                        //    field.SetValue(newEntity, new System.Collections.ArrayList((System.Collections.ICollection)fieldValue));
                        //}
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

        #region Private method(s)

        private static void CheckEntities(string parentEntityName, string fieldName, EntityBase item)
        {
            if (item.Id == null)
            {
                throw new EntityCloneException(string.Format("Unsaved entity found in a set or list. Field name of the set '{0}'. Unsaved entity type: '{1}'. Parent entity type: '{2}'.", fieldName, item.GetType().FullName, parentEntityName));
            }
        }

        #endregion

    }

}
