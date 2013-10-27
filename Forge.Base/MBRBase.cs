/* *********************************************************************
 * Date: 13 Jun 2007
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Security.Permissions;

namespace Forge
{

    /// <summary>
    /// Base class for MarshalByRefObject classes
    /// </summary>
    [Serializable]
    public abstract class MBRBase : MarshalByRefObject
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MBRBase"/> class.
        /// </summary>
        [DebuggerHidden]
        protected MBRBase()
            : base()
        {
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease" /> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime" /> property.
        /// </returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure" />
        ///   </PermissionSet>
        /// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            //ILease lease = (ILease)base.InitializeLifetimeService();
            //if (lease.CurrentState == LeaseState.Initial)
            //{
            //    lease.InitialLeaseTime = TimeSpan.Zero; // lease time does not expire
            //}
            //return lease;
            return null;
        }

    }
}
