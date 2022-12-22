using System;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Forge.Native;
using Forge.Native.Helpers;
using Forge.Native.Hooks;
using Forge.Native.Structures;

namespace Testing.Hooks
{

    public partial class ViewForm : Form
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewForm"/> class.
        /// </summary>
        public ViewForm()
        {
            InitializeComponent();

            //MouseEventHookManager.Instance.MouseWheelVerticalExtended += new EventHandler<MouseEventExtendedArgs>(Instance_MouseWheelVerticalExtended);
            //MouseEventHookManager.Instance.Start();

            //pictureBox1.MouseWheel += new MouseEventHandler(Event_MouseWheel);
            this.MouseWheel += new MouseEventHandler(Event_MouseWheel);
            
            //MouseEventHookManager.Instance.MouseMove += new MouseEventHandler(HookManager_MouseMove);
            //MouseEventHookManager.Instance.Start();

            KeyboardEventHookManager.Instance.KeyDown += new KeyEventHandler(Event_KeyDown);
            //KeyboardEventHookManager.Instance.KeyPress += new KeyPressEventHandler(Event_KeyPress);
            KeyboardEventHookManager.Instance.KeyUp += new KeyEventHandler(Event_KeyUp);
            KeyboardEventHookManager.Instance.Start();

            Console.WriteLine(string.Format("CAPS: {0}", KeyboardOperations.IsCapsLockDown.ToString()));
            Console.WriteLine(string.Format("SHIFT: {0}", KeyboardOperations.IsShiftDown.ToString()));
            // wait for double click
            //System.Windows.Forms.SystemInformation.DoubleClickTime;

            ClipboardEventManager.Instance.EventClipboardChanged += new EventHandler<ClipboardChangedEventArgs>(Instance_EventClipboardChanged);
            ClipboardEventManager.Instance.Start();
        }

        /**
MSG, hwnd: 9111404, lparam: 25102400, wparam: -7864320
MSG, hwnd: 9111404, lparam: 0, wparam: 0
ClientPoint, X: 139, Y: 143
MSG, hwnd: 9111404, lparam: 25102400, wparam: -7864320
MSG, hwnd: 9111404, lparam: 0, wparam: 0
ClientPoint, X: 139, Y: 263
MSG, hwnd: 9111404, lparam: 25167937, wparam: 7864320
MSG, hwnd: 9111404, lparam: 0, wparam: 0
ClientPoint, X: 140, Y: 144
MSG, hwnd: 9111404, lparam: 25167937, wparam: 7864320
MSG, hwnd: 9111404, lparam: 0, wparam: 0
ClientPoint, X: 140, Y: 24
         
MSG, hwnd: 5769574, lparam: 27789432, wparam: 7864320
MSG, hwnd: 5769574, lparam: 0, wparam: 0
ClientPoint, X: 195, Y: 64
MSG, hwnd: 5769574, lparam: 27723895, wparam: -7864320
MSG, hwnd: 5769574, lparam: 0, wparam: 0
ClientPoint, X: 194, Y: 183
MSG, hwnd: 5769574, lparam: 27723895, wparam: 7864320
MSG, hwnd: 5769574, lparam: 0, wparam: 0          
          
         */

        //private bool mWheelState = false;

        private const int MK_LBUTTON = 0x1;
        private const int MK_RBUTTON = 0x2;
        private const int MK_MBUTTON = 0x10;

        private static int LoWord(IntPtr n)
        {
            return LoWord(n.ToInt32());
        }

        private static int LoWord(int n)
        {
            return (n & 0xffff);
        }

        private static int HiWord(IntPtr n)
        {
            return HiWord(n.ToInt32());
        }

