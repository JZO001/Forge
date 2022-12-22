/* *********************************************************************
 * Date: 9 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Linq;
using System.Windows.Forms;
using Forge.RemoteDesktop.Contracts;
using Forge.RemoteDesktop.Service.Configuration;
using Forge.RemoteDesktop.Service.Properties;

namespace Forge.RemoteDesktop.Service
{

    /// <summary>
    /// Authentication handler form
    /// </summary>
    public partial class AuthenticationAdminForm : Form
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationAdminForm"/> class.
        /// </summary>
        public AuthenticationAdminForm()
        {
            InitializeComponent();

            Text = Resources.AuthenticationFormTitle;
            lCurrentAuthMode.Text = Resources.CurrentAuthMode;
            btSaveAuthMode.Text = Resources.Button_Save;
            btSaveGlobalPassword.Text = Resources.Button_Save;
            gbPassword.Text = Resources.EnterPassword;
            gbUsers.Text = Resources.Users;
            chUserNames.Text = Resources.Column_UserName;
            btAddUser.Text = Resources.Button_Add;
            btEditUser.Text = Resources.Button_Edit;
            btRemoveUser.Text = Resources.Button_Remove;
            lUserName.Text = Resources.lUserName;
            btOk.Text = Resources.Button_OK;
            btCancel.Text = Resources.Button_Cancel;

        }

        private void AuthenticationAdminForm_Shown(object sender, EventArgs e)
        {
            cbAuthenticationModes.Items.Add(new ComboBoxItem<AuthenticationModeEnum>(AuthenticationModeEnum.Off));
            cbAuthenticationModes.Items.Add(new ComboBoxItem<AuthenticationModeEnum>(AuthenticationModeEnum.OnlyPassword));
            cbAuthenticationModes.Items.Add(new ComboBoxItem<AuthenticationModeEnum>(AuthenticationModeEnum.UsernameAndPassword));

            switch (Settings.AuthenticationMode)
            {
                case AuthenticationModeEnum.OnlyPassword:
                    cbAuthenticationModes.SelectedItem = cbAuthenticationModes.Items[1];
                    gbPassword.Enabled = true;
                    pUsers.Enabled = false;
                    break;

                case AuthenticationModeEnum.UsernameAndPassword:
                    cbAuthenticationModes.SelectedItem = cbAuthenticationModes.Items[2];
                    gbPassword.Enabled = false;
                    pUsers.Enabled = true;
                    break;

                case AuthenticationModeEnum.Off:
                    cbAuthenticationModes.SelectedItem = cbAuthenticationModes.Items[0];
                    gbPassword.Enabled = false;
                    pUsers.Enabled = false;
                    break;
            }

            foreach (string username in AuthenticationHandlerModule.GetUsers())
            {
                ListViewItem item = new ListViewItem(username);
                lvUsers.Items.Add(item);
            }
        }

        private void btSaveGlobalPassword_Click(object sender, EventArgs e)
        {
            AuthenticationHandlerModule.SetPassword(tbPassword.Text);
            tbPassword.Text = string.Empty;
            MessageBox.Show(this, Resources.MsgBox_PasswordSaved, Resources.MsgBox_Title_Save, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btAddUser_Click(object sender, EventArgs e)
        {
            pUserEditor.Enabled = true;
            gbUsers.Enabled = false;
            tbUsername.Enabled = true;
            tbUsername.Focus();
        }

        private void btEditUser_Click(object sender, EventArgs e)
        {
            pUserEditor.Enabled = true;
            gbUsers.Enabled = false;
            tbUsername.Text = lvUsers.SelectedItems[0].Text;
            tbUsername.Enabled = false;
            tbUsername.Focus();
        }

        private void btRemoveUser_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, Resources.MsgBox_AreYouSure, Resources.MsgBox_Title_Save, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                AuthenticationHandlerModule.RemoveUser(lvUsers.SelectedItems[0].Text);
                lvUsers.SelectedItems[0].Remove();
            }
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            if (tbUsername.Enabled)
            {
                if (tbUsername.Text.Length == 0)
                {
                    MessageBox.Show(this, Resources.MsgBox_UserMandatory, Resources.MsgBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (AuthenticationHandlerModule.GetUsers().Contains<string>(tbUsername.Text))
                {
                    MessageBox.Show(this, Resources.MsgBox_UserNameExist, Resources.MsgBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                AuthenticationHandlerModule.SetPassword(tbUsername.Text, tbUserPassword.Text);
                lvUsers.Items.Add(new ListViewItem(tbUsername.Text));
            }
            else
            {
                AuthenticationHandlerModule.SetPassword(tbUsername.Text, tbUserPassword.Text);
            }
            btCancel_Click(sender, e);
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            tbUsername.Text = string.Empty;
            tbUserPassword.Text = string.Empty;
            pUserEditor.Enabled = false;
            gbUsers.Enabled = true;
        }

        private void lvUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvUsers.SelectedIndices.Count == 0)
            {
                btEditUser.Enabled = false;
                btRemoveUser.Enabled = false;
            }
            else
            {
                btEditUser.Enabled = true;
                btRemoveUser.Enabled = true;
            }
        }

        private void btSaveAuthMode_Click(object sender, EventArgs e)
        {
            ComboBoxItem<AuthenticationModeEnum> selectedModeItem = cbAuthenticationModes.SelectedItem as ComboBoxItem<AuthenticationModeEnum>;
            if (Settings.AuthenticationMode != selectedModeItem.Item)
            {
                Settings.AuthenticationMode = selectedModeItem.Item;
                Settings.Save(System.Configuration.ConfigurationSaveMode.Modified);
                MessageBox.Show(this, Resources.MsgBox_AuthModeSaved, Resources.MsgBox_Title_Save, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cbAuthenticationModes_SelectedIndexChanged(object sender, EventArgs e)
        {
            AuthenticationModeEnum authMode = ((ComboBoxItem<AuthenticationModeEnum>)cbAuthenticationModes.SelectedItem).Item;
            switch (authMode)
            {
                case AuthenticationModeEnum.OnlyPassword:
                    cbAuthenticationModes.SelectedItem = cbAuthenticationModes.Items[1];
                    gbPassword.Enabled = true;
                    pUsers.Enabled = false;
                    break;

                case AuthenticationModeEnum.UsernameAndPassword:
                    cbAuthenticationModes.SelectedItem = cbAuthenticationModes.Items[2];
                    gbPassword.Enabled = false;
                    pUsers.Enabled = true;
                    break;

                case AuthenticationModeEnum.Off:
                    cbAuthenticationModes.SelectedItem = cbAuthenticationModes.Items[0];
                    gbPassword.Enabled = false;
                    pUsers.Enabled = false;
                    break;
            }
        }

        private void cbAuthenticationModes_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

    }

}
