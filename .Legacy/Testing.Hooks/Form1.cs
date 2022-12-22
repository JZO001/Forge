using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Forge.Native.Helpers;
using Forge.Native.Hooks;

namespace Testing.Hooks
{

    // http://www.pinvoke.net/default.aspx/user32.mouse_event

    public partial class Form1 : Form
    {

        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const uint MOUSEEVENTF_RIGHTUP = 0x10;

        public Form1()
        {
            InitializeComponent();
        }

        private void btCapture_Click(object sender, EventArgs e)
        {
            ScreenShot.CaptureImage(true);
            MessageBox.Show("OK");
        }

        private void btMouseClick_Click(object sender, EventArgs e)
        {
            uint x = 53;
            uint y = 53;
            Cursor.Position = new Point((int)x, (int)y); // enélkül nem megy

            MouseOperations.PerformClick(MouseButtons.Left);
            Thread.Sleep(2000);
            MouseOperations.PerformClick(MouseButtons.Right);

            //NativeMethods.mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, x, y, 0, 0);
            //Thread.Sleep(2000);
            //NativeMethods.mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, x, y, 0, 0);
        }

        private void btMotionStart_Click(object sender, EventArgs e)
        {
            ScreenShot.StartMotion();
        }

        private void btMotionStop_Click(object sender, EventArgs e)
        {
            ScreenShot.StopMotion();
        }

        private void btTimer_Click(object sender, EventArgs e)
        {
            ScreenShot.StartTimer(Convert.ToInt32(nudTimer.Value));
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            StringBuilder sb = new StringBuilder("KEYDOWN, Alt: ");
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

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Console.WriteLine(string.Format("KEYPRESS, KeyChar: {0}", e.KeyChar.ToString()));
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            StringBuilder sb = new StringBuilder("KEYUP, Alt: ");
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

        private void btView_Click(object sender, EventArgs e)
        {
            using (ViewForm form = new ViewForm())
            {
                form.ShowDialog(this);
            }
        }

        private void btKeyDemo_Click(object sender, EventArgs e)
        {
            using (KeyDemoForm form = new KeyDemoForm())
            {
                form.ShowDialog(this);
            }
        }

        private void btMouseMoveRel_Click(object sender, EventArgs e)
        {
            MouseOperations.MoveRelative(50, 50);
        }

        private void btMouseMoveAbs_Click(object sender, EventArgs e)
        {
            MouseOperations.MoveAbsolute(53, 53);
        }

        private void btMouseLeftClick_Click(object sender, EventArgs e)
        {
            MouseOperations.PerformClick(MouseButtons.Left);
        }

        private void btMouseRightClick_Click(object sender, EventArgs e)
        {
            MouseOperations.PerformClick(MouseButtons.Right);
        }

        private void btClipboard_Click(object sender, EventArgs e)
        {
            //KeyboardEventHookManager.Instance.KeyDown += new KeyEventHandler(Instance_KeyDown);
            //KeyboardEventHookManager.Instance.KeyUp += new KeyEventHandler(Instance_KeyUp);
            //KeyboardEventHookManager.Instance.Start();
        }

        void Instance_KeyDown(object sender, KeyEventArgs e)
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

        void Instance_KeyUp(object sender, KeyEventArgs e)
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

        private void btPrintClipboard_Click(object sender, EventArgs e)
        {
            Console.WriteLine("CLIPBOARD: " + Clipboard.GetText());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MouseEventHookManager.Instance.MouseMoveExtended += new EventHandler<MouseEventExtendedArgs>(Program.Instance_MouseMoveExtended);
            MouseEventHookManager.Instance.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PopupForm form = new PopupForm();
            form.ShowDialog();
        }

    }

    class ScreenShot
    {

        private static bool mMotionActive = false;

        private static Thread mThread = null;

        private static Thread mTimer = null;

        public static void StartMotion()
        {
            mMotionActive = true;
            mThread = new Thread(new ThreadStart(MotionMain));
            mThread.IsBackground = true;
            mThread.Start();
        }

        public static void StopMotion()
        {
            mMotionActive = false;
        }

        private static void MotionMain()
        {
            while (mMotionActive)
            {
                CaptureImage(false);
                Thread.Sleep(500);
            }
        }

        public static void StartTimer(int delay)
        {
            mTimer = new Thread(new ParameterizedThreadStart(TimerMain));
            mTimer.IsBackground = true;
            mTimer.Start(delay);
        }

        private static void TimerMain(object value)
        {
            int delay = (int)value;
            Thread.Sleep(delay);
            CaptureImage(true);
        }

        public static void CaptureImage(bool saveImage)
        {
            int index = 0;
            foreach (Screen scr in Screen.AllScreens)
            {
                using (Bitmap bitmap = new Bitmap(scr.Bounds.Width, scr.Bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        Size size = scr.Bounds.Size; // full screen
                        //g.CopyFromScreen(scr.Bounds.Location, new Point(0, 0), size);
                        g.CopyFromScreen(scr.Bounds.Location, new Point(0, 0), new Size(50, 50));
                        
                        // itt állapítom meg, hogy hol a cursor
                        Rectangle cursorBounds = new Rectangle(Cursor.Position, Cursor.Current.Size);
                        if (scr.Equals(Screen.FromRectangle(cursorBounds)))
                        {
                            Point p = Cursor.Current.HotSpot; // ez jelenti, hogy a kurzor melyik pontja végzi az interakciót
                            // ezen a képernyőn van, de a koordináták abszolút értékek, nem a screen-re vonatkoznak
                            Cursor.Current.Draw(g, cursorBounds);
                        }
                    }

                    if (saveImage)
                    {
                        ImageCodecInfo jgpEncoder = GetImageEncoder(ImageFormat.Jpeg);

                        EncoderParameters encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 10L);

                        bitmap.Save(string.Format("screen_{0}.jpg", index.ToString()), jgpEncoder, encoderParameters);
                    }
                }
                index++;
            }
        }

        private static ImageCodecInfo GetImageEncoder(ImageFormat format)
        {
            ImageCodecInfo result = null;

            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
            {
                if (codec.FormatID == format.Guid)
                {
                    result = codec;
                    break;
                }
            }

            return result;
        }

    }

}
