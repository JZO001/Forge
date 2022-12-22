/* *********************************************************************
 * Date: 15 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Forge.Configuration.Shared;
using Forge.ErrorReport.ConfigSection;
using Forge.ErrorReport.Filter;
using Forge.ErrorReport.Sink;
using Forge.Legacy;
using Forge.Logging.Abstraction;
using Forge.Reflection;

namespace Forge.ErrorReport.Service
{

    /// <summary>
    /// The error report service
    /// </summary>
    public class ErrorReportServiceImpl : MBRBase, Forge.ErrorReport.Contracts.IErrorReportSendContract
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger<ErrorReportServiceImpl>();

        private const string CONFIG_SINKS = "Service/Sinks";

        private const string CONFIG_FILTER = "Service/Filter";

        private IErrorReportFilter mErrorReportFilter = null;

        private readonly List<IErrorReportPackageSink> mReportPackageSinks = new List<IErrorReportPackageSink>();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReportServiceImpl"/> class.
        /// </summary>
        public ErrorReportServiceImpl()
        {
            ErrorReportConfiguration.SectionHandler.OnConfigurationChanged += new EventHandler<EventArgs>(SectionHandler_OnConfigurationChanged);
            SectionHandler_OnConfigurationChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Sends the error report.
        /// </summary>
        /// <param name="package">The package.</param>
        public void SendErrorReport(ReportPackage package)
        {
            if (package != null)
            {
                if (LOGGER.IsDebugEnabled) LOGGER.Debug("ERROR_REPORT_SERVICE, new error report package arrived.");

                IErrorReportFilter filter = null;
                List<IErrorReportPackageSink> sinks = new List<IErrorReportPackageSink>();
                lock (mReportPackageSinks)
                {
                    filter = mErrorReportFilter;
                    sinks.AddRange(mReportPackageSinks);
                }

                bool allowed = true;
                if (filter != null)
                {
                    try
                    {
                        allowed = filter.Filter(package);
                    }
                    catch (Exception ex)
                    {
                        allowed = false;
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("ERROR_REPORT_SERVICE, filter threw an exception. Filter type: '{0}'", filter.GetType().AssemblyQualifiedName), ex);
                    }
                }

                if (allowed && sinks.Count > 0)
                {
                    foreach (IErrorReportPackageSink sink in sinks)
                    {
                        allowed = true;
                        if (sink.Filter != null)
                        {
                            try
                            {
                                allowed = sink.Filter.Filter(package);
                            }
                            catch (Exception ex)
                            {
                                allowed = false;
                                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("ERROR_REPORT_SERVICE, sink filter threw an exception. Sink type: '{0}', filter type: '{1}'", sink.GetType().AssemblyQualifiedName, filter.GetType().AssemblyQualifiedName), ex);
                            }
                        }

                        if (allowed)
                        {
                            try
                            {
                                sink.ProcessReportPackage(package);
                            }
                            catch (Exception ex)
                            {
                                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("ERROR_REPORT_SERVICE, a sink threw an exception. SinkId: '{0}', sink type: '{1}'", sink.SinkId, sink.GetType().AssemblyQualifiedName), ex);
                            }
                        }
                    }
                    sinks.Clear();
                }

            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            ErrorReportConfiguration.SectionHandler.OnConfigurationChanged -= new EventHandler<EventArgs>(SectionHandler_OnConfigurationChanged);
        }

        #endregion

        #region Private method(s)

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void SectionHandler_OnConfigurationChanged(object sender, EventArgs e)
        {
            List<IErrorReportPackageSink> sinks = new List<IErrorReportPackageSink>();
            CategoryPropertyItem rootItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(ErrorReportConfiguration.Settings.CategoryPropertyItems, CONFIG_SINKS);
            if (rootItem != null)
            {
                if (rootItem.PropertyItems != null)
                {
                    foreach (CategoryPropertyItem sinkItem in rootItem.PropertyItems)
                    {
                        try
                        {
                            Type sinkType = TypeHelper.GetTypeFromString(sinkItem.EntryValue, TypeLookupModeEnum.AllowAll, true, true, true);
                            IErrorReportPackageSink sink = (IErrorReportPackageSink)sinkType.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
                            sink.Initialize(sinkItem);
                            sinks.Add(sink);
                        }
                        catch (Exception ex)
                        {
                            if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("ERROR_REPORT_SERVICE, failed to create error report sink. Type: '{0}'", sinkItem.EntryValue), ex);
                        }
                    }
                }
            }

            IErrorReportFilter filter = null;
            rootItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(ErrorReportConfiguration.Settings.CategoryPropertyItems, CONFIG_FILTER);
            if (rootItem != null)
            {
                try
                {
                    Type filterType = TypeHelper.GetTypeFromString(rootItem.EntryValue, TypeLookupModeEnum.AllowAll, true, true, true);
                    filter = (IErrorReportFilter)filterType.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
                    filter.Initialize(rootItem);
                }
                catch (Exception ex)
                {
                    filter = null;
                    if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("ERROR_REPORT_SERVICE, failed to create error report filter. Type: '{0}'", rootItem.EntryValue), ex);
                }
            }

            lock (mReportPackageSinks)
            {
                mErrorReportFilter = filter;
                mReportPackageSinks.ForEach(s => s.Close());
                mReportPackageSinks.Clear();
                mReportPackageSinks.AddRange(sinks);
                sinks.Clear();
                if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("ERROR_REPORT_SERVICE, current active sink(s): {0}", mReportPackageSinks.Count.ToString()));
            }
        }

        #endregion

    }

}
