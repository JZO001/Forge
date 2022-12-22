/* *********************************************************************
 * Date: 30 Jul 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Runtime.InteropServices;

namespace Forge.Native.Structures
{

    /// <summary>
    /// Represents the mouse, keyboard and hardware union structure
    /// http://stackoverflow.com/questions/6830651/sendinput-and-64bits
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct MouseKeyboardHardwareUnion
    {

        /// <summary>
        /// The mouse input
        /// </summary>
        [FieldOffset(0)]
        public MouseInput MouseInput;

        /// <summary>
        /// The keyboard input
        /// </summary>
        [FieldOffset(0)]
        public KeyboardInput KeyboardInput;

        /// <summary>
        /// The hardware input
        /// </summary>
        [FieldOffset(0)]
        public HardwareInput HardwareInput;

    }

}
