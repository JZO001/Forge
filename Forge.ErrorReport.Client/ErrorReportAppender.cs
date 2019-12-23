/* *********************************************************************
 * Date: 15 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Threading;
using Forge.ErrorReport.Contracts;
using Forge.Management;
using Forge.Net.Remoting.Proxy;
using Forge.Net.Services.Locators;
using log4net.Appender;
using log4net.Core;

namespace Forge.ErrorReport.Client
{

    /// <summary>
    /// Error Report Appender
    /// </summary>
    public class ErrorReportAppender : AppenderSkeleton
    {

        #region Field(s)

        /// <summary>
        /// The logger
        /// </summary>
        protected static readonly Forge.Logging.ILog LOGGER = Forge.Logging.LogManager.GetLogger(typeof(ErrorReportAppender));

        private static bool mInitialized = false;

        private static readonly object LOCK_FOR_LISTS = new object();

        private readonly static List<LoggingEvent> mLogEvents = new List<LoggingEvent>();

        private readonly static List<ReportPackage> mReportPackages = new List<ReportPackage>();

        private static int mPreviousHistoryItemsToAppendBeforeSend = 0;

        private static string mChannelId = string.Empty;

        private static bool mFlushImmediatelly = false;

        private static Thread mSenderThread = null;

        private static readonly AutoResetEvent mSendErrorReportSignalEvent = new AutoResetEvent(false);

        private static readonly AutoResetEvent mSendErrorReportFlushedSignalEvent = new AutoResetEvent(false);

        private static IRemoteServiceLocator<IErrorReportSendContract> mServiceLocator = null;

        private static IErrorReportSendContract mProxy = null;

        private static object LOCK_PROXY = new object();

        private static bool mShutdown = false;

        private static AutoResetEvent mShutdownEvent = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes the <see cref="ErrorReportAppender"/> class.
        /// </summary>
        /// <remarks>
        /// Empty default constructor
        /// </remarks>
        static ErrorReportAppender()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            if (Forge.Net.TerraGraf.NetworkManager.Instance.ManagerState == Management.ManagerStateEnum.Started)
            {
                // init
                Initialize();
            }
            else
            {
                // wait for init
                Forge.Net.TerraGraf.NetworkManager.Instance.EventStart += new EventHandler<Management.ManagerEventStateEventArgs>(Instance_EventStart);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReportAppender"/> class.
        /// </summary>
        /// <remarks>
        /// Empty default constructor
        /// </remarks>
        public ErrorReportAppender()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the append history items before send.
        /// </summary>
        /// <value>
        /// The append history items before send.
        /// </value>
        public int PreviousHistoryItemsToAppendBeforeSend
        {
            get { return mPreviousHistoryItemsToAppendBeforeSend; }
            set
            {
                if (value >= 0)
                {
                    lock (LOCK_FOR_LISTS)
                    {
                        mPreviousHistoryItemsToAppendBeforeSend = value;
                        if (value == 0)
                        {
                            mLogEvents.Clear();
                        }
                        else if (mLogEvents.Count > value)
                        {
                            while (mLogEvents.Count > value)
                            {
                                mLogEvents.RemoveAt(0);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [handle application domain unhandled exception].
        /// </summary>
        /// <value>
        /// <c>true</c> if [handle application domain unhandled exception]; otherwise, <c>false</c>.
        /// </value>
        public bool HandleAppDomainUnhandledException
        {
            get { return Forge.Logging.LogUtils.IsSubscribedForAppDomainUnhandledException; }
            set
            {
                Forge.Logging.LogUtils.IsSubscribedForAppDomainUnhandledException = value;
                if (Forge.Logging.LogUtils.IsSubscribedForAppDomainUnhandledException)
                {
                    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                }
                else
                {
                    AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                }
            }
        }

        /// <summary>
        /// Gets or sets the channel unique identifier.
        /// </summary>
        /// <value>
        /// The channel unique identifier.
        /// </value>
        public string ChannelId
        {
            get { return mChannelId; }
            set
            {
                if (value != null)
                {
                    mChannelId = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether [is closed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is closed]; otherwise, <c>false</c>.
        /// </value>
        public bool IsClosed { get; private set; }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Subclasses of <see cref="T:log4net.Appender.AppenderSkeleton" /> should implement this method
        /// to perform actual logging.
        /// </summary>
        /// <param name="loggingEvent">The event to append.</param>
        /// <remarks>
        ///   <para>
        /// A subclass must implement this method to perform
        /// logging of the <paramref name="loggingEvent" />.
        ///   </para>
        ///   <para>This method will be called by <see cref="M:log4net.Appender.AppenderSkeleton.DoAppend(log4net.Core.LoggingEvent)" />
        /// if all the conditions listed for that method are met.
        ///   </para>
        ///   <para>
        /// To restrict the logging of events in the appender
        /// override the <see cref="M:log4net.Appender.AppenderSkeleton.PreAppendCheck" /> method.
        ///   </para>
        /// </remarks>
        protected override void Append(LoggingEvent loggingEvent)
        {
            if (IsClosed)
            {
                throw new InvalidOperationException("Appender closed.");
            }

            loggingEvent.Domain.ToString();
            loggingEvent.Identity.ToString();
            loggingEvent.RenderedMessage.ToString();
            loggingEvent.GetExceptionString();
            loggingEvent.LocationInformation.ToString();
            loggingEvent.ThreadName.ToString();
            loggingEvent.UserName.ToString();
            if (Level.Compare(loggingEvent.Level, Level.Error) < 0)
            {
                // non-error level
                if (PreviousHistoryItemsToAppendBeforeSend > 0)
                {
                    lock (LOCK_FOR_LISTS)
                    {
                        mLogEvents.Add(loggingEvent);
                        if (mLogEvents.Count > mPreviousHistoryItemsToAppendBeforeSend)
                        {
                            mLogEvents.RemoveAt(0);
                        }
                    }
                }
            }
            else
            {
                // error level
                lock (LOCK_FOR_LISTS)
                {
                    mReportPackages.Add(new ReportPackage(loggingEvent, mLogEvents));
                    mLogEvents.Clear();
                }
                if (mInitialized)
                {
                    mSendErrorReportSignalEvent.Set();
                    if (mFlushImmediatelly)
                    {
                        mSendErrorReportFlushedSignalEvent.WaitOne();
                    }
                }
            }
        }

        /// <summary>
        /// Is called when the appender is closed. Derived classes should override
        /// this method if resources need to be released.
        /// </summary>
        /// <remarks>
        ///   <para>
        /// Releases any resources allocated within the appender such as file handles,
        /// network connections, etc.
        ///   </para>
        ///   <para>
        /// It is a programming error to append to a closed appender.
        ///   </para>
        /// </remarks>
        protected override void OnClose()
        {
            if (!IsClosed)
            {
                IsClosed = true;
                base.OnClose();
            }
        }

        #endregion

        #region Private method(s)

        private static void Instance_EventStart(object sender, ManagerEventStateEventArgs e)
        {
            if (e.EventState == ManagerEventStateEnum.After)
            {
                // init
                Net.TerraGraf.NetworkManager.Instance.EventStart -= new EventHandler<Management.ManagerEventStateEventArgs>(Instance_EventStart);
                Initialize();
            }
        }

        private static void Initialize()
        {
            mServiceLocator = new ErrorReportServiceLocator(); //RemoteServiceLocatorManager.GetServiceLocator<IErrorReport, ErrorReportServiceLocator>();
            mServiceLocator.EventPreferedServiceProviderChanged += new EventHandler<PreferedServiceProviderChangedEventArgs>(ServiceLocatorPreferedServiceProviderChangedHandler);
            mServiceLocator.EventSyncInvocation = false;
            mServiceLocator.EventParallelInvocation = false;
            mServiceLocator.Start();

            mSenderThread = new Thread(new ThreadStart(SenderThreadMain));
            mSenderThread.Name = "ErrorReportAppender_SenderThread";
            mSenderThread.IsBackground = true;
            mSenderThread.Start();

            mInitialized = true;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (mInitialized)
            {
                if (e.IsTerminating)
                {
                    CurrentDomain_ProcessExit(sender, e);
                }
            }
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.ProcessExit -= new EventHandler(CurrentDomain_ProcessExit);
            if (mInitialized)
            {
                //Console.WriteLine("BEFORE EXIT... " + mReportPackages.Count.ToString());

                mShutdownEvent = new AutoResetEvent(false);
                mShutdown = true;
                mFlushImmediatelly = true;
                mSendErrorReportSignalEvent.Set();
                mSendErrorReportFlushedSignalEvent.WaitOne();
                mShutdownEvent.WaitOne();

                //Console.WriteLine("AFTER EXIT... " + mReportPackages.Count.ToString());

                if (!AppDomain.CurrentDomain.IsFinalizingForUnload())
                {
                    try
                    {
                        AppDomain.Unload(AppDomain.CurrentDomain);
                    }
                    catch (Exception) { }
                }
            }
        }

        private static void SenderThreadMain()
        {
            while (!mShutdown)
            {
                mSendErrorReportFlushedSignalEvent.Reset();
                mSendErrorReportSignalEvent.WaitOne();
                try
                {
                    if (mServiceLocator.ServiceState == Net.Services.ServiceStateEnum.Available && mProxy != null)
                    {
                        int counter = 0;
                        lock (LOCK_FOR_LISTS)
                        {
                            counter = mReportPackages.Count;
                        }

                        while (counter > 0)
                        {
                            lock (LOCK_PROXY)
                            {
                                if (mProxy != null)
                                {
                                    ReportPackage rp = null;

                                    lock (LOCK_FOR_LISTS)
                                    {
                                        // csomag kiválasztása küldésre
                                        if (mReportPackages.Count > 0)
                                        {
                                            rp = mReportPackages[0];
                                        }
                                        else
                                        {
                                            counter = 0;
                                        }
                                    }

                                    // a küldést már a lock context-en kívül valósítom meg.
                                    if (rp != null)
                                    {
                                        // meg van mit kell küldeni
                                        mProxy.SendErrorReport(rp);

                                        // ha nem dobott hibát a küldés, akkor törlöm a tárolóból a packaget
                                        lock (LOCK_FOR_LISTS)
                                        {
                                            mReportPackages.RemoveAt(0);
                                            counter = mReportPackages.Count;
                                        }
                                    }

                                }
                            }
                        }

                    }
                }
                catch (Exception) { }

                mSendErrorReportFlushedSignalEvent.Set();
            }

            mShutdownEvent.Set();
        }

        private static void ServiceLocatorPreferedServiceProviderChangedHandler(object sender, PreferedServiceProviderChangedEventArgs e)
        {
            if (LOGGER.IsDebugEnabled) LOGGER.Debug("ERROR_REPORT_APPENDER, service locator notifies about prefered service provider changed.");
            lock (LOCK_PROXY)
            {
                if (mProxy != null)
                {
                    mProxy.Dispose();
                    mProxy = null;
                }
                ServiceProvider provider = mServiceLocator.PreferedServiceProvider;
                if (provider != null)
                {
                    try
                    {
                        if (LOGGER.IsDebugEnabled) LOGGER.Info(string.Format("ERROR_REPORT_APPENDER, service locator notifies the new service provider is '{0}' on endpoint '{1}'.", provider.NetworkPeer.Id, provider.RemoteEndPoint.ToString()));
                        ProxyFactory<IErrorReportSendContract> factory = new ProxyFactory<IErrorReportSendContract>(mServiceLocator.ChannelId, provider.RemoteEndPoint);
                        mProxy = factory.CreateProxy();
                        // meglököm a feldolgozó szálat, hogy ha van mit, akkor küldje
                        mSendErrorReportSignalEvent.Set();
                    }
                    catch (Exception ex)
                    {
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("ERROR_REPORT_APPENDER, failed to create proxy for network peer: {0}", provider.NetworkPeer.Id), ex);
                    }
                }
                else
                {
                    if (LOGGER.IsInfoEnabled) LOGGER.Info("ERROR_REPORT_APPENDER, service locator notifies about no service provider exist.");
                }
            }
        }

        #endregion

    }

}
