using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Testing.TerraGraf
{
    public partial class BroadcastForm : Form
    {

        private Wrapper mWrapper = null;

        public BroadcastForm()
        {
            InitializeComponent();
        }

        public BroadcastForm(Wrapper wrapper)
            : this()
        {
            this.mWrapper = wrapper;
        }

        private void BroadcastForm_Shown(object sender, EventArgs e)
        {
            LoadServerList();
            LoadClientList();
        }

        private void LoadServerList()
        {
            ICollection<long> serverIds = mWrapper.GetActiveBroadcastServers();
            lvServers.Items.Clear();
            foreach (long id in serverIds)
            {
                lvServers.Items.Add(id.ToString());
            }
        }

        private void LoadClientList()
        {
            ICollection<long> clientIds = mWrapper.GetActiveBroadcastClients();
            lvClients.Items.Clear();
            foreach (long id in clientIds)
            {
                lvClients.Items.Add(id.ToString());
            }
        }

        private void btStartServer_Click(object sender, EventArgs e)
        {
            mWrapper.StartBroadcastUdpServer(Convert.ToInt32(nudLocalPortServer.Value));
            LoadServerList();
        }

        private void btStartClient_Click(object sender, EventArgs e)
        {
            mWrapper.StartBroadcastUdpClient(Convert.ToInt32(nudLocalPortClient.Value), Convert.ToInt32(nudTargetPortClient.Value));
            LoadClientList();
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
                mWrapper.StopBroadcastUdpServer(id);
                LoadServerList();
            }
        }

        private void btStopClient_Click(object sender, EventArgs e)
        {
            if (lvClients.SelectedIndices.Count > 0)
            {
                long id = long.Parse(lvClients.SelectedItems[0].Text);
                mWrapper.StopBroadcastUdpClient(id);
                LoadClientList();
            }
        }

    }
}
