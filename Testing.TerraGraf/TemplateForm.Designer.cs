namespace Testing.TerraGraf
{
    partial class TemplateForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Node1");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Node2");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Node0", new System.Windows.Forms.TreeNode[] {
            treeNode7,
            treeNode8});
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Node4");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Node5");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Node3", new System.Windows.Forms.TreeNode[] {
            treeNode10,
            treeNode11});
            this.tcDomains = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btSaveRules = new System.Windows.Forms.Button();
            this.btEverlight = new System.Windows.Forms.Button();
            this.btRemoting = new System.Windows.Forms.Button();
            this.btSockets = new System.Windows.Forms.Button();
            this.btTcp = new System.Windows.Forms.Button();
            this.btUDP = new System.Windows.Forms.Button();
            this.btBroadcast = new System.Windows.Forms.Button();
            this.btConnect = new System.Windows.Forms.Button();
            this.tbConnect = new System.Windows.Forms.TextBox();
            this.lConnect = new System.Windows.Forms.Label();
            this.btSendContext = new System.Windows.Forms.Button();
            this.tbContext = new System.Windows.Forms.TextBox();
            this.lContext = new System.Windows.Forms.Label();
            this.pServers = new System.Windows.Forms.Panel();
            this.btStopServer = new System.Windows.Forms.Button();
            this.btStartServer = new System.Windows.Forms.Button();
            this.lvServers = new System.Windows.Forms.ListView();
            this.chServers = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btDisconnectActive = new System.Windows.Forms.Button();
            this.btDisconnect = new System.Windows.Forms.Button();
            this.cbBlackHole = new System.Windows.Forms.CheckBox();
            this.lvNetworkConnections = new System.Windows.Forms.ListView();
            this.chNetworkConnections = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button1 = new System.Windows.Forms.Button();
            this.btInitialize = new System.Windows.Forms.Button();
            this.tvDemo = new System.Windows.Forms.TreeView();
            this.btUpdateWF = new System.Windows.Forms.Button();
            this.tcDomains.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.pServers.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcDomains
            // 
            this.tcDomains.Controls.Add(this.tabPage1);
            this.tcDomains.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcDomains.Location = new System.Drawing.Point(0, 0);
            this.tcDomains.Name = "tcDomains";
            this.tcDomains.SelectedIndex = 0;
            this.tcDomains.Size = new System.Drawing.Size(579, 369);
            this.tcDomains.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btUpdateWF);
            this.tabPage1.Controls.Add(this.btSaveRules);
            this.tabPage1.Controls.Add(this.btEverlight);
            this.tabPage1.Controls.Add(this.btRemoting);
            this.tabPage1.Controls.Add(this.btSockets);
            this.tabPage1.Controls.Add(this.btTcp);
            this.tabPage1.Controls.Add(this.btUDP);
            this.tabPage1.Controls.Add(this.btBroadcast);
            this.tabPage1.Controls.Add(this.btConnect);
            this.tabPage1.Controls.Add(this.tbConnect);
            this.tabPage1.Controls.Add(this.lConnect);
            this.tabPage1.Controls.Add(this.btSendContext);
            this.tabPage1.Controls.Add(this.tbContext);
            this.tabPage1.Controls.Add(this.lContext);
            this.tabPage1.Controls.Add(this.pServers);
            this.tabPage1.Controls.Add(this.btDisconnectActive);
            this.tabPage1.Controls.Add(this.btDisconnect);
            this.tabPage1.Controls.Add(this.cbBlackHole);
            this.tabPage1.Controls.Add(this.lvNetworkConnections);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.btInitialize);
            this.tabPage1.Controls.Add(this.tvDemo);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(571, 343);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "DEMO";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btSaveRules
            // 
            this.btSaveRules.Location = new System.Drawing.Point(311, 290);
            this.btSaveRules.Name = "btSaveRules";
            this.btSaveRules.Size = new System.Drawing.Size(75, 23);
            this.btSaveRules.TabIndex = 17;
            this.btSaveRules.Text = "Save Rules";
            this.btSaveRules.UseVisualStyleBackColor = true;
            this.btSaveRules.Click += new System.EventHandler(this.btSaveRules_Click);
            // 
            // btEverlight
            // 
            this.btEverlight.Location = new System.Drawing.Point(475, 261);
            this.btEverlight.Name = "btEverlight";
            this.btEverlight.Size = new System.Drawing.Size(75, 23);
            this.btEverlight.TabIndex = 16;
            this.btEverlight.Text = "Everlight";
            this.btEverlight.UseVisualStyleBackColor = true;
            this.btEverlight.Click += new System.EventHandler(this.btEverlight_Click);
            // 
            // btRemoting
            // 
            this.btRemoting.Location = new System.Drawing.Point(393, 261);
            this.btRemoting.Name = "btRemoting";
            this.btRemoting.Size = new System.Drawing.Size(75, 23);
            this.btRemoting.TabIndex = 15;
            this.btRemoting.Text = "Remoting";
            this.btRemoting.UseVisualStyleBackColor = true;
            this.btRemoting.Click += new System.EventHandler(this.btRemoting_Click);
            // 
            // btSockets
            // 
            this.btSockets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSockets.Location = new System.Drawing.Point(311, 261);
            this.btSockets.Name = "btSockets";
            this.btSockets.Size = new System.Drawing.Size(75, 23);
            this.btSockets.TabIndex = 14;
            this.btSockets.Text = "Sockets";
            this.btSockets.UseVisualStyleBackColor = true;
            this.btSockets.Click += new System.EventHandler(this.btSockets_Click);
            // 
            // btTcp
            // 
            this.btTcp.Location = new System.Drawing.Point(475, 232);
            this.btTcp.Name = "btTcp";
            this.btTcp.Size = new System.Drawing.Size(75, 23);
            this.btTcp.TabIndex = 13;
            this.btTcp.Text = "TCP";
            this.btTcp.UseVisualStyleBackColor = true;
            this.btTcp.Click += new System.EventHandler(this.btTcp_Click);
            // 
            // btUDP
            // 
            this.btUDP.Location = new System.Drawing.Point(393, 232);
            this.btUDP.Name = "btUDP";
            this.btUDP.Size = new System.Drawing.Size(75, 23);
            this.btUDP.TabIndex = 12;
            this.btUDP.Text = "UDP";
            this.btUDP.UseVisualStyleBackColor = true;
            // 
            // btBroadcast
            // 
            this.btBroadcast.Location = new System.Drawing.Point(311, 232);
            this.btBroadcast.Name = "btBroadcast";
            this.btBroadcast.Size = new System.Drawing.Size(75, 23);
            this.btBroadcast.TabIndex = 11;
            this.btBroadcast.Text = "Broadcast";
            this.btBroadcast.UseVisualStyleBackColor = true;
            this.btBroadcast.Click += new System.EventHandler(this.btBroadcast_Click);
            // 
            // btConnect
            // 
            this.btConnect.Location = new System.Drawing.Point(482, 206);
            this.btConnect.Name = "btConnect";
            this.btConnect.Size = new System.Drawing.Size(75, 23);
            this.btConnect.TabIndex = 10;
            this.btConnect.Text = "Connect";
            this.btConnect.UseVisualStyleBackColor = true;
            this.btConnect.Click += new System.EventHandler(this.btConnect_Click);
            // 
            // tbConnect
            // 
            this.tbConnect.Location = new System.Drawing.Point(366, 206);
            this.tbConnect.Name = "tbConnect";
            this.tbConnect.Size = new System.Drawing.Size(100, 20);
            this.tbConnect.TabIndex = 9;
            // 
            // lConnect
            // 
            this.lConnect.AutoSize = true;
            this.lConnect.Location = new System.Drawing.Point(311, 209);
            this.lConnect.Name = "lConnect";
            this.lConnect.Size = new System.Drawing.Size(47, 13);
            this.lConnect.TabIndex = 8;
            this.lConnect.Text = "Connect";
            // 
            // btSendContext
            // 
            this.btSendContext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSendContext.Location = new System.Drawing.Point(482, 178);
            this.btSendContext.Name = "btSendContext";
            this.btSendContext.Size = new System.Drawing.Size(75, 23);
            this.btSendContext.TabIndex = 7;
            this.btSendContext.Text = "Send";
            this.btSendContext.UseVisualStyleBackColor = true;
            this.btSendContext.Click += new System.EventHandler(this.btSendContext_Click);
            // 
            // tbContext
            // 
            this.tbContext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbContext.Location = new System.Drawing.Point(366, 179);
            this.tbContext.Name = "tbContext";
            this.tbContext.Size = new System.Drawing.Size(100, 20);
            this.tbContext.TabIndex = 6;
            // 
            // lContext
            // 
            this.lContext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lContext.AutoSize = true;
            this.lContext.Location = new System.Drawing.Point(314, 183);
            this.lContext.Name = "lContext";
            this.lContext.Size = new System.Drawing.Size(46, 13);
            this.lContext.TabIndex = 5;
            this.lContext.Text = "Context:";
            // 
            // pServers
            // 
            this.pServers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pServers.Controls.Add(this.btStopServer);
            this.pServers.Controls.Add(this.btStartServer);
            this.pServers.Controls.Add(this.lvServers);
            this.pServers.Location = new System.Drawing.Point(311, 37);
            this.pServers.Name = "pServers";
            this.pServers.Size = new System.Drawing.Size(252, 139);
            this.pServers.TabIndex = 4;
            // 
            // btStopServer
            // 
            this.btStopServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btStopServer.Enabled = false;
            this.btStopServer.Location = new System.Drawing.Point(171, 32);
            this.btStopServer.Name = "btStopServer";
            this.btStopServer.Size = new System.Drawing.Size(75, 23);
            this.btStopServer.TabIndex = 2;
            this.btStopServer.Text = "Stop";
            this.btStopServer.UseVisualStyleBackColor = true;
            this.btStopServer.Click += new System.EventHandler(this.btStopServer_Click);
            // 
            // btStartServer
            // 
            this.btStartServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btStartServer.Location = new System.Drawing.Point(171, 3);
            this.btStartServer.Name = "btStartServer";
            this.btStartServer.Size = new System.Drawing.Size(75, 23);
            this.btStartServer.TabIndex = 1;
            this.btStartServer.Text = "Start";
            this.btStartServer.UseVisualStyleBackColor = true;
            this.btStartServer.Click += new System.EventHandler(this.btStartServer_Click);
            // 
            // lvServers
            // 
            this.lvServers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvServers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chServers});
            this.lvServers.FullRowSelect = true;
            this.lvServers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvServers.HideSelection = false;
            this.lvServers.Location = new System.Drawing.Point(3, 3);
            this.lvServers.MultiSelect = false;
            this.lvServers.Name = "lvServers";
            this.lvServers.Size = new System.Drawing.Size(162, 133);
            this.lvServers.TabIndex = 0;
            this.lvServers.UseCompatibleStateImageBehavior = false;
            this.lvServers.View = System.Windows.Forms.View.Details;
            this.lvServers.SelectedIndexChanged += new System.EventHandler(this.lvServers_SelectedIndexChanged);
            // 
            // chServers
            // 
            this.chServers.Text = "Endpoints";
            this.chServers.Width = 128;
            // 
            // btDisconnectActive
            // 
            this.btDisconnectActive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btDisconnectActive.Location = new System.Drawing.Point(230, 312);
            this.btDisconnectActive.Name = "btDisconnectActive";
            this.btDisconnectActive.Size = new System.Drawing.Size(75, 23);
            this.btDisconnectActive.TabIndex = 3;
            this.btDisconnectActive.Text = "Dis. Active";
            this.btDisconnectActive.UseVisualStyleBackColor = true;
            this.btDisconnectActive.Click += new System.EventHandler(this.btDisconnectActive_Click);
            // 
            // btDisconnect
            // 
            this.btDisconnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btDisconnect.Location = new System.Drawing.Point(230, 283);
            this.btDisconnect.Name = "btDisconnect";
            this.btDisconnect.Size = new System.Drawing.Size(75, 23);
            this.btDisconnect.TabIndex = 1;
            this.btDisconnect.Text = "Disconnect";
            this.btDisconnect.UseVisualStyleBackColor = true;
            this.btDisconnect.Click += new System.EventHandler(this.btDisconnect_Click);
            // 
            // cbBlackHole
            // 
            this.cbBlackHole.AutoSize = true;
            this.cbBlackHole.Enabled = false;
            this.cbBlackHole.Location = new System.Drawing.Point(170, 7);
            this.cbBlackHole.Name = "cbBlackHole";
            this.cbBlackHole.Size = new System.Drawing.Size(75, 17);
            this.cbBlackHole.TabIndex = 2;
            this.cbBlackHole.Text = "BlackHole";
            this.cbBlackHole.UseVisualStyleBackColor = true;
            this.cbBlackHole.CheckedChanged += new System.EventHandler(this.cbBlackHole_CheckedChanged);
            // 
            // lvNetworkConnections
            // 
            this.lvNetworkConnections.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvNetworkConnections.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chNetworkConnections});
            this.lvNetworkConnections.FullRowSelect = true;
            this.lvNetworkConnections.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvNetworkConnections.HideSelection = false;
            this.lvNetworkConnections.Location = new System.Drawing.Point(230, 37);
            this.lvNetworkConnections.MultiSelect = false;
            this.lvNetworkConnections.Name = "lvNetworkConnections";
            this.lvNetworkConnections.Size = new System.Drawing.Size(75, 240);
            this.lvNetworkConnections.TabIndex = 0;
            this.lvNetworkConnections.UseCompatibleStateImageBehavior = false;
            this.lvNetworkConnections.View = System.Windows.Forms.View.Details;
            // 
            // chNetworkConnections
            // 
            this.chNetworkConnections.Text = "Id";
            this.chNetworkConnections.Width = 50;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(89, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Debug";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btInitialize
            // 
            this.btInitialize.Location = new System.Drawing.Point(8, 3);
            this.btInitialize.Name = "btInitialize";
            this.btInitialize.Size = new System.Drawing.Size(75, 23);
            this.btInitialize.TabIndex = 1;
            this.btInitialize.Text = "Initialize";
            this.btInitialize.UseVisualStyleBackColor = true;
            // 
            // tvDemo
            // 
            this.tvDemo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvDemo.Location = new System.Drawing.Point(8, 37);
            this.tvDemo.Name = "tvDemo";
            treeNode7.Name = "Node1";
            treeNode7.Text = "Node1";
            treeNode8.Name = "Node2";
            treeNode8.Text = "Node2";
            treeNode9.Name = "Node0";
            treeNode9.Text = "Node0";
            treeNode10.Name = "Node4";
            treeNode10.Text = "Node4";
            treeNode11.Name = "Node5";
            treeNode11.Text = "Node5";
            treeNode12.Name = "Node3";
            treeNode12.Text = "Node3";
            this.tvDemo.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode9,
            treeNode12});
            this.tvDemo.Size = new System.Drawing.Size(216, 298);
            this.tvDemo.TabIndex = 0;
            this.tvDemo.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvDemo_AfterSelect);
            // 
            // btUpdateWF
            // 
            this.btUpdateWF.Location = new System.Drawing.Point(393, 291);
            this.btUpdateWF.Name = "btUpdateWF";
            this.btUpdateWF.Size = new System.Drawing.Size(75, 23);
            this.btUpdateWF.TabIndex = 18;
            this.btUpdateWF.Text = "UpdateFW";
            this.btUpdateWF.UseVisualStyleBackColor = true;
            this.btUpdateWF.Click += new System.EventHandler(this.btUpdateWF_Click);
            // 
            // TemplateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 369);
            this.Controls.Add(this.tcDomains);
            this.Name = "TemplateForm";
            this.Text = "TemplateForm";
            this.tcDomains.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.pServers.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tcDomains;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btInitialize;
        private System.Windows.Forms.TreeView tvDemo;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox cbBlackHole;
        private System.Windows.Forms.Button btDisconnect;
        private System.Windows.Forms.ListView lvNetworkConnections;
        private System.Windows.Forms.ColumnHeader chNetworkConnections;
        private System.Windows.Forms.Button btDisconnectActive;
        private System.Windows.Forms.Panel pServers;
        private System.Windows.Forms.Button btStopServer;
        private System.Windows.Forms.Button btStartServer;
        private System.Windows.Forms.ListView lvServers;
        private System.Windows.Forms.ColumnHeader chServers;
        private System.Windows.Forms.Button btSendContext;
        private System.Windows.Forms.TextBox tbContext;
        private System.Windows.Forms.Label lContext;
        private System.Windows.Forms.Button btConnect;
        private System.Windows.Forms.TextBox tbConnect;
        private System.Windows.Forms.Label lConnect;
        private System.Windows.Forms.Button btBroadcast;
        private System.Windows.Forms.Button btUDP;
        private System.Windows.Forms.Button btTcp;
        private System.Windows.Forms.Button btSockets;
        private System.Windows.Forms.Button btRemoting;
        private System.Windows.Forms.Button btEverlight;
        private System.Windows.Forms.Button btSaveRules;
        private System.Windows.Forms.Button btUpdateWF;
    }
}