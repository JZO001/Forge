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
    /// Represents the required structure for the native SendInput method
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms646270(v=vs.85).aspx
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct SendInputInfo
    {

        /// <summary>
        /// Type of the device, see InputTypeDeviceEnum.
        /// </summary>
        public int Type;

        /// <summary>
        /// The inputs for mouse, keyboard and hardware
        /// </summary>
        public MouseKeyboardHardwareUnion Inputs;

    }

}
