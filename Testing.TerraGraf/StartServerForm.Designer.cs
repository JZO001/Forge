namespace Testing.TerraGraf
{
    partial class StartServerForm
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
            this.lvInterfaces = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.cbAutoPort = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nudPort = new System.Windows.Forms.NumericUpDown();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btCreateServer = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.SuspendLayout();
            // 
            // lvInterfaces
            // 
            this.lvInterfaces.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lvInterfaces.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvInterfaces.FullRowSelect = true;
            this.lvInterfaces.Location = new System.Drawing.Point(12, 25);
            this.lvInterfaces.Name = "lvInterfaces";
            this.lvInterfaces.Size = new System.Drawing.Size(165, 101);
            this.lvInterfaces.TabIndex = 0;
            this.lvInterfaces.UseCompatibleStateImageBehavior = false;
            this.lvInterfaces.View = System.Windows.Forms.View.Details;
            this.lvInterfaces.SelectedIndexChanged += new System.EventHandler(this.lvInterfaces_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Interface(s):";
            // 
            // cbAutoPort
            // 
            this.cbAutoPort.AutoSize = true;
            this.cbAutoPort.Checked = true;
            this.cbAutoPort.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoPort.Location = new System.Drawing.Point(183, 25);
            this.cbAutoPort.Name = "cbAutoPort";
            this.cbAutoPort.Size = new System.Drawing.Size(69, 17);
            this.cbAutoPort.TabIndex = 3;
            this.cbAutoPort.Text = "Auto port";
            this.cbAutoPort.UseVisualStyleBackColor = true;
            this.cbAutoPort.CheckedChanged += new System.EventHandler(this.cbAutoPort_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 134);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Port:";
            // 
            // nudPort
            // 
            this.nudPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudPort.Enabled = false;
            this.nudPort.Location = new System.Drawing.Point(15, 150);
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
            this.nudPort.Size = new System.Drawing.Size(120, 20);
            this.nudPort.TabIndex = 5;
            this.nudPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudPort.Value = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Interfaces";
            this.columnHeader1.Width = 130;
            // 
            // btCreateServer
            // 
            this.btCreateServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCreateServer.Enabled = false;
            this.btCreateServer.Location = new System.Drawing.Point(188, 150);
            this.btCreateServer.Name = "btCreateServer";
            this.btCreateServer.Size = new System.Drawing.Size(75, 23);
            this.btCreateServer.TabIndex = 6;
            this.btCreateServer.Text = "Create";
            this.btCreateServer.UseVisualStyleBackColor = true;
            this.btCreateServer.Click += new System.EventHandler(this.btCreateServer_Click);
            // 
            // StartServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 182);
            this.Controls.Add(this.btCreateServer);
            this.Controls.Add(this.nudPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbAutoPort);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lvInterfaces);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StartServerForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Start TCP Server";
            this.Shown += new System.EventHandler(this.StartServerForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvInterfaces;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbAutoPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.Button btCreateServer;
    }
}