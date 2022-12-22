namespace Forge.RemoteDesktop.Client
{
    partial class RemoteDesktopWinFormsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pbClient = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbClient)).BeginInit();
            this.SuspendLayout();
            // 
            // pbClient
            // 
            this.pbClient.BackColor = System.Drawing.SystemColors.Control;
            this.pbClient.Location = new System.Drawing.Point(0, 0);
            this.pbClient.Margin = new System.Windows.Forms.Padding(0);
            this.pbClient.Name = "pbClient";
            this.pbClient.Size = new System.Drawing.Size(1024, 768);
            this.pbClient.TabIndex = 1;
            this.pbClient.TabStop = false;
            this.pbClient.MouseEnter += new System.EventHandler(this.pbClient_MouseEnter);
            // 
            // RemoteDesktopWinFormsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.pbClient);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "RemoteDesktopWinFormsControl";
            this.Size = new System.Drawing.Size(236, 58);
            ((System.ComponentModel.ISupportInitialize)(this.pbClient)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbClient;
    }
}
