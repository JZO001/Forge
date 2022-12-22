/* *********************************************************************
 * Date: 11 Sep 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Windows.Forms;
using Forge.RemoteDesktop.Client.Properties;
using Forge.RemoteDesktop.Contracts;

namespace Forge.RemoteDesktop.Client
{

    /// <summary>
    /// Implementation of the authentication form
    /// </summary>
    public partial class AuthenticationForm : Form
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationForm"/> class.
        /// </summary>
        public AuthenticationForm()
        {
            InitializeComponent();
            Text = Resources.Authentication_Title;
            Text = Resources.lUserName;
            Text = Resources.lPassword;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationForm"/> class.
        /// </summary>
        /// <param name="authMode">The authentication mode.</param>
        public AuthenticationForm(AuthenticationModeEnum authMode) : this()
        {
            if (authMode != AuthenticationModeEnum.UsernameAndPassword)
            {
                Enabled = false;
            }
        }

        /// <summary>
        /// Gets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username
        {
            get
            {
                return Text;
            }
        }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password
        {
            get
            {
                return Text;
            }
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

    }

}
