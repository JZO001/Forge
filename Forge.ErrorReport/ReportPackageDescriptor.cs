/* *********************************************************************
 * Date: 21 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.ORM.NHibernateExtension.Model.Distributed;
using Forge.Shared;

namespace Forge.ErrorReport
{

    /// <summary>
    /// Represents a report package without log infos
    /// </summary>
    [Serializable]
    public class ReportPackageDescriptor
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportPackageDescriptor" /> class.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="applicationId">The application unique identifier.</param>
        /// <param name="errorLevelType">Type of the error level.</param>
        /// <param name="reportCreated">The report created.</param>
        public ReportPackageDescriptor(EntityId id, string applicationId, int errorLevelType, DateTime reportCreated)
        {
            if (id == null)
            {
                ThrowHelper.ThrowArgumentNullException("id");
            }
            if (string.IsNullOrEmpty(applicationId))
            {
                ThrowHelper.ThrowArgumentNullException("applicationId");
            }

            Id = id;
            ApplicationId = applicationId;
            ErrorLevelType = errorLevelType;
            ReportCreated = reportCreated;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public EntityId Id { get; private set; }

        /// <summary>
        /// Gets the application unique identifier.
        /// </summary>
        /// <value>
        /// The application unique identifier.
        /// </value>
        public string ApplicationId { get; private set; }

        /// <summary>
        /// Gets the type of the error level.
        /// </summary>
        /// <value>
        /// The type of the error level.
        /// </value>
        public int ErrorLevelType { get; private set; }

        /// <summary>
        /// Gets the report created.
        /// </summary>
        /// <value>
        /// The report created.
        /// </value>
        public DateTime ReportCreated { get; private set; }

        #endregion

    }

}
