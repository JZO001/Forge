namespace Testing.TerraGraf
{
    partial class RemotingForm
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
            this.btStartInterNetwork = new System.Windows.Forms.Button();
            this.btStartTerragraf = new System.Windows.Forms.Button();
            this.btStopInternetwork = new System.Windows.Forms.Button();
            this.btStopTerraGraf = new System.Windows.Forms.Button();
            this.lvChannels = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvNetworkPeers = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btRefreshChannels = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbAddress = new System.Windows.Forms.TextBox();
            this.nudPort = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.gbTestWith = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbChannelId = new System.Windows.Forms.TextBox();
            this.lChannelId = new System.Windows.Forms.Label();
            this.btStartServer = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.nudServerPort = new System.Windows.Forms.NumericUpDown();
            this.tbServerAddress = new System.Windows.Forms.TextBox();
            this.rbInterNetwork = new System.Windows.Forms.RadioButton();
            this.rbTerraGraf = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.gbTestWith.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudServerPort)).BeginInit();
            this.SuspendLayout();
            // 
            // btStartInterNetwork
            // 
            this.btStartInterNetwork.Location = new System.Drawing.Point(137, 232);
            this.btStartInterNetwork.Name = "btStartInterNetwork";
            this.btStartInterNetwork.Size = new System.Drawing.Size(147, 23);
            this.btStartInterNetwork.TabIndex = 0;
            this.btStartInterNetwork.Text = "Start InterNetwork";
            this.btStartInterNetwork.UseVisualStyleBackColor = true;
            this.btStartInterNetwork.Click += new System.EventHandler(this.btStartInterNetwork_Click);
            // 
            // btStartTerragraf
            // 
            this.btStartTerragraf.Location = new System.Drawing.Point(137, 261);
            this.btStartTerragraf.Name = "btStartTerragraf";
            this.btStartTerragraf.Size = new System.Drawing.Size(147, 23);
            this.btStartTerragraf.TabIndex = 1;
            this.btStartTerragraf.Text = "Start TerraGraf";
            this.btStartTerragraf.UseVisualStyleBackColor = true;
            this.btStartTerragraf.Click += new System.EventHandler(this.btStartTerragraf_Click);
            // 
            // btStopInternetwork
            // 
            this.btStopInternetwork.Enabled = false;
            this.btStopInternetwork.Location = new System.Drawing.Point(290, 232);
            this.btStopInternetwork.Name = "btStopInternetwork";
            this.btStopInternetwork.Size = new System.Drawing.Size(147, 23);
            this.btStopInternetwork.TabIndex = 2;
            this.btStopInternetwork.Text = "Stop";
            this.btStopInternetwork.UseVisualStyleBackColor = true;
            this.btStopInternetwork.Click += new System.EventHandler(this.btStopInternetwork_Click);
            // 
            // btStopTerraGraf
            // 
            this.btStopTerraGraf.Enabled = false;
            this.btStopTerraGraf.Location = new System.Drawing.Point(290, 261);
            this.btStopTerraGraf.Name = "btStopTerraGraf";
            this.btStopTerraGraf.Size = new System.Drawing.Size(147, 23);
            this.btStopTerraGraf.TabIndex = 3;
            this.btStopTerraGraf.Text = "Stop";
            this.btStopTerraGraf.UseVisualStyleBackColor = true;
            this.btStopTerraGraf.Click += new System.EventHandler(this.btStopTerraGraf_Click);
            // 
            // lvChannels
            // 
            this.lvChannels.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvChannels.FullRowSelect = true;
            this.lvChannels.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvChannels.HideSelection = false;
            this.lvChannels.Location = new System.Drawing.Point(11, 12);
            this.lvChannels.MultiSelect = false;
            this.lvChannels.Name = "lvChannels";
            this.lvChannels.Size = new System.Drawing.Size(147, 123);
            this.lvChannels.TabIndex = 4;
            this.lvChannels.UseCompatibleStateImageBehavior = false;
            this.lvChannels.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Channel Id";
            this.columnHeader1.Width = 114;
            // 
            // lvNetworkPeers
            // 
            this.lvNetworkPeers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lvNetworkPeers.FullRowSelect = true;
            this.lvNetworkPeers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvNetworkPeers.HideSelection = false;
            this.lvNetworkPeers.Location = new System.Drawing.Point(6, 19);
            this.lvNetworkPeers.MultiSelect = false;
            this.lvNetworkPeers.Name = "lvNetworkPeers";
            this.lvNetworkPeers.Size = new System.Drawing.Size(147, 123);
            this.lvNetworkPeers.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvNetworkPeers.TabIndex = 5;
            this.lvNetworkPeers.UseCompatibleStateImageBehavior = false;
            this.lvNetworkPeers.View = System.Windows.Forms.View.Details;
            this.lvNetworkPeers.DoubleClick += new System.EventHandler(this.lvNetworkPeers_DoubleClick);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Peer Name";
            this.columnHeader2.Width = 116;
            // 
            // btRefreshChannels
            // 
            this.btRefreshChannels.Location = new System.Drawing.Point(12, 141);
            this.btRefreshChannels.Name = "btRefreshChannels";
            this.btRefreshChannels.Size = new System.Drawing.Size(146, 23);
            this.btRefreshChannels.TabIndex = 6;
            this.btRefreshChannels.Text = "Refresh";
            this.btRefreshChannels.UseVisualStyleBackColor = true;
            this.btRefreshChannels.Click += new System.EventHandler(this.btRefreshChannels_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 153);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Address:";
            // 
            // tbAddress
            // 
            this.tbAddress.Location = new System.Drawing.Point(61, 149);
            this.tbAddress.Name = "tbAddress";
            this.tbAddress.Size = new System.Drawing.Size(92, 20);
            this.tbAddress.TabIndex = 8;
            this.tbAddress.Text = "127.0.0.1";
            // 
            // nudPort
            // 
            this.nudPort.Location = new System.Drawing.Point(61, 176);
            this.nudPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPort.Name = "nudPort";
            this.nudPort.Size = new System.Drawing.Size(92, 20);
            this.nudPort.TabIndex = 9;
            this.nudPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudPort.ThousandsSeparator = true;
            this.nudPort.Value = new decimal(new int[] {
            57000,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 178);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Port:";
            // 
            // gbTestWith
            // 
            this.gbTestWith.Controls.Add(this.lvNetworkPeers);
            this.gbTestWith.Controls.Add(this.label2);
            this.gbTestWith.Controls.Add(this.label1);
            this.gbTestWith.Controls.Add(this.nudPort);
            this.gbTestWith.Controls.Add(this.tbAddress);
            this.gbTestWith.Location = new System.Drawing.Point(164, 12);
            this.gbTestWith.Name = "gbTestWith";
            this.gbTestWith.Size = new System.Drawing.Size(162, 205);
            this.gbTestWith.TabIndex = 11;
            this.gbTestWith.TabStop = false;
            this.gbTestWith.Text = "Test with ...";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbTerraGraf);
            this.groupBox1.Controls.Add(this.rbInterNetwork);
            this.groupBox1.Controls.Add(this.tbChannelId);
            this.groupBox1.Controls.Add(this.lChannelId);
            this.groupBox1.Controls.Add(this.btStartServer);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.nudServerPort);
            this.groupBox1.Controls.Add(this.tbServerAddress);
            this.groupBox1.Location = new System.Drawing.Point(341, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(194, 176);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Start Channel";
            // 
            // tbChannelId
            // 
            this.tbChannelId.Location = new System.Drawing.Point(82, 19);
            this.tbChannelId.Name = "tbChannelId";
            this.tbChannelId.Size = new System.Drawing.Size(100, 20);
            this.tbChannelId.TabIndex = 17;
            this.tbChannelId.Text = "INTNET_SERVER";
            // 
            // lChannelId
            // 
            this.lChannelId.AutoSize = true;
            this.lChannelId.Location = new System.Drawing.Point(14, 20);
            this.lChannelId.Name = "lChannelId";
            this.lChannelId.Size = new System.Drawing.Size(61, 13);
            this.lChannelId.TabIndex = 16;
            this.lChannelId.Text = "Channel Id:";
            // 
            // btStartServer
            // 
            this.btStartServer.Location = new System.Drawing.Point(82, 143);
            this.btStartServer.Name = "btStartServer";
            this.btStartServer.Size = new System.Drawing.Size(100, 23);
            this.btStartServer.TabIndex = 15;
            this.btStartServer.Text = "Start";
            this.btStartServer.UseVisualStyleBackColor = true;
            this.btStartServer.Click += new System.EventHandler(this.btStartServer_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(46, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Port:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Address:";
            // 
            // nudServerPort
            // 
            this.nudServerPort.Location = new System.Drawing.Point(82, 71);
            this.nudServerPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudServerPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudServerPort.Name = "nudServerPort";
            this.nudServerPort.Size = new System.Drawing.Size(100, 20);
            this.nudServerPort.TabIndex = 13;
            this.nudServerPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudServerPort.ThousandsSeparator = true;
            this.nudServerPort.Value = new decimal(new int[] {
            57000,
            0,
            0,
            0});
            // 
            // tbServerAddress
            // 
            this.tbServerAddress.Location = new System.Drawing.Point(82, 45);
            this.tbServerAddress.Name = "tbServerAddress";
            this.tbServerAddress.Size = new System.Drawing.Size(100, 20);
            this.tbServerAddress.TabIndex = 12;
            this.tbServerAddress.Text = "127.0.0.1";
            // 
            // rbInterNetwork
            // 
            this.rbInterNetwork.AutoSize = true;
            this.rbInterNetwork.Checked = true;
            this.rbInterNetwork.Location = new System.Drawing.Point(82, 97);
            this.rbInterNetwork.Name = "rbInterNetwork";
            this.rbInterNetwork.Size = new System.Drawing.Size(86, 17);
            this.rbInterNetwork.TabIndex = 18;
            this.rbInterNetwork.TabStop = true;
            this.rbInterNetwork.Text = "InterNetwork";
            this.rbInterNetwork.UseVisualStyleBackColor = true;
            // 
            // rbTerraGraf
            // 
            this.rbTerraGraf.AutoSize = true;
            this.rbTerraGraf.Location = new System.Drawing.Point(82, 120);
            this.rbTerraGraf.Name = "rbTerraGraf";
            this.rbTerraGraf.Size = new System.Drawing.Size(70, 17);
            this.rbTerraGraf.TabIndex = 19;
            this.rbTerraGraf.Text = "TerraGraf";
            this.rbTerraGraf.UseVisualStyleBackColor = true;
            // 
            // RemotingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 303);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbTestWith);
            this.Controls.Add(this.btRefreshChannels);
            this.Controls.Add(this.lvChannels);
            this.Controls.Add(this.btStopTerraGraf);
            this.Controls.Add(this.btStopInternetwork);
            this.Controls.Add(this.btStartTerragraf);
            this.Controls.Add(this.btStartInterNetwork);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RemotingForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Remoting Test";
            this.Shown += new System.EventHandler(this.RemotingForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            this.gbTestWith.ResumeLayout(false);
            this.gbTestWith.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudServerPort)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btStartInterNetwork;
        private System.Windows.Forms.Button btStartTerragraf;
        private System.Windows.Forms.Button btStopInternetwork;
        private System.Windows.Forms.Button btStopTerraGraf;
        private System.Windows.Forms.ListView lvChannels;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ListView lvNetworkPeers;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button btRefreshChannels;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbAddress;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox gbTestWith;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbChannelId;
        private System.Windows.Forms.Label lChannelId;
        private System.Windows.Forms.Button btStartServer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudServerPort;
        private System.Windows.Forms.TextBox tbServerAddress;
        private System.Windows.Forms.RadioButton rbTerraGraf;
        private System.Windows.Forms.RadioButton rbInterNetwork;
    }
}