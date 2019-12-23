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
    /// Helpers methods to performs simulated mouse operations
    /// </summary>
    public static class MouseOperations
    {

        /// <summary>
        /// Performs mouse down.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns></returns>
        public static bool PerformDown(MouseButtons button)
        {
            SendInputInfo[] inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Mouse;
            inputs[0].Inputs.MouseInput.DwExtraInfo = NativeMethods.GetMessageExtraInfo();
            switch (button)
            {
                case MouseButtons.Left:
                    inputs[0].Inputs.MouseInput.DwFlags = (uint)MouseEventEnum.LeftDown;
                    break;
                case MouseButtons.Middle:
                    inputs[0].Inputs.MouseInput.DwFlags = (uint)MouseEventEnum.MiddleDown;
                    break;
                case MouseButtons.Right:
                    inputs[0].Inputs.MouseInput.DwFlags = (uint)MouseEventEnum.RightDown;
                    break;
                case MouseButtons.XButton1:
                    inputs[0].Inputs.MouseInput.DwFlags = (uint)MouseEventEnum.XDown;
                    inputs[0].Inputs.MouseInput.MouseData = (int)MouseXButtonEnum.XButton1;
                    break;
                case MouseButtons.XButton2:
                    inputs[0].Inputs.MouseInput.DwFlags = (uint)MouseEventEnum.XDown;
                    inputs[0].Inputs.MouseInput.MouseData = (int)MouseXButtonEnum.XButton2;
                    break;
            }

            return NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0])) == 1;
        }

        /// <summary>
        /// Performs mouse up.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns></returns>
        public static bool PerformUp(MouseButtons button)
        {
            SendInputInfo[] inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Mouse;
            inputs[0].Inputs.MouseInput.DwExtraInfo = NativeMethods.GetMessageExtraInfo();
            switch (button)
            {
                case MouseButtons.Left:
                    inputs[0].Inputs.MouseInput.DwFlags = (uint)MouseEventEnum.LeftUp;
                    break;
                case MouseButtons.Middle:
                    inputs[0].Inputs.MouseInput.DwFlags = (uint)MouseEventEnum.MiddleUp;
                    break;
                case MouseButtons.Right:
                    inputs[0].Inputs.MouseInput.DwFlags = (uint)MouseEventEnum.RightUp;
                    break;
                case MouseButtons.XButton1:
                    inputs[0].Inputs.MouseInput.DwFlags = (uint)MouseEventEnum.XUp;
                    inputs[0].Inputs.MouseInput.MouseData = (int)MouseXButtonEnum.XButton1;
                    break;
                case MouseButtons.XButton2:
                    inputs[0].Inputs.MouseInput.DwFlags = (uint)MouseEventEnum.XUp;
                    inputs[0].Inputs.MouseInput.MouseData = (int)MouseXButtonEnum.XButton2;
                    break;
            }

            return NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0])) == 1;
        }

        /// <summary>
        /// Performs the click.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns></returns>
        public static bool PerformClick(MouseButtons button)
        {
            bool result = false;

            if (PerformDown(button))
            {
                result = PerformUp(button);
            }

            return result;
        }

        /// <summary>
        /// Moves the cursor with relative coordinates.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public static bool MoveRelative(int x, int y)
        {
            SendInputInfo[] inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Mouse;
            inputs[0].Inputs.MouseInput.Dx = x;
            inputs[0].Inputs.MouseInput.Dy = y;
            inputs[0].Inputs.MouseInput.DwFlags = (uint)MouseEventEnum.Move;
            inputs[0].Inputs.MouseInput.DwExtraInfo = NativeMethods.GetMessageExtraInfo();

            return NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0])) == 1;
        }

        /// <summary>
        /// Moves the cursor to the provided absolute coordinates.
        /// This is a WinApi call. Using Cursor.Position property is recommended.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public static bool MoveAbsolute(int x, int y)
        {
            bool result = false;

            SendInputInfo[] inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Mouse;
            inputs[0].Inputs.MouseInput.Dx = 0;
            inputs[0].Inputs.MouseInput.Dy = 0;
            inputs[0].Inputs.MouseInput.DwFlags = (uint)MouseEventEnum.Move | (uint)MouseEventEnum.Absolute;
            inputs[0].Inputs.MouseInput.DwExtraInfo = NativeMethods.GetMessageExtraInfo();

            if (NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0])) == 1)
            {
                result = MoveRelative(x, y);
            }

            return result;
        }

        /// <summary>
        /// Moves the wheel vertically. Specify a negative number to scroll down.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public static bool MoveWheelVertically(int amount)
        {
            SendInputInfo[] inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Mouse;
            inputs[0].Inputs.MouseInput.MouseData = (int)amount * 120;
            inputs[0].Inputs.MouseInput.DwFlags = (uint)MouseEventEnum.Wheel;
            inputs[0].Inputs.MouseInput.DwExtraInfo = NativeMethods.GetMessageExtraInfo();

            return NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0])) == 1;
        }

        /// <summary>
        /// Moves the wheel horizontally. Specify a negative number to scroll down.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public static bool MoveWheelHorizontally(int amount)
        {
            SendInputInfo[] inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Mouse;
            inputs[0].Inputs.MouseInput.MouseData = (int)amount * 120;
            inputs[0].Inputs.MouseInput.DwFlags = (uint)MouseEventEnum.HWheel;
            inputs[0].Inputs.MouseInput.DwExtraInfo = NativeMethods.GetMessageExtraInfo();

            return NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0])) == 1;
        }

    }

}
