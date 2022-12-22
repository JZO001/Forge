/* *********************************************************************
 * Date: 14 Aug 2009
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

#if IS_WINDOWS

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Forge.Legacy;
using Forge.Shared;
using NetFwTypeLib;

namespace Forge.Net.Synapse.Firewall
{

    /// <summary>
    /// Represents the Windows Firewall Manager implementation
    /// </summary>
    public sealed class WindowsFirewallManager : MBRBase, IDisposable
    {

        #region Field(s)

        private INetFwProfile mFirewallProfile = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mInitialized = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mDisposed = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsFirewallManager"/> class.
        /// </summary>
        public WindowsFirewallManager()
        {
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="WindowsFirewallManager"/> is reclaimed by garbage collection.
        /// </summary>
        ~WindowsFirewallManager()
        {
            Dispose(false);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool IsInitialized
        {
            get { return mInitialized; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool IsDisposed
        {
            get { return mDisposed; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Initialize()
        {
            DoDisposeCheck();
            if (mFirewallProfile == null)
            {
                Type firewallManagerType = null;
                INetFwMgr firewallManager = null;

                firewallManagerType = Type.GetTypeFromCLSID(new Guid("{304CE942-6E39-40D8-943A-B913C40C9CD4}"));
                firewallManager = (INetFwMgr)Activator.CreateInstance(firewallManagerType);

                if (firewallManager == null)
                {
                    throw new InitializationException("Failed to create Settings Manager instance.");
                }
                else
                {
                    INetFwPolicy fwPolicy = firewallManager.LocalPolicy;

                    if (fwPolicy == null)
                    {
                        throw new InitializationException("Failed to get local policy.");
                    }
                    else
                    {
                        try
                        {
                            mFirewallProfile = fwPolicy.GetProfileByType(firewallManager.CurrentProfileType);
                            mInitialized = true;
                        }
                        catch (Exception ex)
                        {
                            throw new InitializationException("Failed to get firewall profile.", ex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether [is windows firewall on].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is windows firewall on]; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool IsWindowsFirewallOn()
        {
            DoDisposeCheck();
            DoInitializationCheck();
            return mFirewallProfile.FirewallEnabled;
        }

        /// <summary>
        /// Turn on the windows firewall.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void TurnOnWindowsFirewall()
        {
            DoDisposeCheck();
            DoInitializationCheck();

            if (!IsWindowsFirewallOn())
            {
                mFirewallProfile.FirewallEnabled = true;
            }
        }

        /// <summary>
        /// Turns the off windows firewall.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void TurnOffWindowsFirewall()
        {
            DoDisposeCheck();
            DoInitializationCheck();

            if (IsWindowsFirewallOn())
            {
                mFirewallProfile.FirewallEnabled = false;
            }
        }

        /// <summary>
        /// Determines whether [is application enabled] [the specified application file name with path].
        /// </summary>
        /// <param name="applicationFileNameWithPath">The application file name with path.</param>
        /// <returns>
        ///   <c>true</c> if [is application enabled] [the specified application file name with path]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="FirewallException">Failed to get authorized applications.</exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool IsApplicationEnabled(string applicationFileNameWithPath)
        {
            DoDisposeCheck();
            DoInitializationCheck();
            if (string.IsNullOrEmpty(applicationFileNameWithPath))
            {
                ThrowHelper.ThrowArgumentNullException("applicationFileNameWithPath");
            }

            INetFwAuthorizedApplications firewallApps = mFirewallProfile.AuthorizedApplications;
            if (firewallApps == null)
            {
                throw new FirewallException("Failed to get authorized applications.");
            }

            bool result = false;

            try
            {
                INetFwAuthorizedApplication firewallApp = firewallApps.Item(applicationFileNameWithPath);
                // If FAILED, the application is not in the collection list
                if (firewallApp != null)
                {
                    result = firewallApp.Enabled;
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        /// <summary>
        /// Adds the application.
        /// </summary>
        /// <param name="applicationFileNameWithPath">The application file name with path.</param>
        /// <param name="registeredName">Name of the registered.</param>
        /// <returns>True, if the exception registration was successful, otherwise False.</returns>
        /// <exception cref="FirewallException">
        /// Failed to get authorized applications.
        /// or
        /// Failed to authorize application.
        /// or
        /// Failed to authorize application.
        /// </exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool AddApplication(string applicationFileNameWithPath, string registeredName)
        {
            DoDisposeCheck();
            DoInitializationCheck();
            if (string.IsNullOrEmpty(applicationFileNameWithPath))
            {
                ThrowHelper.ThrowArgumentNullException("applicationFileNameWithPath");
            }
            if (string.IsNullOrEmpty(registeredName))
            {
                ThrowHelper.ThrowArgumentNullException("registeredName");
            }

            bool changed = false;

            // First of all, check the application is already authorized;
            if (!IsApplicationEnabled(applicationFileNameWithPath))
            {
                // Retrieve the authorized application collection
                INetFwAuthorizedApplications firewallApplications = mFirewallProfile.AuthorizedApplications;
                if (firewallApplications == null)
                {
                    throw new FirewallException("Failed to get authorized applications.");
                }

                // Create an instance of an authorized application
                Type firewallApplicationType = Type.GetTypeFromCLSID(new Guid("{EC9846B3-2762-4A6B-A214-6ACB603462D2}"));

                INetFwAuthorizedApplication firewallApplication = (INetFwAuthorizedApplication)Activator.CreateInstance(firewallApplicationType);
                if (firewallApplication == null)
                {
                    throw new FirewallException("Failed to authorize application.");
                }

                // Set the process image file name
                firewallApplication.ProcessImageFileName = applicationFileNameWithPath;
                firewallApplication.Name = registeredName;

                try
                {
                    firewallApplications.Add(firewallApplication);
                    changed = true;
                }
                catch (Exception)
                {
                    throw new FirewallException("Failed to authorize application.");
                }
            }

            return changed;
        }

        /// <summary>
        /// Removes the application.
        /// </summary>
        /// <param name="applicationFileNameWithPath">The application file name with path.</param>
        /// <returns>True, if the unregister of the exception was successful, otherwise False.</returns>
        /// <exception cref="FirewallException">
        /// Failed to get authorized applications.
        /// or
        /// Failed to delete authorization for the application.
        /// </exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool RemoveApplication(string applicationFileNameWithPath)
        {
            DoDisposeCheck();
            DoInitializationCheck();
            if (string.IsNullOrEmpty(applicationFileNameWithPath))
            {
                ThrowHelper.ThrowArgumentNullException("applicationFileNameWithPath");
            }

            bool changed = true;

            if (IsApplicationEnabled(applicationFileNameWithPath))
            {
                // Retrieve the authorized application collection
                INetFwAuthorizedApplications firewallApplications = mFirewallProfile.AuthorizedApplications;
                if (firewallApplications == null)
                {
                    throw new FirewallException("Failed to get authorized applications.");
                }

                try
                {
                    firewallApplications.Remove(applicationFileNameWithPath);
                    changed = true;
                }
                catch (Exception)
                {
                    throw new FirewallException("Failed to delete authorization for the application.");
                }
            }

            return changed;
        }

        /// <summary>
        /// Determines whether [is port enabled] [the specified port].
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="ipProtocol">The ip protocol.</param>
        /// <returns>
        ///   <c>true</c> if [is port enabled] [the specified port]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="FirewallException">Failed to get globally open ports.</exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool IsPortEnabled(int port, NET_FW_IP_PROTOCOL_ ipProtocol)
        {
            DoDisposeCheck();
            DoInitializationCheck();

            bool result = false;

            // Retrieve the open ports collection
            INetFwOpenPorts firewallOpenPorts = mFirewallProfile.GloballyOpenPorts;
            if (firewallOpenPorts == null)
            {
                throw new FirewallException("Failed to get globally open ports.");
            }

            // Get the open port
            try
            {
                INetFwOpenPort firewallOpenPort = firewallOpenPorts.Item(port, ipProtocol);
                if (firewallOpenPort != null)
                {
                    result = firewallOpenPort.Enabled;
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        /// <summary>
        /// Adds the port.
        /// </summary>
        /// <param name="port">The n port number.</param>
        /// <param name="ipProtocol">The ip protocol.</param>
        /// <param name="registeredName">Name of the registered.</param>
        /// <returns>True, if the port adding was successful</returns>
        /// <exception cref="FirewallException">
        /// Failed to get globally open ports.
        /// or
        /// Failed to create port instance.
        /// or
        /// Failed to add port.
        /// </exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool AddPort(int port, NET_FW_IP_PROTOCOL_ ipProtocol, string registeredName)
        {
            DoDisposeCheck();
            DoInitializationCheck();
            if (string.IsNullOrEmpty(registeredName))
            {
                ThrowHelper.ThrowArgumentNullException("registeredName");
            }

            bool result = false;

            if (!IsPortEnabled(port, ipProtocol))
            {
                // Retrieve the collection of globally open ports
                INetFwOpenPorts firewallOpenPorts = mFirewallProfile.GloballyOpenPorts;
                if (firewallOpenPorts == null)
                {
                    throw new FirewallException("Failed to get globally open ports.");
                }

                // Create an instance of an open port
                Type firewallPortType = Type.GetTypeFromCLSID(new Guid("{0CA545C6-37AD-4A6C-BF92-9F7610067EF5}"));
                INetFwOpenPort firewallOpenPort = (INetFwOpenPort)Activator.CreateInstance(firewallPortType);
                if (firewallOpenPort == null)
                {
                    throw new FirewallException("Failed to create port instance.");
                }

                // Set the port number
                firewallOpenPort.Port = port;

                // Set the IP Protocol
                firewallOpenPort.Protocol = ipProtocol;

                // Set the registered name
                firewallOpenPort.Name = registeredName;

                try
                {
                    firewallOpenPorts.Add(firewallOpenPort);
                    result = true;
                }
                catch (Exception ex)
                {
                    throw new FirewallException("Failed to add port.", ex);
                }
            }

            return result;
        }

        /// <summary>
        /// Removes the port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="ipProtocol">The ip protocol.</param>
        /// <returns>True, if the revocation was successful</returns>
        /// <exception cref="FirewallException">
        /// Failed to get globally open ports.
        /// or
        /// Failed to remove port.
        /// </exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool RemovePort(int port, NET_FW_IP_PROTOCOL_ ipProtocol)
        {
            DoDisposeCheck();
            DoInitializationCheck();

            bool changed = false;

            if (IsPortEnabled(port, ipProtocol))
            {
                // Retrieve the collection of globally open ports
                INetFwOpenPorts firewallOpenPorts = mFirewallProfile.GloballyOpenPorts;
                if (firewallOpenPorts == null)
                {
                    throw new FirewallException("Failed to get globally open ports.");
                }

                try
                {
                    firewallOpenPorts.Remove(port, ipProtocol);
                    changed = true;
                }
                catch (Exception ex)
                {
                    throw new FirewallException("Failed to remove port.", ex);
                }
            }

            return changed;
        }

        /// <summary>
        /// Determines whether [is exception not allowed].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is exception not allowed]; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool IsExceptionNotAllowed()
        {
            DoDisposeCheck();
            DoInitializationCheck();

            return mFirewallProfile.ExceptionsNotAllowed;
        }

        /// <summary>
        /// Sets the exception not allowed.
        /// </summary>
        /// <param name="notAllowed">if set to <c>true</c> [not allowed].</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetExceptionNotAllowed(bool notAllowed)
        {
            DoDisposeCheck();
            DoInitializationCheck();

            mFirewallProfile.ExceptionsNotAllowed = notAllowed;
        }

        /// <summary>
        /// Determines whether [is notification disabled].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is notification disabled]; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool IsNotificationDisabled()
        {
            DoDisposeCheck();
            DoInitializationCheck();

            return mFirewallProfile.NotificationsDisabled;
        }

        /// <summary>
        /// Sets the notification disabled.
        /// </summary>
        /// <param name="disabled">if set to <c>true</c> [disabled].</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetNotificationDisabled(bool disabled)
        {
            DoDisposeCheck();
            DoInitializationCheck();

            mFirewallProfile.NotificationsDisabled = disabled;
        }

        /// <summary>
        /// Determines whether [is unicast responses to multicast broadcast disabled].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is unicast responses to multicast broadcast disabled]; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool IsUnicastResponsesToMulticastBroadcastDisabled()
        {
            DoDisposeCheck();
            DoInitializationCheck();

            return mFirewallProfile.UnicastResponsesToMulticastBroadcastDisabled;
        }

        /// <summary>
        /// Sets the unicast responses to multicast broadcast disabled.
        /// </summary>
        /// <param name="disabled">if set to <c>true</c> [disabled].</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetUnicastResponsesToMulticastBroadcastDisabled(bool disabled)
        {
            DoDisposeCheck();
            DoInitializationCheck();

            mFirewallProfile.UnicastResponsesToMulticastBroadcastDisabled = disabled;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private method(s)

        private void DoInitializationCheck()
        {
            if (!mInitialized)
            {
                throw new InitializationException();
            }
        }

        private void DoDisposeCheck()
        {
            if (mDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        private void Dispose(bool disposing)
        {
            if (!mDisposed)
            {
                if (mFirewallProfile != null)
                {
                    try
                    {
                        Marshal.ReleaseComObject(mFirewallProfile);
                    }
                    catch (Exception) { }
                    mFirewallProfile = null;
                }
                mDisposed = true;
            }
        }

        #endregion

    }

}

#endif
