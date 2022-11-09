/* *********************************************************************
 * Date: 11 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Forge.Reflection;

namespace Forge.Logging.Utils
{

    /// <summary>
    /// Utilities for logging
    /// </summary>
    public static class LogUtils
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(LogUtils));

        private static bool mIsSubscribedForAppDomainUnhandledException = false;

        private static bool mIsDynamicAvailable = true;

        private static bool mIsFullyTrustedAvailable = true;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets a value indicating whether [is subscribed for assembly load].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is subscribed for assembly load]; otherwise, <c>false</c>.
        /// </value>
        public static bool IsSubscribedForAssemblyLoad { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [is subscribed for application domain unhandled exception].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is subscribed for application domain unhandled exception]; otherwise, <c>false</c>.
        /// </value>
        public static bool IsSubscribedForAppDomainUnhandledException
        {
            get
            {
                return mIsSubscribedForAppDomainUnhandledException;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (!mIsSubscribedForAppDomainUnhandledException && value)
                {
                    mIsSubscribedForAppDomainUnhandledException = true;
                    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                }
                else if (!value && mIsSubscribedForAppDomainUnhandledException)
                {
                    mIsSubscribedForAppDomainUnhandledException = false;
                    AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                }
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Traces the assembly loads.
        /// </summary>
        /// <param name="state">if set to <c>true</c> [state].</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void TraceAssemblyLoads(bool state)
        {
            if (state && !IsSubscribedForAssemblyLoad)
            {
                AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);
            }
            else if (IsSubscribedForAssemblyLoad)
            {
                AppDomain.CurrentDomain.AssemblyLoad -= new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);
            }
        }

        /// <summary>
        /// Logs the current process information.
        /// </summary>
        public static void LogProcessInfo()
        {
            if (LOGGER.IsInfoEnabled)
            {
                Process p = Process.GetCurrentProcess();

                LOGGER.Info("LOGUTILS, Current process information:");
                LOGGER.Info(string.Format("LOGUTILS, ProcessId: {0}", p.Id.ToString()));
                LOGGER.Info(string.Format("LOGUTILS, Base priority: {0}", p.BasePriority.ToString()));
                LOGGER.Info(string.Format("LOGUTILS, Enable raising events: {0}", p.EnableRaisingEvents.ToString()));
                LOGGER.Info(string.Format("LOGUTILS, Machine name: {0}", p.MachineName));
                LOGGER.Info(string.Format("LOGUTILS, Process name: {0}", p.ProcessName));
                LOGGER.Info(string.Format("LOGUTILS, SessionId: {0}", p.SessionId.ToString()));
                LOGGER.Info(string.Format("LOGUTILS, Start time: {0}", p.StartTime.ToString()));

                try
                {
                    if (p.StartInfo != null)
                    {
                        LOGGER.Info(string.Format("LOGUTILS, StartInfo, arguments: {0}", p.StartInfo.Arguments));
                        LOGGER.Info(string.Format("LOGUTILS, StartInfo, create no window: {0}", p.StartInfo.CreateNoWindow.ToString()));
                        LOGGER.Info(string.Format("LOGUTILS, StartInfo, file name: {0}", p.StartInfo.FileName));
#if IS_WINDOWS
                        LOGGER.Info(string.Format("LOGUTILS, StartInfo, domain: {0}", p.StartInfo.Domain));
                        LOGGER.Info(string.Format("LOGUTILS, StartInfo, load user profile: {0}", p.StartInfo.LoadUserProfile.ToString()));
#endif
                        LOGGER.Info(string.Format("LOGUTILS, StartInfo, redirect standard error: {0}", p.StartInfo.RedirectStandardError.ToString()));
                        LOGGER.Info(string.Format("LOGUTILS, StartInfo, redirect standard input: {0}", p.StartInfo.RedirectStandardInput.ToString()));
                        LOGGER.Info(string.Format("LOGUTILS, StartInfo, user name: {0}", p.StartInfo.UserName));
                        LOGGER.Info(string.Format("LOGUTILS, StartInfo, use shell execute: {0}", p.StartInfo.UseShellExecute.ToString()));
                        LOGGER.Info(string.Format("LOGUTILS, StartInfo, verb: {0}", p.StartInfo.Verb));
                        LOGGER.Info(string.Format("LOGUTILS, StartInfo, working directory: {0}", p.StartInfo.WorkingDirectory));
                    }
                }
                catch (Exception)
                {
                    LOGGER.Info("LOGUTILS, StartInfo is not available.");
                }

                LOGGER.Info("--------------------------------------------------------");
            }
        }

        /// <summary>
        /// Logs the loaded assemblies.
        /// </summary>
        public static void LogDomainInfo()
        {
            if (LOGGER.IsInfoEnabled)
            {
                AppDomain domain = AppDomain.CurrentDomain;

                LOGGER.Info(string.Format("LOGUTILS, Domain, Id: {0}", domain.Id.ToString()));
                LOGGER.Info(string.Format("LOGUTILS, Domain, base directory: {0}", domain.BaseDirectory));
                LOGGER.Info(string.Format("LOGUTILS, Domain, dynamic directory: {0}", domain.DynamicDirectory));
                LOGGER.Info(string.Format("LOGUTILS, Domain, friendly name: {0}", domain.FriendlyName));

                try
                {
                    ExtractObjectData e = ExtractObjectData.Create("IsFullyTrusted");
                    LOGGER.Info(string.Format("LOGUTILS, Domain, is fully trusted: {0}", e.GetValue(domain).ToString()));
                }
                catch (Exception) { }

                try
                {
                    ExtractObjectData e = ExtractObjectData.Create("IsHomogenous");
                    LOGGER.Info(string.Format("LOGUTILS, Domain, is homogenous: {0}", e.GetValue(domain).ToString()));
                }
                catch (Exception) { }

                LOGGER.Info(string.Format("LOGUTILS, Domain, relative search path: {0}", domain.RelativeSearchPath));
                LOGGER.Info(string.Format("LOGUTILS, Domain, shadow copy files: {0}", domain.ShadowCopyFiles));

                if (domain.SetupInformation != null)
                {
                    try
                    {
                        ExtractObjectData e = ExtractObjectData.Create("SetupInformation.AppDomainManagerAssembly");
                        LOGGER.Info(string.Format("LOGUTILS, Domain, Setup Information, AppDomainManagerAssembly: {0}", e.GetValue(domain).ToString()));
                    }
                    catch (Exception) { }
                    try
                    {
                        ExtractObjectData e = ExtractObjectData.Create("SetupInformation.AppDomainManagerType");
                        LOGGER.Info(string.Format("LOGUTILS, Domain, Setup Information, AppDomainManagerType: {0}", e.GetValue(domain).ToString()));
                    }
                    catch (Exception) { }

                    LOGGER.Info(string.Format("LOGUTILS, Domain, Setup Information, ApplicationBase: {0}", domain.SetupInformation.ApplicationBase));
#if NETCOREAPP3_1_OR_GREATER
#else
                    LOGGER.Info(string.Format("LOGUTILS, Domain, Setup Information, ApplicationName: {0}", domain.SetupInformation.ApplicationName));
                    LOGGER.Info(string.Format("LOGUTILS, Domain, Setup Information, CachePath: {0}", domain.SetupInformation.CachePath));
                    LOGGER.Info(string.Format("LOGUTILS, Domain, Setup Information, ConfigurationFile: {0}", domain.SetupInformation.ConfigurationFile));
                    LOGGER.Info(string.Format("LOGUTILS, Domain, Setup Information, DisallowApplicationBaseProbing: {0}", domain.SetupInformation.DisallowApplicationBaseProbing.ToString()));
                    LOGGER.Info(string.Format("LOGUTILS, Domain, Setup Information, DisallowBindingRedirects: {0}", domain.SetupInformation.DisallowBindingRedirects.ToString()));
                    LOGGER.Info(string.Format("LOGUTILS, Domain, Setup Information, DisallowCodeDownload: {0}", domain.SetupInformation.DisallowCodeDownload.ToString()));
                    LOGGER.Info(string.Format("LOGUTILS, Domain, Setup Information, DisallowPublisherPolicy: {0}", domain.SetupInformation.DisallowPublisherPolicy.ToString()));
                    LOGGER.Info(string.Format("LOGUTILS, Domain, Setup Information, DynamicBase: {0}", domain.SetupInformation.DynamicBase));
                    LOGGER.Info(string.Format("LOGUTILS, Domain, Setup Information, LicenseFile: {0}", domain.SetupInformation.LicenseFile));
                    LOGGER.Info(string.Format("LOGUTILS, Domain, Setup Information, LoaderOptimization: {0}", domain.SetupInformation.LoaderOptimization.ToString()));
                    LOGGER.Info(string.Format("LOGUTILS, Domain, Setup Information, PrivateBinPath: {0}", domain.SetupInformation.PrivateBinPath));
                    LOGGER.Info(string.Format("LOGUTILS, Domain, Setup Information, PrivateBinPathProbe: {0}", domain.SetupInformation.PrivateBinPathProbe));
                    //LOGGER.Info(string.Format("LOGUTILS, Domain, Setup Information, SandboxInterop: {0}", domain.SetupInformation.SandboxInterop.ToString()));
                    LOGGER.Info(string.Format("LOGUTILS, Domain, Setup Information, ShadowCopyDirectories: {0}", domain.SetupInformation.ShadowCopyDirectories));
                    LOGGER.Info(string.Format("LOGUTILS, Domain, Setup Information, ShadowCopyFiles: {0}", domain.SetupInformation.ShadowCopyFiles));
#endif

                }

                LOGGER.Info("--------------------------------------------------------");
            }
        }

        /// <summary>
        /// Logs the loaded assemblies.
        /// </summary>
        public static void LogLoadedAssemblies()
        {
            if (LOGGER.IsInfoEnabled)
            {
                bool logMark = false;
                LOGGER.Info("LOGUTILS, Loaded assemblies:");
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (logMark)
                    {
                        LOGGER.Info("***********");
                    }
                    else
                    {
                        logMark = true;
                    }
                    LOGGER.Info(string.Format("LOGUTILS, Assembly, full name: {0}", a.FullName));
#if NET5_0_OR_GREATER || NETCOREAPP3_1
                    try
                    {
                        LOGGER.Info(string.Format("LOGUTILS, Assembly, code base: {0}", a.Location));
                    }
                    catch (Exception) { }
#else
                    try
                    {
                        LOGGER.Info(string.Format("LOGUTILS, Assembly, code base: {0}", a.CodeBase));
                    }
                    catch (Exception) { }
#endif
#if NET5_0_OR_GREATER || NETCOREAPP3_1
#else
                    LOGGER.Info(string.Format("LOGUTILS, Assembly, global assembly cache: {0}", a.GlobalAssemblyCache.ToString()));
#endif
                    LOGGER.Info(string.Format("LOGUTILS, Assembly, host context: {0}", a.HostContext.ToString()));
                    LOGGER.Info(string.Format("LOGUTILS, Assembly, ImageRuntimeVersion: {0}", a.ImageRuntimeVersion));
                    LogAssemblyNewProperties(a);
                    try
                    {
                        LOGGER.Info(string.Format("LOGUTILS, Assembly, Location: {0}", a.Location));
                    }
                    catch (Exception) { }
                    LOGGER.Info(string.Format("LOGUTILS, Assembly, ReflectionOnly: {0}", a.ReflectionOnly.ToString()));
                }
                LOGGER.Info("--------------------------------------------------------");
            }
        }

        /// <summary>
        /// Logs all.
        /// </summary>
        public static void LogAll()
        {
            LogProcessInfo();
            LogDomainInfo();
            LogLoadedAssemblies();
            TraceAssemblyLoads(true);
            IsSubscribedForAppDomainUnhandledException = true;
        }

        #endregion

        #region Private method(s)

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            if (LOGGER.IsInfoEnabled)
            {
                Assembly a = args.LoadedAssembly;
                LOGGER.Info("LOGUTILS, new assembly loaded.");
                LOGGER.Info(string.Format("LOGUTILS, Assembly, full name: {0}", a.FullName));
                try
                {
#if NET5_0_OR_GREATER || NETCOREAPP3_1
                    LOGGER.Info(string.Format("LOGUTILS, Assembly, code base: {0}", a.Location));
#else
                    LOGGER.Info(string.Format("LOGUTILS, Assembly, code base: {0}", a.CodeBase));
#endif
                }
                catch (Exception) { }
#if NET5_0_OR_GREATER || NETCOREAPP3_1
#else
                LOGGER.Info(string.Format("LOGUTILS, Assembly, global assembly cache: {0}", a.GlobalAssemblyCache.ToString()));
#endif
                LOGGER.Info(string.Format("LOGUTILS, Assembly, host context: {0}", a.HostContext.ToString()));
                LOGGER.Info(string.Format("LOGUTILS, Assembly, ImageRuntimeVersion: {0}", a.ImageRuntimeVersion));
                LogAssemblyNewProperties(a);
                try
                {
                    LOGGER.Info(string.Format("LOGUTILS, Assembly, Location: {0}", a.Location));
                }
                catch (Exception) { }
                LOGGER.Info(string.Format("LOGUTILS, Assembly, ReflectionOnly: {0}", a.ReflectionOnly.ToString()));
                LOGGER.Info("--------------------------------------------------------");
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (LOGGER.IsFatalEnabled) LOGGER.Fatal(string.Format("LOGUTILS, unhandled exception detected in the application domain which will {0}terminate the current process.", e.IsTerminating ? string.Empty : "NOT "), e.ExceptionObject as Exception);
        }

        private static void LogAssemblyNewProperties(Assembly a)
        {
            // Log assembly properties which are available in newer version of Framework.NET
            if (mIsDynamicAvailable)
            {
                try
                {
                    ExtractObjectData e = ExtractObjectData.Create("IsDynamic");
                    LOGGER.Info(string.Format("LOGUTILS, Assembly, IsDynamic: {0}", e.GetValue(a).ToString()));
                }
                catch (MissingFieldException)
                {
                    mIsDynamicAvailable = false;

                }
                catch (MissingMemberException)
                {
                    mIsDynamicAvailable = false;
                }
                catch (Exception) { }
            }
            if (mIsFullyTrustedAvailable)
            {
                try
                {
                    ExtractObjectData e = ExtractObjectData.Create("IsFullyTrusted");
                    LOGGER.Info(string.Format("LOGUTILS, Assembly, IsFullyTrusted: {0}", e.GetValue(a).ToString()));
                }
                catch (MissingFieldException)
                {
                    mIsFullyTrustedAvailable = false;

                }
                catch (MissingMemberException)
                {
                    mIsFullyTrustedAvailable = false;
                }
                catch (Exception) { }
            }
        }

        #endregion

    }

}
