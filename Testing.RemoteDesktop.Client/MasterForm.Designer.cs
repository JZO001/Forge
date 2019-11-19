using System;
namespace Forge.Testing.RemoteDesktop.Client
{
    partial class MasterForm
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
                connectControl.EventConnect -= new EventHandler<EventArgs>(ConnectEventHandler);
                connectControl.Dispose();
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
            this.connectControl = new Forge.RemoteDesktop.Client.ConnectControl();
            this.SuspendLayout();
            // 
            // connectControl
            // 
            this.connectControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connectControl.Location = new System.Drawing.Point(12, 12);
            this.connectControl.Name = "connectControl";
            this.connectControl.ShowCancelButton = true;
            this.connectControl.Size = new System.Drawing.Size(411, 215);
            this.connectControl.TabIndex = 0;
            // 
            // MasterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 239);
            this.Controls.Add(this.connectControl);
            this.Name = "MasterForm";
            this.Text = "Remote Desktop Server";
            this.Shown += new System.EventHandler(this.MasterForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private Forge.RemoteDesktop.Client.ConnectControl connectControl;
    }
}