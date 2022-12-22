using System;
using System.Text;
using System.Windows.Forms;
using Forge.Native;
using Forge.Native.Hooks;
using Forge.Native.Structures;

namespace Testing.Hooks
{

    public partial class KeyDemoForm : Form
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyDemoForm"/> class.
        /// </summary>
        public KeyDemoForm()
        {
            InitializeComponent();

            KeyboardEventHookManager.Instance.KeyDown += new KeyEventHandler(Event_KeyDown);
            KeyboardEventHookManager.Instance.KeyPress += new KeyPressEventHandler(Event_KeyPress);
            KeyboardEventHookManager.Instance.KeyUp += new KeyEventHandler(Event_KeyUp);
            KeyboardEventHookManager.Instance.Start();
        }

        private void Event_KeyDown(object sender, KeyEventArgs e)
        {
            StringBuilder sb = new StringBuilder("KEYDOWN (");
            sb.Append(sender == null ? "null" : sender.GetType().Name);
            sb.Append("), Alt: ");
            sb.Append(e.Alt.ToString());
            sb.Append(", Control: ");
            sb.Append(e.Control.ToString());
            sb.Append(", KeyCode: ");
            sb.Append(e.KeyCode.ToString());
            sb.Append(", KeyData: ");
            sb.Append(e.KeyData.ToString());
            sb.Append(", KeyValue: ");
            sb.Append(e.KeyValue.ToString());
            sb.Append(", Modifiers: ");
            sb.Append(e.Modifiers.ToString());
            sb.Append(", Shift: ");
            sb.Append(e.Shift.ToString());
            sb.Append(", SuppressKeyPress: ");
            sb.Append(e.SuppressKeyPress.ToString());
            Console.WriteLine(sb.ToString());
            e.Handled = true;
        }

        private void Event_KeyPress(object sender, KeyPressEventArgs e)
        {
            Console.WriteLine(string.Format("KEYPRESS ({0}), KeyChar: {1}", sender == null ? "null" : sender.GetType().Name, e.KeyChar.ToString()));
            e.Handled = true;
        }

        private void Event_KeyUp(object sender, KeyEventArgs e)
        {
            StringBuilder sb = new StringBuilder("KEYUP (");
            sb.Append(sender == null ? "null" : sender.GetType().Name);
            sb.Append("), Alt: ");
            sb.Append(e.Alt.ToString());
            sb.Append(", Control: ");
            sb.Append(e.Control.ToString());
            sb.Append(", KeyCode: ");
            sb.Append(e.KeyCode.ToString());
            sb.Append(", KeyData: ");
            sb.Append(e.KeyData.ToString());
            sb.Append(", KeyValue: ");
            sb.Append(e.KeyValue.ToString());
            sb.Append(", Modifiers: ");
            sb.Append(e.Modifiers.ToString());
            sb.Append(", Shift: ");
            sb.Append(e.Shift.ToString());
            sb.Append(", SuppressKeyPress: ");
            sb.Append(e.SuppressKeyPress.ToString());
            Console.WriteLine(sb.ToString());
            e.Handled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Enabled = false;

            // 1. példa
            // down & press
            //WindowsAPI.keybd_event((byte)Keys.B, 0, WindowsAPI.KEYEVENTF_KEYDOWN, 0);

            // up
            //WindowsAPI.keybd_event((byte)Keys.B, 0, WindowsAPI.KEYEVENTF_KEYUP, 0);

            // 2. példa
            //byte vk = (byte)Keys.B; //WindowsAPI.VkKeyScan("a".ToCharArray()[0]);
            byte virtualKeyOrModifier = (byte)Keys.LControlKey;
            ushort scanCode = (ushort)NativeMethods.MapVirtualKey(virtualKeyOrModifier, 0);
            //ProcessKeyDown(vk, scanCode);
            //ProcessKeyUp(vk, scanCode);

            virtualKeyOrModifier = (byte)Keys.RControlKey;
            scanCode = (ushort)NativeMethods.MapVirtualKey(virtualKeyOrModifier, 0);
            //ProcessKeyDown(virtualKeyOrModifier, scanCode);
            //ProcessKeyUp(virtualKeyOrModifier, scanCode);

            virtualKeyOrModifier = NativeMethods.VkKeyScan("ő".ToCharArray()[0]);
            scanCode = (ushort)NativeMethods.MapVirtualKey(virtualKeyOrModifier, 0);
            //ProcessKeyDown(virtualKeyOrModifier, scanCode);
            //ProcessKeyUp(virtualKeyOrModifier, scanCode);

            virtualKeyOrModifier = 0x12; // VK_MENU
            scanCode = (ushort)NativeMethods.MapVirtualKey(virtualKeyOrModifier, 0);
            //ProcessKeyDown(virtualKeyOrModifier, scanCode);
            //ProcessKeyUp(virtualKeyOrModifier, scanCode);

            virtualKeyOrModifier = (byte)Keys.LMenu;
            scanCode = (ushort)NativeMethods.MapVirtualKey(virtualKeyOrModifier, 0);
            //ProcessKeyDown(virtualKeyOrModifier, scanCode);
            //ProcessKeyUp(virtualKeyOrModifier, scanCode);

            virtualKeyOrModifier = (byte)Keys.ShiftKey;
            scanCode = (ushort)NativeMethods.MapVirtualKey(virtualKeyOrModifier, 0);
            //ProcessKeyDown(virtualKeyOrModifier, scanCode);
            //ProcessKeyUp(virtualKeyOrModifier, scanCode);

            // 3. példa
            //SendAltPlusChar();

            // 4. példa
            //SendCtrlAltDel(); // its not possible in that way, skip

        }

