/* *********************************************************************
 * Date: 29 Jul 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Native
{

    /// <summary>
    /// Represents the parameters of a simulated mouse event
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms646273(v=vs.85).aspx
    /// </summary>
    [Serializable]
    public enum MouseEventEnum : uint
    {

        /// <summary>
        /// The dx and dy members contain normalized absolute coordinates. If the flag is not set, dxand dy contain relative data 
        /// (the change in position since the last reported position). This flag can be set, or not set, regardless of what kind of 
        /// mouse or other pointing device, if any, is connected to the system. For further information about relative mouse motion, 
        /// see the following Remarks section.
        /// </summary>
        Absolute = 0x8000,

        /// <summary>
        /// The wheel was moved horizontally, if the mouse has a wheel. The amount of movement is specified in mouseData. 
        ///
        /// Windows XP/2000:  This value is not supported.
        /// </summary>
        HWheel = 0x1000,

        /// <summary>
        /// Movement occurred.
        /// </summary>
        Move = 0x0001,

        /// <summary>
        /// The WM_MOUSEMOVE messages will not be coalesced. The default behavior is to coalesce WM_MOUSEMOVE messages. 
        ///
        /// Windows XP/2000:  This value is not supported.
        /// </summary>
        Move_NoCoalesce = 0x2000,

        /// <summary>
        /// The left button was pressed.
        /// </summary>
        LeftDown = 0x0002,

        /// <summary>
        /// The left button was released.
        /// </summary>
        LeftUp = 0x0004,

        /// <summary>
        /// The right button was pressed.
        /// </summary>
        RightDown = 0x0008,

        /// <summary>
        /// The right button was released.
        /// </summary>
        RightUp = 0x0010,

        /// <summary>
        /// The middle button was pressed.
        /// </summary>
        MiddleDown = 0x0020,

        /// <summary>
        /// The middle button was released.
        /// </summary>
        MiddleUp = 0x0040,

        /// <summary>
        /// Maps coordinates to the entire desktop. Must be used with MOUSEEVENTF_ABSOLUTE.
        /// </summary>
        VirtualDesk = 0x4000,

        /// <summary>
        /// The wheel was moved, if the mouse has a wheel. The amount of movement is specified in mouseData.
        /// </summary>
        Wheel = 0x0800,

        /// <summary>
        /// An X button was pressed.
        /// </summary>
        XDown = 0x0080,

        /// <summary>
        /// An X button was released.
        /// </summary>
        XUp = 0x0100

    }

}
