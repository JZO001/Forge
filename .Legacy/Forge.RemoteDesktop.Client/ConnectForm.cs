/* *********************************************************************
 * Date: 4 Sep 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Windows.Forms;
using Forge.Net.Services.Locators;
using Forge.RemoteDesktop.Contracts;

namespace Forge.RemoteDesktop.Client
{

    /// <summary>
    /// Represents a base implementation for connection form
    /// </summary>
    public partial class ConnectForm : Form
    {

        #region Constructor(s)
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectForm"/> class.
        /// </summary>
        public ConnectForm()
        {
            InitializeComponent();
            connectControl.ShowCancelButton = false;
        }

        #endregion

        #region Public properties
        
        /// <summary>
        /// Gets the selected provider.
        /// </summary>
        /// <value>
        /// The selected provider.
        /// </value>
        public ServiceProvider SelectedProvider { get { return connectControl.SelectedProvider; } }

        /// <summary>
        /// Gets the locator.
        /// </summary>
        /// <value>
        /// The locator.
        /// </value>
        public IRemoteServiceLocator<IRemoteDesktop> Locator
        {
            get { return connectControl.Locator; }
        } 

        #endregion

        #region Private method(s)

        private void ConnectForm_Shown(object sender, EventArgs e)
        {
            connectControl.EventConnect += new EventHandler<EventArgs>(ConnectEventHandler);
        }

        private void ConnectEventHandler(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        #endregion

    }

}
