/* *********************************************************************
 * Date: 30 Jul 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Forge.Management;
using Forge.Native.Structures;

namespace Forge.Native.Hooks
{

    /// <summary>
    /// Mouse event hook manager singleton
    /// </summary>
    public sealed class MouseEventHookManager : ManagerSingletonBase<MouseEventHookManager>
    {

        #region Field(s)

        /// <summary>
        /// This field is not objectively needed but we need to keep a reference on a delegate which will be 
        /// passed to unmanaged code. To avoid GC to clean it up.
        /// When passing delegates to unmanaged code, they must be kept alive by the managed application 
        /// until it is guaranteed that they will never be called.
        /// </summary>
        private HookProcessDelegate mMouseDelegateHandler;

        /// <summary>
        /// Stores the handle to the mouse hook procedure.
        /// </summary>
        private int mMouseHookHandle;

        // the last mouse X coordinate
        private int mOldX;

        // the last mouse Y coordinate
        private int mOldY;

        // This field remembers mouse button pressed because in addition to the short interval it must be also the same button.
        private MouseButtons mPrevClickedButton;

        // The timer to monitor time interval between two clicks.
        private Timer mDoubleClickTimer;

        private readonly object mLockObjectForEvents = new object();

        private event MouseEventHandler mMouseDoubleClick;

        private event EventHandler<MouseEventExtendedArgs> mMouseDoubleClickExtended;

        /// <summary>
        /// Occurs when the mouse pointer is moved. 
        /// </summary>
        public event MouseEventHandler MouseMove;

        /// <summary>
        /// Occurs when the mouse pointer is moved. 
        /// </summary>
        /// <remarks>
        /// This event provides extended arguments of type <see cref="MouseEventArgs"/> enabling you to 
        /// supress further processing of mouse movement in other applications.
        /// </remarks>
        public event EventHandler<MouseEventExtendedArgs> MouseMoveExtended;

        /// <summary>
        /// Occurs when a click was performed by the mouse. 
        /// </summary>
        public event MouseEventHandler MouseClick;

        /// <summary>
        /// Occurs when a click was performed by the mouse. 
        /// </summary>
        /// <remarks>
        /// This event provides extended arguments of type <see cref="MouseEventArgs"/> enabling you to 
        /// supress further processing of mouse click in other applications.
        /// </remarks>
        public event EventHandler<MouseEventExtendedArgs> MouseClickExtended;

        /// <summary>
        /// Occurs when the mouse a mouse button is pressed. 
        /// </summary>
        public event MouseEventHandler MouseDown;

        /// <summary>
        /// Occurs when [mouse down extended].
        /// </summary>
        public event EventHandler<MouseEventExtendedArgs> MouseDownExtended;

        /// <summary>
        /// Occurs when a mouse button is released. 
        /// </summary>
        public event MouseEventHandler MouseUp;

        /// <summary>
        /// Occurs when [mouse up extended].
        /// </summary>
        public event EventHandler<MouseEventExtendedArgs> MouseUpExtended;

        /// <summary>
        /// Occurs when the mouse wheel moves. 
        /// </summary>
        public event MouseEventHandler MouseWheelVertical;

        /// <summary>
        /// Occurs when [mouse wheel extended].
        /// </summary>
        public event EventHandler<MouseEventExtendedArgs> MouseWheelVerticalExtended;

        /// <summary>
        /// Occurs when [mouse wheel horizontal].
        /// </summary>
        public event MouseEventHandler MouseWheelHorizontal;

        /// <summary>
        /// Occurs when [mouse wheel horizontal extended].
        /// </summary>
        public event EventHandler<MouseEventExtendedArgs> MouseWheelHorizontalExtended;

        /// <summary>
        /// Occurs when a double clicked was performed by the mouse. 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public event MouseEventHandler MouseDoubleClick
        {
            add
            {
                lock (mLockObjectForEvents)
                {
                    // The double click event will not be provided directly from hook.
                    // To fire the double click event wee need to monitor mouse up event and when it occures 
                    // Two times during the time interval which is defined in Windows as a doble click time
                    // we fire this event.
                    if (mMouseDoubleClick == null && mMouseDoubleClickExtended == null)
                    {
                        // We create a timer to monitor interval between two clicks
                        mDoubleClickTimer = new Timer
                        {
                            // This interval will be set to the value we retrive from windows. This is a windows setting from contro planel.
                            Interval = System.Windows.Forms.SystemInformation.DoubleClickTime,
                            // We do not start timer yet. It will be start when the click occures.
                            Enabled = false
                        };
                        // We define the callback function for the timer
                        mDoubleClickTimer.Tick += DoubleClickTimeElapsed;
                        // We start to monitor mouse up event.
                        MouseUpExtended += OnMouseUpExtended;
                    }
                    mMouseDoubleClick += value;
                }
            }
            remove
            {
                lock (mLockObjectForEvents)
                {
                    if (mMouseDoubleClick != null)
                    {
                        mMouseDoubleClick -= value;
                        if (mMouseDoubleClick == null && mMouseDoubleClickExtended == null)
                        {
                            // Stop monitoring mouse up
                            MouseUpExtended -= OnMouseUpExtended;
                            // Dispose the timer
                            mDoubleClickTimer.Tick -= DoubleClickTimeElapsed;
                            mDoubleClickTimer = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when [mouse double click extended].
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public event EventHandler<MouseEventExtendedArgs> MouseDoubleClickExtended
        {
            add
            {
                lock (mLockObjectForEvents)
                {
                    // The double click event will not be provided directly from hook.
                    // To fire the double click event wee need to monitor mouse up event and when it occures 
                    // Two times during the time interval which is defined in Windows as a doble click time
                    // we fire this event.
                    if (mMouseDoubleClick == null && mMouseDoubleClickExtended == null)
                    {
                        // We create a timer to monitor interval between two clicks
                        mDoubleClickTimer = new Timer
                        {
                            // This interval will be set to the value we retrive from windows. This is a windows setting from contro planel.
                            Interval = System.Windows.Forms.SystemInformation.DoubleClickTime,
                            // We do not start timer yet. It will be start when the click occures.
                            Enabled = false
                        };
                        // We define the callback function for the timer
                        mDoubleClickTimer.Tick += DoubleClickTimeElapsed;
                        // We start to monitor mouse up event.
                        MouseUpExtended += OnMouseUpExtended;
                    }
                    mMouseDoubleClickExtended += value;
                }
            }
            remove
            {
                lock (mLockObjectForEvents)
                {
                    if (mMouseDoubleClickExtended != null)
                    {
                        mMouseDoubleClickExtended -= value;
                        if (mMouseDoubleClick == null && mMouseDoubleClickExtended == null)
                        {
                            // Stop monitoring mouse up
                            MouseUpExtended -= OnMouseUpExtended;
                            // Dispose the timer
                            mDoubleClickTimer.Tick -= DoubleClickTimeElapsed;
                            mDoubleClickTimer = null;
                        }
                    }
                }
            }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Prevents a default instance of the <see cref="MouseEventHookManager"/> class from being created.
        /// </summary>
        private MouseEventHookManager()
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
                    if (ApplicationHelper.IsUIThread())
                    {
                        SubscribeToGlobalMouseEvents();
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
                    UnsunscribeFromGlobalMouseEvents();
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

        private void DoubleClickTimeElapsed(object sender, EventArgs e)
        {
            // Timer is alapsed and no second click occured
            mDoubleClickTimer.Enabled = false;
            mPrevClickedButton = MouseButtons.None;
        }

        /// <summary>
        /// This method is designed to monitor mouse clicks in order to fire a double click event if interval between 
        /// clicks was short enaugh.
        /// </summary>
        /// <param name="sender">Is always null</param>
        /// <param name="e">Some information about click heppened.</param>
        private void OnMouseUpExtended(object sender, MouseEventExtendedArgs e)
        {
            if (e.Clicks > 0)
            {
                // If the second click heppened on the same button
                if (e.Button.Equals(mPrevClickedButton))
                {
                    RaiseEvent(mMouseDoubleClick, null, e);
                    RaiseEvent(mMouseDoubleClickExtended, null, e);

                    // Stop timer
                    mDoubleClickTimer.Enabled = false;
                    mPrevClickedButton = MouseButtons.None;
                }
                else
                {
                    // If it was the first click start the timer
                    mDoubleClickTimer.Enabled = true;
                    mPrevClickedButton = e.Button;
                }
            }
        }

        /// <summary>
        /// A callback function which will be called every Time a mouse activity detected.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>
        private int MouseHookProcEventHandler(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                // Marshall the data from callback.
                MouseInput mouseHookStruct = (MouseInput)Marshal.PtrToStructure(lParam, typeof(MouseInput));

                // detect button clicked
                MouseButtons button = MouseButtons.None;
                short mouseDelta = 0;
                int clickCount = 0;
                bool mouseDown = false;
                bool mouseUp = false;
                bool wheel = false;

                MouseInputNotificationEnum hookData = (MouseInputNotificationEnum)wParam;
                switch (hookData)
                {
                    case MouseInputNotificationEnum.WM_LBUTTONDOWN:
                        mouseDown = true;
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;

                    case MouseInputNotificationEnum.WM_LBUTTONUP:
                        mouseUp = true;
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;

                    case MouseInputNotificationEnum.WM_LBUTTONDBLCLK:
                        button = MouseButtons.Left;
                        clickCount = 2;
                        break;

                    case MouseInputNotificationEnum.WM_MBUTTONDOWN:
                        mouseDown = true;
                        button = MouseButtons.Middle;
                        clickCount = 1;
                        break;

                    case MouseInputNotificationEnum.WM_MBUTTONUP:
                        mouseUp = true;
                        button = MouseButtons.Middle;
                        clickCount = 1;
                        break;

                    case MouseInputNotificationEnum.WM_MBUTTONDBLCLK:
                        button = MouseButtons.Middle;
                        clickCount = 2;
                        break;

                    case MouseInputNotificationEnum.WM_RBUTTONDOWN:
                        mouseDown = true;
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;

                    case MouseInputNotificationEnum.WM_RBUTTONUP:
                        mouseUp = true;
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;

                    case MouseInputNotificationEnum.WM_RBUTTONDBLCLK:
                        button = MouseButtons.Right;
                        clickCount = 2;
                        break;

                    case MouseInputNotificationEnum.WM_MOUSEWHEEL:
                        // If the message is WM_MOUSEWHEEL, the high-order word of MouseData member is the wheel delta. 
                        // One wheel click is defined as WHEEL_DELTA, which is 120. 
                        // (value >> 16) & 0xffff; retrieves the high-order word from the given 32-bit value
                        mouseDelta = (short)((mouseHookStruct.MouseData >> 16) & 0xffff);
                        wheel = true;
                        break;

                    case MouseInputNotificationEnum.WM_MOUSEHWHEEL:
                        // If the message is WM_MOUSEHWHEEL, the high-order word of MouseData member is the wheel delta. 
                        // One wheel click is defined as WHEEL_DELTA, which is 120. 
                        // (value >> 16) & 0xffff; retrieves the high-order word from the given 32-bit value
                        mouseDelta = (short)((mouseHookStruct.MouseData >> 16) & 0xffff);

                        // TODO: X BUTTONS (I havent them so was unable to test)
                        // If the message is WM_XBUTTONDOWN, WM_XBUTTONUP, WM_XBUTTONDBLCLK, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP, 
                        // or WM_NCXBUTTONDBLCLK, the high-order word specifies which X button was pressed or released, 
                        // and the low-order word is reserved. This value can be one or more of the following values. 
                        // Otherwise, MouseData is not used. 
                        break;
                }

                // generate event
                MouseEventExtendedArgs e = new MouseEventExtendedArgs(
                                                   button,
                                                   clickCount,
                                                   mouseHookStruct.Dx,
                                                   mouseHookStruct.Dy,
                                                   mouseDelta);

                // Mouse up
                if (mouseUp)
                {
                    RaiseEvent(MouseUp, null, e);
                    RaiseEvent(MouseUpExtended, null, e);
                }

                // Mouse down
                if (mouseDown)
                {
                    RaiseEvent(MouseDown, null, e);
                    RaiseEvent(MouseDownExtended, null, e);
                }

                // If someone listens to click and a click is happened
                if (clickCount > 0)
                {
                    RaiseEvent(MouseClick, null, e);
                    RaiseEvent(MouseClickExtended, null, e);
                }

                // If someone listens to double click and a click is happened
                if (clickCount == 2)
                {
                    //RaiseEvent(mMouseDoubleClick, e);
                    RaiseEvent(mMouseDoubleClickExtended, null, e);
                }

                // Wheel was moved
                if (mouseDelta != 0 && wheel)
                {
                    RaiseEvent(MouseWheelVertical, null, e);
                    RaiseEvent(MouseWheelVerticalExtended, null, e);
                }
                else if (mouseDelta != 0 && !wheel)
                {
                    RaiseEvent(MouseWheelHorizontal, null, e);
                    RaiseEvent(MouseWheelHorizontalExtended, null, e);
                }

                // If someone listens to move and there was a change in coordinates raise move event
                if ((MouseMove != null || MouseMoveExtended != null) && (mOldX != mouseHookStruct.Dx || mOldY != mouseHookStruct.Dy))
                {
                    mOldX = mouseHookStruct.Dx;
                    mOldY = mouseHookStruct.Dy;

                    RaiseEvent(MouseMove, null, e);
                    RaiseEvent(MouseMoveExtended, null, e);
                }

                if (e.Handled) return -1;
            }

            // call next hook
            return NativeMethods.CallNextHookEx(new IntPtr(mMouseHookHandle), nCode, wParam, lParam);
        }

        private void SubscribeToGlobalMouseEvents()
        {
            // install Mouse hook only if it is not installed and must be installed
            if (mMouseHookHandle == 0)
            {
                // See comment of this field. To avoid GC to clean it up.
                mMouseDelegateHandler = MouseHookProcEventHandler;

                // install hook
                // Marshal.GetHINSTANCE(typeof(MouseEventHookManager).Assembly.GetModules()[0])
                mMouseHookHandle = NativeMethods.SetWindowsHookEx(
                    (int)HookEventEnum.WH_MOUSE_LL,
                    mMouseDelegateHandler,
                    NativeMethods.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 
                    0);

                // If SetWindowsHookEx fails.
                if (mMouseHookHandle == 0)
                {
                    // Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    // do cleanup

                    // Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }

        private void UnsunscribeFromGlobalMouseEvents()
        {
            if (mMouseHookHandle != 0)
            {
                //uninstall hook
                int result = NativeMethods.UnhookWindowsHookEx(mMouseHookHandle);

                //reset invalid handle
                mMouseHookHandle = 0;

                //Free up for GC
                mMouseDelegateHandler = null;

                //if failed and exception must be thrown
                if (result == 0)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();

                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }

        #endregion

    }

}
