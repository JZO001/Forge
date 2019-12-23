/* *********************************************************************
 * Date: 29 Jul 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Runtime.InteropServices;

namespace Forge.Native.Structures
{

    /// <summary>
    /// Contains information about a simulated keyboard event.
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms646271(v=vs.85).aspx
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardInput
    {

        /// <summary>
        /// A virtual-key code. The code must be a value in the range 1 to 254. 
        /// If the dwFlags member specifies KEYEVENTF_UNICODE, wVk must be 0.
        /// </summary>
        public ushort WVirtualKeyCode;

        /// <summary>
        /// A hardware scan code for the key. If dwFlags specifies KEYEVENTF_UNICODE, wScan specifies a 
        /// Unicode character which is to be sent to the foreground application.
        /// </summary>
        public ushort WScanCode;

        /// <summary>
        /// Specifies various aspects of a keystroke. This member can be certain combinations of the following values.
        /// See KeyboardKeyEventEnum values
        /// </summary>
        public uint DwFlags;

        /// <summary>
        /// The time stamp for the event, in milliseconds. If this parameter is zero, the system will provide its own time stamp.
        /// </summary>
        public uint Time;

        /// <summary>
        /// An additional value associated with the keystroke. Use the GetMessageExtraInfo function to obtain this information.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible")]
        public IntPtr DwExtraInfo;

    }

}
