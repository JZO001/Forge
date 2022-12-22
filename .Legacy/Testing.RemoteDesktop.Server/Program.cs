using System;
using System.Windows.Forms;
using Forge.Logging.Utils;

namespace Forge.Testing.RemoteDesktop.Server
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
            LogUtils.LogAll();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ServerForm());
        }
    }
}
