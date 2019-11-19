/* *********************************************************************
 * Date: 11 Sep 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Forge.Collections;
using Forge.EventRaiser;
using Forge.Net.Services.Locators;
using Forge.Net.Synapse;
using Forge.RemoteDesktop.Client.Properties;
using Forge.RemoteDesktop.Contracts;
using log4net;

namespace Forge.RemoteDesktop.Client
{

    /// <summary>
    /// Reference implementation of a remote desktop client form
    /// </summary>
    public partial class RemoteDesktopClientForm : Form
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(RemoteDesktopClientForm));

        private string mLastChannelId = string.Empty;

        private AddressEndPoint mLastAddressEp = null;

        private string mLastUsername = string.Empty;

        private string mLastPassword = string.Empty;

        private bool mTopMostStateBeforeFullScreen = false;

        private Rectangle mBoundaryBeforeFullScreen = Rectangle.Empty;

        private FormWindowState mPreviousWindowState = FormWindowState.Normal;

        /// <summary>
        /// Occurs when [event connection state change].
        /// </summary>
        public event EventHandler<ConnectionStateChangeEventArgs> EventConnectionStateChange;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDesktopClientForm"/> class.
        /// </summary>
        public RemoteDesktopClientForm()
        {
            InitializeComponent();

            this.Text = Resources.FormTitle;
            fullScreenToolStripMenuItem.Text = Resources.cmFullScreen;
            pauseToolStripMenuItem.Text = Resources.cmPause;
            setQualityToolStripMenuItem.Text = Resources.cmSetQuality;
            refreshScreenToolStripMenuItem.Text = Resources.cmRefreshScreen;
            sendAFileToolStripMenuItem.Text = Resources.cmSendFile;
            reconnectToolStripMenuItem.Text = Resources.cmReconnect;
            disconnectToolStripMenuItem.Text = Resources.cmDisconnect;
            showMenuToolStripMenuItem.Text = Resources.cmShowMenu;

            rdpClient.SubscribeForKeys(Keys.F12, new EventHandler<SubscribedKeyPressEventArgs>(ShowMenu));
            rdpClient.EventConnectionStateChange += new EventHandler<ConnectionStateChangeEventArgs>(rdpClient_EventConnectionStateChange);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the known services on the network.
        /// </summary>
        /// <value>
        /// The services.
        /// </value>
        public static IListSpecialized<ServiceProvider> Services
        {
            get
            {
                return RemoteDesktopWinFormsControl.Services;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [is connected].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is connected]; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get { return rdpClient.IsConnected; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [is authenticated].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is authenticated]; otherwise, <c>false</c>.
        /// </value>
        public bool IsAuthenticated
        {
            get { return rdpClient.IsAuthenticated; }
        }

        /// <summary>
        /// Gets a value indicating whether [is active].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is active]; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive
        {
            get { return rdpClient.IsActive; }
        }

        /// <summary>
        /// Gets a value indicating whether [is event pump active].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is event pump active]; otherwise, <c>false</c>.
        /// </value>
        public bool IsEventPumpActive
        {
            get
            {
                return rdpClient.IsEventPumpActive;
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="endPoint">The end point.</param>
        public void Connect(string channelId, AddressEndPoint endPoint)
        {
            rdpClient.Connect(channelId, endPoint);
            this.mLastChannelId = channelId;
            this.mLastAddressEp = endPoint;
        }

        /// <summary>
        /// Disconnects this instance from the service.
        /// </summary>
        public void Disconnect()
        {
            rdpClient.Disconnect();
        }

        /// <summary>
        /// Gets the authentication mode.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Client was not connected.</exception>
        public AuthenticationModeEnum GetAuthenticationMode()
        {
            return rdpClient.GetAuthenticationMode();
        }

        /// <summary>
        /// Logins with the specified username and password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Client was not connected.</exception>
        public LoginResponseStateEnum Login(string username, string password)
        {
            LoginResponseStateEnum result = rdpClient.Login(username, password);
            if (result == LoginResponseStateEnum.AccessGranted)
            {
                mLastUsername = username;
                mLastPassword = password;
                fullScreenToolStripMenuItem.Enabled = true;
                pauseToolStripMenuItem.Enabled = true;
                setQualityToolStripMenuItem.Enabled = true;
                refreshScreenToolStripMenuItem.Enabled = true;
                sendAFileToolStripMenuItem.Enabled = true;
                rdpClient.SubscribeForKeys(new KeysSubscription(Keys.F, false, true, true), new EventHandler<SubscribedKeyPressEventArgs>(FullScreenHandler));
            }
            return result;
        }

        /// <summary>
        /// Starts the event pump.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// Client was not connected.
        /// or
        /// Client was not authenticated.
        /// </exception>
        public void StartEventPump()
        {
            rdpClient.StartEventPump();
        }

        /// <summary>
        /// Stops the event pump.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// Client was not connected.
        /// or
        /// Client was not authenticated.
        /// </exception>
        public void StopEventPump()
        {
            rdpClient.StopEventPump();
        }

        /// <summary>
        /// Refreshes the desktop.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// Client was not connected.
        /// or
        /// Client was not authenticated.
        /// or
        /// Client is not active.
        /// </exception>
        public void RefreshDesktop()
        {
            rdpClient.RefreshDesktop();
        }

        /// <summary>
        /// Sets the desktop quality manually.
        /// </summary>
        /// <param name="qualityPercent">The quality percent.</param>
        /// <exception cref="System.InvalidOperationException">
        /// Client was not connected.
        /// or
        /// Client was not authenticated.
        /// </exception>
        public void SetDesktopQualityManually(int qualityPercent)
        {
            rdpClient.SetDesktopQualityManually(qualityPercent);
        }

        /// <summary>
        /// Sends the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="stream">The stream.</param>
        /// <exception cref="System.InvalidOperationException">
        /// Client was not connected.
        /// or
        /// Client was not authenticated.
        /// or
        /// Client is not active.
        /// </exception>
        public void SendFile(string fileName, Stream stream)
        {
            rdpClient.SendFile(fileName, stream);
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Raises the <see cref="E:ConnectionStateChange" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ConnectionStateChangeEventArgs"/> instance containing the event data.</param>
        protected virtual void OnConnectionStateChange(ConnectionStateChangeEventArgs e)
        {
            if (e.IsConnected)
            {
                reconnectToolStripMenuItem.Enabled = false;
                disconnectToolStripMenuItem.Enabled = true;
            }
            else
            {
                reconnectToolStripMenuItem.Enabled = true;
                disconnectToolStripMenuItem.Enabled = false;
                fullScreenToolStripMenuItem.Enabled = false;
                pauseToolStripMenuItem.Enabled = false;
                pauseToolStripMenuItem.Text = Resources.cmPause;
                setQualityToolStripMenuItem.Enabled = false;
                refreshScreenToolStripMenuItem.Enabled = false;
                sendAFileToolStripMenuItem.Enabled = false;
                rdpClient.UnsubscribeForKeys(new KeysSubscription(Keys.F, false, true, true), new EventHandler<SubscribedKeyPressEventArgs>(FullScreenHandler));
            }
            Raiser.CallDelegatorBySync(EventConnectionStateChange, new object[] { e });
        }

        /// <summary>
        /// Called when [titlebar click].
        /// </summary>
        /// <param name="pos">The position.</param>
        protected virtual void OnTitlebarRightClick(Point pos)
        {
            cmRemoteDesktopMenu.Show(pos);
        }

        /// <summary>
        /// Handle messages for this instance of form
        /// </summary>
        /// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message" /> to process.</param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0xa4)
            {
                // Trap WM_NCRBUTTONDOWN
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                OnTitlebarRightClick(pos);
                return;
            }
            base.WndProc(ref m);
        }

        #endregion

        #region Private method(s)

        private void ShowMenu(object sender, SubscribedKeyPressEventArgs e)
        {
            if (this.InvokeRequired)
            {
                EventHandler<SubscribedKeyPressEventArgs> d = new EventHandler<SubscribedKeyPressEventArgs>(ShowMenu);
                ((RemoteDesktopClientForm)d.Target).Invoke(d, sender, e);
                return;
            }

            cmRemoteDesktopMenu.Show(Cursor.Position);
        }

        private void FullScreenHandler(object sender, SubscribedKeyPressEventArgs e)
        {
            if (this.InvokeRequired)
            {
                EventHandler<SubscribedKeyPressEventArgs> d = new EventHandler<SubscribedKeyPressEventArgs>(FullScreenHandler);
                ((RemoteDesktopClientForm)d.Target).Invoke(d, sender, e);
                return;
            }

            if (this.FormBorderStyle == System.Windows.Forms.FormBorderStyle.None)
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                this.TopMost = mTopMostStateBeforeFullScreen;
                this.DesktopBounds = mBoundaryBeforeFullScreen;
                this.WindowState = mPreviousWindowState;
                fullScreenToolStripMenuItem.Text = Resources.cmFullScreen;
            }
            else
            {
                mPreviousWindowState = this.WindowState;
                mTopMostStateBeforeFullScreen = this.TopMost;
                mBoundaryBeforeFullScreen = this.DesktopBounds;
                Screen currentScreen = Screen.FromControl(this);
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.TopMost = true;
                this.DesktopBounds = currentScreen.Bounds;
                fullScreenToolStripMenuItem.Text = Resources.cmFullScreenExit;
            }
        }

        private void rdpClient_EventConnectionStateChange(object sender, ConnectionStateChangeEventArgs e)
        {
            if (this.InvokeRequired)
            {
                EventHandler<ConnectionStateChangeEventArgs> d = new EventHandler<ConnectionStateChangeEventArgs>(rdpClient_EventConnectionStateChange);
                try
                {
                    ((RemoteDesktopClientForm)d.Target).Invoke(d, sender, e);
                }
                catch (Exception) { }
                return;
            }

            OnConnectionStateChange(e);
        }

        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullScreenHandler(rdpClient, new SubscribedKeyPressEventArgs(new KeysSubscription(Keys.F, false, true, true)));
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (rdpClient.IsEventPumpActive)
                {
                    rdpClient.StopEventPump();
                    pauseToolStripMenuItem.Text = Resources.cmResume;
                }
                else
                {
                    pauseToolStripMenuItem.Text = Resources.cmPause;
                    rdpClient.StartEventPump();
                }
            }
            catch (Exception ex)
            {
                if (LOGGER.IsErrorEnabled) LOGGER.Error(ex.Message, ex);
            }
        }

        private void setQualityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (DesktopQualityForm form = new DesktopQualityForm())
            {
                if (form.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        rdpClient.SetDesktopQualityManually(form.QualityPercent);
                        rdpClient.RefreshDesktop();
                    }
                    catch (Exception ex)
                    {
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(ex.Message, ex);
                    }
                }
            }
        }

        private void refreshScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                rdpClient.RefreshDesktop();
            }
            catch (Exception ex)
            {
                if (LOGGER.IsErrorEnabled) LOGGER.Error(ex.Message, ex);
            }
        }

        private void sendAFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = string.Format("{0} (*.*)|*.*", Resources.Dialog_AllFiles);
                if (ofd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        using (ExtendedFileStream fs = new ExtendedFileStream(ofd.FileName))
                        {
                            using (FileSendProgressForm form = new FileSendProgressForm(new FileInfo(ofd.FileName).Name, fs, rdpClient))
                            {
                                form.ShowDialog(this);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("Cannot open file '{0}'", ofd.FileName), ex);
                        MessageBox.Show(this, string.Format(Resources.Error_CannotOpenFile, ex.Message), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void reconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reconnectToolStripMenuItem.Enabled = false;
            try
            {
                rdpClient.Connect(mLastChannelId, mLastAddressEp);
                if (this.Login(mLastUsername, mLastPassword) == LoginResponseStateEnum.AccessGranted)
                {
                    rdpClient.StartEventPump();
                }
                else
                {
                    rdpClient.Disconnect();
                    MessageBox.Show(this, Resources.Error_AuthFailed, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                rdpClient.Disconnect();
                MessageBox.Show(this, string.Format(Resources.Error_FailedToLogin, ex.Message), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rdpClient.Disconnect();
        }

        #endregion

    }

}
