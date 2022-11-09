using System;
using System.Windows.Forms;
using Forge.Native.Hooks;

namespace Testing.Hooks
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static void Instance_MouseMoveExtended(object sender, MouseEventExtendedArgs e)
        {
            Console.WriteLine("X: " + e.Location.X.ToString() + ", Y:" + e.Location.Y.ToString());
        }

    }
}
