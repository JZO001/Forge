using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using Forge.Net.Synapse;

namespace Testing.TerraGraf
{

    public partial class StartServerForm : Form
    {

        public StartServerForm()
        {
            InitializeComponent();
        }

        public Wrapper Wrapper { get; set; }

        private void StartServerForm_Shown(object sender, EventArgs e)
        {
            foreach (IPAddress ip in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork || (ip.AddressFamily == AddressFamily.InterNetworkV6 &&
                    Wrapper.NetworkManager.Configuration.Settings.EnableIPV6))
                {
                    ListViewItem item = new ListViewItem(ip.ToString());
                    item.Tag = ip;
                    lvInterfaces.Items.Add(item);
                }
            }
        }

        private void cbAutoPort_CheckedChanged(object sender, EventArgs e)
        {
            nudPort.Enabled = !cbAutoPort.Checked;
        }

        private void lvInterfaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            btCreateServer.Enabled = lvInterfaces.SelectedIndices.Count > 0;
        }

        private void btCreateServer_Click(object sender, EventArgs e)
        {
            int port = cbAutoPort.Checked ? 0 : Convert.ToInt32(nudPort.Value);
            AddressEndPoint aep = new AddressEndPoint(lvInterfaces.SelectedItems[0].Tag.ToString(), port);
            try
            {
                Wrapper.NetworkManager.StartServer(aep);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Failed to create server.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }

}
