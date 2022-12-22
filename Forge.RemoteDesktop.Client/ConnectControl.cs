/* *********************************************************************
 * Date: 19 Sep 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Forge.Collections;
using Forge.Invoker;
using Forge.Net.Remoting.Proxy;
using Forge.Net.Services.Locators;
using Forge.Net.TerraGraf;
using Forge.RemoteDesktop.Client.Properties;
using Forge.RemoteDesktop.Contracts;

namespace Forge.RemoteDesktop.Client
{

    /// <summary>
    /// Implementation of the connection control
    /// </summary>
    public partial class ConnectControl : UserControl
    {

        #region Field(s)

        private IRemoteServiceLocator<IRemoteDesktop> mLocator = null;

        /// <summary>
        /// The event connect
        /// </summary>
        public EventHandler<EventArgs> EventConnect;

        /// <summary>
        /// The event cancel
        /// </summary>
        public EventHandler<EventArgs> EventCancel;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectControl"/> class.
        /// </summary>
        public ConnectControl()
        {
            InitializeComponent();
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

        /// <summary>
        /// Gets the selected provider.
        /// </summary>
        /// <value>
        /// The selected provider.
        /// </value>
        public ServiceProvider SelectedProvider { get; protected set; }

        /// <summary>
        /// Gets the locator.
        /// </summary>
        /// <value>
        /// The locator.
        /// </value>
        public IRemoteServiceLocator<IRemoteDesktop> Locator
        {
            get { return mLocator; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show cancel button].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show cancel button]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowCancelButton
        {
            get { return btCancel.Visible; }
            set { btCancel.Visible = value; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public virtual void Initialize()
        {
            if (!IsInitialized)
            {
                try
                {
                    Net.TerraGraf.NetworkManager.Instance.Start();
                    ProxyServices.Initialize();

                    mLocator = RemoteServiceLocatorManager.GetServiceLocator<IRemoteDesktop, RemoteDesktopServiceLocator>();
                    mLocator.EventAvailableServiceProvidersChanged += new EventHandler<ServiceProvidersChangedEventArgs>(AvailableServiceProvidersChangedEventHandler);
                    mLocator.Start();
                    AvailableServiceProvidersChangedEventHandler(mLocator, new ServiceProvidersChangedEventArgs(mLocator.AvailableServiceProviders));
                    IsInitialized = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, string.Format(Resources.Error_FailedToStartNetwork, ex.Message), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Availables the service providers changed event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ServiceProvidersChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void AvailableServiceProvidersChangedEventHandler(object sender, ServiceProvidersChangedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                EventHandler<ServiceProvidersChangedEventArgs> d = new EventHandler<ServiceProvidersChangedEventArgs>(AvailableServiceProvidersChangedEventHandler);
                ((ConnectControl)d.Target).Invoke(d, sender, e);
                return;
            }

            ListSpecialized<ServiceProvider> providers = e.ServiceProviders;

            List<ListViewItem> removableItems = new List<ListViewItem>();
            // kidobom azokat az elemeket a listviewból, amelyek már nincsennek
            foreach (ListViewItem item in lvServices.Items)
            {
                ServiceProvider savedProvider = item.Tag as ServiceProvider;
                if (!providers.Contains(savedProvider))
                {
                    removableItems.Add(item);
                }
            }
            removableItems.ForEach(i => i.Remove());
            removableItems.Clear();

            // felveszem azokat, akik még nincsennek a listában
            IEnumeratorSpecialized<ServiceProvider> spEnum = providers.GetEnumerator();
            while (spEnum.MoveNext())
            {
                ServiceProvider provider = spEnum.Current;
                if (!NetworkManager.Instance.Localhost.Equals(provider.NetworkPeer))
                {
                    bool found = false;
                    foreach (ListViewItem item in lvServices.Items)
                    {
                        ServiceProvider savedProvider = item.Tag as ServiceProvider;
                        if (savedProvider.Equals(provider))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        ListViewItem item = new ListViewItem(provider.NetworkPeer.Id);
                        item.Tag = provider;
                        lvServices.Items.Add(item);
                    }
                }
            }
            lvServices.Sort();
            lvServices_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// Handles the Click event of the btConnect control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void btConnect_Click(object sender, EventArgs e)
        {
            this.SelectedProvider = lvServices.SelectedItems[0].Tag as ServiceProvider;
            Executor.Invoke(EventConnect, this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the Click event of the btCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void btCancel_Click(object sender, EventArgs e)
        {
            Executor.Invoke(EventCancel, this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the lvServices control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void lvServices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvServices.SelectedIndices.Count == 0)
            {
                btConnect.Enabled = false;
            }
            else
            {
                btConnect.Enabled = true;
            }
        }

        #endregion

    }

}
