/* *********************************************************************
 * Date: 11 Sep 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Windows.Forms;
using Forge.RemoteDesktop.Client.Properties;

namespace Forge.RemoteDesktop.Client
{

    /// <summary>
    /// Get desktop image quality from user
    /// </summary>
    public partial class DesktopQualityForm : Form
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopQualityForm"/> class.
        /// </summary>
        public DesktopQualityForm()
        {
            InitializeComponent();

            this.Text = Resources.lDesktopQuality;
            lInfo.Text = Resources.lDesktopQuality;
            cbAuto.Text = Resources.cbAuto;
            btOk.Text = Resources.Button_OK;
            btCancel.Text = Resources.Button_Cancel;
        }

        /// <summary>
        /// Gets the quality percent.
        /// </summary>
        /// <value>
        /// The quality percent.
        /// </value>
        public int QualityPercent
        {
            get
            {
                return cbAuto.Checked ? -1 : Convert.ToInt32(nudPercent.Value);
            }
        }

        private void cbAuto_CheckedChanged(object sender, EventArgs e)
        {
            nudPercent.Enabled = !cbAuto.Checked;
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

    }

}
