using System;

namespace Forge.RemoteDesktop.Service
{
    partial class ConnectedClientsControl
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
            if (disposing)
            {
                RemoteDesktopServiceManager.Instance.EventConnectionStateChange -= new EventHandler<ConnectionStateChangedEventArgs>(RemoteDesktopServiceManager_EventConnectionStateChange);
                RemoteDesktopServiceManager.Instance.EventAcceptClient -= new EventHandler<AcceptClientEventArgs>(RemoteDesktopServiceManager_EventAcceptClient);
            }
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.btDisconnectActiveClient = new System.Windows.Forms.Button();
            this.lvActiveClients = new System.Windows.Forms.ListView();
            this.chClientId2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSessionId2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btDisconnectConnectedClient = new System.Windows.Forms.Button();
            this.lConnectedClients = new System.Windows.Forms.Label();
            this.lvConnectedClients = new System.Windows.Forms.ListView();
            this.chClientId1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSessionId1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 182);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Active clients";
            // 
            // btDisconnectActiveClient
            // 
            this.btDisconnectActiveClient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btDisconnectActiveClient.Enabled = false;
            this.btDisconnectActiveClient.Location = new System.Drawing.Point(346, 334);
            this.btDisconnectActiveClient.Name = "btDisconnectActiveClient";
            this.btDisconnectActiveClient.Size = new System.Drawing.Size(151, 23);
            this.btDisconnectActiveClient.TabIndex = 12;
            this.btDisconnectActiveClient.Text = "Disconnect";
            this.btDisconnectActiveClient.UseVisualStyleBackColor = true;
            this.btDisconnectActiveClient.Click += new System.EventHandler(this.btDisconnectActiveClient_Click);
            // 
            // lvActiveClients
            // 
            this.lvActiveClients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvActiveClients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chClientId2,
            this.chSessionId2});
            this.lvActiveClients.FullRowSelect = true;
            this.lvActiveClients.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvActiveClients.HideSelection = false;
            this.lvActiveClients.Location = new System.Drawing.Point(3, 198);
            this.lvActiveClients.MultiSelect = false;
            this.lvActiveClients.Name = "lvActiveClients";
            this.lvActiveClients.Size = new System.Drawing.Size(494, 130);
            this.lvActiveClients.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvActiveClients.TabIndex = 11;
            this.lvActiveClients.UseCompatibleStateImageBehavior = false;
            this.lvActiveClients.View = System.Windows.Forms.View.Details;
            this.lvActiveClients.SelectedIndexChanged += new System.EventHandler(this.lvActiveClients_SelectedIndexChanged);
            // 
            // chClientId2
            // 
            this.chClientId2.Text = "ClientId";
            this.chClientId2.Width = 231;
            // 
            // chSessionId2
            // 
            this.chSessionId2.Text = "SessionId";
            this.chSessionId2.Width = 235;
            // 
            // btDisconnectConnectedClient
            // 
            this.btDisconnectConnectedClient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btDisconnectConnectedClient.Enabled = false;
            this.btDisconnectConnectedClient.Location = new System.Drawing.Point(346, 154);
            this.btDisconnectConnectedClient.Name = "btDisconnectConnectedClient";
            this.btDisconnectConnectedClient.Size = new System.Drawing.Size(151, 23);
            this.btDisconnectConnectedClient.TabIndex = 10;
            this.btDisconnectConnectedClient.Text = "Disconnect";
            this.btDisconnectConnectedClient.UseVisualStyleBackColor = true;
            this.btDisconnectConnectedClient.Click += new System.EventHandler(this.btDisconnectConnectedClient_Click);
            // 
            // lConnectedClients
            // 
            this.lConnectedClients.AutoSize = true;
            this.lConnectedClients.Location = new System.Drawing.Point(6, 5);
            this.lConnectedClients.Name = "lConnectedClients";
            this.lConnectedClients.Size = new System.Drawing.Size(92, 13);
            this.lConnectedClients.TabIndex = 9;
            this.lConnectedClients.Text = "Connected clients";
            // 
            // lvConnectedClients
            // 
            this.lvConnectedClients.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvConnectedClients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chClientId1,
            this.chSessionId1});
            this.lvConnectedClients.FullRowSelect = true;
            this.lvConnectedClients.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvConnectedClients.HideSelection = false;
            this.lvConnectedClients.Location = new System.Drawing.Point(3, 21);
            this.lvConnectedClients.Name = "lvConnectedClients";
            this.lvConnectedClients.Size = new System.Drawing.Size(494, 127);
            this.lvConnectedClients.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvConnectedClients.TabIndex = 8;
            this.lvConnectedClients.UseCompatibleStateImageBehavior = false;
            this.lvConnectedClients.View = System.Windows.Forms.View.Details;
            this.lvConnectedClients.SelectedIndexChanged += new System.EventHandler(this.lvConnectedClients_SelectedIndexChanged);
            // 
            // chClientId1
            // 
            this.chClientId1.Text = "ClientId";
            this.chClientId1.Width = 231;
            // 
            // chSessionId1
            // 
            this.chSessionId1.Text = "SessionId";
            this.chSessionId1.Width = 235;
            // 
            // ConnectedClientsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btDisconnectActiveClient);
            this.Controls.Add(this.lvActiveClients);
            this.Controls.Add(this.btDisconnectConnectedClient);
            this.Controls.Add(this.lConnectedClients);
            this.Controls.Add(this.lvConnectedClients);
            this.MinimumSize = new System.Drawing.Size(500, 360);
            this.Name = "ConnectedClientsControl";
            this.Size = new System.Drawing.Size(500, 360);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btDisconnectActiveClient;
        private System.Windows.Forms.ListView lvActiveClients;
        private System.Windows.Forms.ColumnHeader chClientId2;
        private System.Windows.Forms.ColumnHeader chSessionId2;
        private System.Windows.Forms.Button btDisconnectConnectedClient;
        private System.Windows.Forms.Label lConnectedClients;
        private System.Windows.Forms.ListView lvConnectedClients;
        private System.Windows.Forms.ColumnHeader chClientId1;
        private System.Windows.Forms.ColumnHeader chSessionId1;
    }
}
