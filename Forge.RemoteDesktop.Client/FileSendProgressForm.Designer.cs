namespace Forge.RemoteDesktop.Client
{
    partial class FileSendProgressForm
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
            this.lTotalSizeText = new System.Windows.Forms.Label();
            this.lSentBytesText = new System.Windows.Forms.Label();
            this.lPercentText = new System.Windows.Forms.Label();
            this.pbFileUpload = new System.Windows.Forms.ProgressBar();
            this.btCancel = new System.Windows.Forms.Button();
            this.lTotalSizeValue = new System.Windows.Forms.Label();
            this.lSentBytesValue = new System.Windows.Forms.Label();
            this.lPercentValue = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lTotalSizeText
            // 
            this.lTotalSizeText.AutoSize = true;
            this.lTotalSizeText.Location = new System.Drawing.Point(23, 19);
            this.lTotalSizeText.Name = "lTotalSizeText";
            this.lTotalSizeText.Size = new System.Drawing.Size(55, 13);
            this.lTotalSizeText.TabIndex = 0;
            this.lTotalSizeText.Text = "Total size:";
            // 
            // lSentBytesText
            // 
            this.lSentBytesText.AutoSize = true;
            this.lSentBytesText.Location = new System.Drawing.Point(23, 44);
            this.lSentBytesText.Name = "lSentBytesText";
            this.lSentBytesText.Size = new System.Drawing.Size(66, 13);
            this.lSentBytesText.TabIndex = 1;
            this.lSentBytesText.Text = "Sent byte(s):";
            // 
            // lPercentText
            // 
            this.lPercentText.AutoSize = true;
            this.lPercentText.Location = new System.Drawing.Point(23, 70);
            this.lPercentText.Name = "lPercentText";
            this.lPercentText.Size = new System.Drawing.Size(64, 13);
            this.lPercentText.TabIndex = 2;
            this.lPercentText.Text = "Percent (%):";
            // 
            // pbFileUpload
            // 
            this.pbFileUpload.Location = new System.Drawing.Point(45, 97);
            this.pbFileUpload.Name = "pbFileUpload";
            this.pbFileUpload.Size = new System.Drawing.Size(281, 23);
            this.pbFileUpload.Step = 1;
            this.pbFileUpload.TabIndex = 3;
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(147, 140);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 4;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // lTotalSizeValue
            // 
            this.lTotalSizeValue.AutoSize = true;
            this.lTotalSizeValue.Location = new System.Drawing.Point(144, 19);
            this.lTotalSizeValue.Name = "lTotalSizeValue";
            this.lTotalSizeValue.Size = new System.Drawing.Size(13, 13);
            this.lTotalSizeValue.TabIndex = 5;
            this.lTotalSizeValue.Text = "0";
            // 
            // lSentBytesValue
            // 
            this.lSentBytesValue.AutoSize = true;
            this.lSentBytesValue.Location = new System.Drawing.Point(144, 44);
            this.lSentBytesValue.Name = "lSentBytesValue";
            this.lSentBytesValue.Size = new System.Drawing.Size(13, 13);
            this.lSentBytesValue.TabIndex = 6;
            this.lSentBytesValue.Text = "0";
            // 
            // lPercentValue
            // 
            this.lPercentValue.AutoSize = true;
            this.lPercentValue.Location = new System.Drawing.Point(144, 70);
            this.lPercentValue.Name = "lPercentValue";
            this.lPercentValue.Size = new System.Drawing.Size(13, 13);
            this.lPercentValue.TabIndex = 7;
            this.lPercentValue.Text = "0";
            // 
            // FileSendProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(371, 184);
            this.Controls.Add(this.lPercentValue);
            this.Controls.Add(this.lSentBytesValue);
            this.Controls.Add(this.lTotalSizeValue);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.pbFileUpload);
            this.Controls.Add(this.lPercentText);
            this.Controls.Add(this.lSentBytesText);
            this.Controls.Add(this.lTotalSizeText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FileSendProgressForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sending file...";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FileSendProgressForm_FormClosed);
            this.Shown += new System.EventHandler(this.FileSendProgressForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lTotalSizeText;
        private System.Windows.Forms.Label lSentBytesText;
        private System.Windows.Forms.Label lPercentText;
        private System.Windows.Forms.ProgressBar pbFileUpload;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Label lTotalSizeValue;
        private System.Windows.Forms.Label lSentBytesValue;
        private System.Windows.Forms.Label lPercentValue;
    }
}