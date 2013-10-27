/* *********************************************************************
 * Date: 29 Jul 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Native
{

    /// <summary>
    /// Specifies various aspects of a keystroke. This member can be certain combinations of the following values.
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms646271(v=vs.85).aspx
    /// </summary>
    [Serializable]
    public enum KeyboardKeyEventEnum : uint
    {

        /// <summary>
        /// If specified, the scan code was preceded by a prefix byte that has the value 0xE0 (224).
        /// </summary>
        KeyDown = 0x0001,

        /// <summary>
        /// If specified, the key is being released. If not specified, the key is being pressed.
        /// </summary>
        KeyUp = 0x0002,

        /// <summary>
        /// If specified, wScan identifies the key and wVk is ignored.
        /// </summary>
        Unicode = 0x0004,

        /// <summary>
        /// If specified, the system synthesizes a VK_PACKET keystroke. The wVk parameter must be zero. 
        /// This flag can only be combined with the KEYEVENTF_KEYUP flag. For more information, see the Remarks section.
        /// </summary>
        Scancode = 0x0008

    }

}
