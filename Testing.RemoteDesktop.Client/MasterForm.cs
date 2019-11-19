using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Forge.RemoteDesktop.Client;
using Forge.RemoteDesktop.Client.Properties;
using Forge.RemoteDesktop.Contracts;

namespace Forge.Testing.RemoteDesktop.Client
{

    /// <summary>
    /// Example for remote desktop client
    /// </summary>
    public partial class MasterForm : Form
    {

        #region Field(s)

        private readonly List<RemoteDesktopClientForm> mActiveWindows = new List<RemoteDesktopClientForm>();

        #endregion
        
        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterForm"/> class.
        /// </summary>
        public MasterForm()
        {
            InitializeComponent();
            connectControl.ShowCancelButton = false;
            connectControl.EventConnect += new EventHandler<EventArgs>(ConnectEventHandler);
        } 

        #endregion

        #region Private method(s)

        private void MasterForm_Shown(object sender, EventArgs e)
        {
            connectControl.Initialize();
        }

        private void ConnectEventHandler(object sender, EventArgs e)
        {
            AuthenticationModeEnum mode = AuthenticationModeEnum.UsernameAndPassword;
            RemoteDesktopClientForm clientForm = new RemoteDesktopClientForm();
            try
            {
                clientForm.Connect(connectControl.Locator.ChannelId, connectControl.SelectedProvider.RemoteEndPoint);
                mode = clientForm.GetAuthenticationMode();
            }
            catch (Exception)
            {
                clientForm.Dispose();
                MessageBox.Show(this, "Failed to connect to the remote host.", Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            mActiveWindows.Add(clientForm);
            clientForm.FormClosed += new FormClosedEventHandler(ClientForm_FormClosed);
            clientForm.Show(this);

            while (true)
            {
                switch (mode)
                {
                    case AuthenticationModeEnum.OnlyPassword:
                    case AuthenticationModeEnum.UsernameAndPassword:
                        using (AuthenticationForm authForm = new AuthenticationForm(mode))
                        {
                            if (authForm.ShowDialog(clientForm) == System.Windows.Forms.DialogResult.OK)
                            {
                                try
                                {
                                    if (HandleLoginResult(clientForm, clientForm.Login(authForm.Username, authForm.Password)))
                                    {
                                        return;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(clientForm, string.Format("Failed to login. Reason: {0}", ex.Message), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    if (!clientForm.IsConnected)
                                    {
                                        clientForm.Close();
                                        //clientForm.Dispose();
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                clientForm.Close();
                                clientForm.Dispose();
                                return;
                            }
                        }
                        break;

                    case AuthenticationModeEnum.Off:
                        {
                            try
                            {
                                if (HandleLoginResult(clientForm, clientForm.Login(string.Empty, string.Empty)))
                                {
                                    return;
                                }
                            }
                            catch (Exception ex)
                            {
                                clientForm.Close();
                                clientForm.Dispose();
                                MessageBox.Show(clientForm, string.Format("Failed to login. Reason: {0}", ex.Message), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        break;
                }
            }
        }

        private void ClientForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            RemoteDesktopClientForm clientForm = (RemoteDesktopClientForm)sender;
            clientForm.FormClosed -= new FormClosedEventHandler(ClientForm_FormClosed);
            mActiveWindows.Remove(clientForm);
        }

        private bool HandleLoginResult(RemoteDesktopClientForm clientForm, LoginResponseStateEnum loginResult)
        {
            bool result = false;

            switch (loginResult)
            {
                case LoginResponseStateEnum.AccessDenied:
                    MessageBox.Show(clientForm, "Unable to authenticate. Access denied.", Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case LoginResponseStateEnum.AccessGranted:
                    result = true;
                    clientForm.StartEventPump();
                    break;

                case LoginResponseStateEnum.ServiceBusy:
                    MessageBox.Show(clientForm, "Unable to authenticate. Service is busy.", Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case LoginResponseStateEnum.ServiceInactive:
                    MessageBox.Show(clientForm, "Unable to authenticate. Service is inactive.", Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }

            return result;
        }

        #endregion

    }

}
