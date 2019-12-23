/* *********************************************************************
 * Date: 25 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

#if NETCOREAPP3_1
#else

using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using Forge.Configuration.Shared;
using Forge.Logging;
using log4net.Appender;
using log4net.Layout;

namespace Forge.ErrorReport.Sink
{

    /// <summary>
    /// Represents an SMTP sender sink
    /// </summary>
    [Serializable]
    public class SmtpSink : SinkBase
    {

        #region Field(s)

        private readonly ILog LOGGER = null;

        /// <summary>
        /// The default layout conversion pattern
        /// </summary>
        protected const string DEFAULT_LAYOUT_PATTERN = "%d %-5p [%t] [%c] - %m%n";

        /// <summary>
        /// The configuration authentication
        /// </summary>
        protected const string CONFIG_AUTHENTICATION = "Authentication";

        /// <summary>
        /// The configuration BCC
        /// </summary>
        protected const string CONFIG_BCC = "Bcc";

        /// <summary>
        /// The configuration bodyencoding
        /// </summary>
        protected const string CONFIG_BODYENCODING = "BodyEncoding";

        /// <summary>
        /// The configuration cc
        /// </summary>
        protected const string CONFIG_CC = "Cc";

        /// <summary>
        /// The configuration enablessl
        /// </summary>
        protected const string CONFIG_ENABLESSL = "EnableSsl";

        /// <summary>
        /// The configuration from
        /// </summary>
        protected const string CONFIG_FROM = "From";

        /// <summary>
        /// The configuration password
        /// </summary>
        protected const string CONFIG_PASSWORD = "Password";

        /// <summary>
        /// The configuration port
        /// </summary>
        protected const string CONFIG_PORT = "Port";

        /// <summary>
        /// The configuration mailpriority
        /// </summary>
        protected const string CONFIG_MAILPRIORITY = "MailPriority";

        /// <summary>
        /// The configuration replyto
        /// </summary>
        protected const string CONFIG_REPLYTO = "ReplyTo";

        /// <summary>
        /// The configuration layout conversion pattern
        /// </summary>
        protected const string CONFIG_LAYOUT_PATTERN = "ConversionPattern";

        /// <summary>
        /// The configuration SMTP host
        /// </summary>
        protected const string CONFIG_SMTP_HOST = "SmtpHost";

        /// <summary>
        /// The configuration subject
        /// </summary>
        protected const string CONFIG_SUBJECT = "Subject";

        /// <summary>
        /// The configuration subject encoding
        /// </summary>
        protected const string CONFIG_SUBJECTENCODING = "SubjectEncoding";

        /// <summary>
        /// The configuration automatic
        /// </summary>
        protected const string CONFIG_TO = "To";

        /// <summary>
        /// The configuration username
        /// </summary>
        protected const string CONFIG_USERNAME = "Username";

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpSink"/> class.
        /// </summary>
        public SmtpSink()
        {
            this.LOGGER = LogManager.GetLogger(this.GetType());
            this.Authentication = SmtpAppender.SmtpAuthentication.None;
            this.BodyEncoding = Encoding.Default;
            this.Port = 25;
            this.Priority = MailPriority.Normal;
            this.SubjectEncoding = Encoding.Default;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the authentication.
        /// </summary>
        /// <value>
        /// The authentication.
        /// </value>
        public SmtpAppender.SmtpAuthentication Authentication
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the BCC.
        /// </summary>
        /// <value>
        /// The BCC.
        /// </value>
        public string Bcc
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the body encoding.
        /// </summary>
        /// <value>
        /// The body encoding.
        /// </value>
        public Encoding BodyEncoding
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the cc.
        /// </summary>
        /// <value>
        /// The cc.
        /// </value>
        public string Cc
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable SSL].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable SSL]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableSsl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>
        /// From.
        /// </value>
        public string From
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public MailPriority Priority
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the reply automatic.
        /// </summary>
        /// <value>
        /// The reply automatic.
        /// </value>
        public string ReplyTo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the SMTP host.
        /// </summary>
        /// <value>
        /// The SMTP host.
        /// </value>
        public string SmtpHost
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        public string Subject
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the subject encoding.
        /// </summary>
        /// <value>
        /// The subject encoding.
        /// </value>
        public Encoding SubjectEncoding
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the automatic.
        /// </summary>
        /// <value>
        /// The automatic.
        /// </value>
        public string To
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the conversion pattern.
        /// </summary>
        /// <value>
        /// The conversion pattern.
        /// </value>
        public string ConversionPattern { get; set; }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public override void Initialize(CategoryPropertyItem item)
        {
            if (item == null)
            {
                ThrowHelper.ThrowArgumentNullException("item");
            }

            base.Initialize(item);

            SmtpAppender.SmtpAuthentication authMode = SmtpAppender.SmtpAuthentication.None;
            if (ConfigurationAccessHelper.ParseEnumValue<SmtpAppender.SmtpAuthentication>(item.PropertyItems, CONFIG_AUTHENTICATION, ref authMode))
            {
                this.Authentication = authMode;
            }

            string bcc = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item.PropertyItems, CONFIG_BCC, ref bcc))
            {
                this.Bcc = bcc;
            }

            #region Body Encoding

            string bodyEncoding = "Default";
            if (ConfigurationAccessHelper.ParseStringValue(item.PropertyItems, CONFIG_BODYENCODING, ref bodyEncoding))
            {
                switch (bodyEncoding)
                {
                    case "Default":
                        {
                            this.BodyEncoding = Encoding.Default;
                        }
                        break;

                    case "ASCII":
                        {
                            this.BodyEncoding = Encoding.ASCII;
                        }
                        break;

                    case "BigEndianUnicode":
                        {
                            this.BodyEncoding = Encoding.BigEndianUnicode;
                        }
                        break;

                    case "Unicode":
                        {
                            this.BodyEncoding = Encoding.Unicode;
                        }
                        break;

                    case "UTF32":
                        {
                            this.BodyEncoding = Encoding.UTF32;
                        }
                        break;

                    case "UTF7":
                        {
                            this.BodyEncoding = Encoding.UTF7;
                        }
                        break;

                    case "UTF8":
                        {
                            this.BodyEncoding = Encoding.UTF8;
                        }
                        break;

                    case "":
                    case null:
                        break;

                    default:
                        {
                            this.BodyEncoding = Encoding.GetEncoding(bodyEncoding);
                        }
                        break;
                }
            }

            #endregion

            string cc = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item.PropertyItems, CONFIG_CC, ref cc))
            {
                this.Cc = cc;
            }

            bool enableSsl = false;
            if (ConfigurationAccessHelper.ParseBooleanValue(item.PropertyItems, CONFIG_ENABLESSL, ref enableSsl))
            {
                this.EnableSsl = enableSsl;
            }

            string from = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item.PropertyItems, CONFIG_FROM, ref from))
            {
                this.From = from;
            }

            string password = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item.PropertyItems, CONFIG_PASSWORD, ref password))
            {
                this.Password = password;
            }

            int port = 25;
            if (ConfigurationAccessHelper.ParseIntValue(item.PropertyItems, CONFIG_PORT, 1, 65535, ref port))
            {
                this.Port = port;
            }

            MailPriority prio = MailPriority.Normal;
            if (ConfigurationAccessHelper.ParseEnumValue<MailPriority>(item.PropertyItems, CONFIG_MAILPRIORITY, ref prio))
            {
                this.Priority = prio;
            }

            string replyTo = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item.PropertyItems, CONFIG_REPLYTO, ref replyTo))
            {
                this.ReplyTo = replyTo;
            }

            string pattern = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item.PropertyItems, CONFIG_LAYOUT_PATTERN, ref pattern))
            {
                this.ConversionPattern = pattern;
            }
            else
            {
                this.ConversionPattern = DEFAULT_LAYOUT_PATTERN;
            }

            string smtpHost = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item.PropertyItems, CONFIG_SMTP_HOST, ref smtpHost))
            {
                this.SmtpHost = smtpHost;
            }

            string subject = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item.PropertyItems, CONFIG_SUBJECT, ref subject))
            {
                this.Subject = subject;
            }

            #region Subject Encoding

            string subjectEncoding = "Default";
            if (ConfigurationAccessHelper.ParseStringValue(item.PropertyItems, CONFIG_SUBJECTENCODING, ref subjectEncoding))
            {
                switch (bodyEncoding)
                {
                    case "Default":
                        {
                            this.SubjectEncoding = Encoding.Default;
                        }
                        break;

                    case "ASCII":
                        {
                            this.SubjectEncoding = Encoding.ASCII;
                        }
                        break;

                    case "BigEndianUnicode":
                        {
                            this.SubjectEncoding = Encoding.BigEndianUnicode;
                        }
                        break;

                    case "Unicode":
                        {
                            this.SubjectEncoding = Encoding.Unicode;
                        }
                        break;

                    case "UTF32":
                        {
                            this.SubjectEncoding = Encoding.UTF32;
                        }
                        break;

                    case "UTF7":
                        {
                            this.SubjectEncoding = Encoding.UTF7;
                        }
                        break;

                    case "UTF8":
                        {
                            this.SubjectEncoding = Encoding.UTF8;
                        }
                        break;

                    case "":
                    case null:
                        break;

                    default:
                        {
                            this.SubjectEncoding = Encoding.GetEncoding(bodyEncoding);
                        }
                        break;
                }
            }

            #endregion

            string to = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item.PropertyItems, CONFIG_TO, ref to))
            {
                this.To = to;
            }

            string username = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item.PropertyItems, CONFIG_USERNAME, ref username))
            {
                this.Username = username;
            }

            LOGGER.Info(string.Format("{0}, Authentication: {1}, Bcc: {2}, BodyEncoding: {3}, Cc: {4}, EnableSSL: {5}, From: {6}, Password: {7}, Port: {8}, Priority: {9}, ReplyTo: {10}, SmtpHost: {11}, SubjectEncoding: {12}, To: {13}, Username: {14}",
                this.GetType().Name, this.Authentication.ToString(), this.Bcc, this.BodyEncoding.EncodingName, this.Cc,
                this.EnableSsl.ToString(), this.From, string.IsNullOrEmpty(this.Password) ? "not set" : "exist", this.Port.ToString(), this.Priority.ToString(), this.ReplyTo, this.SmtpHost, this.SubjectEncoding.EncodingName, this.To, this.Username));

            this.IsInitialized = true;
        }

        /// <summary>
        /// Processes the report package.
        /// </summary>
        /// <param name="package">The package.</param>
        public override void ProcessReportPackage(ReportPackage package)
        {
            DoInitializationCheck();
            LOGGER.Debug(string.Format("{0}, an error report package arrived. SinkId: '{0}'", this.GetType().Name, this.SinkId));

            PatternLayout layout = new PatternLayout(this.ConversionPattern);
            StringBuilder sb = new StringBuilder();
            sb.Append("Created (UTC): ");
            sb.AppendLine(package.ReportCreated.ToString());
            sb.Append("ApplicationId: ");
            sb.AppendLine(package.ApplicationId);
            sb.AppendLine();
            sb.AppendLine(layout.Format(package.LoggingEvent));

            using (SmtpClient client = new SmtpClient())
            {
                if (!string.IsNullOrEmpty(this.SmtpHost))
                {
                    client.Host = this.SmtpHost;
                }
                client.Port = this.Port;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = this.EnableSsl;

                if (this.Authentication == SmtpAppender.SmtpAuthentication.Basic)
                {
                    client.Credentials = new NetworkCredential(this.Username, this.Password);
                }
                else if (this.Authentication == SmtpAppender.SmtpAuthentication.Ntlm)
                {
                    client.Credentials = CredentialCache.DefaultNetworkCredentials;
                }

                using (MailMessage message = new MailMessage())
                {
                    message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    message.IsBodyHtml = false;
                    message.Body = sb.ToString();
                    message.BodyEncoding = this.BodyEncoding;
                    message.From = new MailAddress(this.From);
                    message.To.Add(this.To);
                    if (!string.IsNullOrEmpty(this.Cc))
                    {
                        message.CC.Add(this.Cc);
                    }
                    if (!string.IsNullOrEmpty(this.Bcc))
                    {
                        message.Bcc.Add(this.Bcc);
                    }
                    if (!string.IsNullOrEmpty(this.ReplyTo))
                    {
                        message.ReplyToList.Add(new MailAddress(this.ReplyTo));
                    }
                    message.Subject = this.Subject;
                    message.SubjectEncoding = this.SubjectEncoding;
                    message.Priority = this.Priority;
                    client.Send(message);
                }

            }
        }

        #endregion

    }

}

#endif
