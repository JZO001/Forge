/* *********************************************************************
 * Date: 9 Sep 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Windows.Forms;
using Forge.RemoteDesktop.Contracts;
using Forge.RemoteDesktop.Service.Properties;

namespace Forge.RemoteDesktop.Service
{

    /// <summary>
    /// Represents the connected clients
    /// </summary>
    public partial class ConnectedClientsControl : UserControl
    {

        #region Constructor(s)
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectedClientsControl"/> class.
        /// </summary>
        public ConnectedClientsControl()
        {
            InitializeComponent();

            lConnectedClients.Text = Resources.lConnectedClients;
            chClientId1.Text = Resources.Column_ClientId;
            chClientId2.Text = Resources.Column_ClientId;
            chSessionId1.Text = Resources.Column_SessionId;
            chSessionId2.Text = Resources.Column_SessionId;
            btDisconnectActiveClient.Text = Resources.Button_Disconnect;
            btDisconnectConnectedClient.Text = Resources.Button_Disconnect;

        } 

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets a value indicating whether [is initialized].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is initialized]; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized { get; protected set; }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public virtual void Initialize()
        {
            if (!IsInitialized)
            {
                RemoteDesktopServiceManager.Instance.EventConnectionStateChange += new EventHandler<ConnectionStateChangedEventArgs>(RemoteDesktopServiceManager_EventConnectionStateChange);
                RemoteDesktopServiceManager.Instance.EventAcceptClient += new EventHandler<AcceptClientEventArgs>(RemoteDesktopServiceManager_EventAcceptClient);
                IsInitialized = true;
            }
        } 

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Handles the SelectedIndexChanged event of the lvConnectedClients control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void lvConnectedClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvConnectedClients.SelectedIndices.Count == 0)
            {
                btDisconnectConnectedClient.Enabled = false;
            }
            else
            {
                btDisconnectConnectedClient.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the lvActiveClients control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void lvActiveClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvActiveClients.SelectedIndices.Count == 0)
            {
                btDisconnectActiveClient.Enabled = false;
            }
            else
            {
                btDisconnectActiveClient.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the EventConnectionStateChange event of the RemoteDesktopServiceManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ConnectionStateChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void RemoteDesktopServiceManager_EventConnectionStateChange(object sender, ConnectionStateChangedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    EventHandler<ConnectionStateChangedEventArgs> d = new EventHandler<ConnectionStateChangedEventArgs>(RemoteDesktopServiceManager_EventConnectionStateChange);
                    ((ConnectedClientsControl)d.Target).Invoke(d, sender, e);
                }
                catch (Exception) { }
                return;
            }

            if (e.IsConnected)
            {
                ListViewItem item = new ListViewItem(e.Client.RemoteHost);
                item.Tag = e.Client;

                ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem(item, e.Client.SessionId);
                item.SubItems.Add(subItem);

                lvConnectedClients.Items.Add(item);
                lvConnectedClients.Sort();
            }
            else
            {
                foreach (ListViewItem item in lvConnectedClients.Items)
                {
                    if (item.Tag.Equals(e.Client))
                    {
                        if (item.Selected)
                        {
                            btDisconnectConnectedClient.Enabled = false;
                        }
                        item.Remove();
                        break;
                    }
                }
                foreach (ListViewItem item in lvActiveClients.Items)
                {
                    if (item.Tag.Equals(e.Client))
                    {
                        if (item.Selected)
                        {
                            btDisconnectActiveClient.Enabled = false;
                        }
                        item.Remove();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the EventAcceptClient event of the RemoteDesktopServiceManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="AcceptClientEventArgs"/> instance containing the event data.</param>
        protected virtual void RemoteDesktopServiceManager_EventAcceptClient(object sender, AcceptClientEventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    EventHandler<AcceptClientEventArgs> d = new EventHandler<AcceptClientEventArgs>(RemoteDesktopServiceManager_EventAcceptClient);
                    ((ConnectedClientsControl)d.Target).Invoke(d, sender, e);
                }
                catch (Exception)
                {
                }
                return;
            }

            ListViewItem item = new ListViewItem(e.Client.RemoteHost);
            item.Tag = e.Client;

            ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem(item, e.Client.SessionId);
            item.SubItems.Add(subItem);

            lvActiveClients.Items.Add(item);
            lvActiveClients.Sort();
        }

        /// <summary>
        /// Handles the Click event of the btDisconnectConnectedClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void btDisconnectConnectedClient_Click(object sender, EventArgs e)
        {
            IRemoteDesktopPeer peer = lvConnectedClients.SelectedItems[0].Tag as IRemoteDesktopPeer;
            RemoteDesktopServiceManager.Instance.Disconnect(peer);
        }

        /// <summary>
        /// Handles the Click event of the btDisconnectActiveClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void btDisconnectActiveClient_Click(object sender, EventArgs e)
        {
            IRemoteDesktopPeer peer = lvActiveClients.SelectedItems[0].Tag as IRemoteDesktopPeer;
            RemoteDesktopServiceManager.Instance.Disconnect(peer);
        } 

        #endregion

    }

}
