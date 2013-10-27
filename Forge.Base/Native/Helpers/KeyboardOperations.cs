/* *********************************************************************
 * Date: 29 Jul 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Windows.Forms;
using Forge.Native.Structures;

namespace Forge.Native.Helpers
{

    /// <summary>
    /// Helpers methods to performs simulated keyboard operations
    /// </summary>
    public static class KeyboardOperations
    {

        #region Public properties

        /// <summary>
        /// Gets a value indicating whether the shift button down or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is shift down; otherwise, <c>false</c>.
        /// </value>
        public static bool IsShiftDown
        {
            get
            {
                return (NativeMethods.GetKeyState((int)Keys.ShiftKey) & 0x80) == 0x80 ? true : false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the caps lock down.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is caps lock down; otherwise, <c>false</c>.
        /// </value>
        public static bool IsCapsLockDown
        {
            get
            {
                return NativeMethods.GetKeyState((int)Keys.Capital) != 0 ? true : false;
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Performs a key down operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static bool PerformKeyDown(Keys key)
        {
            byte virtualKeyOrModifier = (byte)key;
            ushort scanCode = (ushort)NativeMethods.MapVirtualKey(virtualKeyOrModifier, 0);

            SendInputInfo[] inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Keyboard;
            inputs[0].Inputs.KeyboardInput.WVirtualKeyCode = virtualKeyOrModifier;
            inputs[0].Inputs.KeyboardInput.WScanCode = scanCode;
            inputs[0].Inputs.KeyboardInput.DwFlags = (uint)KeyboardKeyEventEnum.KeyDown | 0;

            return NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0])) == 1;
        }

        /// <summary>
        /// Performs a key up operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static bool PerformKeyUp(Keys key)
        {
            byte virtualKeyOrModifier = (byte)key;
            ushort scanCode = (ushort)NativeMethods.MapVirtualKey(virtualKeyOrModifier, 0);

            SendInputInfo[] inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Keyboard;
            inputs[0].Inputs.KeyboardInput.WVirtualKeyCode = virtualKeyOrModifier;
            inputs[0].Inputs.KeyboardInput.WScanCode = scanCode;
            inputs[0].Inputs.KeyboardInput.DwFlags = (uint)KeyboardKeyEventEnum.KeyDown | (uint)KeyboardKeyEventEnum.KeyUp;

            return NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0])) == 1;
        }

        /// <summary>
        /// Determines the current state of the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the key is down; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsKeyDown(Keys key)
        {
            return NativeMethods.GetKeyState((int)key) != 0 ? true : false;
        }

        #endregion

    }

}
