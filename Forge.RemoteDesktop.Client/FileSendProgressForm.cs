/* *********************************************************************
 * Date: 12 Sep 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Threading;
using System.Windows.Forms;
using Forge.RemoteDesktop.Client.Properties;
using log4net;

namespace Forge.RemoteDesktop.Client
{

    internal delegate void ShowExceptionMessageDelegate(Exception ex);

    /// <summary>
    /// File Send Progress Indicator Form
    /// </summary>
    public partial class FileSendProgressForm : Form
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(FileSendProgressForm));

        private readonly string mFileName = string.Empty;

        private readonly ExtendedFileStream mStream = null;

        private readonly RemoteDesktopWinFormsControl mService = null;

        private Thread mThread = null;

        private string mNumberPattern = "### ### ### ##0";

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSendProgressForm"/> class.
        /// </summary>
        public FileSendProgressForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSendProgressForm" /> class.
        /// </summary>
        /// <param name="fileNameWithoutPath">The file name without path.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="service">The service.</param>
        public FileSendProgressForm(string fileNameWithoutPath, ExtendedFileStream stream, RemoteDesktopWinFormsControl service)
            : this()
        {
            if (string.IsNullOrEmpty(fileNameWithoutPath))
            {
                ThrowHelper.ThrowArgumentNullException("fileNameWithoutPath");
            }
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }
            if (service == null)
            {
                ThrowHelper.ThrowArgumentNullException("service");
            }

            mFileName = fileNameWithoutPath;
            mStream = stream;
            mService = service;

            lTotalSizeValue.Text = mStream.Length.ToString(mNumberPattern);
            lSentBytesValue.Text = mStream.Position.ToString(mNumberPattern);
            lPercentValue.Text = Convert.ToInt32(mStream.Position / (mStream.Length / 100)).ToString(mNumberPattern);
        }

        #endregion

        #region Private method(s)

        private void FileSendProgressForm_Shown(object sender, EventArgs e)
        {
            mStream.EventFileRead += new EventHandler<FileStreamProgressEventArgs>(Stream_EventFileRead);
            mThread = new Thread(new ThreadStart(SenderThreadMain));
            mThread.IsBackground = true;
            mThread.Name = string.Format("RemoteDesktopFileSender_{0}", mService.SessionId);
            mThread.Start();
        }

        private void Stream_EventFileRead(object sender, FileStreamProgressEventArgs e)
        {
            if (this.InvokeRequired)
            {
                EventHandler<FileStreamProgressEventArgs> d = new EventHandler<FileStreamProgressEventArgs>(Stream_EventFileRead);
                ((FileSendProgressForm)d.Target).Invoke(d, sender, e);
                return;
            }

            lTotalSizeValue.Text = e.Length.ToString(mNumberPattern);
            lSentBytesValue.Text = e.Position.ToString(mNumberPattern);
            lPercentValue.Text = e.Percent.ToString("##0");
            pbFileUpload.Value = Convert.ToInt32(e.Percent);
        }

        private void SenderThreadMain()
        {
            try
            {
                mService.StopEventPump();
                mService.SendFile(mFileName, mStream);
                mService.StartEventPump();
            }
            catch (Exception ex)
            {
                if (LOGGER.IsErrorEnabled) LOGGER.Error(mFileName, ex);
                try
                {
                    ShowExceptionMessageDelegate sd = new ShowExceptionMessageDelegate(ShowException);
                    ((FileSendProgressForm)sd.Target).Invoke(sd, ex);
                }
                catch (Exception) { }
            }

            try
            {
                Action d = new Action(this.Close);
                ((FileSendProgressForm)d.Target).Invoke(d);
            }
            catch (Exception) { }
        }

        private void ShowException(Exception ex)
        {
            Exception exc = ex.InnerException == null ? ex : ex.InnerException;
            MessageBox.Show(this, string.Format(Resources.Error_FailedToSendFile, exc.Message), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, Resources.Dialog_AbortFileSend, Resources.Confirmation, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                mService.Disconnect();
                this.Close();
            }
        }

        private void FileSendProgressForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mStream.EventFileRead -= new EventHandler<FileStreamProgressEventArgs>(Stream_EventFileRead);
        }

        #endregion

    }

}
