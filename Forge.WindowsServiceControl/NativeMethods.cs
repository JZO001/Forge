/* *********************************************************************
 * Date: 26 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Runtime.InteropServices;

namespace Forge.WindowsServiceControl
{

    /// <summary>
    /// Contains the unmanaged native method declarations
    /// </summary>
    internal static class NativeMethods
    {

        /// <summary>
        /// Opens the SC manager.
        /// </summary>
        /// <param name="machineName">Name of the machine.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="desiredAccess">The desired access.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "1"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "0"), DllImport("advapi32.dll", EntryPoint = "OpenSCManager")]
        public static extern IntPtr OpenSCManager(
            string machineName,
            string databaseName,
            ServiceControlAccessRightsEnum desiredAccess);

        /// <summary>
        /// Closes the service handle.
        /// </summary>
        /// <param name="hSCObject">The h SC object.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
        [DllImport("advapi32.dll", EntryPoint = "CloseServiceHandle")]
        public static extern int CloseServiceHandle(IntPtr hSCObject);

        /// <summary>
        /// Opens the service.
        /// </summary>
        /// <param name="hSCManager">The h SC manager.</param>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="desiredAccess">The desired access.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "1"), DllImport("advapi32.dll", EntryPoint = "OpenService")]
        public static extern IntPtr OpenService(
            IntPtr hSCManager,
            string serviceName,
            ServiceAccessRightsEnum desiredAccess);

        /// <summary>
        /// Queries the service config2.
        /// </summary>
        /// <param name="hService">The h service.</param>
        /// <param name="dwInfoLevel">The dw info level.</param>
        /// <param name="lpBuffer">The lp buffer.</param>
        /// <param name="cbBufSize">Size of the cb buf.</param>
        /// <param name="pcbBytesNeeded">The PCB bytes needed.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
        [DllImport("advapi32.dll", EntryPoint = "QueryServiceConfig2")]
        public static extern int QueryServiceConfig2(
            IntPtr hService,
            ServiceConfig2InfoLevelEnum dwInfoLevel,
            IntPtr lpBuffer,
            int cbBufSize,
            out int pcbBytesNeeded);

        /// <summary>
        /// Changes the service config2.
        /// </summary>
        /// <param name="hService">The h service.</param>
        /// <param name="dwInfoLevel">The dw info level.</param>
        /// <param name="lpInfo">The lp info.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2")]
        public static extern int ChangeServiceConfig2(
            IntPtr hService,
            ServiceConfig2InfoLevelEnum dwInfoLevel,
            IntPtr lpInfo);

    }

}
