/* *********************************************************************
 * Date: 26 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Legacy;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Forge.WindowsServiceControl
{

    #region Win32 API Structs

    struct SERVICE_FAILURE_ACTIONS
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint dwResetPeriod;

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpRebootMsg;

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpCommand;

        [MarshalAs(UnmanagedType.U4)]
        public uint cActions;

        public IntPtr lpsaActions;
    }

    struct SC_ACTION
    {
        [MarshalAs(UnmanagedType.U4)]
        public ServiceActionTypeEnum Type;

        [MarshalAs(UnmanagedType.U4)]
        public uint Delay;
    }

    #endregion

    /// <summary>
    /// Represents the Service Control Manager
    /// </summary>
    public class ServiceControlManager : MBRBase, IDisposable
    {

        #region Field(s)

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
        private IntPtr mSCManager;

        private bool mDisposed = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceControlManager" /> class.
        /// </summary>
        /// <exception cref="System.ComponentModel.Win32Exception">Unable to open Service Control Manager.</exception>
        public ServiceControlManager()
        {
            // Open the service control manager
            mSCManager = NativeMethods.OpenSCManager(
                null,
                null,
                ServiceControlAccessRightsEnum.SC_MANAGER_CONNECT);

            // Verify if the SC is opened
            if (mSCManager == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to open Service Control Manager.");
            }
        }

        /// <summary>
        /// Finalizer for the <see cref="ServiceControlManager"/> class.
        /// </summary>
        ~ServiceControlManager()
        {
            Dispose(false);
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Dertermines whether the nominated service is set to restart on failure.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>
        ///   <c>true</c> if [has restart on failure] [the specified service name]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">Unable to query the Service configuration.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Forge.WindowsServiceControl.NativeMethods.CloseServiceHandle(System.IntPtr)")]
        public bool HasRestartOnFailure(string serviceName)
        {
            const int bufferSize = 1024 * 8;

            IntPtr service = IntPtr.Zero;
            IntPtr bufferPtr = IntPtr.Zero;
            bool result = false;

            try
            {
                // Open the service
                service = OpenService(serviceName, ServiceAccessRightsEnum.SERVICE_QUERY_CONFIG);

                int dwBytesNeeded = 0;

                // Allocate memory for struct
                bufferPtr = Marshal.AllocHGlobal(bufferSize);
                int queryResult = NativeMethods.QueryServiceConfig2(
                    service,
                    ServiceConfig2InfoLevelEnum.SERVICE_CONFIG_FAILURE_ACTIONS,
                    bufferPtr,
                    bufferSize,
                    out dwBytesNeeded);

                if (queryResult == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to query the Service configuration.");
                }

                // Cast the buffer to a QUERY_SERVICE_CONFIG struct
                SERVICE_FAILURE_ACTIONS config =
                    (SERVICE_FAILURE_ACTIONS)Marshal.PtrToStructure(bufferPtr, typeof(SERVICE_FAILURE_ACTIONS));

                // Determine whether the service is set to auto restart
                if (config.cActions != 0)
                {
                    SC_ACTION action = (SC_ACTION)Marshal.PtrToStructure(config.lpsaActions, typeof(SC_ACTION));
                    result = (action.Type == ServiceActionTypeEnum.SC_ACTION_RESTART);
                }

                return result;
            }
            finally
            {
                // Clean up
                if (bufferPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(bufferPtr);
                }

                if (service != IntPtr.Zero)
                {
                    NativeMethods.CloseServiceHandle(service);
                }
            }
        }

        /// <summary>
        /// Sets the nominated service to restart on failure.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <exception cref="System.ComponentModel.Win32Exception">Unable to change the Service configuration.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Forge.WindowsServiceControl.NativeMethods.CloseServiceHandle(System.IntPtr)")]
        public void SetRestartOnFailure(string serviceName)
        {
            const int actionCount = 2;
            const uint delay = 60000;

            IntPtr service = IntPtr.Zero;
            IntPtr failureActionsPtr = IntPtr.Zero;
            IntPtr actionPtr = IntPtr.Zero;

            try
            {
                // Open the service
                service = OpenService(serviceName,
                    ServiceAccessRightsEnum.SERVICE_CHANGE_CONFIG |
                    ServiceAccessRightsEnum.SERVICE_START);

                // Allocate memory for the individual actions
                actionPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SC_ACTION)) * actionCount);

                // Set up the restart action
                SC_ACTION action1 = new SC_ACTION();
                action1.Type = ServiceActionTypeEnum.SC_ACTION_RESTART;
                action1.Delay = delay;
                Marshal.StructureToPtr(action1, actionPtr, false);

                // Set up the restart action
                SC_ACTION action2 = new SC_ACTION();
                action2.Type = ServiceActionTypeEnum.SC_ACTION_RESTART;
                action2.Delay = delay;
                Marshal.StructureToPtr(action2, (IntPtr)((Int64)actionPtr + Marshal.SizeOf(typeof(SC_ACTION))), false);

                // Set up the restart action
                SC_ACTION action3 = new SC_ACTION();
                action3.Type = ServiceActionTypeEnum.SC_ACTION_RESTART;
                action3.Delay = delay;
                Marshal.StructureToPtr(action3, (IntPtr)((Int64)actionPtr + Marshal.SizeOf(typeof(SC_ACTION))), false);

                // Set up the failure actions
                SERVICE_FAILURE_ACTIONS failureActions = new SERVICE_FAILURE_ACTIONS();
                failureActions.dwResetPeriod = 0;
                failureActions.cActions = actionCount;
                failureActions.lpsaActions = actionPtr;

                failureActionsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SERVICE_FAILURE_ACTIONS)));
                Marshal.StructureToPtr(failureActions, failureActionsPtr, false);

                // Make the change
                int changeResult = NativeMethods.ChangeServiceConfig2(
                    service,
                    ServiceConfig2InfoLevelEnum.SERVICE_CONFIG_FAILURE_ACTIONS,
                    failureActionsPtr);

                // Check that the change occurred
                if (changeResult == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to change the Service configuration.");
                }
            }
            finally
            {
                // Clean up
                if (failureActionsPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(failureActionsPtr);
                }

                if (actionPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(actionPtr);
                }

                if (service != IntPtr.Zero)
                {
                    NativeMethods.CloseServiceHandle(service);
                }
            }
        }

        /// <summary>
        /// See <see cref="IDisposable.Dispose" />.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Implements the Dispose(bool) pattern outlined by MSDN and enforced by FxCop.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Forge.WindowsServiceControl.NativeMethods.CloseServiceHandle(System.IntPtr)")]
        protected virtual void Dispose(bool disposing)
        {
            if (!mDisposed)
            {
                // Unmanaged resources always need disposing
                if (mSCManager != IntPtr.Zero)
                {
                    NativeMethods.CloseServiceHandle(mSCManager);
                    mSCManager = IntPtr.Zero;
                }
                mDisposed = true;
            }
        }

        #endregion

        #region Private method(s)

        /// <summary>
        /// Calls the Win32 OpenService function and performs error checking.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="desiredAccess">The desired access.</param>
        /// <returns></returns>
        /// <exception cref="System.ComponentModel.Win32Exception">Unable to open the requested Service.</exception>
        private IntPtr OpenService(string serviceName, ServiceAccessRightsEnum desiredAccess)
        {
            // Open the service
            IntPtr service = NativeMethods.OpenService(
                mSCManager,
                serviceName,
                desiredAccess);

            // Verify if the service is opened
            if (service == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to open the requested Service.");
            }

            return service;
        }

        #endregion

    }

}
