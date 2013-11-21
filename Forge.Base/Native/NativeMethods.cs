/* *********************************************************************
 * Date: 29 Jul 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Runtime.InteropServices;
using Forge.Native.Structures;

namespace Forge.Native
{

    /// <summary>
    /// The CallWndProc hook procedure is an application-defined or library-defined callback 
    /// function used with the SetWindowsHookEx function. The HOOKPROC type defines a pointer 
    /// to this callback function. CallWndProc is a placeholder for the application-defined 
    /// or library-defined function name.
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
    /// <remarks>
    /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/callwndproc.asp
    /// </remarks>
    public delegate int HookProcessDelegate(int nCode, int wParam, IntPtr lParam);

    /// <summary>
    /// Represents a set of native methods
    /// </summary>
    public static class NativeMethods
    {

        /// <summary>
        /// Mouse_events the specified dw flags.
        /// </summary>
        /// <param name="dwFlags">The dw flags.</param>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        /// <param name="cButtons">The c buttons.</param>
        /// <param name="dwExtraInfo">The dw extra info.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, int dwExtraInfo);

        /// <summary>
        /// Keybd_events the specified b vk.
        /// </summary>
        /// <param name="bVk">The b vk.</param>
        /// <param name="bScan">The b scan.</param>
        /// <param name="dwFlags">The dw flags.</param>
        /// <param name="dwExtraInfo">The dw extra info.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        /// <summary>
        /// Sends the input.
        /// </summary>
        /// <param name="nInputs">The n inputs.</param>
        /// <param name="pInputs">The p inputs.</param>
        /// <param name="cbSize">Size of the cb.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("user32.dll")]
        public static extern UInt32 SendInput(UInt32 nInputs, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] SendInputInfo[] pInputs, Int32 cbSize);

        /// <summary>
        /// Vks the key scan.
        /// </summary>
        /// <param name="ch">The ch.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern byte VkKeyScan(char ch);

        /// <summary>
        /// Maps the virtual key.
        /// </summary>
        /// <param name="uCode">The u code.</param>
        /// <param name="uMapType">Type of the u map.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);

        /// <summary>
        /// Gets the message extra info.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetMessageExtraInfo();

        /// <summary>
        /// The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain. 
        /// A hook procedure can call this function either before or after processing the hook information. 
        /// </summary>
        /// <param name="idHook">Ignored.</param>
        /// <param name="nCode">
        /// [in] Specifies the hook code passed to the current hook procedure. 
        /// The next hook procedure uses this code to determine how to process the hook information.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies the wParam value passed to the current hook procedure. 
        /// The meaning of this parameter depends on the type of hook associated with the current hook chain. 
        /// </param>
        /// <param name="lParam">
        /// [in] Specifies the lParam value passed to the current hook procedure. 
        /// The meaning of this parameter depends on the type of hook associated with the current hook chain. 
        /// </param>
        /// <returns>
        /// This value is returned by the next hook procedure in the chain. 
        /// The current hook procedure must also return this value. The meaning of the return value depends on the hook type. 
        /// For more information, see the descriptions of the individual hook procedures.
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        /// <summary>
        /// The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain. 
        /// You would install a hook procedure to monitor the system for certain types of events. These events 
        /// are associated either with a specific thread or with all threads in the same desktop as the calling thread. 
        /// </summary>
        /// <param name="idHook">
        /// [in] Specifies the type of hook procedure to be installed. This parameter can be one of the following values.
        /// </param>
        /// <param name="lpfn">
        /// [in] Pointer to the hook procedure. If the dwThreadId parameter is zero or specifies the identifier of a 
        /// thread created by a different process, the lpfn parameter must point to a hook procedure in a dynamic-link 
        /// library (DLL). Otherwise, lpfn can point to a hook procedure in the code associated with the current process.
        /// </param>
        /// <param name="hMod">
        /// [in] Handle to the DLL containing the hook procedure pointed to by the lpfn parameter. 
        /// The hMod parameter must be set to NULL if the dwThreadId parameter specifies a thread created by 
        /// the current process and if the hook procedure is within the code associated with the current process. 
        /// </param>
        /// <param name="dwThreadId">
        /// [in] Specifies the identifier of the thread with which the hook procedure is to be associated. 
        /// If this parameter is zero, the hook procedure is associated with all existing threads running in the 
        /// same desktop as the calling thread. 
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is the handle to the hook procedure.
        /// If the function fails, the return value is NULL. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int SetWindowsHookEx(int idHook, HookProcessDelegate lpfn, IntPtr hMod, int dwThreadId);

        /// <summary>
        /// The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx function. 
        /// </summary>
        /// <param name="idHook">
        /// [in] Handle to the hook to be removed. This parameter is a hook handle obtained by a previous call to SetWindowsHookEx. 
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int UnhookWindowsHookEx(int idHook);

        /// <summary>
        /// The ToAscii function translates the specified virtual-key code and keyboard 
        /// state to the corresponding character or characters. The function translates the code 
        /// using the input language and physical keyboard layout identified by the keyboard layout handle.
        /// </summary>
        /// <param name="uVirtKey">
        /// [in] Specifies the virtual-key code to be translated. 
        /// </param>
        /// <param name="uScanCode">
        /// [in] Specifies the hardware scan code of the key to be translated. 
        /// The high-order bit of this value is set if the key is up (not pressed). 
        /// </param>
        /// <param name="lpbKeyState">
        /// [in] Pointer to a 256-byte array that contains the current keyboard state. 
        /// Each element (byte) in the array contains the state of one key. 
        /// If the high-order bit of a byte is set, the key is down (pressed). 
        /// The low bit, if set, indicates that the key is toggled on. In this function, 
        /// only the toggle bit of the CAPS LOCK key is relevant. The toggle state 
        /// of the NUM LOCK and SCROLL LOCK keys is ignored.
        /// </param>
        /// <param name="lpwTransKey">
        /// [out] Pointer to the buffer that receives the translated character or characters. 
        /// </param>
        /// <param name="fuState">
        /// [in] Specifies whether a menu is active. This parameter must be 1 if a menu is active, or 0 otherwise. 
        /// </param>
        /// <returns>
        /// If the specified key is a dead key, the return value is negative. Otherwise, it is one of the following values. 
        /// Value Meaning 
        /// 0 The specified virtual key has no translation for the current state of the keyboard. 
        /// 1 One character was copied to the buffer. 
        /// 2 Two characters were copied to the buffer. This usually happens when a dead-key character 
        /// (accent or diacritic) stored in the keyboard layout cannot be composed with the specified 
        /// virtual key to form a single character. 
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/toascii.asp
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("user32")]
        public static extern int ToAscii(uint uVirtKey, uint uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, uint fuState);

        /// <summary>
        /// The GetKeyboardState function copies the status of the 256 virtual keys to the 
        /// specified buffer. 
        /// </summary>
        /// <param name="pbKeyState">
        /// [in] Pointer to a 256-byte array that contains keyboard key states. 
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError. 
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/toascii.asp
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetKeyboardState(byte[] pbKeyState);

        /// <summary>
        /// Sets the state of the keyboard.
        /// </summary>
        /// <param name="keystate">The keystate.</param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError. 
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int SetKeyboardState(byte[] keystate);

        /// <summary>
        /// The GetKeyState function retrieves the status of the specified virtual key. The status specifies whether the key is up, down, or toggled 
        /// (on, off—alternating each time the key is pressed). 
        /// </summary>
        /// <param name="vKey">
        /// [in] Specifies a virtual key. If the desired virtual key is a letter or digit (A through Z, a through z, or 0 through 9), nVirtKey must be set to the ASCII value of that character. For other keys, it must be a virtual-key code. 
        /// </param>
        /// <returns>
        /// The return value specifies the status of the specified virtual key, as follows: 
        /// If the high-order bit is 1, the key is down; otherwise, it is up.
        /// If the low-order bit is 1, the key is toggled. A key, such as the CAPS LOCK key, is toggled if it is turned on. The key is off and untoggled if the low-order bit is 0. A toggle key's indicator light (if any) on the keyboard will be on when the key is toggled, and off when the key is untoggled.
        /// </returns>
        /// <remarks>http://msdn.microsoft.com/en-us/library/ms646301.aspx</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern short GetKeyState(int vKey);

        /// <summary>
        /// Subscribe a clipboard viewer.
        /// </summary>
        /// <param name="hWndNewViewer">The handle of the new viewer.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        /// <summary>
        /// Changes the clipboard chain.
        /// </summary>
        /// <param name="hWndRemove">The h WND remove.</param>
        /// <param name="hWndNewNext">The h WND new next.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        /// <summary>
        /// Adds the clipboard format listener.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        /// <summary>
        /// Removes the clipboard format listener.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        /// <summary>
        /// Sets the parent.
        /// See http://msdn.microsoft.com/en-us/library/ms633541%28v=vs.85%29.aspx
        /// See http://msdn.microsoft.com/en-us/library/ms649033%28VS.85%29.aspx
        /// </summary>
        /// <param name="hWndChild">The h WND child.</param>
        /// <param name="hWndNewParent">The h WND new parent.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <param name="wMsg">The w MSG.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Gets the cursor.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetCursor();

        /// <summary>
        /// Sets the cursor.
        /// </summary>
        /// <param name="hcursor">The hcursor.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr SetCursor(HandleRef hcursor);

        /// <summary>
        /// Gets the cursor information.
        /// </summary>
        /// <param name="pci">The pci.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("user32.dll")]
        public static extern bool GetCursorInfo(out CursorInfo pci);

        /// <summary>
        /// Gets the system time in UTC.
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms724390(v=vs.85).aspx
        /// </summary>
        /// <param name="lpSystemTime">The lp system time.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("Kernel32.dll")]
        public extern static void GetSystemTime(ref SystemTime lpSystemTime);

        /// <summary>
        /// Sets the system time in UTC.
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms724942(v=vs.85).aspx
        /// </summary>
        /// <param name="lpSystemTime">The lp system time.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("Kernel32.dll")]
        public extern static uint SetSystemTime(ref SystemTime lpSystemTime);

    }

}