        public static void ProcessKeyDown(ushort virtualKeyOrModifier, ushort scanCode)
        {
            SendInputInfo[] inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Keyboard;
            inputs[0].Inputs.KeyboardInput.WVirtualKeyCode = virtualKeyOrModifier;
            inputs[0].Inputs.KeyboardInput.WScanCode = scanCode; // (ushort)((ushort)WindowsAPI.MapVirtualKey((uint)Keys.Menu, 0) & 0xff);
            inputs[0].Inputs.KeyboardInput.DwFlags = (uint)KeyboardKeyEventEnum.KeyDown | 0; // 0

            uint intReturn = NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send key: " + scanCode);
            }
        }

        public static void ProcessKeyUp(ushort virtualKeyOrModifier, ushort scanCode)
        {
            SendInputInfo[] inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Keyboard;
            inputs[0].Inputs.KeyboardInput.WVirtualKeyCode = virtualKeyOrModifier;
            inputs[0].Inputs.KeyboardInput.WScanCode = scanCode;
            inputs[0].Inputs.KeyboardInput.DwFlags = (uint)KeyboardKeyEventEnum.KeyDown | (uint)KeyboardKeyEventEnum.KeyUp;

            uint intReturn = NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send key: " + scanCode);
            }
        }

        public static void SendAltPlusChar()
        {
            byte virtualKeyForChar = NativeMethods.VkKeyScan("ő".ToCharArray()[0]);
            ushort scanCodeForChar = (ushort)NativeMethods.MapVirtualKey(virtualKeyForChar, 0);

            byte virtualKeyOrModifier = (byte)Keys.LMenu;
            ushort scanCodeForModifier = (ushort)NativeMethods.MapVirtualKey(virtualKeyOrModifier, 0);

            // down
            SendInputInfo[] inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Keyboard;
            inputs[0].Inputs.KeyboardInput.WVirtualKeyCode = virtualKeyOrModifier;
            inputs[0].Inputs.KeyboardInput.WScanCode = scanCodeForModifier;
            inputs[0].Inputs.KeyboardInput.DwFlags = (uint)KeyboardKeyEventEnum.KeyDown | 0;
            //inputs[0].ki.dwExtraInfo = WindowsAPI.GetMessageExtraInfo();

            uint intReturn = NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send key");
            }

            inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Keyboard;
            inputs[0].Inputs.KeyboardInput.WVirtualKeyCode = virtualKeyForChar;
            inputs[0].Inputs.KeyboardInput.WScanCode = scanCodeForChar;
            inputs[0].Inputs.KeyboardInput.DwFlags = (uint)KeyboardKeyEventEnum.KeyDown | 0;
            //inputs[0].ki.dwExtraInfo = WindowsAPI.GetMessageExtraInfo();

            intReturn = NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send key");
            }




            // up
            inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Keyboard;
            inputs[0].Inputs.KeyboardInput.WVirtualKeyCode = virtualKeyForChar;
            inputs[0].Inputs.KeyboardInput.WScanCode = scanCodeForChar;
            inputs[0].Inputs.KeyboardInput.DwFlags = (uint)KeyboardKeyEventEnum.KeyDown | (uint)KeyboardKeyEventEnum.KeyUp;
            //inputs[0].ki.dwExtraInfo = WindowsAPI.GetMessageExtraInfo();

            intReturn = NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send key");
            }

            inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Keyboard;
            inputs[0].Inputs.KeyboardInput.WVirtualKeyCode = virtualKeyOrModifier;
            inputs[0].Inputs.KeyboardInput.WScanCode = scanCodeForModifier;
            inputs[0].Inputs.KeyboardInput.DwFlags = (uint)KeyboardKeyEventEnum.KeyDown | (uint)KeyboardKeyEventEnum.KeyUp;
            //inputs[0].ki.dwExtraInfo = WindowsAPI.GetMessageExtraInfo();

            intReturn = NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send key");
            }
            
        }

        public static void SendCtrlAltDel()
        {
            byte virtualKeyLeftAlt = (byte)Keys.LMenu;
            ushort scanCodeForLeftAlt = (ushort)NativeMethods.MapVirtualKey(virtualKeyLeftAlt, 0);

            byte virtualKeyLeftControl = (byte)Keys.LControlKey;
            ushort scanCodeForLeftControl = (ushort)NativeMethods.MapVirtualKey(virtualKeyLeftControl, 0);

            byte virtualKeyDel = (byte)Keys.Menu;
            ushort scanCodeForDel = (ushort)NativeMethods.MapVirtualKey(virtualKeyDel, 0);

            // down
            SendInputInfo[] inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Keyboard;
            inputs[0].Inputs.KeyboardInput.WVirtualKeyCode = virtualKeyLeftControl;
            inputs[0].Inputs.KeyboardInput.WScanCode = scanCodeForLeftControl;
            inputs[0].Inputs.KeyboardInput.DwFlags = (uint)KeyboardKeyEventEnum.KeyDown | 0;
            //inputs[0].ki.dwExtraInfo = WindowsAPI.GetMessageExtraInfo();

            uint intReturn = NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send key");
            }
            
            inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Keyboard;
            inputs[0].Inputs.KeyboardInput.WVirtualKeyCode = virtualKeyLeftAlt;
            inputs[0].Inputs.KeyboardInput.WScanCode = scanCodeForLeftAlt;
            inputs[0].Inputs.KeyboardInput.DwFlags = (uint)KeyboardKeyEventEnum.KeyDown | 0;
            //inputs[0].ki.dwExtraInfo = WindowsAPI.GetMessageExtraInfo();

            intReturn = NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send key");
            }

            inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Keyboard;
            inputs[0].Inputs.KeyboardInput.WVirtualKeyCode = virtualKeyDel;
            inputs[0].Inputs.KeyboardInput.WScanCode = scanCodeForDel;
            inputs[0].Inputs.KeyboardInput.DwFlags = (uint)KeyboardKeyEventEnum.KeyDown | 0;
            //inputs[0].ki.dwExtraInfo = WindowsAPI.GetMessageExtraInfo();

            intReturn = NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send key");
            }




            // up
            inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Keyboard;
            inputs[0].Inputs.KeyboardInput.WVirtualKeyCode = virtualKeyDel;
            inputs[0].Inputs.KeyboardInput.WScanCode = scanCodeForDel;
            inputs[0].Inputs.KeyboardInput.DwFlags = (uint)KeyboardKeyEventEnum.KeyDown | (uint)KeyboardKeyEventEnum.KeyUp;
            //inputs[0].ki.dwExtraInfo = WindowsAPI.GetMessageExtraInfo();

            intReturn = NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send key");
            }

            inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Keyboard;
            inputs[0].Inputs.KeyboardInput.WVirtualKeyCode = virtualKeyLeftAlt;
            inputs[0].Inputs.KeyboardInput.WScanCode = scanCodeForLeftAlt;
            inputs[0].Inputs.KeyboardInput.DwFlags = (uint)KeyboardKeyEventEnum.KeyDown | (uint)KeyboardKeyEventEnum.KeyUp;
            //inputs[0].ki.dwExtraInfo = WindowsAPI.GetMessageExtraInfo();

            intReturn = NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send key");
            }

            inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Keyboard;
            inputs[0].Inputs.KeyboardInput.WVirtualKeyCode = virtualKeyLeftControl;
            inputs[0].Inputs.KeyboardInput.WScanCode = scanCodeForLeftControl;
            inputs[0].Inputs.KeyboardInput.DwFlags = (uint)KeyboardKeyEventEnum.KeyDown | (uint)KeyboardKeyEventEnum.KeyUp;
            //inputs[0].ki.dwExtraInfo = WindowsAPI.GetMessageExtraInfo();

            intReturn = NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send key");
            }
        
        }

    }

}
