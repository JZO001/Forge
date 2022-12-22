using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Forge.Configuration;
using Forge.Configuration.Shared;
using Forge.Net.Remoting.ProxyGenerator;
using Forge.Net.TerraGraf;
using Forge.Net.TerraGraf.Contexts;
using Forge.Net.TerraGraf.NetworkPeers;
using Testing.TerraGraf.ConfigSection;
using Testing.TerraGraf.Contracts;
using log4net;

namespace Testing.TerraGraf
{

    internal delegate void Unaccessible(object sender, NetworkPeerChangedEventArgs e);
    internal delegate void DistanceChanged(object sender, NetworkPeerChangedEventArgs e);
    internal delegate void Discovered(object sender, NetworkPeerChangedEventArgs e);
    internal delegate void ContextChanged(object sender, NetworkPeerContextEventArgs e);

    public partial class MasterForm : Form
    {

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(MasterForm));

        private Dictionary<string, TreeView> mTreeViewsById = new Dictionary<string, TreeView>();
        private Dictionary<string, Wrapper> mWrapper = new Dictionary<string, Wrapper>();
        private Dictionary<string, Button> mButtonDebugs = new Dictionary<string, Button>();
        private Dictionary<string, CheckBox> mBlackHoleCheckBoxes = new Dictionary<string, CheckBox>();
        private Dictionary<string, Dictionary<string, TreeNode>> mRootNodes = new Dictionary<string, Dictionary<string, TreeNode>>();
        private Dictionary<string, Dictionary<string, TreeNode>> mPeerNodes = new Dictionary<string, Dictionary<string, TreeNode>>();
        private Dictionary<string, ListView> mListViewNetworkConnections = new Dictionary<string, ListView>();
        private Dictionary<string, ListView> mListViewServers = new Dictionary<string, ListView>();
        private Dictionary<string, Button> mButtonStartServers = new Dictionary<string, Button>();
        private Dictionary<string, Button> mButtonStopServers = new Dictionary<string, Button>();
        private Dictionary<string, TextBox> mContexts = new Dictionary<string, TextBox>();
        private Dictionary<string, TextBox> mConnections = new Dictionary<string, TextBox>();

        public MasterForm()
        {
            InitializeComponent();
        }

        public void MbrPing()
        {
        }

        private void MasterForm_Shown(object sender, EventArgs e)
        {
            foreach (CategoryPropertyItem item in TerraGrafTestConfiguration.Settings.CategoryPropertyItems)
            {
                lvConfigs.Items.Add(item.Id);
            }
        }

        public override object InitializeLifetimeService()
        {
            //ILease lease = (ILease)base.InitializeLifetimeService();
            //if (lease.CurrentState == LeaseState.Initial)
            //{
            //    lease.InitialLeaseTime = TimeSpan.Zero; // lease time does not expire
            //}
            //return lease;
            return null;
        }

