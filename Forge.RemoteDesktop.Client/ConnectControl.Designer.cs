using System;
using Forge.Net.Services.Locators;
namespace Forge.RemoteDesktop.Client
{
    partial class ConnectControl
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
                if (mLocator != null)
                {
                    mLocator.EventAvailableServiceProvidersChanged -= new EventHandler<ServiceProvidersChangedEventArgs>(AvailableServiceProvidersChangedEventHandler);
                }
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
            this.btCancel = new System.Windows.Forms.Button();
            this.btConnect = new System.Windows.Forms.Button();
            this.lvServices = new System.Windows.Forms.ListView();
            this.chServiceId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // btCancel
            // 
            this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(126, 165);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(112, 23);
            this.btCancel.TabIndex = 5;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // btConnect
            // 
            this.btConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btConnect.Enabled = false;
            this.btConnect.Location = new System.Drawing.Point(244, 165);
            this.btConnect.Name = "btConnect";
            this.btConnect.Size = new System.Drawing.Size(112, 23);
            this.btConnect.TabIndex = 4;
            this.btConnect.Text = "Connect";
            this.btConnect.UseVisualStyleBackColor = true;
            this.btConnect.Click += new System.EventHandler(this.btConnect_Click);
            // 
            // lvServices
            // 
            this.lvServices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvServices.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chServiceId});
            this.lvServices.FullRowSelect = true;
            this.lvServices.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvServices.HideSelection = false;
            this.lvServices.Location = new System.Drawing.Point(3, 3);
            this.lvServices.MultiSelect = false;
            this.lvServices.Name = "lvServices";
            this.lvServices.Size = new System.Drawing.Size(353, 156);
            this.lvServices.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvServices.TabIndex = 3;
            this.lvServices.UseCompatibleStateImageBehavior = false;
            this.lvServices.View = System.Windows.Forms.View.Details;
            this.lvServices.SelectedIndexChanged += new System.EventHandler(this.lvServices_SelectedIndexChanged);
            // 
            // chServiceId
            // 
            this.chServiceId.Text = "ServiceId";
            this.chServiceId.Width = 318;
            // 
            // ConnectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btConnect);
            this.Controls.Add(this.lvServices);
            this.Name = "ConnectControl";
            this.Size = new System.Drawing.Size(361, 193);
            this.ResumeLayout(false);

        }

        #endregion

        /// <summary>
        /// The bt cancel
        /// </summary>
        protected System.Windows.Forms.Button btCancel;

        /// <summary>
        /// The bt connect
        /// </summary>
        protected System.Windows.Forms.Button btConnect;

        /// <summary>
        /// The level services
        /// </summary>
        protected System.Windows.Forms.ListView lvServices;

        /// <summary>
        /// The character service unique identifier
        /// </summary>
        protected System.Windows.Forms.ColumnHeader chServiceId;

    }
}
