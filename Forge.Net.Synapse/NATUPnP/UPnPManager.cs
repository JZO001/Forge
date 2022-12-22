/* *********************************************************************
 * Date: 14 Aug 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

#if IS_WINDOWS

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Forge.Legacy;
using Forge.Shared;
using NATUPNPLib;

namespace Forge.Net.Synapse.NATUPnP
{

    /// <summary>
    /// Represents a NAT UPnP Manager implementation
    /// </summary>
    public sealed class UPnPManager : MBRBase, IDisposable
    {

        #region Field(s)

        private UPnPNAT mNatMgr = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mInitialized = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mDisposed = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="UPnPManager"/> class.
        /// </summary>
        public UPnPManager()
        {
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="UPnPManager"/> is reclaimed by garbage collection.
        /// </summary>
        ~UPnPManager()
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
            if (!mInitialized)
            {
                mNatMgr = new UPnPNAT();
                mInitialized = true;
            }
        }

        /// <summary>
        /// Adds the NATUPnP port mapping.
        /// </summary>
        /// <param name="externalPort">The external port.</param>
        /// <param name="protocol">The protocol.</param>
        /// <param name="internalPort">The internal port.</param>
        /// <param name="localIp">The local ip.</param>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <param name="description">The description.</param>
        /// <returns>Static Port Mapping info</returns>
        public IStaticPortMapping AddNATUPnPPortMapping(int externalPort, ProtocolEnum protocol, int internalPort, IPAddress localIp, bool enable, string description)
        {
            DoDisposeCheck();
            DoInitializationCheck();
            if (externalPort < 1 || externalPort > 65535)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("externalPort");
            }
            if (internalPort < 1 || internalPort > 65535)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("internalPort");
            }
            if (localIp == null)
            {
                ThrowHelper.ThrowArgumentNullException("localIp");
            }

            IStaticPortMapping result = null;

            try
            {
                IStaticPortMappingCollection mappings = mNatMgr.StaticPortMappingCollection;
                if (mappings != null)
                {
                    result = mappings.Add(externalPort, protocol.ToString(), internalPort, localIp.ToString(), enable, string.IsNullOrEmpty(description) ? string.Empty : description);
                }
            }
            catch (Exception) { }

            return result;
        }

        /// <summary>
        /// Deletes the NATUPnP port mapping.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="protocol">The protocol.</param>
        /// <returns>True, if the port mapping removed, otherwise False.</returns>
        public bool DeleteNATUPnPPortMapping(int port, ProtocolEnum protocol)
        {
            DoDisposeCheck();
            DoInitializationCheck();

            if (port < 1 || port > 65535)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("port");
            }

            try
            {
                IStaticPortMappingCollection mappings = mNatMgr.StaticPortMappingCollection;
                mappings.Remove(port, protocol.ToString());
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the static port mappings.
        /// </summary>
        /// <returns>Collection of the static port mappings</returns>
        public ICollection<IStaticPortMapping> GetStaticPortMappings()
        {
            DoDisposeCheck();
            DoInitializationCheck();

            List<IStaticPortMapping> result = new List<IStaticPortMapping>();

            IStaticPortMappingCollection staticMappings = mNatMgr.StaticPortMappingCollection;
            if (staticMappings != null && staticMappings.Count > 0)
            {
                int index = 1;
                foreach (IStaticPortMapping pm in staticMappings)
                {
                    result.Add(pm);
                    if (index == staticMappings.Count)
                    {
                        break;
                    }
                    index++;
                }
            }

            return result;
        }

        /// <summary>
        /// Clears the mappings.
        /// </summary>
        public void Clear()
        {
            DoDisposeCheck();
            DoInitializationCheck();

            ICollection<IStaticPortMapping> mappings = GetStaticPortMappings();
            if (mappings.Count > 0)
            {
                IStaticPortMappingCollection staticMappings = mNatMgr.StaticPortMappingCollection;
                if (staticMappings != null && staticMappings.Count > 0)
                {
                    foreach (IStaticPortMapping m in mappings)
                    {
                        staticMappings.Remove(m.ExternalPort, m.Protocol);
                    }
                }
                mappings.Clear();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
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
                if (mNatMgr != null)
                {
                    try
                    {
                        Marshal.ReleaseComObject(mNatMgr);
                    }
                    catch (Exception) { }
                    mNatMgr = null;
                }
                mDisposed = true;
            }
        }

        #endregion

    }

}

#endif
