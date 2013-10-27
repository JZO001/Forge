/***********************************************************************
 * Date: 30 Jul 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Native.Hooks
{

    /// <summary>
    /// Values from Winuser.h in Microsoft SDK.
    /// </summary>
    [Serializable]
    public enum MouseInputNotificationEnum
    {
        /// <summary>
        /// The WM_MOUSEMOVE message is posted to a window when the cursor moves. 
        /// </summary>
        WM_MOUSEMOVE = 0x200,

        /// <summary>
        /// The WM_LBUTTONDOWN message is posted when the user presses the left mouse button 
        /// </summary>
        WM_LBUTTONDOWN = 0x201,

        /// <summary>
        /// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button
        /// </summary>
        WM_RBUTTONDOWN = 0x204,

        /// <summary>
        /// The WM_MBUTTONDOWN message is posted when the user presses the middle mouse button 
        /// </summary>
        WM_MBUTTONDOWN = 0x207,

        /// <summary>
        /// Posted when the user presses the first or second X button while the cursor is in the client area of a window. 
        /// If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted 
        /// to the window that has captured the mouse.
        /// </summary>
        WM_XBUTTONDOWN = 0x020B,

        /// <summary>
        /// The WM_LBUTTONUP message is posted when the user releases the left mouse button 
        /// </summary>
        WM_LBUTTONUP = 0x202,

        /// <summary>
        /// The WM_RBUTTONUP message is posted when the user releases the right mouse button 
        /// </summary>
        WM_RBUTTONUP = 0x205,

        /// <summary>
        /// The WM_MBUTTONUP message is posted when the user releases the middle mouse button 
        /// </summary>
        WM_MBUTTONUP = 0x208,

        /// <summary>
        /// Posted when the user releases the first or second X button while the cursor is in the client area of a window. 
        /// If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted 
        /// to the window that has captured the mouse.
        /// </summary>
        WM_XBUTTONUP = 0x020C,

        /// <summary>
        /// The WM_LBUTTONDBLCLK message is posted when the user double-clicks the left mouse button 
        /// </summary>
        WM_LBUTTONDBLCLK = 0x203,

        /// <summary>
        /// The WM_RBUTTONDBLCLK message is posted when the user double-clicks the right mouse button 
        /// </summary>
        WM_RBUTTONDBLCLK = 0x206,

        /// <summary>
        /// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button 
        /// </summary>
        WM_MBUTTONDBLCLK = 0x209,

        /// <summary>
        /// Posted when the user double-clicks the first or second X button while the cursor is in the client area of a window. 
        /// If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        WM_XBUTTONDBLCLK = 0x020D,

        /// <summary>
        /// The WM_MOUSEWHEEL message is posted when the user presses the mouse wheel. 
        /// </summary>
        WM_MOUSEWHEEL = 0x020A,

        /// <summary>
        /// The WM_MOUSEWHEEL message is posted when the user presses the mouse wheel. 
        /// </summary>
        WM_MOUSEHWHEEL = 0x020E

    }

}
