/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections;
using System.Reflection;
using Forge.Collections;
using Forge.Logging.Abstraction;
using Forge.ORM.NHibernateExtension.Model;
using Forge.Shared;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Proxy;

namespace Forge.ORM.NHibernateExtension
{

    /// <summary>
    /// Helper methods for query entities.
    /// </summary>
    public static class QueryHelper
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(QueryHelper));

        private static readonly bool LOG_QUERY = true;

        private static readonly MethodInfo mGenericQueryMethod = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes the <see cref="QueryHelper" /> class.
        /// </summary>
        static QueryHelper()
        {
            foreach (MethodInfo mi in typeof(QueryHelper).GetMethods())
            {
                if (mi.Name.Equals("Query"))
                {
                    if (mi.ContainsGenericParameters &&
                        mi.IsGenericMethod &&
                        mi.IsGenericMethodDefinition &&
                        mi.GetParameters().Length == 3)
                    {
                        mGenericQueryMethod = mi;
                        break;
                    }
                }
            }

        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Queries the specified session.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="session">The session.</param>
        /// <param name="queryData">The query data.</param>
        /// <returns>The result list</returns>
        /// <example>
        /// <code>
        /// QueryParams&lt;People&gt; qpPeople = new QueryParams&lt;People&gt;(
        ///     new ArithmeticCriteria("id", new EntityId(1, 1, 2)));
        /// IList&lt;People&gt; resultListPeople = QueryHelper.Query&lt;People&gt;(session, qpPeople);
        /// </code>
        /// </example>
        public static IListSpecialized<TEntity> Query<TEntity>(ISession session, QueryParamsBase queryData) where TEntity : EntityBaseWithoutId
        {
            return Query<TEntity>(session, queryData, LOG_QUERY);
        }

        /// <summary>
        /// Queries the specified session.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="queryData">The query data.</param>
        /// <returns>The result list</returns>
        /// <example>
        /// <code>
        /// IList&lt;EntityBase&gt; queryResult = QueryHelper.Query(session, new QueryParams(entityRule.EntityType));
        /// </code>
        /// </example>
        public static IListSpecialized<EntityBaseWithoutId> Query(ISession session, QueryParamsBase queryData)
        {
            return Query(session, queryData, LOG_QUERY);
        }

        /// <summary>
        /// Queries the specified session.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="session">The session.</param>
        /// <param name="queryData">The query data.</param>
        /// <param name="logQuery">if set to <c>true</c> [log query].</param>
        /// <returns>The result list</returns>
        /// <example>
        /// <code>
        /// QueryParams&lt;PersistentStorageAllocationTable&gt; query = new QueryParams&lt;PersistentStorageAllocationTable&gt;(
        ///         GetAllocationTableCriteria(), 1);
        /// IList&lt;PersistentStorageAllocationTable> resultList = QueryHelper.Query&lt;PersistentStorageAllocationTable&gt;(session, query, LOG_QUERY);
        /// if (resultList.Count == 0)
        /// {
        ///     this.mAllocationTable = new ItemTable();
        /// }
        /// </code>
        /// </example>
        public static IListSpecialized<TEntity> Query<TEntity>(ISession session, QueryParamsBase queryData, bool logQuery) where TEntity : EntityBaseWithoutId
        {
            if (session == null)
            {
                ThrowHelper.ThrowArgumentNullException("session");
            }
            if (queryData == null)
            {
                ThrowHelper.ThrowArgumentNullException("queryData");
            }

            DetachedCriteria dc = queryData.GetDetachedCriteria();
            ICriteria criteria = dc.GetExecutableCriteria(session);
            queryData.PrepareCriteria(criteria);

            return ExecuteQuery<TEntity>(session, queryData, dc, criteria, logQuery);
        }

        /// <summary>
        /// Queries the specified session.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="queryData">The query data.</param>
        /// <param name="logQuery">if set to <c>true</c> [log query].</param>
        /// <returns>The result list</returns>
        public static IListSpecialized<EntityBaseWithoutId> Query(ISession session, QueryParamsBase queryData, bool logQuery)
        {
            IList queryResult = (System.Collections.IList)mGenericQueryMethod.MakeGenericMethod(queryData.EntityType).Invoke(null, new object[] { session, queryData, logQuery });

            IListSpecialized<EntityBaseWithoutId> result = new ListSpecialized<EntityBaseWithoutId>();
            foreach (object b in queryResult)
            {
                result.Add((EntityBaseWithoutId)b);
            }

            return result;
        }

        #endregion

        #region Private method(s)

        private static ListSpecialized<TEntity> ExecuteQuery<TEntity>(ISession session, QueryParamsBase queryData, DetachedCriteria dc, ICriteria criteria, bool logQuery) where TEntity : EntityBaseWithoutId
        {
            if (logQuery)
            {
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("<--- QUERY (ID: {0}) BEGIN --->", queryData.Id));
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("(ID: {0}) {1}: {2}", queryData.Id, queryData.EntityType.FullName, criteria.ToString()));
            }

            ListSpecialized<TEntity> result = null;
            try
            {
                result = new ListSpecialized<TEntity>(criteria.List<TEntity>());
                if (logQuery)
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("<--- QUERY (ID: {0}) END, SIZE OF THE RESULT SET: {1} --->", queryData.Id, result == null ? "0" : result.Count.ToString()));
                }
            }
            catch (Exception ex)
            {
                if (logQuery)
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("<--- QUERY (ID: {0}) END, FAILED --->", queryData.Id));
                }
                throw new QueryException(ex.Message, ex);
            }

            if (result == null)
            {
                result = new ListSpecialized<TEntity>();
            }
            else
            {
                for (int i = 0; i < result.Count; i++)
                {
                    EntityBaseWithoutId eb = result[i];
                    if (eb is INHibernateProxy)
                    {
                        result[i] = (TEntity)session.GetSessionImplementation().PersistenceContext.UnproxyAndReassociate(eb);
                    }
                }
            }

            return result;
        }

        #endregion

    }

}
