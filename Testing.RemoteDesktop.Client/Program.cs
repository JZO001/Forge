using System;
using System.Windows.Forms;

namespace Forge.Testing.RemoteDesktop.Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Forge.Logging.Log4net.Log4NetManager.InitializeFromAppConfig();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MasterForm());
        }
    }
}
