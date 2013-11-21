/* *********************************************************************
 * Date: 30 Jul 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Forge.Management;
using Forge.Native.Structures;

namespace Forge.Native.Hooks
{

    /// <summary>
    /// Keyboard event hook manager singleton
    /// </summary>
    public sealed class KeyboardEventHookManager : ManagerSingletonBase<KeyboardEventHookManager>
    {

        #region Field(s)

        /// <summary>
        /// This field is not objectively needed but we need to keep a reference on a delegate which will be 
        /// passed to unmanaged code. To avoid GC to clean it up.
        /// When passing delegates to unmanaged code, they must be kept alive by the managed application 
        /// until it is guaranteed that they will never be called.
        /// </summary>
        private HookProcessDelegate mKeyboardDelegate;

        /// <summary>
        /// Stores the handle to the Keyboard hook procedure.
        /// </summary>
        private int mKeyboardHookHandle;

        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
        /// <remarks>
        /// Key events occur in the following order: 
        /// <list type="number">
        /// <item>KeyDown</item>
        /// <item>KeyPress</item>
        /// <item>KeyUp</item>
        /// </list>
        ///The KeyPress event is not raised by noncharacter keys; however, the noncharacter keys do raise the KeyDown and KeyUp events. 
        ///Use the KeyChar property to sample keystrokes at run time and to consume or modify a subset of common keystrokes. 
        ///To handle keyboard events only in your application and not enable other applications to receive keyboard events, 
        /// set the KeyPressEventArgs.Handled property in your form's KeyPress event-handling method to <b>true</b>. 
        /// </remarks>
        public event KeyPressEventHandler KeyPress;

        /// <summary>
        /// Occurs when a key is released. 
        /// </summary>
        public event KeyEventHandler KeyUp;

        /// <summary>
        /// Occurs when a key is preseed. 
        /// </summary>
        public event KeyEventHandler KeyDown;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Prevents a default instance of the <see cref="KeyboardEventHookManager"/> class from being created.
        /// </summary>
        private KeyboardEventHookManager()
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
                    SubscribeToGlobalKeyboardEvents();
                    this.ManagerState = ManagerStateEnum.Started;
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
                    UnsunscribeFromGlobalKeyboardEvents();
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

        /// <summary>
        /// A callback function which will be called every Time a keyboard activity detected.
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
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            //indicates if any of underlaing events set e.Handled flag
            bool handled = false;

            if (nCode >= 0)
            {
                //read structure KeyboardHookStruct at lParam
                KeyboardInput myKeyboardHookStruct = (KeyboardInput)Marshal.PtrToStructure(lParam, typeof(KeyboardInput));
                //raise KeyDown
                if (KeyDown != null && (wParam == (int)KeyboardInputNotificationEnum.WM_KEYDOWN || wParam == (int)KeyboardInputNotificationEnum.WM_SYSKEYDOWN))
                {
                    Keys keyData = (Keys)myKeyboardHookStruct.WVirtualKeyCode;
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    RaiseEvent(KeyDown, null, e);
                    handled = e.Handled;
                }

                // raise KeyPress
                if (KeyPress != null && wParam == (int)KeyboardInputNotificationEnum.WM_KEYDOWN)
                {
                    bool isDownShift = ((NativeMethods.GetKeyState((int)Keys.ShiftKey) & 0x80) == 0x80 ? true : false);
                    bool isDownCapslock = (NativeMethods.GetKeyState((int)Keys.Capital) != 0 ? true : false);

                    byte[] keyState = new byte[256];
                    NativeMethods.GetKeyboardState(keyState);
                    byte[] inBuffer = new byte[2];
                    if (NativeMethods.ToAscii(myKeyboardHookStruct.WVirtualKeyCode,
                              myKeyboardHookStruct.WScanCode,
                              keyState,
                              inBuffer,
                              myKeyboardHookStruct.DwFlags) == 1)
                    {
                        char key = (char)inBuffer[0];
                        if ((isDownCapslock ^ isDownShift) && Char.IsLetter(key)) key = Char.ToUpper(key);
                        KeyPressEventArgs e = new KeyPressEventArgs(key);
                        RaiseEvent(KeyPress, null, e);
                        handled = handled || e.Handled;
                    }
                }

                // raise KeyUp
                if (KeyUp != null && (wParam == (int)KeyboardInputNotificationEnum.WM_KEYUP || wParam == (int)KeyboardInputNotificationEnum.WM_SYSKEYUP))
                {
                    Keys keyData = (Keys)myKeyboardHookStruct.WVirtualKeyCode;
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    RaiseEvent(KeyUp, null, e);
                    handled = handled || e.Handled;
                }

            }

            // if event handled in application do not handoff to other listeners
            if (handled) return -1;

            // forward to other application
            return NativeMethods.CallNextHookEx(mKeyboardHookHandle, nCode, wParam, lParam);
        }

        private void SubscribeToGlobalKeyboardEvents()
        {
            // install Keyboard hook only if it is not installed and must be installed
            if (mKeyboardHookHandle == 0)
            {
                // See comment of this field. To avoid GC to clean it up.
                mKeyboardDelegate = KeyboardHookProc;

                // install hook
                mKeyboardHookHandle = NativeMethods.SetWindowsHookEx(
                    (int)HookEventEnum.WH_KEYBOARD_LL,
                    mKeyboardDelegate,
                    Marshal.GetHINSTANCE(typeof(KeyboardEventHookManager).Assembly.GetModules()[0]), 0);

                // If SetWindowsHookEx fails.
                if (mKeyboardHookHandle == 0)
                {
                    // Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();

                    // Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }

        private void UnsunscribeFromGlobalKeyboardEvents()
        {
            if (mKeyboardHookHandle != 0)
            {
                // uninstall hook
                int result = NativeMethods.UnhookWindowsHookEx(mKeyboardHookHandle);

                // reset invalid handle
                mKeyboardHookHandle = 0;

                // Free up for GC
                mKeyboardDelegate = null;

                // if failed and exception must be thrown
                if (result == 0)
                {
                    // Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();

                    // Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }

        #endregion

    }

}
