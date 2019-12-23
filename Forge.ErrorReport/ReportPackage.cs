/* *********************************************************************
 * Date: 15 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using Forge.Collections;
using log4net.Core;

namespace Forge.ErrorReport
{

    /// <summary>
    /// Represents a package of an error report
    /// </summary>
    [Serializable]
    public class ReportPackage
    {

        #region Field(s)

        private static string mApplicationId = string.Empty;

        private ListSpecialized<LoggingEvent> mLogEvents = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes the <see cref="ReportPackage"/> class.
        /// </summary>
        static ReportPackage()
        {
            try
            {
                mApplicationId = ApplicationHelper.ApplicationId;
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportPackage" /> class.
        /// </summary>
        /// <param name="loggingEvent">The logging event.</param>
        /// <param name="logEvents">The log events.</param>
        public ReportPackage(LoggingEvent loggingEvent, ICollection<LoggingEvent> logEvents)
        {
            if (loggingEvent == null)
            {
                ThrowHelper.ThrowArgumentNullException("loggingEvent");
            }
            if (logEvents == null)
            {
                ThrowHelper.ThrowArgumentNullException("logEvents");
            }

            this.mLogEvents = new ListSpecialized<LoggingEvent>(logEvents);
            this.ApplicationId = mApplicationId;
            this.ReportCreated = DateTime.UtcNow;
            this.LoggingEvent = loggingEvent;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the application unique identifier.
        /// </summary>
        /// <value>
        /// The application unique identifier.
        /// </value>
        public string ApplicationId { get; private set; }

        /// <summary>
        /// Gets the report created.
        /// </summary>
        /// <value>
        /// The report created.
        /// </value>
        public DateTime ReportCreated { get; private set; }

        /// <summary>
        /// Gets the logging event.
        /// </summary>
        /// <value>
        /// The logging event.
        /// </value>
        public LoggingEvent LoggingEvent { get; private set; }

        /// <summary>
        /// Gets the log events.
        /// </summary>
        /// <value>
        /// The log events.
        /// </value>
        public IListSpecialized<LoggingEvent> LogEvents
        {
            get { return new ListSpecialized<LoggingEvent>(mLogEvents); }
        }

        #endregion

    }

}