        private static int HiWord(int n)
        {
            return (n / 0xffff);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)MouseInputNotificationEnum.WM_MOUSEWHEEL || m.Msg == (int)MouseInputNotificationEnum.WM_MOUSEHWHEEL)
            {
                int xpos = LoWord(m.LParam); //m.LParam.ToInt32() & 65535;
                int ypos = HiWord(m.LParam); // m.LParam.ToInt32() / 65536;
                int MouseKeys = LoWord(m.WParam); //m.WParam.ToInt32() & 65535;
                //int Rotation = m.WParam.ToInt32() / 65536;
                int delta = HiWord(m.WParam); //Rotation / 120;
                MouseButtons mb = new MouseButtons();
                switch (MouseKeys)
                {
                    case MK_LBUTTON: mb = MouseButtons.Left;
                        break;
                    case MK_RBUTTON: mb = MouseButtons.Right;
                        break;
                    case MK_MBUTTON: mb = MouseButtons.Middle;
                        break;
                    default: mb = MouseButtons.None;
                        break;
                }

                OnInternalMouseWheel(new MouseEventExtendedArgs(mb, 0, xpos, ypos, delta));
                //base.WndProc(ref m);
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        private void OnInternalMouseWheel(MouseEventExtendedArgs e)
        {
            LogMouseEvent("WHEEL-WND", e);
            Instance_MouseWheelVerticalExtended(this, e);
        }

        private void Instance_MouseWheelVerticalExtended(object sender, MouseEventExtendedArgs e)
        {
            if (this.IsDisposed)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                EventHandler<MouseEventExtendedArgs> d = new EventHandler<MouseEventExtendedArgs>(Instance_MouseWheelVerticalExtended);
                ((ViewForm)d.Target).Invoke(d, sender, e);
                return;
            }

            if (this.Focused)
            {
                Point clientPoint = pictureBox1.PointToClient(e.Location);

                int x = 0;
                int y = 0;

                if (HorizontalScroll.Visible)
                {
                    x = HorizontalScroll.Value;
                }
                if (VerticalScroll.Visible)
                {
                    y = VerticalScroll.Value;
                }

                int endX = this.ClientRectangle.Width + x;
                int endY = this.ClientRectangle.Height + y;

                if (clientPoint.X >= x && clientPoint.X <= endX && clientPoint.Y >= y && clientPoint.Y <= endY)
                {
                    Console.WriteLine(string.Format("ClientPoint, X: {0}, Y: {1}", clientPoint.X.ToString(), clientPoint.Y.ToString()));
                }
            }
        }

        private void Instance_EventClipboardChanged(object sender, ClipboardChangedEventArgs e)
        {
            Console.WriteLine(string.Format("CLIPBOARD: {0}", e.GetText()));
        }

        private void HookManager_MouseMove(object sender, MouseEventArgs e)
        {
            LogMouseEvent("HOOK-MOVE", e);
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
        }

        private void Event_KeyPress(object sender, KeyPressEventArgs e)
        {
            Console.WriteLine(string.Format("KEYPRESS ({0}), KeyChar: {1}", sender == null ? "null" : sender.GetType().Name, e.KeyChar.ToString()));
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
        }

        private void Event_MouseWheel(object sender, MouseEventArgs e)
        {
            LogMouseEvent("WHEEL", e);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            LogMouseEvent("DOWN", e);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            LogMouseEvent("CLICK", e);
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            LogMouseEvent("DOUBLECLICK", e);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            LogMouseEvent("UP", e);
        }

        private void LogMouseEvent(string eventName, MouseEventArgs e)
        {
            StringBuilder sb = new StringBuilder(eventName);
            sb.Append(", Button: ");
            sb.Append(e.Button.ToString());
            sb.Append(", Clicks: ");
            sb.Append(e.Clicks);
            sb.Append(", Delta: ");
            sb.Append(e.Delta);
            sb.Append(", X: ");
            sb.Append(e.X.ToString());
            sb.Append(", Y: ");
            sb.Append(e.Y.ToString());
            Console.WriteLine(sb.ToString());
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //LogMouseEvent("MOVE", e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            // move cursor relative
            //MoveMouseRelative();

            // move cursor absolute
            //MoveMouseAbsolute();
            //MoveMouseRelative();

            // left down and up
            //LeftMouseDown();
            //LeftMouseUp();

            // double click
            LeftMouseDown();
            LeftMouseUp();
            Thread.Sleep(1);
            LeftMouseDown();
            LeftMouseUp();

            // XButton down and up
            //XMouse1Down();
            //XMouse1Up();

            //MouseOperations.PerformClick(MouseButtons.Middle);
            //MouseOperations.MoveWheelVertically(-1);
            MouseOperations.MoveWheelHorizontally(-1);
        }

        private void MoveMouseRelative()
        {
            SendInputInfo[] inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Mouse;
            inputs[0].Inputs.MouseInput.Dx = 53;
            inputs[0].Inputs.MouseInput.Dy = 53;
            inputs[0].Inputs.MouseInput.DwFlags = (uint)MouseEventEnum.Move;
            inputs[0].Inputs.MouseInput.DwExtraInfo = NativeMethods.GetMessageExtraInfo();

            uint intReturn = NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send mouse event.");
            }
        }

        private void MoveMouseAbsolute()
        {
            SendInputInfo[] inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Mouse;
            inputs[0].Inputs.MouseInput.Dx = 0;
            inputs[0].Inputs.MouseInput.Dy = 0;
            inputs[0].Inputs.MouseInput.DwFlags = (uint)MouseEventEnum.Move | (uint)MouseEventEnum.Absolute;
            inputs[0].Inputs.MouseInput.DwExtraInfo = NativeMethods.GetMessageExtraInfo();

            uint intReturn = NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send mouse event.");
            }
        }

        private void LeftMouseDown()
        {
            SendInputInfo[] inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Mouse;
            inputs[0].Inputs.MouseInput.DwFlags = (uint)MouseEventEnum.LeftDown;
            inputs[0].Inputs.MouseInput.DwExtraInfo = NativeMethods.GetMessageExtraInfo();

            uint intReturn = NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send mouse event.");
            }
        }

        private void LeftMouseUp()
        {
            SendInputInfo[] inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Mouse;
            inputs[0].Inputs.MouseInput.DwFlags = (uint)MouseEventEnum.LeftUp;
            inputs[0].Inputs.MouseInput.DwExtraInfo = NativeMethods.GetMessageExtraInfo();

            uint intReturn = NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send mouse event.");
            }
        }

        private void XMouse1Down()
        {
            SendInputInfo[] inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Mouse;
            inputs[0].Inputs.MouseInput.DwFlags = (uint)MouseEventEnum.XDown;
            inputs[0].Inputs.MouseInput.MouseData = (int)MouseXButtonEnum.XButton1;
            inputs[0].Inputs.MouseInput.DwExtraInfo = NativeMethods.GetMessageExtraInfo();

            uint intReturn = NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send mouse event.");
            }
        }

        private void XMouse1Up()
        {
            SendInputInfo[] inputs = new SendInputInfo[1];
            inputs[0].Type = (int)InputDeviceTypeEnum.Mouse;
            inputs[0].Inputs.MouseInput.DwFlags = (uint)MouseEventEnum.XUp;
            inputs[0].Inputs.MouseInput.MouseData = (int)MouseXButtonEnum.XButton1;
            inputs[0].Inputs.MouseInput.DwExtraInfo = NativeMethods.GetMessageExtraInfo();

            uint intReturn = NativeMethods.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send mouse event.");
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Console.WriteLine(string.Format("POS, X: {0}, Y: {1}", Cursor.Position.X.ToString(), Cursor.Position.Y.ToString()));
        }

        private void ViewForm_Shown(object sender, EventArgs e)
        {
        }

    }

}
