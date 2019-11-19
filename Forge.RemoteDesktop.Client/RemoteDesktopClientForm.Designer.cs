namespace Forge.RemoteDesktop.Client
{
    partial class RemoteDesktopClientForm
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
                Disconnect();
                rdpClient.EventConnectionStateChange -= EventConnectionStateChange;
                rdpClient.Dispose();
            }
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
            this.components = new System.ComponentModel.Container();
            this.cmRemoteDesktopMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.fullScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setQualityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendAFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.reconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rdpClient = new Forge.RemoteDesktop.Client.RemoteDesktopWinFormsControl();
            this.showMenuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmRemoteDesktopMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmRemoteDesktopMenu
            // 
            this.cmRemoteDesktopMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showMenuToolStripMenuItem,
            this.fullScreenToolStripMenuItem,
            this.pauseToolStripMenuItem,
            this.setQualityToolStripMenuItem,
            this.refreshScreenToolStripMenuItem,
            this.sendAFileToolStripMenuItem,
            this.toolStripMenuItem1,
            this.reconnectToolStripMenuItem,
            this.disconnectToolStripMenuItem});
            this.cmRemoteDesktopMenu.Name = "cmRemoteDesktopMenu";
            this.cmRemoteDesktopMenu.Size = new System.Drawing.Size(203, 208);
            // 
            // fullScreenToolStripMenuItem
            // 
            this.fullScreenToolStripMenuItem.Enabled = false;
            this.fullScreenToolStripMenuItem.Name = "fullScreenToolStripMenuItem";
            this.fullScreenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F)));
            this.fullScreenToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.fullScreenToolStripMenuItem.Text = "Full screen";
            this.fullScreenToolStripMenuItem.Click += new System.EventHandler(this.fullScreenToolStripMenuItem_Click);
            // 
            // pauseToolStripMenuItem
            // 
            this.pauseToolStripMenuItem.Enabled = false;
            this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            this.pauseToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.pauseToolStripMenuItem.Text = "Pause";
            this.pauseToolStripMenuItem.Click += new System.EventHandler(this.pauseToolStripMenuItem_Click);
            // 
            // setQualityToolStripMenuItem
            // 
            this.setQualityToolStripMenuItem.Enabled = false;
            this.setQualityToolStripMenuItem.Name = "setQualityToolStripMenuItem";
            this.setQualityToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.setQualityToolStripMenuItem.Text = "Set quality";
            this.setQualityToolStripMenuItem.Click += new System.EventHandler(this.setQualityToolStripMenuItem_Click);
            // 
            // refreshScreenToolStripMenuItem
            // 
            this.refreshScreenToolStripMenuItem.Enabled = false;
            this.refreshScreenToolStripMenuItem.Name = "refreshScreenToolStripMenuItem";
            this.refreshScreenToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.refreshScreenToolStripMenuItem.Text = "Refresh screen";
            this.refreshScreenToolStripMenuItem.Click += new System.EventHandler(this.refreshScreenToolStripMenuItem_Click);
            // 
            // sendAFileToolStripMenuItem
            // 
            this.sendAFileToolStripMenuItem.Enabled = false;
            this.sendAFileToolStripMenuItem.Name = "sendAFileToolStripMenuItem";
            this.sendAFileToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.sendAFileToolStripMenuItem.Text = "Send a file";
            this.sendAFileToolStripMenuItem.Click += new System.EventHandler(this.sendAFileToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(199, 6);
            // 
            // reconnectToolStripMenuItem
            // 
            this.reconnectToolStripMenuItem.Enabled = false;
            this.reconnectToolStripMenuItem.Name = "reconnectToolStripMenuItem";
            this.reconnectToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.reconnectToolStripMenuItem.Text = "Reconnect";
            this.reconnectToolStripMenuItem.Click += new System.EventHandler(this.reconnectToolStripMenuItem_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Enabled = false;
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.disconnectToolStripMenuItem.Text = "Disconnect";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // rdpClient
            // 
            this.rdpClient.AutoScroll = true;
            this.rdpClient.BackColor = System.Drawing.Color.Black;
            this.rdpClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdpClient.Location = new System.Drawing.Point(0, 0);
            this.rdpClient.Margin = new System.Windows.Forms.Padding(0);
            this.rdpClient.Name = "rdpClient";
            this.rdpClient.Size = new System.Drawing.Size(611, 399);
            this.rdpClient.TabIndex = 0;
            // 
            // showMenuToolStripMenuItem
            // 
            this.showMenuToolStripMenuItem.Enabled = false;
            this.showMenuToolStripMenuItem.Name = "showMenuToolStripMenuItem";
            this.showMenuToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F12;
            this.showMenuToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.showMenuToolStripMenuItem.Text = "Show this menu";
            // 
            // RemoteDesktopClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 399);
            this.Controls.Add(this.rdpClient);
            this.Name = "RemoteDesktopClientForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Remote Desktop Client";
            this.cmRemoteDesktopMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private RemoteDesktopWinFormsControl rdpClient;
        private System.Windows.Forms.ContextMenuStrip cmRemoteDesktopMenu;
        private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setQualityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendAFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem reconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fullScreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshScreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showMenuToolStripMenuItem;
    }
}