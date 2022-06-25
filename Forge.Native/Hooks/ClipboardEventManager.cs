/* *********************************************************************
 * Date: 8 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

#if IS_WINDOWS

using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Forge.Management;

namespace Forge.Native.Hooks
{

    /// <summary>
    /// Manages clipboard changes
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public sealed class ClipboardEventManager : ManagerSingletonBase<ClipboardEventManager>
    {

        #region Field(s)

        private ClipboardViewer mViewer = null;

        /// <summary>
        /// Occurs when [event clipboard changed].
        /// </summary>
        public event EventHandler<ClipboardChangedEventArgs> EventClipboardChanged;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Prevents a default instance of the <see cref="ClipboardEventManager"/> class from being created.
        /// </summary>
        private ClipboardEventManager()
            : base()
        {
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Starts this manager instance.
        /// </summary>
        /// <returns>
        /// Manager State
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override ManagerStateEnum Start()
        {
            if (this.ManagerState != ManagerStateEnum.Started)
            {
                OnStart(ManagerEventStateEnum.Before);
                try
                {
                    if (Forge.Windows.UI.ApplicationHelper.IsUIThread())
                    {
                        mViewer = new ClipboardViewer(this);
                        this.ManagerState = ManagerStateEnum.Started;
                    }
                    else
                    {
                        throw new InvalidOperationException("Manager must be started on the STA thread.");
                    }
                }
                catch (Exception)
                {
                    this.ManagerState = ManagerStateEnum.Fault;
                    throw;
                }
                finally
                {
                    OnStart(ManagerEventStateEnum.After);
                }
            }
            return this.ManagerState;
        }

        /// <summary>
        /// Stops this manager instance.
        /// </summary>
        /// <returns>
        /// Manager State
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override ManagerStateEnum Stop()
        {
            if (this.ManagerState == ManagerStateEnum.Started)
            {
                OnStop(ManagerEventStateEnum.Before);
                try
                {
                    mViewer.Dispose();
                    mViewer = null;
                    this.ManagerState = ManagerStateEnum.Stopped;
                }
                catch (Exception)
                {
                    this.ManagerState = ManagerStateEnum.Fault;
                    throw;
                }
                finally
                {
                    OnStop(ManagerEventStateEnum.After);
                }
            }
            return this.ManagerState;
        }

        #endregion

        #region Private method(s)

        private void OnClipboardChanged()
        {
            RaiseEvent(EventClipboardChanged, this, new ClipboardChangedEventArgs(Clipboard.GetDataObject()));
        }

        #endregion

        #region Nested type(s)

        private sealed class ClipboardViewer : Control
        {

            #region Field(s)

            private const int WM_DRAWCLIPBOARD = 0x308;
            private const int WM_CHANGECBCHAIN = 0x030D;
            private const int WM_CLIPBOARDUPDATE = 0x031D;
            private static readonly IntPtr HWND_MESSAGE = new IntPtr(-3); // represents the message only window

            private IntPtr mNextClipboardViewer;

            private ClipboardEventManager mManager = null;

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="ClipboardViewer" /> class.
            /// </summary>
            /// <param name="manager">The manager.</param>
            public ClipboardViewer(ClipboardEventManager manager)
                : base()
            {
                this.mManager = manager;
                if (UseNewerMethod)
                {
                    NativeMethods.SetParent(this.Handle, HWND_MESSAGE);
                    NativeMethods.AddClipboardFormatListener(this.Handle);
                }
                else
                {
                    mNextClipboardViewer = NativeMethods.SetClipboardViewer(this.Handle);
                }
            }

            #endregion

            #region Protected method(s)

            /// <summary>
            /// Processes Windows messages.
            /// </summary>
            /// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message" /> to process.</param>
            protected override void WndProc(ref System.Windows.Forms.Message m)
            {
                if (UseNewerMethod)
                {
                    if (m.Msg == WM_CLIPBOARDUPDATE)
                    {
                        mManager.OnClipboardChanged();
                    }
                    else
                    {
                        base.WndProc(ref m);
                    }
                }
                else
                {
                    switch (m.Msg)
                    {
                        case WM_DRAWCLIPBOARD:
                            {
                                mManager.OnClipboardChanged();
                                NativeMethods.SendMessage(mNextClipboardViewer, m.Msg, m.WParam, m.LParam);
                            }
                            break;

                        case WM_CHANGECBCHAIN:
                            {
                                if (m.WParam == mNextClipboardViewer)
                                {
                                    mNextClipboardViewer = m.LParam;
                                }
                                else
                                {
                                    NativeMethods.SendMessage(mNextClipboardViewer, m.Msg, m.WParam, m.LParam);
                                }
                            }
                            break;

                        default:
                            base.WndProc(ref m);
                            break;
                    }
                }
            }

            /// <summary>
            /// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control" /> and its child controls and optionally releases the managed resources.
            /// </summary>
            /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
            protected override void Dispose(bool disposing)
            {
                if (this.IsHandleCreated)
                {
                    if (UseNewerMethod)
                    {
                        NativeMethods.RemoveClipboardFormatListener(this.Handle);
                    }
                    else if (mNextClipboardViewer != null && mNextClipboardViewer.ToInt64() != 0)
                    {
                        NativeMethods.ChangeClipboardChain(this.Handle, mNextClipboardViewer);
                    }
                }
                base.Dispose(disposing);
            }

            #endregion

            #region Private method(s)

            private bool UseNewerMethod
            {
                get
                {
                    return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major >= 6);
                }
            }

            #endregion

        }

        #endregion

    }

}

#endif
