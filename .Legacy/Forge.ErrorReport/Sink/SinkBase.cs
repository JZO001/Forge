/* *********************************************************************
 * Date: 24 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Configuration.Shared;
using Forge.ErrorReport.Filter;
using Forge.Logging;
using Forge.Reflection;

namespace Forge.ErrorReport.Sink
{

    /// <summary>
    /// Represents the base functionality of a sink
    /// </summary>
    [Serializable]
    public abstract class SinkBase : IErrorReportPackageSink
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(SinkBase));

        /// <summary>
        /// The configuration g_ filter
        /// </summary>
        protected const string CONFIG_FILTER = "Filter";

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="SinkBase"/> class.
        /// </summary>
        protected SinkBase()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the sink unique identifier.
        /// </summary>
        /// <value>
        /// The sink unique identifier.
        /// </value>
        public string SinkId
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public IErrorReportFilter Filter { get; set; }

        /// <summary>
        /// Gets a value indicating whether [is initialized].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is initialized]; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized
        {
            get;
            set;
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual void Initialize(CategoryPropertyItem item)
        {
            this.Filter = null;
            if (item != null)
            {
                this.SinkId = item.Id;
                CategoryPropertyItem rootItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(item.PropertyItems, CONFIG_FILTER);
                if (rootItem != null)
                {
                    try
                    {
                        Type filterType = TypeHelper.GetTypeFromString(rootItem.EntryValue, TypeLookupModeEnum.AllowAll, true, true, true);
                        this.Filter = (IErrorReportFilter)filterType.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
                        this.Filter.Initialize(rootItem);
                    }
                    catch (Exception ex)
                    {
                        this.Filter = null;
                        string message = string.Format("Failed to create error report filter. Type: '{0}'. Sink type: '{1}'", rootItem.EntryValue, this.GetType().AssemblyQualifiedName);
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(message, ex);
                        throw new InitializationException(message, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Processes the report package.
        /// </summary>
        /// <param name="package">The package.</param>
        public abstract void ProcessReportPackage(ReportPackage package);

        /// <summary>
        /// Closes the sink.
        /// </summary>
        public virtual void Close()
        {
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Documents the initialization check.
        /// </summary>
        /// <exception cref="InitializationException"></exception>
        protected virtual void DoInitializationCheck()
        {
            if (!IsInitialized)
            {
                throw new InitializationException("Sink has not been initialized.");
            }
        }

        #endregion

    }

}
