namespace Testing.TerraGraf
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
            this.tcDomains = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btGenerator = new System.Windows.Forms.Button();
            this.btLoadConfig = new System.Windows.Forms.Button();
            this.lvConfigs = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tcDomains.SuspendLayout();
            this.tabPage1.SuspendLayout();
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
            this.tcDomains.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btGenerator);
            this.tabPage1.Controls.Add(this.btLoadConfig);
            this.tabPage1.Controls.Add(this.lvConfigs);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(571, 343);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "SETUP";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btGenerator
            // 
            this.btGenerator.Location = new System.Drawing.Point(489, 33);
            this.btGenerator.Name = "btGenerator";
            this.btGenerator.Size = new System.Drawing.Size(75, 23);
            this.btGenerator.TabIndex = 2;
            this.btGenerator.Text = "Generate";
            this.btGenerator.UseVisualStyleBackColor = true;
            this.btGenerator.Click += new System.EventHandler(this.btGenerator_Click);
            // 
            // btLoadConfig
            // 
            this.btLoadConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btLoadConfig.Enabled = false;
            this.btLoadConfig.Location = new System.Drawing.Point(488, 3);
            this.btLoadConfig.Name = "btLoadConfig";
            this.btLoadConfig.Size = new System.Drawing.Size(75, 23);
            this.btLoadConfig.TabIndex = 1;
            this.btLoadConfig.Text = "LOAD";
            this.btLoadConfig.UseVisualStyleBackColor = true;
            this.btLoadConfig.Click += new System.EventHandler(this.btLoadConfig_Click);
            // 
            // lvConfigs
            // 
            this.lvConfigs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvConfigs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvConfigs.FullRowSelect = true;
            this.lvConfigs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvConfigs.HideSelection = false;
            this.lvConfigs.Location = new System.Drawing.Point(8, 3);
            this.lvConfigs.MultiSelect = false;
            this.lvConfigs.Name = "lvConfigs";
            this.lvConfigs.Size = new System.Drawing.Size(474, 332);
            this.lvConfigs.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvConfigs.TabIndex = 0;
            this.lvConfigs.UseCompatibleStateImageBehavior = false;
            this.lvConfigs.View = System.Windows.Forms.View.Details;
            this.lvConfigs.SelectedIndexChanged += new System.EventHandler(this.lvConfigs_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Configuration Name";
            this.columnHeader1.Width = 438;
            // 
            // MasterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 369);
            this.Controls.Add(this.tcDomains);
            this.MinimumSize = new System.Drawing.Size(595, 407);
            this.Name = "MasterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TerraGraf tester";
            this.Shown += new System.EventHandler(this.MasterForm_Shown);
            this.tcDomains.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tcDomains;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListView lvConfigs;
        private System.Windows.Forms.Button btLoadConfig;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button btGenerator;

    }
}

