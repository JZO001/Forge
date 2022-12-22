using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Forge.Net.TerraGraf.NetworkPeers;

namespace Testing.TerraGraf
{

    public partial class RemotingForm : Form
    {

        private Wrapper mWrapper = null;

        public RemotingForm()
        {
            InitializeComponent();
        }

        public RemotingForm(Wrapper wrapper)
            : this()
        {
            this.mWrapper = wrapper;
        }

        private void RemotingForm_Shown(object sender, EventArgs e)
        {
            FillNetworkPeers();
            FillChannels();
            btStartInterNetwork.Enabled = !mWrapper.IsRemoteTestingInterNetwork;
            btStartTerragraf.Enabled = !mWrapper.IsRemoteTestingTerraGraf;
            btStopInternetwork.Enabled = mWrapper.IsRemoteTestingInterNetwork;
            btStopTerraGraf.Enabled = mWrapper.IsRemoteTestingTerraGraf;
        }

        private void btStartInterNetwork_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbAddress.Text.Trim()))
            {
                MessageBox.Show(this, "Missing remote address.", "Test", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            if (lvChannels.SelectedIndices.Count == 0)
            {
                MessageBox.Show(this, "Please select a channel.", "Test", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            mWrapper.StartRemotingTestInterNetwork(lvChannels.SelectedItems[0].Text, new Forge.Net.Synapse.AddressEndPoint(tbAddress.Text, Convert.ToInt32(nudPort.Value), System.Net.Sockets.AddressFamily.InterNetwork));
            btStartInterNetwork.Enabled = false;
            btStopInternetwork.Enabled = true;
        }

        private void btStopInternetwork_Click(object sender, EventArgs e)
        {
            mWrapper.StopRemotingTestInterNetwork();
            btStartInterNetwork.Enabled = true;
            btStopInternetwork.Enabled = false;
        }

        private void btStartTerragraf_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbAddress.Text.Trim()))
            {
                MessageBox.Show(this, "Missing remote address.", "Test", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            if (lvChannels.SelectedIndices.Count == 0)
            {
                MessageBox.Show(this, "Please select a channel.", "Test", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            mWrapper.StartRemotingTestTerraGraf(lvChannels.SelectedItems[0].Text, new Forge.Net.Synapse.AddressEndPoint(tbAddress.Text, Convert.ToInt32(nudPort.Value), System.Net.Sockets.AddressFamily.InterNetwork));
            btStartTerragraf.Enabled = false;
            btStopTerraGraf.Enabled = true;
        }

        private void btStopTerraGraf_Click(object sender, EventArgs e)
        {
            mWrapper.StopRemotingTestTerraGraf();
            btStartTerragraf.Enabled = true;
            btStopTerraGraf.Enabled = false;
        }

        private void btRefreshChannels_Click(object sender, EventArgs e)
        {
            FillChannels();
        }

        private void FillChannels()
        {
            while (lvChannels.Items.Count > 0)
            {
                lvChannels.Items.RemoveAt(0);
            }
            ICollection<string> channels = mWrapper.RegisteredChannels;
            foreach (string channelId in channels)
            {
                lvChannels.Items.Add(channelId);
            }
        }

        private void FillNetworkPeers()
        {
            while (lvNetworkPeers.Items.Count > 0)
            {
                lvNetworkPeers.Items.RemoveAt(0);
            }
            foreach (INetworkPeerRemote peer in mWrapper.KnownNetworkPeers)
            {
                lvNetworkPeers.Items.Add(peer.Id);
            }
            lvNetworkPeers.Items.Add(mWrapper.NetworkManager.Localhost.Id);
        }

        private void lvNetworkPeers_DoubleClick(object sender, EventArgs e)
        {
            if (lvNetworkPeers.SelectedIndices.Count > 0)
            {
                tbAddress.Text = lvNetworkPeers.SelectedItems[0].Text;
            }
        }

        private void btStartServer_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbChannelId.Text.Trim()))
            {
                MessageBox.Show(this, "Missing channel id.", "Channel", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            if (string.IsNullOrEmpty(tbServerAddress.Text.Trim()))
            {
                MessageBox.Show(this, "Missing local endpoint address.", "Channel", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            mWrapper.InitializeIntNetServerChannel(tbChannelId.Text, new Forge.Net.Synapse.AddressEndPoint(tbServerAddress.Text, Convert.ToInt32(nudServerPort.Value), System.Net.Sockets.AddressFamily.InterNetwork), rbInterNetwork.Checked ? NetworkFactoryType.InterNetwork : NetworkFactoryType.TerraGraf);
            FillChannels();
        }

    }

}
