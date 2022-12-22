using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Forge.Net.TerraGraf.NetworkPeers;

namespace Testing.TerraGraf
{

    public partial class UDPForm : Form
    {

        private Wrapper mWrapper = null;

        public UDPForm()
        {
            InitializeComponent();
        }

        public UDPForm(Wrapper wrapper) : this()
        {
            this.mWrapper = wrapper;
        }

        private void BroadcastForm_Shown(object sender, EventArgs e)
        {
            LoadServerList();
            LoadClientList();
            LoadRemoteHosts();
        }

        private void LoadServerList()
        {
            ICollection<long> serverIds = mWrapper.GetActiveUDPServers();
            lvServers.Items.Clear();
            foreach (long id in serverIds)
            {
                lvServers.Items.Add(id.ToString());
            }
        }

        private void LoadClientList()
        {
            ICollection<long> clientIds = mWrapper.GetActiveUdpClients();
            lvClients.Items.Clear();
            foreach (long id in clientIds)
            {
                lvClients.Items.Add(id.ToString());
            }
        }

        private void LoadRemoteHosts()
        {
            ICollection<INetworkPeerRemote> peers = mWrapper.KnownNetworkPeers;
            lvHosts.Items.Clear();
            foreach (INetworkPeerRemote id in peers)
            {
                lvHosts.Items.Add(id.Id);
            }
            lvHosts.Items.Add(mWrapper.NetworkManager.Localhost.Id);
        }

        private void btStartServer_Click(object sender, EventArgs e)
        {
            mWrapper.StartUdpServer(Convert.ToInt32(nudLocalPortServer.Value));
            LoadServerList();
        }

        private void btStartClient_Click(object sender, EventArgs e)
        {
            if (lvHosts.SelectedIndices.Count > 0)
            {
                mWrapper.StartUdpClient(Convert.ToInt32(nudLocalPortClient.Value), lvHosts.SelectedItems[0].Text, Convert.ToInt32(nudTargetPortClient.Value), cbHugeData.Checked);
                LoadClientList();
            }
            else
            {
                MessageBox.Show(this, "Start Client", "Select a remote host.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void lvServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            btStopServer.Enabled = lvServers.SelectedIndices.Count > 0;
        }

        private void lvClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            btStopClient.Enabled = lvClients.SelectedIndices.Count > 0;
        }

        private void btStopServer_Click(object sender, EventArgs e)
        {
            if (lvServers.SelectedIndices.Count > 0)
            {
                long id = long.Parse(lvServers.SelectedItems[0].Text);
                mWrapper.StopUdpServer(id);
                LoadServerList();
            }
        }

        private void btStopClient_Click(object sender, EventArgs e)
        {
            if (lvClients.SelectedIndices.Count > 0)
            {
                long id = long.Parse(lvClients.SelectedItems[0].Text);
                mWrapper.StopUdpClient(id);
                LoadClientList();
            }
        }

    }

}
