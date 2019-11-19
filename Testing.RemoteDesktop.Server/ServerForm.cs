using System;
using System.Windows.Forms;
using Forge.RemoteDesktop.Service;

namespace Forge.Testing.RemoteDesktop.Server
{

    public partial class ServerForm : Form
    {

        public ServerForm()
        {
            InitializeComponent();
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            if (RemoteDesktopServiceManager.Instance.Start() == Management.ManagerStateEnum.Started)
            {
                startToolStripMenuItem.Enabled = false;
                stopToolStripMenuItem.Enabled = true;
            }
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            RemoteDesktopServiceManager.Instance.Stop();
            startToolStripMenuItem.Enabled = true;
            stopToolStripMenuItem.Enabled = false;
        }

        private void passwordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AuthenticationAdminForm form = new AuthenticationAdminForm())
            {
                form.ShowDialog(this);
            }
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            RemoteDesktopServiceManager.Instance.Stop();
        }

        private void ServerForm_Shown(object sender, EventArgs e)
        {
            connectedClientsControl1.Initialize();
            btStart_Click(null, null);
        }

    }

}
