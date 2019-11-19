namespace Testing.TerraGraf
{
    partial class TCPForm
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
            this.cbHugeData = new System.Windows.Forms.CheckBox();
            this.lvHosts = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvSockets = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvServers = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btCloseSocket = new System.Windows.Forms.Button();
            this.btStopServer = new System.Windows.Forms.Button();
            this.btStartClient = new System.Windows.Forms.Button();
            this.btStartServer = new System.Windows.Forms.Button();
            this.nudLocalPortServer = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nudRemotePort = new System.Windows.Forms.NumericUpDown();
            this.btConnect = new System.Windows.Forms.Button();
            this.cbRepeatSend = new System.Windows.Forms.CheckBox();
            this.lvSenders = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btCloseSender = new System.Windows.Forms.Button();
            this.btRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudLocalPortServer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRemotePort)).BeginInit();
            this.SuspendLayout();
            // 
            // cbHugeData
            // 
            this.cbHugeData.AutoSize = true;
            this.cbHugeData.Location = new System.Drawing.Point(202, 307);
            this.cbHugeData.Name = "cbHugeData";
            this.cbHugeData.Size = new System.Drawing.Size(102, 17);
            this.cbHugeData.TabIndex = 35;
            this.cbHugeData.Text = "Send huge data";
            this.cbHugeData.UseVisualStyleBackColor = true;
            // 
            // lvHosts
            // 
            this.lvHosts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.lvHosts.FullRowSelect = true;
            this.lvHosts.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvHosts.HideSelection = false;
            this.lvHosts.Location = new System.Drawing.Point(406, 67);
            this.lvHosts.MultiSelect = false;
            this.lvHosts.Name = "lvHosts";
            this.lvHosts.Size = new System.Drawing.Size(128, 176);
            this.lvHosts.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvHosts.TabIndex = 34;
            this.lvHosts.UseCompatibleStateImageBehavior = false;
            this.lvHosts.View = System.Windows.Forms.View.Details;
            this.lvHosts.SelectedIndexChanged += new System.EventHandler(this.lvHosts_SelectedIndexChanged);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Host ID";
            this.columnHeader3.Width = 101;
            // 
            // lvSockets
            // 
            this.lvSockets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lvSockets.FullRowSelect = true;
            this.lvSockets.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSockets.HideSelection = false;
            this.lvSockets.Location = new System.Drawing.Point(202, 67);
            this.lvSockets.MultiSelect = false;
            this.lvSockets.Name = "lvSockets";
            this.lvSockets.Size = new System.Drawing.Size(96, 176);
            this.lvSockets.TabIndex = 32;
            this.lvSockets.UseCompatibleStateImageBehavior = false;
            this.lvSockets.View = System.Windows.Forms.View.Details;
            this.lvSockets.SelectedIndexChanged += new System.EventHandler(this.lvSockets_SelectedIndexChanged);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "ID";
            this.columnHeader2.Width = 62;
            // 
            // lvServers
            // 
            this.lvServers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvServers.FullRowSelect = true;
            this.lvServers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvServers.HideSelection = false;
            this.lvServers.Location = new System.Drawing.Point(12, 67);
            this.lvServers.MultiSelect = false;
            this.lvServers.Name = "lvServers";
            this.lvServers.Size = new System.Drawing.Size(128, 107);
            this.lvServers.TabIndex = 30;
            this.lvServers.UseCompatibleStateImageBehavior = false;
            this.lvServers.View = System.Windows.Forms.View.Details;
            this.lvServers.SelectedIndexChanged += new System.EventHandler(this.lvServers_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "ID";
            this.columnHeader1.Width = 62;
            // 
            // btCloseSocket
            // 
            this.btCloseSocket.Enabled = false;
            this.btCloseSocket.Location = new System.Drawing.Point(202, 249);
            this.btCloseSocket.Name = "btCloseSocket";
            this.btCloseSocket.Size = new System.Drawing.Size(96, 23);
            this.btCloseSocket.TabIndex = 33;
            this.btCloseSocket.Text = "Close";
            this.btCloseSocket.UseVisualStyleBackColor = true;
            this.btCloseSocket.Click += new System.EventHandler(this.btCloseSocket_Click);
            // 
            // btStopServer
            // 
            this.btStopServer.Enabled = false;
            this.btStopServer.Location = new System.Drawing.Point(12, 180);
            this.btStopServer.Name = "btStopServer";
            this.btStopServer.Size = new System.Drawing.Size(128, 23);
            this.btStopServer.TabIndex = 31;
            this.btStopServer.Text = "Stop";
            this.btStopServer.UseVisualStyleBackColor = true;
            this.btStopServer.Click += new System.EventHandler(this.btStopServer_Click);
            // 
            // btStartClient
            // 
            this.btStartClient.Enabled = false;
            this.btStartClient.Location = new System.Drawing.Point(202, 278);
            this.btStartClient.Name = "btStartClient";
            this.btStartClient.Size = new System.Drawing.Size(96, 23);
            this.btStartClient.TabIndex = 29;
            this.btStartClient.Text = "Start Client";
            this.btStartClient.UseVisualStyleBackColor = true;
            this.btStartClient.Click += new System.EventHandler(this.btStartClient_Click);
            // 
            // btStartServer
            // 
            this.btStartServer.Location = new System.Drawing.Point(12, 38);
            this.btStartServer.Name = "btStartServer";
            this.btStartServer.Size = new System.Drawing.Size(128, 23);
            this.btStartServer.TabIndex = 28;
            this.btStartServer.Text = "Start Server";
            this.btStartServer.UseVisualStyleBackColor = true;
            this.btStartServer.Click += new System.EventHandler(this.btStartServer_Click);
            // 
            // nudLocalPortServer
            // 
            this.nudLocalPortServer.Location = new System.Drawing.Point(72, 12);
            this.nudLocalPortServer.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudLocalPortServer.Name = "nudLocalPortServer";
            this.nudLocalPortServer.Size = new System.Drawing.Size(68, 20);
            this.nudLocalPortServer.TabIndex = 27;
            this.nudLocalPortServer.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudLocalPortServer.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Local port:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(403, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 36;
            this.label2.Text = "Connect on port:";
            // 
            // nudRemotePort
            // 
            this.nudRemotePort.Location = new System.Drawing.Point(406, 38);
            this.nudRemotePort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudRemotePort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRemotePort.Name = "nudRemotePort";
            this.nudRemotePort.Size = new System.Drawing.Size(120, 20);
            this.nudRemotePort.TabIndex = 37;
            this.nudRemotePort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudRemotePort.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btConnect
            // 
            this.btConnect.Enabled = false;
            this.btConnect.Location = new System.Drawing.Point(406, 249);
            this.btConnect.Name = "btConnect";
            this.btConnect.Size = new System.Drawing.Size(128, 23);
            this.btConnect.TabIndex = 38;
            this.btConnect.Text = "Connect";
            this.btConnect.UseVisualStyleBackColor = true;
            this.btConnect.Click += new System.EventHandler(this.btConnect_Click);
            // 
            // cbRepeatSend
            // 
            this.cbRepeatSend.AutoSize = true;
            this.cbRepeatSend.Location = new System.Drawing.Point(202, 330);
            this.cbRepeatSend.Name = "cbRepeatSend";
            this.cbRepeatSend.Size = new System.Drawing.Size(87, 17);
            this.cbRepeatSend.TabIndex = 39;
            this.cbRepeatSend.Text = "Repeat send";
            this.cbRepeatSend.UseVisualStyleBackColor = true;
            // 
            // lvSenders
            // 
            this.lvSenders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4});
            this.lvSenders.FullRowSelect = true;
            this.lvSenders.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSenders.HideSelection = false;
            this.lvSenders.Location = new System.Drawing.Point(304, 67);
            this.lvSenders.MultiSelect = false;
            this.lvSenders.Name = "lvSenders";
            this.lvSenders.Size = new System.Drawing.Size(96, 176);
            this.lvSenders.TabIndex = 40;
            this.lvSenders.UseCompatibleStateImageBehavior = false;
            this.lvSenders.View = System.Windows.Forms.View.Details;
            this.lvSenders.SelectedIndexChanged += new System.EventHandler(this.lvSenders_SelectedIndexChanged);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "ID";
            this.columnHeader4.Width = 62;
            // 
            // btCloseSender
            // 
            this.btCloseSender.Enabled = false;
            this.btCloseSender.Location = new System.Drawing.Point(304, 249);
            this.btCloseSender.Name = "btCloseSender";
            this.btCloseSender.Size = new System.Drawing.Size(96, 23);
            this.btCloseSender.TabIndex = 41;
            this.btCloseSender.Text = "Close";
            this.btCloseSender.UseVisualStyleBackColor = true;
            this.btCloseSender.Click += new System.EventHandler(this.btCloseSender_Click);
            // 
            // btRefresh
            // 
            this.btRefresh.Location = new System.Drawing.Point(202, 38);
            this.btRefresh.Name = "btRefresh";
            this.btRefresh.Size = new System.Drawing.Size(75, 23);
            this.btRefresh.TabIndex = 42;
            this.btRefresh.Text = "Refresh";
            this.btRefresh.UseVisualStyleBackColor = true;
            this.btRefresh.Click += new System.EventHandler(this.btRefresh_Click);
            // 
            // TCPForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 354);
            this.Controls.Add(this.btRefresh);
            this.Controls.Add(this.lvSenders);
            this.Controls.Add(this.btCloseSender);
            this.Controls.Add(this.cbRepeatSend);
            this.Controls.Add(this.btConnect);
            this.Controls.Add(this.nudRemotePort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbHugeData);
            this.Controls.Add(this.lvHosts);
            this.Controls.Add(this.lvSockets);
            this.Controls.Add(this.lvServers);
            this.Controls.Add(this.btCloseSocket);
            this.Controls.Add(this.btStopServer);
            this.Controls.Add(this.btStartClient);
            this.Controls.Add(this.btStartServer);
            this.Controls.Add(this.nudLocalPortServer);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TCPForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TCP Control";
            this.Shown += new System.EventHandler(this.TCPForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.nudLocalPortServer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRemotePort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbHugeData;
        private System.Windows.Forms.ListView lvHosts;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ListView lvSockets;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ListView lvServers;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button btCloseSocket;
        private System.Windows.Forms.Button btStopServer;
        private System.Windows.Forms.Button btStartClient;
        private System.Windows.Forms.Button btStartServer;
        private System.Windows.Forms.NumericUpDown nudLocalPortServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudRemotePort;
        private System.Windows.Forms.Button btConnect;
        private System.Windows.Forms.CheckBox cbRepeatSend;
        private System.Windows.Forms.ListView lvSenders;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button btCloseSender;
        private System.Windows.Forms.Button btRefresh;
    }
}