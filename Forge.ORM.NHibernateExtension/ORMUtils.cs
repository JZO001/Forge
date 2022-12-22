/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Logging.Abstraction;
using Forge.ORM.NHibernateExtension.Model;
using Forge.Shared;
using NHibernate;
using NHibernate.Proxy;

namespace Forge.ORM.NHibernateExtension
{

    /// <summary>
    /// Util methods for ORM
    /// </summary>
    public static class ORMUtils
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(ORMUtils));

        #endregion

        #region Public method(s)

        /// <summary>
        /// Saves the entity into the provided session.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="session">The session.</param>
        /// <example>
        ///   <code>
        /// Product p = new Product();
        /// p.Id = new EntityId(1, 1, DateTime.Now.Ticks);
        /// p.Version = new EntityVersion(1);
        /// p.Category = "category_" + Guid.NewGuid().ToString();
        /// p.Name = "name_" + Guid.NewGuid().ToString();
        /// p.Price = new decimal(102.23);
        /// p.Deleted = true;
        /// EFUtils.SaveEntity(p, session);
        ///   </code>
        ///   </example>
        /// <exception cref="EntitySaveException">Occurs on any fail while the entity saving</exception>
        public static void SaveEntity(EntityBaseWithoutId entity, ISession session)
        {
            if (entity == null)
            {
                ThrowHelper.ThrowArgumentNullException("entity");
            }
            if (session == null)
            {
                ThrowHelper.ThrowArgumentNullException("session");
            }

            bool previousSavedState = entity.IsSaved;
            bool previousChangedState = entity.IsChanged;
            DateTime previousModificationTime = entity.EntityModificationTime;
            try
            {
                entity.EntityModificationTime = DateTime.UtcNow;
                if (entity.IsSaved)
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("ENTITY_FRAMEWORK, saving existing entity. Type: '{0}'", entity.GetType().FullName));
                    session.Update(entity);
                }
                else
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("ENTITY_FRAMEWORK, saving new entity. Type: '{0}'", entity.GetType().FullName));
                    session.Save(entity);
                    entity.IsSaved = true;
                }
                entity.IsChanged = false;
            }
            catch (Exception ex)
            {
                entity.IsSaved = previousSavedState;
                entity.IsChanged = previousChangedState;
                entity.EntityModificationTime = previousModificationTime;
                throw new EntitySaveException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Refreshes the entity content from the provided session.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="session">The session.</param>
        /// <exception cref="EntityRefreshException">Occurs on any fail while the entity content refreshing</exception>
        public static void RefreshEntity(EntityBaseWithoutId entity, ISession session)
        {
            if (entity == null)
            {
                ThrowHelper.ThrowArgumentNullException("entity");
            }
            if (session == null)
            {
                ThrowHelper.ThrowArgumentNullException("session");
            }

            try
            {
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("ENTITY_FRAMEWORK, refreshing entity content. Type: '{0}'", entity.GetType().FullName));
                session.Refresh(entity);
                entity.IsChanged = false;
            }
            catch (Exception ex)
            {
                throw new EntityRefreshException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Deletes the entity in the provided session.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="session">The session.</param>
        /// <example>
        /// <code>
        /// QueryParams&lt;PersistentStorageItem&gt; query = new QueryParams&lt;PersistentStorageItem&gt;(
        ///         new GroupCriteria(
        ///         new ArithmeticCriteria("id.systemId", entityId.SystemId),
        ///         new ArithmeticCriteria("id.deviceId", entityId.DeviceId),
        ///         new ArithmeticCriteria("id.id", entityId.Id)), 1);
        /// IList&lt;PersistentStorageItem&gt; resultList = QueryHelper.Query&lt;PersistentStorageItem&gt;(session, query, LOG_QUERY);
        /// PersistentStorageItem item = null;
        /// if (resultList.Count == 0)
        /// {
        ///     throw new Exception(String.Format("Unable to remove object. Id: {0}", entityId));
        /// }
        /// else
        /// {
        ///     item = resultList[0];
        ///     EFUtils.DeleteEntity(item, session);
        /// }
        /// </code>
        /// </example>
        /// <exception cref="EntityDeleteException">Occurs on any fail while the entity deleting</exception>
        public static void DeleteEntity(EntityBaseWithoutId entity, ISession session)
        {
            if (entity == null)
            {
                ThrowHelper.ThrowArgumentNullException("entity");
            }
            if (session == null)
            {
                ThrowHelper.ThrowArgumentNullException("session");
            }

            try
            {
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("ENTITY_FRAMEWORK, delete entity. Type: '{0}'", entity.GetType().FullName));
                session.Delete(entity);
            }
            catch (Exception ex)
            {
                throw new EntityDeleteException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Unproxies the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="session">The session.</param>
        /// <returns>TEntity</returns>
        public static TEntity UnproxyEntity<TEntity>(TEntity entity, ISession session) where TEntity : EntityBaseWithoutId
        {
            if (entity == null)
            {
                ThrowHelper.ThrowArgumentNullException("entity");
            }
            if (session == null)
            {
                ThrowHelper.ThrowArgumentNullException("session");
            }

            if (entity is INHibernateProxy)
            {
                return (TEntity)session.GetSessionImplementation().PersistenceContext.UnproxyAndReassociate(entity);
            }

            return (TEntity)entity;
        }

        /// <summary>
        /// Unproxies the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        public static EntityBaseWithoutId UnproxyEntity(EntityBaseWithoutId entity, ISession session)
        {
            if (entity == null)
            {
                ThrowHelper.ThrowArgumentNullException("entity");
            }
            if (session == null)
            {
                ThrowHelper.ThrowArgumentNullException("session");
            }

            if (entity is INHibernateProxy)
            {
                return (EntityBaseWithoutId)session.GetSessionImplementation().PersistenceContext.UnproxyAndReassociate(entity);
            }

            return entity;
        }

        #endregion

    }

}