        private void lvConfigs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvConfigs.SelectedIndices.Count > 0)
            {
                btLoadConfig.Enabled = true;
            }
            else
            {
                btLoadConfig.Enabled = false;
            }
        }

        private void btLoadConfig_Click(object sender, EventArgs e)
        {
            string configId = lvConfigs.SelectedItems[0].Text;
            CategoryPropertyItem configItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(TerraGrafTestConfiguration.Settings.CategoryPropertyItems, configId);
            int i = 0;
            foreach (CategoryPropertyItem item in configItem)
            {
                string domainId = item.Id;
                string configDir = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration"), configId);
                string configFile = Path.Combine(configDir, item.EntryValue);

                AppDomainSetup domainInfo = new AppDomainSetup();
                domainInfo.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
                domainInfo.ConfigurationFile = configFile;
                domainInfo.ApplicationName = AppDomain.CurrentDomain.SetupInformation.ApplicationName;

                AppDomain domain = AppDomain.CreateDomain(domainId, AppDomain.CurrentDomain.Evidence, domainInfo);

                TreeView treeView = new TreeView();
                Button btInit = new Button();
                Button btDebug = new Button();
                ListView lvNetworkConnections = new ListView();
                ColumnHeader chNetworkConnections = new ColumnHeader();
                Button btDisconnect = new Button();
                Button btDisconnectActive = new Button();
                CheckBox cbBlackHole = new CheckBox();
                Panel pServers = new System.Windows.Forms.Panel();
                ListView lvServers = new System.Windows.Forms.ListView();
                Button btStartServer = new System.Windows.Forms.Button();
                Button btStopServer = new System.Windows.Forms.Button();
                ColumnHeader chServers = new System.Windows.Forms.ColumnHeader();
                Label lContext = new System.Windows.Forms.Label();
                TextBox tbContext = new System.Windows.Forms.TextBox();
                Button btSendContext = new System.Windows.Forms.Button();
                Label lConnect = new Label();
                Button btConnect = new Button();
                TextBox tbConnect = new TextBox();
                Button btBroadcast = new Button();
                Button btUDP = new Button();
                Button btTcp = new Button();
                Button btSockets = new System.Windows.Forms.Button();
                Button btRemoting = new System.Windows.Forms.Button();
                Button btEverlight = new System.Windows.Forms.Button();
                Button btUpdateWF = new Button();

                TabPage page = new TabPage(domainId);
                //page.Controls.Add(btUpdateWF);
                //page.Controls.Add(btEverlight);
                page.Controls.Add(btRemoting);
                page.Controls.Add(btSockets);
                page.Controls.Add(btTcp);
                page.Controls.Add(btUDP);
                page.Controls.Add(btBroadcast);
                page.Controls.Add(btConnect);
                page.Controls.Add(tbConnect);
                page.Controls.Add(lConnect);
                page.Controls.Add(btSendContext);
                page.Controls.Add(tbContext);
                page.Controls.Add(lContext);
                page.Controls.Add(pServers);
                page.Controls.Add(btDisconnectActive);
                page.Controls.Add(btDisconnect);
                page.Controls.Add(cbBlackHole);
                page.Controls.Add(lvNetworkConnections);
                page.Controls.Add(btDebug);
                page.Controls.Add(btInit);
                page.Controls.Add(treeView);
                page.Location = new System.Drawing.Point(4, 22);
                page.Size = new System.Drawing.Size(571, 343);
                page.TabIndex = i + 1;
                page.Text = domainId;
                page.UseVisualStyleBackColor = true;
                page.Tag = domain;

                cbBlackHole.AutoSize = true;
                cbBlackHole.Location = new System.Drawing.Point(170, 7);
                cbBlackHole.Size = new System.Drawing.Size(75, 17);
                cbBlackHole.TabIndex = 2;
                cbBlackHole.Text = "BlackHole";
                cbBlackHole.UseVisualStyleBackColor = true;
                cbBlackHole.Enabled = false;
                cbBlackHole.Tag = domain;
                // TODO: blackhole értéket kiolvasni
                cbBlackHole.CheckedChanged += new System.EventHandler(this.cbBlackHole_CheckedChanged);

                btDisconnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
                btDisconnect.Location = new System.Drawing.Point(230, 283);
                btDisconnect.Size = new System.Drawing.Size(75, 23);
                btDisconnect.TabIndex = 1;
                btDisconnect.Text = "Disconnect";
                btDisconnect.UseVisualStyleBackColor = true;
                btDisconnect.Click += new System.EventHandler(this.btDisconnect_Click);

                btDisconnectActive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
                btDisconnectActive.Location = new System.Drawing.Point(230, 312);
                btDisconnectActive.Size = new System.Drawing.Size(75, 23);
                btDisconnectActive.TabIndex = 3;
                btDisconnectActive.Text = "Dis. Active";
                btDisconnectActive.UseVisualStyleBackColor = true;
                btDisconnectActive.Click += new System.EventHandler(this.btDisconnectActive_Click);

                treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
                treeView.Location = new System.Drawing.Point(8, 37);
                treeView.Size = new System.Drawing.Size(216, 298);
                treeView.TabIndex = 1;
                treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView_AfterSelect);

                btInit.Location = new System.Drawing.Point(8, 3);
                btInit.Size = new System.Drawing.Size(75, 23);
                btInit.TabIndex = 0;
                btInit.Text = "Initialize";
                btInit.UseVisualStyleBackColor = true;
                btInit.Click += new System.EventHandler(this.btInitialize_Click);

                btDebug.Location = new System.Drawing.Point(89, 3);
                btDebug.Size = new System.Drawing.Size(75, 23);
                btDebug.TabIndex = 2;
                btDebug.Text = "Debug";
                btDebug.UseVisualStyleBackColor = true;
                btDebug.Click += new EventHandler(btDebug_Click);
                btDebug.Enabled = false;

                chNetworkConnections.Text = "Id";
                chNetworkConnections.Width = 50;

                lvNetworkConnections.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Right)));
                lvNetworkConnections.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { chNetworkConnections });
                lvNetworkConnections.FullRowSelect = true;
                lvNetworkConnections.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
                lvNetworkConnections.HideSelection = false;
                lvNetworkConnections.Location = new System.Drawing.Point(230, 37);
                lvNetworkConnections.MultiSelect = false;
                lvNetworkConnections.Size = new System.Drawing.Size(75, 240);
                lvNetworkConnections.TabIndex = 0;
                lvNetworkConnections.UseCompatibleStateImageBehavior = false;
                lvNetworkConnections.View = System.Windows.Forms.View.Details;

                pServers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
                pServers.Controls.Add(btStopServer);
                pServers.Controls.Add(btStartServer);
                pServers.Controls.Add(lvServers);
                pServers.Location = new System.Drawing.Point(311, 37);
                pServers.Size = new System.Drawing.Size(252, 139);
                pServers.TabIndex = 4;

                lvServers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
                lvServers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { chServers });
                lvServers.FullRowSelect = true;
                lvServers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
                lvServers.HideSelection = false;
                lvServers.Location = new System.Drawing.Point(3, 3);
                lvServers.MultiSelect = false;
                lvServers.Size = new System.Drawing.Size(162, 133);
                lvServers.TabIndex = 0;
                lvServers.UseCompatibleStateImageBehavior = false;
                lvServers.View = System.Windows.Forms.View.Details;
                lvServers.SelectedIndexChanged += new System.EventHandler(this.lvServers_SelectedIndexChanged);

                btStartServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
                btStartServer.Location = new System.Drawing.Point(171, 3);
                btStartServer.Size = new System.Drawing.Size(75, 23);
                btStartServer.TabIndex = 1;
                btStartServer.Text = "Start";
                btStartServer.UseVisualStyleBackColor = true;
                btStartServer.Click += new System.EventHandler(this.btStartServer_Click);

                btStopServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
                btStopServer.Enabled = false;
                btStopServer.Location = new System.Drawing.Point(171, 32);
                btStopServer.Size = new System.Drawing.Size(75, 23);
                btStopServer.TabIndex = 2;
                btStopServer.Text = "Stop";
                btStopServer.UseVisualStyleBackColor = true;
                btStopServer.Click += new System.EventHandler(this.btStopServer_Click);

                chServers.Text = "Endpoints";
                chServers.Width = 128;

                lContext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
                lContext.AutoSize = true;
                lContext.Location = new System.Drawing.Point(314, 183);
                lContext.Size = new System.Drawing.Size(46, 13);
                lContext.TabIndex = 5;
                lContext.Text = "Context:";

                tbContext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
                tbContext.Location = new System.Drawing.Point(366, 179);
                tbContext.Size = new System.Drawing.Size(100, 20);
                tbContext.TabIndex = 6;

                btSendContext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
                btSendContext.Location = new System.Drawing.Point(482, 178);
                btSendContext.Size = new System.Drawing.Size(75, 23);
                btSendContext.TabIndex = 7;
                btSendContext.Text = "Send";
                btSendContext.UseVisualStyleBackColor = true;
                btSendContext.Click += new System.EventHandler(this.btSendContext_Click);

                lConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
                lConnect.AutoSize = true;
                lConnect.Location = new System.Drawing.Point(311, 209);
                lConnect.Size = new System.Drawing.Size(47, 13);
                lConnect.TabIndex = 8;
                lConnect.Text = "Connect";

                tbConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
                tbConnect.Location = new System.Drawing.Point(366, 206);
                tbConnect.Size = new System.Drawing.Size(100, 20);
                tbConnect.TabIndex = 9;

                btConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
                btConnect.Location = new System.Drawing.Point(482, 206);
                btConnect.Size = new System.Drawing.Size(75, 23);
                btConnect.TabIndex = 10;
                btConnect.Text = "Connect";
                btConnect.UseVisualStyleBackColor = true;
                btConnect.Click += new System.EventHandler(this.btConnect_Click);

                btBroadcast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
                btBroadcast.Location = new System.Drawing.Point(311, 232);
                btBroadcast.Size = new System.Drawing.Size(75, 23);
                btBroadcast.TabIndex = 11;
                btBroadcast.Text = "Broadcast";
                btBroadcast.UseVisualStyleBackColor = true;
                btBroadcast.Click += new System.EventHandler(this.btBroadcast_Click);

                btUDP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
                btUDP.Location = new System.Drawing.Point(393, 232);
                btUDP.Size = new System.Drawing.Size(75, 23);
                btUDP.TabIndex = 12;
                btUDP.Text = "UDP";
                btUDP.UseVisualStyleBackColor = true;
                btUDP.Click += new System.EventHandler(this.btUDP_Click);

                btTcp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
                btTcp.Location = new System.Drawing.Point(475, 232);
                btTcp.Size = new System.Drawing.Size(75, 23);
                btTcp.TabIndex = 13;
                btTcp.Text = "TCP";
                btTcp.UseVisualStyleBackColor = true;
                btTcp.Click += new System.EventHandler(this.btTcp_Click);

                btSockets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
                btSockets.Location = new System.Drawing.Point(311, 261);
                btSockets.Size = new System.Drawing.Size(75, 23);
                btSockets.TabIndex = 14;
                btSockets.Text = "Sockets";
                btSockets.UseVisualStyleBackColor = true;
                btSockets.Click += new System.EventHandler(this.btSockets_Click);

                btRemoting.Location = new System.Drawing.Point(393, 261);
                btRemoting.Size = new System.Drawing.Size(75, 23);
                btRemoting.TabIndex = 15;
                btRemoting.Text = "Remoting";
                btRemoting.UseVisualStyleBackColor = true;
                btRemoting.Click += new System.EventHandler(this.btRemoting_Click);

                tcDomains.TabPages.Add(page);
                mTreeViewsById.Add(domainId, treeView);
                mButtonDebugs.Add(domainId, btDebug);
                mListViewNetworkConnections.Add(domainId, lvNetworkConnections);
                mBlackHoleCheckBoxes.Add(domainId, cbBlackHole);
                mListViewServers.Add(domainId, lvServers);
                mButtonStartServers.Add(domainId, btStartServer);
                mButtonStopServers.Add(domainId, btStopServer);
                mContexts.Add(domainId, tbContext);
                mConnections.Add(domainId, tbConnect);

                Wrapper wrapper = (Wrapper)domain.CreateInstanceAndUnwrap(typeof(Wrapper).Assembly.FullName, typeof(Wrapper).FullName);
                mWrapper.Add(domainId, wrapper);
                //wrapper.NetworkPeerDiscovered += new EventHandler<NetworkPeerChangedEventArgs>(wrapper_NetworkPeerDiscovered);
                //wrapper.NetworkPeerDistanceChanged += new EventHandler<NetworkPeerChangedEventArgs>(wrapper_NetworkPeerDistanceChanged);
                //wrapper.NetworkPeerUnaccessible += new EventHandler<NetworkPeerChangedEventArgs>(wrapper_NetworkPeerUnaccessible);
                //wrapper.NetworkPeerContextChanged += new EventHandler<NetworkPeerContextEventArgs>(wrapper_NetworkPeerContextChanged);

                mRootNodes.Add(domainId, new Dictionary<string, TreeNode>());
                mPeerNodes.Add(domainId, new Dictionary<string, TreeNode>());

                lvServers.Tag = wrapper;
                btStartServer.Tag = wrapper;
                btStopServer.Tag = wrapper;
                btSendContext.Tag = wrapper;
                btConnect.Tag = wrapper;
                btBroadcast.Tag = wrapper;
                btUDP.Tag = wrapper;
                btTcp.Tag = wrapper;
                btSockets.Tag = wrapper;
                btRemoting.Tag = wrapper;
                btEverlight.Tag = wrapper;
                btUpdateWF.Tag = wrapper;

                i++;
            }
            btLoadConfig.Enabled = false;
            lvConfigs.Enabled = false;
        }

        private void btDebug_Click(object sender, EventArgs e)
        {
            if (tcDomains.SelectedIndex > 0)
            {
                string id = tcDomains.SelectedTab.Text;
                Wrapper wrapper = mWrapper[id];
                wrapper.Debug();
            }
        }

        internal void wrapper_NetworkPeerUnaccessible(object sender, NetworkPeerChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Unaccessible d = new Unaccessible(wrapper_NetworkPeerUnaccessible);
                ((MasterForm)d.Target).Invoke(d, new object[] { sender, e });
                return;
            }

            Wrapper wrapper = (Wrapper)sender;
            LOGGER.Info("NetworkPeerUnaccessible, HostId: " + wrapper.NetworkManager.Localhost.Id + ", Count: " + e.NetworkPeers.Count);

            Dictionary<string, TreeNode> nodes = mPeerNodes[wrapper.Id];
            foreach (INetworkPeerRemote peer in e.NetworkPeers)
            {
                TreeNode node = nodes[peer.Id];
                node.Text = string.Format("{0} ({1})", peer.Id, peer.Distance);
                SetNodeFont(node, peer);
            }
        }

        internal void wrapper_NetworkPeerDistanceChanged(object sender, NetworkPeerChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                DistanceChanged d = new DistanceChanged(wrapper_NetworkPeerDistanceChanged);
                ((MasterForm)d.Target).Invoke(d, new object[] { sender, e });
                return;
            }

            Wrapper wrapper = (Wrapper)sender;
            LOGGER.Info("NetworkPeerDistanceChanged, HostId: " + wrapper.NetworkManager.Localhost.Id + ", Count: " + e.NetworkPeers.Count);

            Dictionary<string, TreeNode> nodes = mPeerNodes[wrapper.Id];
            foreach (INetworkPeerRemote peer in e.NetworkPeers)
            {
                TreeNode node = nodes[peer.Id];
                node.Text = string.Format("{0} ({1})", peer.Id, peer.Distance);
                SetNodeFont(node, peer);
            }
        }

        internal void wrapper_NetworkPeerDiscovered(object sender, NetworkPeerChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Discovered d = new Discovered(wrapper_NetworkPeerDiscovered);
                ((MasterForm)d.Target).Invoke(d, new object[] { sender, e });
                return;
            }

            Wrapper wrapper = (Wrapper)sender;
            LOGGER.Info("NetworkPeerDiscovered, HostId: " + wrapper.NetworkManager.Localhost.Id + ", Count: " + e.NetworkPeers.Count);

            TreeView tv = mTreeViewsById[wrapper.Id];
            ICollection<NetworkContext> contexts = wrapper.KnownNetworkContexts;
            int rootIndex = 0;
            foreach (NetworkContext nc in contexts)
            {
                TreeNode rootNode = null;
                if (tv.Nodes.Count <= rootIndex || !tv.Nodes[rootIndex].Text.Equals(nc.Name))
                {
                    rootNode = new TreeNode(nc.Name);
                    tv.Nodes.Insert(rootIndex, rootNode);
                    mRootNodes[wrapper.Id].Add(nc.Name, rootNode);
                }
                else
                {
                    rootNode = mRootNodes[wrapper.Id][nc.Name];
                }

                ICollection<INetworkPeerRemote> peers = nc.KnownNetworkPeers;
                int i = 0;
                foreach (INetworkPeerRemote peer in peers)
                {
                    string peerId = peer.Id;
                    if (rootNode.Nodes.Count <= i || !rootNode.Nodes[i].Name.Equals(peerId))
                    {
                        TreeNode node = new TreeNode(string.Format("{0} ({1})", peerId, peer.Distance));
                        node.Tag = peer;
                        node.Name = peerId;
                        mPeerNodes[wrapper.Id].Add(peer.Id, node);
                        rootNode.Nodes.Insert(i, node);
                        SetNodeFont(node, peer);
                    }
                    i++;
                }

                rootIndex++;
            }
        }

        internal void wrapper_NetworkPeerContextChanged(object sender, NetworkPeerContextEventArgs e)
        {
            if (InvokeRequired)
            {
                ContextChanged d = new ContextChanged(wrapper_NetworkPeerContextChanged);
                ((MasterForm)d.Target).Invoke(d, new object[] { sender, e });
                return;
            }
            Wrapper wrapper = (Wrapper)sender;
            LOGGER.Info("NetworkPeerContextChanged, HostId: " + wrapper.NetworkManager.Localhost.Id + ", RemotePeer: " + e.NetworkPeer.Id);
        }

        private void SetNodeFont(TreeNode node, INetworkPeerRemote peer)
        {
            if (peer.Distance == 0)
            {
                node.ForeColor = Color.Red;
            }
            else if (peer.Distance == 1)
            {
                node.ForeColor = Color.Green;
            }
            else
            {
                node.ForeColor = Color.Blue;
            }
        }

        private void btInitialize_Click(object sender, EventArgs e)
        {
            if (tcDomains.SelectedIndex > 0)
            {
                string id = tcDomains.SelectedTab.Text;

                Wrapper wrapper = mWrapper[id];
                wrapper.Initialize(id, this);

                Button bt = (Button)sender;
                bt.Enabled = false;
                mButtonDebugs[id].Enabled = true;

                CheckBox cb = mBlackHoleCheckBoxes[id];
                cb.Enabled = true;
                cb.CheckedChanged -= new System.EventHandler(this.cbBlackHole_CheckedChanged);
                cb.Checked = wrapper.IsBlackHole;
                cb.CheckedChanged += new System.EventHandler(this.cbBlackHole_CheckedChanged);

                ListView lvServers = mListViewServers[id];
                foreach (TCPServer server in wrapper.NetworkManager.Localhost.TCPServers)
                {
                    ListViewItem item = new ListViewItem(server.EndPoint.ToString());
                    item.Tag = server;
                    lvServers.Items.Add(item);
                }
            }
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent != null)
            {
                string id = tcDomains.SelectedTab.Text;
                Wrapper wrapper = mWrapper[id];
                INetworkPeerRemote peer = (INetworkPeerRemote)e.Node.Tag;
                ListView lv = mListViewNetworkConnections[id];
                lv.Items.Clear();
                foreach (INetworkConnection c in peer.NetworkConnections)
                {
                    lv.Items.Add(new ListViewItem(c.Id.ToString()) { Tag = c });
                }
            }
        }

        private void btDisconnect_Click(object sender, EventArgs e)
        {
            string id = tcDomains.SelectedTab.Text;
            Wrapper wrapper = mWrapper[id];
            ListView lv = mListViewNetworkConnections[id];
            if (lv.SelectedIndices.Count > 0)
            {
                INetworkConnection c = (INetworkConnection)lv.SelectedItems[0].Tag;
                c.Close();
            }
        }

        private void btDisconnectActive_Click(object sender, EventArgs e)
        {
            string id = tcDomains.SelectedTab.Text;
            Wrapper wrapper = mWrapper[id];
            TreeView tv = mTreeViewsById[id];
            if (tv.SelectedNode != null && tv.SelectedNode.Parent != null)
            {
                INetworkPeerRemote peer = (INetworkPeerRemote)tv.SelectedNode.Tag;
                INetworkConnection c = peer.ActiveNetworkConnection;
                if (c != null)
                {
                    c.Close();
                }
            }
        }

        private void cbBlackHole_CheckedChanged(object sender, EventArgs e)
        {
            string id = tcDomains.SelectedTab.Text;
            Wrapper wrapper = mWrapper[id];
            wrapper.IsBlackHole = ((CheckBox)sender).Checked;
        }

        private void lvServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView lv = (ListView)sender;
            Wrapper wrapper = (Wrapper)lv.Tag;
            Button stopServer = mButtonStopServers[wrapper.Id];
            stopServer.Enabled = lv.SelectedIndices.Count > 0;
        }

        private void btStartServer_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;
            Wrapper wrapper = (Wrapper)bt.Tag;
            using (StartServerForm form = new StartServerForm())
            {
                form.Wrapper = wrapper;
                form.ShowDialog();

                ListView lvServers = mListViewServers[wrapper.Id];
                lvServers.Items.Clear();
                foreach (TCPServer server in wrapper.NetworkManager.Localhost.TCPServers)
                {
                    ListViewItem item = new ListViewItem(server.EndPoint.ToString());
                    item.Tag = server;
                    lvServers.Items.Add(item);
                }

            }
        }

        private void btStopServer_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;
            Wrapper wrapper = (Wrapper)bt.Tag;
            ListView lv = mListViewServers[wrapper.Id];
            TCPServer server = (TCPServer)lv.SelectedItems[0].Tag;
            wrapper.NetworkManager.StopServer(server.ServerId);
            lv.Items.Remove(lv.SelectedItems[0]);
        }

        private void btSendContext_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;
            Wrapper wrapper = (Wrapper)bt.Tag;
            TextBox tb = mContexts[wrapper.Id];

            wrapper.NetworkManager.Localhost.PeerContextLock.Lock();
            try
            {
                NetworkPeerDataContext data = wrapper.NetworkManager.Localhost.PeerContext;

                if (data == null)
                {
                    data = new NetworkPeerDataContext();
                }

                PropertyItem pi = null;
                if (data.PropertyItems.ContainsKey("TEST"))
                {
                    pi = data.PropertyItems["TEST"];
                    pi.Value = tb.Text;
                }
                else
                {
                    pi = new PropertyItem("TEST", tb.Text);
                    data.PropertyItems["TEST"] = pi;
                }
                wrapper.NetworkManager.Localhost.PeerContext = data;
            }
            finally
            {
                wrapper.NetworkManager.Localhost.PeerContextLock.Unlock();
            }

        }

        private void btConnect_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;
            Wrapper wrapper = (Wrapper)bt.Tag;
            TextBox tb = mConnections[wrapper.Id];

            try
            {
                wrapper.Connect(tb.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Connection", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                LOGGER.Error("", ex);
            }
        }

        private void btBroadcast_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;
            Wrapper wrapper = (Wrapper)bt.Tag;
            using (BroadcastForm form = new BroadcastForm(wrapper))
            {
                form.ShowDialog();
            }
        }

        private void btUDP_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;
            Wrapper wrapper = (Wrapper)bt.Tag;
            using (UDPForm form = new UDPForm(wrapper))
            {
                form.ShowDialog();
            }
        }

        private void btTcp_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;
            Wrapper wrapper = (Wrapper)bt.Tag;
            using (TCPForm form = new TCPForm(wrapper))
            {
                form.ShowDialog();
            }
        }

        private void btGenerator_Click(object sender, EventArgs e)
        {
            ProxyGenerator<ITestContract> g1 = new ProxyGenerator<ITestContract>();
            g1.Generate("ITestContractDir");
            File.Copy("ITestContractDir\\TestContractAbstractClientProxy.cs", "..\\..\\RemotingClient\\TestContractAbstractClientProxy.cs", true);
            File.Copy("ITestContractDir\\TestContractAbstractServiceProxy.cs", "..\\..\\RemotingService\\TestContractAbstractServiceProxy.cs", true);
            File.Copy("ITestContractDir\\TestContractClientImpl.cs", "..\\..\\RemotingClient\\TestContractClientImpl.cs", true);
            File.Copy("ITestContractDir\\TestContractServiceImpl.cs", "..\\..\\RemotingService\\TestContractServiceImpl.cs", true);

            ProxyGenerator<ITestContractSimple> g2 = new ProxyGenerator<ITestContractSimple>();
            g2.Generate("ITestContractSimpleDir");
            File.Copy("ITestContractSimpleDir\\TestContractSimpleAbstractClientProxy.cs", "..\\..\\RemotingClient\\TestContractSimpleAbstractClientProxy.cs", true);
            //File.Copy("ITestContractSimpleDir\\TestContractSimpleAbstractServiceProxy.cs", "..\\..\\RemotingService\\TestContractSimpleAbstractServiceProxy.cs", true);
            File.Copy("ITestContractSimpleDir\\TestContractSimpleClientImpl.cs", "..\\..\\RemotingClient\\TestContractSimpleClientImpl.cs", true);
            File.Copy("ITestContractSimpleDir\\TestContractSimpleServiceImpl.cs", "..\\..\\RemotingService\\TestContractSimpleServiceImpl.cs", true);

            ProxyGenerator<ITestContractStream> g3 = new ProxyGenerator<ITestContractStream>();
            g3.Generate("ITestContractStreamDir");
            File.Copy("ITestContractStreamDir\\TestContractStreamAbstractClientProxy.cs", "..\\..\\RemotingClient\\TestContractStreamAbstractClientProxy.cs", true);
            File.Copy("ITestContractStreamDir\\TestContractStreamAbstractServiceProxy.cs", "..\\..\\RemotingService\\TestContractStreamAbstractServiceProxy.cs", true);
            File.Copy("ITestContractStreamDir\\TestContractStreamClientImpl.cs", "..\\..\\RemotingClient\\TestContractStreamClientImpl.cs", true);
            File.Copy("ITestContractStreamDir\\TestContractStreamServiceImpl.cs", "..\\..\\RemotingService\\TestContractStreamServiceImpl.cs", true);

            ProxyGenerator<ITestSingleCall> g4 = new ProxyGenerator<ITestSingleCall>();
            g4.Generate("ITestSingleCallDir");
            File.Copy("ITestSingleCallDir\\TestSingleCallClientImpl.cs", "..\\..\\RemotingClient\\TestSingleCallClientImpl.cs", true);
            File.Copy("ITestSingleCallDir\\TestSingleCallServiceImpl.cs", "..\\..\\RemotingService\\TestSingleCallServiceImpl.cs", true);

            ProxyGenerator<ITestSingleton> g5 = new ProxyGenerator<ITestSingleton>();
            g5.Generate("ITestSingletonDir");
            File.Copy("ITestSingletonDir\\TestSingletonClientImpl.cs", "..\\..\\RemotingClient\\TestSingletonClientImpl.cs", true);
            File.Copy("ITestSingletonDir\\TestSingletonServiceImpl.cs", "..\\..\\RemotingService\\TestSingletonServiceImpl.cs", true);

        }

        private void btSockets_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;
            Wrapper wrapper = (Wrapper)bt.Tag;
            using (SocketListForm form = new SocketListForm(wrapper))
            {
                form.ShowDialog();
            }
        }

        private void btRemoting_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;
            Wrapper wrapper = (Wrapper)bt.Tag;
            using (RemotingForm form = new RemotingForm(wrapper))
            {
                form.ShowDialog();
            }
        }

    }

}
