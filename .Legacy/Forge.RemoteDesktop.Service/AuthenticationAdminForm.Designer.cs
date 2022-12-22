namespace Forge.RemoteDesktop.Service
{
    partial class AuthenticationAdminForm
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
            this.lCurrentAuthMode = new System.Windows.Forms.Label();
            this.cbAuthenticationModes = new System.Windows.Forms.ComboBox();
            this.gbPassword = new System.Windows.Forms.GroupBox();
            this.btSaveGlobalPassword = new System.Windows.Forms.Button();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.gbUsers = new System.Windows.Forms.GroupBox();
            this.btRemoveUser = new System.Windows.Forms.Button();
            this.btEditUser = new System.Windows.Forms.Button();
            this.btAddUser = new System.Windows.Forms.Button();
            this.lvUsers = new System.Windows.Forms.ListView();
            this.chUserNames = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pUserEditor = new System.Windows.Forms.Panel();
            this.btCancel = new System.Windows.Forms.Button();
            this.btOk = new System.Windows.Forms.Button();
            this.tbUserPassword = new System.Windows.Forms.TextBox();
            this.tbUsername = new System.Windows.Forms.TextBox();
            this.lPassword = new System.Windows.Forms.Label();
            this.lUserName = new System.Windows.Forms.Label();
            this.btSaveAuthMode = new System.Windows.Forms.Button();
            this.pUsers = new System.Windows.Forms.Panel();
            this.gbPassword.SuspendLayout();
            this.gbUsers.SuspendLayout();
            this.pUserEditor.SuspendLayout();
            this.pUsers.SuspendLayout();
            this.SuspendLayout();
            // 
            // lCurrentAuthMode
            // 
            this.lCurrentAuthMode.AutoSize = true;
            this.lCurrentAuthMode.Location = new System.Drawing.Point(12, 9);
            this.lCurrentAuthMode.Name = "lCurrentAuthMode";
            this.lCurrentAuthMode.Size = new System.Drawing.Size(143, 13);
            this.lCurrentAuthMode.TabIndex = 0;
            this.lCurrentAuthMode.Text = "Current authentication mode:";
            // 
            // cbAuthenticationModes
            // 
            this.cbAuthenticationModes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAuthenticationModes.FormattingEnabled = true;
            this.cbAuthenticationModes.Location = new System.Drawing.Point(40, 25);
            this.cbAuthenticationModes.Name = "cbAuthenticationModes";
            this.cbAuthenticationModes.Size = new System.Drawing.Size(163, 21);
            this.cbAuthenticationModes.TabIndex = 1;
            this.cbAuthenticationModes.SelectedIndexChanged += new System.EventHandler(this.cbAuthenticationModes_SelectedIndexChanged);
            this.cbAuthenticationModes.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cbAuthenticationModes_KeyPress);
            // 
            // gbPassword
            // 
            this.gbPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbPassword.Controls.Add(this.btSaveGlobalPassword);
            this.gbPassword.Controls.Add(this.tbPassword);
            this.gbPassword.Enabled = false;
            this.gbPassword.Location = new System.Drawing.Point(15, 54);
            this.gbPassword.Name = "gbPassword";
            this.gbPassword.Size = new System.Drawing.Size(449, 61);
            this.gbPassword.TabIndex = 2;
            this.gbPassword.TabStop = false;
            this.gbPassword.Text = "Enter password";
            // 
            // btSaveGlobalPassword
            // 
            this.btSaveGlobalPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSaveGlobalPassword.Location = new System.Drawing.Point(347, 26);
            this.btSaveGlobalPassword.Name = "btSaveGlobalPassword";
            this.btSaveGlobalPassword.Size = new System.Drawing.Size(96, 23);
            this.btSaveGlobalPassword.TabIndex = 2;
            this.btSaveGlobalPassword.Text = "Save";
            this.btSaveGlobalPassword.UseVisualStyleBackColor = true;
            this.btSaveGlobalPassword.Click += new System.EventHandler(this.btSaveGlobalPassword_Click);
            // 
            // tbPassword
            // 
            this.tbPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPassword.Location = new System.Drawing.Point(25, 26);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(316, 20);
            this.tbPassword.TabIndex = 1;
            // 
            // gbUsers
            // 
            this.gbUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbUsers.Controls.Add(this.btRemoveUser);
            this.gbUsers.Controls.Add(this.btEditUser);
            this.gbUsers.Controls.Add(this.btAddUser);
            this.gbUsers.Controls.Add(this.lvUsers);
            this.gbUsers.Location = new System.Drawing.Point(11, 3);
            this.gbUsers.Name = "gbUsers";
            this.gbUsers.Size = new System.Drawing.Size(450, 168);
            this.gbUsers.TabIndex = 3;
            this.gbUsers.TabStop = false;
            this.gbUsers.Text = "Users";
            // 
            // btRemoveUser
            // 
            this.btRemoveUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btRemoveUser.Enabled = false;
            this.btRemoveUser.Location = new System.Drawing.Point(347, 77);
            this.btRemoveUser.Name = "btRemoveUser";
            this.btRemoveUser.Size = new System.Drawing.Size(97, 23);
            this.btRemoveUser.TabIndex = 3;
            this.btRemoveUser.Text = "Remove";
            this.btRemoveUser.UseVisualStyleBackColor = true;
            this.btRemoveUser.Click += new System.EventHandler(this.btRemoveUser_Click);
            // 
            // btEditUser
            // 
            this.btEditUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btEditUser.Enabled = false;
            this.btEditUser.Location = new System.Drawing.Point(347, 48);
            this.btEditUser.Name = "btEditUser";
            this.btEditUser.Size = new System.Drawing.Size(97, 23);
            this.btEditUser.TabIndex = 2;
            this.btEditUser.Text = "Edit";
            this.btEditUser.UseVisualStyleBackColor = true;
            this.btEditUser.Click += new System.EventHandler(this.btEditUser_Click);
            // 
            // btAddUser
            // 
            this.btAddUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btAddUser.Location = new System.Drawing.Point(347, 19);
            this.btAddUser.Name = "btAddUser";
            this.btAddUser.Size = new System.Drawing.Size(97, 23);
            this.btAddUser.TabIndex = 1;
            this.btAddUser.Text = "Add";
            this.btAddUser.UseVisualStyleBackColor = true;
            this.btAddUser.Click += new System.EventHandler(this.btAddUser_Click);
            // 
            // lvUsers
            // 
            this.lvUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvUsers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chUserNames});
            this.lvUsers.FullRowSelect = true;
            this.lvUsers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvUsers.HideSelection = false;
            this.lvUsers.Location = new System.Drawing.Point(6, 19);
            this.lvUsers.MultiSelect = false;
            this.lvUsers.Name = "lvUsers";
            this.lvUsers.Size = new System.Drawing.Size(335, 142);
            this.lvUsers.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvUsers.TabIndex = 0;
            this.lvUsers.UseCompatibleStateImageBehavior = false;
            this.lvUsers.View = System.Windows.Forms.View.Details;
            this.lvUsers.SelectedIndexChanged += new System.EventHandler(this.lvUsers_SelectedIndexChanged);
            // 
            // chUserNames
            // 
            this.chUserNames.Text = "User name";
            this.chUserNames.Width = 246;
            // 
            // pUserEditor
            // 
            this.pUserEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pUserEditor.Controls.Add(this.btCancel);
            this.pUserEditor.Controls.Add(this.btOk);
            this.pUserEditor.Controls.Add(this.tbUserPassword);
            this.pUserEditor.Controls.Add(this.tbUsername);
            this.pUserEditor.Controls.Add(this.lPassword);
            this.pUserEditor.Controls.Add(this.lUserName);
            this.pUserEditor.Enabled = false;
            this.pUserEditor.Location = new System.Drawing.Point(12, 177);
            this.pUserEditor.Name = "pUserEditor";
            this.pUserEditor.Size = new System.Drawing.Size(450, 72);
            this.pUserEditor.TabIndex = 4;
            // 
            // btCancel
            // 
            this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancel.Location = new System.Drawing.Point(347, 36);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(97, 23);
            this.btCancel.TabIndex = 5;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // btOk
            // 
            this.btOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btOk.Location = new System.Drawing.Point(347, 7);
            this.btOk.Name = "btOk";
            this.btOk.Size = new System.Drawing.Size(97, 23);
            this.btOk.TabIndex = 4;
            this.btOk.Text = "OK";
            this.btOk.UseVisualStyleBackColor = true;
            this.btOk.Click += new System.EventHandler(this.btOk_Click);
            // 
            // tbUserPassword
            // 
            this.tbUserPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbUserPassword.HideSelection = false;
            this.tbUserPassword.Location = new System.Drawing.Point(108, 35);
            this.tbUserPassword.Name = "tbUserPassword";
            this.tbUserPassword.PasswordChar = '*';
            this.tbUserPassword.Size = new System.Drawing.Size(233, 20);
            this.tbUserPassword.TabIndex = 3;
            // 
            // tbUsername
            // 
            this.tbUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbUsername.HideSelection = false;
            this.tbUsername.Location = new System.Drawing.Point(108, 9);
            this.tbUsername.Name = "tbUsername";
            this.tbUsername.Size = new System.Drawing.Size(233, 20);
            this.tbUsername.TabIndex = 2;
            // 
            // lPassword
            // 
            this.lPassword.AutoSize = true;
            this.lPassword.Location = new System.Drawing.Point(6, 38);
            this.lPassword.Name = "lPassword";
            this.lPassword.Size = new System.Drawing.Size(56, 13);
            this.lPassword.TabIndex = 1;
            this.lPassword.Text = "Password:";
            // 
            // lUserName
            // 
            this.lUserName.AutoSize = true;
            this.lUserName.Location = new System.Drawing.Point(6, 12);
            this.lUserName.Name = "lUserName";
            this.lUserName.Size = new System.Drawing.Size(61, 13);
            this.lUserName.TabIndex = 0;
            this.lUserName.Text = "User name:";
            // 
            // btSaveAuthMode
            // 
            this.btSaveAuthMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSaveAuthMode.Location = new System.Drawing.Point(362, 23);
            this.btSaveAuthMode.Name = "btSaveAuthMode";
            this.btSaveAuthMode.Size = new System.Drawing.Size(96, 23);
            this.btSaveAuthMode.TabIndex = 4;
            this.btSaveAuthMode.Text = "Save";
            this.btSaveAuthMode.UseVisualStyleBackColor = true;
            this.btSaveAuthMode.Click += new System.EventHandler(this.btSaveAuthMode_Click);
            // 
            // pUsers
            // 
            this.pUsers.Controls.Add(this.gbUsers);
            this.pUsers.Controls.Add(this.pUserEditor);
            this.pUsers.Enabled = false;
            this.pUsers.Location = new System.Drawing.Point(3, 121);
            this.pUsers.Name = "pUsers";
            this.pUsers.Size = new System.Drawing.Size(471, 251);
            this.pUsers.TabIndex = 4;
            // 
            // AuthenticationAdminForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 374);
            this.Controls.Add(this.pUsers);
            this.Controls.Add(this.btSaveAuthMode);
            this.Controls.Add(this.gbPassword);
            this.Controls.Add(this.cbAuthenticationModes);
            this.Controls.Add(this.lCurrentAuthMode);
            this.MinimumSize = new System.Drawing.Size(493, 412);
            this.Name = "AuthenticationAdminForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Authentication Admininistration";
            this.Shown += new System.EventHandler(this.AuthenticationAdminForm_Shown);
            this.gbPassword.ResumeLayout(false);
            this.gbPassword.PerformLayout();
            this.gbUsers.ResumeLayout(false);
            this.pUserEditor.ResumeLayout(false);
            this.pUserEditor.PerformLayout();
            this.pUsers.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lCurrentAuthMode;
        private System.Windows.Forms.ComboBox cbAuthenticationModes;
        private System.Windows.Forms.GroupBox gbPassword;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Button btSaveGlobalPassword;
        private System.Windows.Forms.GroupBox gbUsers;
        private System.Windows.Forms.Panel pUserEditor;
        private System.Windows.Forms.Button btRemoveUser;
        private System.Windows.Forms.Button btEditUser;
        private System.Windows.Forms.Button btAddUser;
        private System.Windows.Forms.ListView lvUsers;
        private System.Windows.Forms.ColumnHeader chUserNames;
        private System.Windows.Forms.TextBox tbUserPassword;
        private System.Windows.Forms.TextBox tbUsername;
        private System.Windows.Forms.Label lPassword;
        private System.Windows.Forms.Label lUserName;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btOk;
        private System.Windows.Forms.Button btSaveAuthMode;
        private System.Windows.Forms.Panel pUsers;
    }
}