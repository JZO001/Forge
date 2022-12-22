namespace Testing.TerraGraf
{
    partial class UDPForm
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
            this.lvClients = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvServers = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.nudTargetPortClient = new System.Windows.Forms.NumericUpDown();
            this.btStopClient = new System.Windows.Forms.Button();
            this.btStopServer = new System.Windows.Forms.Button();
            this.btStartClient = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.nudLocalPortClient = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.btStartServer = new System.Windows.Forms.Button();
            this.nudLocalPortServer = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.lvHosts = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cbHugeData = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudTargetPortClient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLocalPortClient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLocalPortServer)).BeginInit();
            this.SuspendLayout();
            // 
            // lvClients
            // 
            this.lvClients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lvClients.FullRowSelect = true;
            this.lvClients.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvClients.HideSelection = false;
            this.lvClients.Location = new System.Drawing.Point(248, 6);
            this.lvClients.MultiSelect = false;
            this.lvClients.Name = "lvClients";
            this.lvClients.Size = new System.Drawing.Size(96, 107);
            this.lvClients.TabIndex = 22;
            this.lvClients.UseCompatibleStateImageBehavior = false;
            this.lvClients.View = System.Windows.Forms.View.Details;
            this.lvClients.SelectedIndexChanged += new System.EventHandler(this.lvClients_SelectedIndexChanged);
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
            this.lvServers.Location = new System.Drawing.Point(146, 6);
            this.lvServers.MultiSelect = false;
            this.lvServers.Name = "lvServers";
            this.lvServers.Size = new System.Drawing.Size(96, 107);
            this.lvServers.TabIndex = 20;
            this.lvServers.UseCompatibleStateImageBehavior = false;
            this.lvServers.View = System.Windows.Forms.View.Details;
            this.lvServers.SelectedIndexChanged += new System.EventHandler(this.lvServers_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "ID";
            this.columnHeader1.Width = 62;
            // 
            // nudTargetPortClient
            // 
            this.nudTargetPortClient.Location = new System.Drawing.Point(72, 93);
            this.nudTargetPortClient.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudTargetPortClient.Name = "nudTargetPortClient";
            this.nudTargetPortClient.Size = new System.Drawing.Size(68, 20);
            this.nudTargetPortClient.TabIndex = 18;
            this.nudTargetPortClient.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btStopClient
            // 
            this.btStopClient.Enabled = false;
            this.btStopClient.Location = new System.Drawing.Point(248, 119);
            this.btStopClient.Name = "btStopClient";
            this.btStopClient.Size = new System.Drawing.Size(96, 23);
            this.btStopClient.TabIndex = 23;
            this.btStopClient.Text = "Stop";
            this.btStopClient.UseVisualStyleBackColor = true;
            this.btStopClient.Click += new System.EventHandler(this.btStopClient_Click);
            // 
            // btStopServer
            // 
            this.btStopServer.Enabled = false;
            this.btStopServer.Location = new System.Drawing.Point(146, 119);
            this.btStopServer.Name = "btStopServer";
            this.btStopServer.Size = new System.Drawing.Size(96, 23);
            this.btStopServer.TabIndex = 21;
            this.btStopServer.Text = "Stop";
            this.btStopServer.UseVisualStyleBackColor = true;
            this.btStopServer.Click += new System.EventHandler(this.btStopServer_Click);
            // 
            // btStartClient
            // 
            this.btStartClient.Location = new System.Drawing.Point(12, 330);
            this.btStartClient.Name = "btStartClient";
            this.btStartClient.Size = new System.Drawing.Size(128, 23);
            this.btStartClient.TabIndex = 19;
            this.btStartClient.Text = "Start Client";
            this.btStartClient.UseVisualStyleBackColor = true;
            this.btStartClient.Click += new System.EventHandler(this.btStartClient_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Target port:";
            // 
            // nudLocalPortClient
            // 
            this.nudLocalPortClient.Location = new System.Drawing.Point(72, 67);
            this.nudLocalPortClient.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudLocalPortClient.Name = "nudLocalPortClient";
            this.nudLocalPortClient.Size = new System.Drawing.Size(68, 20);
            this.nudLocalPortClient.TabIndex = 16;
            this.nudLocalPortClient.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Local port:";
            // 
            // btStartServer
            // 
            this.btStartServer.Location = new System.Drawing.Point(12, 32);
            this.btStartServer.Name = "btStartServer";
            this.btStartServer.Size = new System.Drawing.Size(128, 23);
            this.btStartServer.TabIndex = 14;
            this.btStartServer.Text = "Start Server";
            this.btStartServer.UseVisualStyleBackColor = true;
            this.btStartServer.Click += new System.EventHandler(this.btStartServer_Click);
            // 
            // nudLocalPortServer
            // 
            this.nudLocalPortServer.Location = new System.Drawing.Point(72, 6);
            this.nudLocalPortServer.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudLocalPortServer.Name = "nudLocalPortServer";
            this.nudLocalPortServer.Size = new System.Drawing.Size(68, 20);
            this.nudLocalPortServer.TabIndex = 13;
            this.nudLocalPortServer.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Local port:";
            // 
            // lvHosts
            // 
            this.lvHosts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.lvHosts.FullRowSelect = true;
            this.lvHosts.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvHosts.HideSelection = false;
            this.lvHosts.Location = new System.Drawing.Point(12, 119);
            this.lvHosts.MultiSelect = false;
            this.lvHosts.Name = "lvHosts";
            this.lvHosts.Size = new System.Drawing.Size(128, 205);
            this.lvHosts.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvHosts.TabIndex = 24;
            this.lvHosts.UseCompatibleStateImageBehavior = false;
            this.lvHosts.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Host ID";
            this.columnHeader3.Width = 97;
            // 
            // cbHugeData
            // 
            this.cbHugeData.AutoSize = true;
            this.cbHugeData.Location = new System.Drawing.Point(146, 330);
            this.cbHugeData.Name = "cbHugeData";
            this.cbHugeData.Size = new System.Drawing.Size(102, 17);
            this.cbHugeData.TabIndex = 25;
            this.cbHugeData.Text = "Send huge data";
            this.cbHugeData.UseVisualStyleBackColor = true;
            // 
            // UDPForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 365);
            this.Controls.Add(this.cbHugeData);
            this.Controls.Add(this.lvHosts);
            this.Controls.Add(this.lvClients);
            this.Controls.Add(this.lvServers);
            this.Controls.Add(this.nudTargetPortClient);
            this.Controls.Add(this.btStopClient);
            this.Controls.Add(this.btStopServer);
            this.Controls.Add(this.btStartClient);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nudLocalPortClient);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btStartServer);
            this.Controls.Add(this.nudLocalPortServer);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UDPForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "UDP Control";
            this.Shown += new System.EventHandler(this.BroadcastForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.nudTargetPortClient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLocalPortClient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLocalPortServer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvClients;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ListView lvServers;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.NumericUpDown nudTargetPortClient;
        private System.Windows.Forms.Button btStopClient;
        private System.Windows.Forms.Button btStopServer;
        private System.Windows.Forms.Button btStartClient;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudLocalPortClient;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btStartServer;
        private System.Windows.Forms.NumericUpDown nudLocalPortServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView lvHosts;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.CheckBox cbHugeData;
    }
}