using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Forge.Net.TerraGraf.NetworkPeers;

namespace Testing.TerraGraf
{

    public partial class TCPForm : Form
    {

        private Wrapper mWrapper = null;

        public TCPForm()
        {
            InitializeComponent();
        }

        public TCPForm(Wrapper wrapper) : this()
        {
            this.mWrapper = wrapper;
        }

        private void TCPForm_Shown(object sender, EventArgs e)
        {
            LoadServerList();
            LoadSocketList();
            LoadRemoteHosts();
            LoadSendersList();
        }

        private void LoadServerList()
        {
            ICollection<long> serverIds = mWrapper.GetActiveTCPServers();
            lvServers.Items.Clear();
            foreach (long id in serverIds)
            {
                lvServers.Items.Add(id.ToString());
            }
        }

        private void LoadSendersList()
        {
            ICollection<long> senderIds = mWrapper.GetActiveSendThreads();
            lvSenders.Items.Clear();
            foreach (long id in senderIds)
            {
                lvSenders.Items.Add(id.ToString());
            }
        }

        private void LoadSocketList()
        {
            ICollection<long> clientIds = mWrapper.GetActiveTCPSockets();
            lvSockets.Items.Clear();
            foreach (long id in clientIds)
            {
                lvSockets.Items.Add(id.ToString());
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
            mWrapper.StartTcpServer(Convert.ToInt32(nudLocalPortServer.Value));
            LoadServerList();
        }

        private void lvServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            btStopServer.Enabled = lvServers.SelectedIndices.Count > 0;
        }

        private void btStopServer_Click(object sender, EventArgs e)
        {
            mWrapper.StopTcpServer(long.Parse(lvServers.SelectedItems[0].Text));
            LoadServerList();
        }

        private void btCloseSocket_Click(object sender, EventArgs e)
        {
            mWrapper.DisconnectTcp(long.Parse(lvSockets.SelectedItems[0].Text));
            LoadSocketList();
        }

        private void lvSockets_SelectedIndexChanged(object sender, EventArgs e)
        {
            btCloseSocket.Enabled = lvSockets.SelectedIndices.Count > 0;
            btStartClient.Enabled = lvSockets.SelectedIndices.Count > 0;
        }

        private void lvHosts_SelectedIndexChanged(object sender, EventArgs e)
        {
            btConnect.Enabled = lvHosts.SelectedIndices.Count > 0;
        }

        private void btStartClient_Click(object sender, EventArgs e)
        {
            mWrapper.SendOnTcp(long.Parse(lvSockets.SelectedItems[0].Text), cbHugeData.Checked, cbRepeatSend.Checked);
            LoadSendersList();
        }

        private void lvSenders_SelectedIndexChanged(object sender, EventArgs e)
        {
            btCloseSender.Enabled = lvSenders.SelectedIndices.Count > 0;
        }

        private void btCloseSender_Click(object sender, EventArgs e)
        {
            mWrapper.StopTcpClient(long.Parse(lvSenders.SelectedItems[0].Text));
            LoadSendersList();
        }

        private void btConnect_Click(object sender, EventArgs e)
        {
            if (mWrapper.ConnectToOnTcp(lvHosts.SelectedItems[0].Text, Convert.ToInt32(nudRemotePort.Value)) == -1)
            {
                MessageBox.Show(this, "Failed to connect.", "Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                LoadSocketList();
            }
        }

        private void btRefresh_Click(object sender, EventArgs e)
        {
            LoadSocketList();
        }

    }

}
