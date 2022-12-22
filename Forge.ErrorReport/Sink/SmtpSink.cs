/* *********************************************************************
 * Date: 25 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

#if NET461

using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using Forge.Configuration;
using Forge.Configuration.Shared;
using Forge.Logging;
using Forge.Logging.Abstraction;
using Forge.Shared;
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
            LOGGER = LogManager.GetLogger<SmtpSink>();
            Authentication = SmtpAppender.SmtpAuthentication.None;
            BodyEncoding = Encoding.Default;
            Port = 25;
            Priority = MailPriority.Normal;
            SubjectEncoding = Encoding.Default;
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
        public override void Initialize(IPropertyItem item)
        {
            if (item == null)
            {
                ThrowHelper.ThrowArgumentNullException("item");
            }

            base.Initialize(item);

            SmtpAppender.SmtpAuthentication authMode = SmtpAppender.SmtpAuthentication.None;
            if (ConfigurationAccessHelper.ParseEnumValue<SmtpAppender.SmtpAuthentication>(item, CONFIG_AUTHENTICATION, ref authMode))
            {
                Authentication = authMode;
            }

            string bcc = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item, CONFIG_BCC, ref bcc))
            {
                Bcc = bcc;
            }

#region Body Encoding

            string bodyEncoding = "Default";
            if (ConfigurationAccessHelper.ParseStringValue(item, CONFIG_BODYENCODING, ref bodyEncoding))
            {
                switch (bodyEncoding)
                {
                    case "Default":
                        {
                            BodyEncoding = Encoding.Default;
                        }
                        break;

                    case "ASCII":
                        {
                            BodyEncoding = Encoding.ASCII;
                        }
                        break;

                    case "BigEndianUnicode":
                        {
                            BodyEncoding = Encoding.BigEndianUnicode;
                        }
                        break;

                    case "Unicode":
                        {
                            BodyEncoding = Encoding.Unicode;
                        }
                        break;

                    case "UTF32":
                        {
                            BodyEncoding = Encoding.UTF32;
                        }
                        break;

                    case "UTF7":
                        {
                            BodyEncoding = Encoding.UTF7;
                        }
                        break;

                    case "UTF8":
                        {
                            BodyEncoding = Encoding.UTF8;
                        }
                        break;

                    case "":
                    case null:
                        break;

                    default:
                        {
                            BodyEncoding = Encoding.GetEncoding(bodyEncoding);
                        }
                        break;
                }
            }

#endregion

            string cc = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item, CONFIG_CC, ref cc))
            {
                Cc = cc;
            }

            bool enableSsl = false;
            if (ConfigurationAccessHelper.ParseBooleanValue(item, CONFIG_ENABLESSL, ref enableSsl))
            {
                EnableSsl = enableSsl;
            }

            string from = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item, CONFIG_FROM, ref from))
            {
                From = from;
            }

            string password = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item, CONFIG_PASSWORD, ref password))
            {
                Password = password;
            }

            int port = 25;
            if (ConfigurationAccessHelper.ParseIntValue(item, CONFIG_PORT, 1, 65535, ref port))
            {
                Port = port;
            }

            MailPriority prio = MailPriority.Normal;
            if (ConfigurationAccessHelper.ParseEnumValue<MailPriority>(item, CONFIG_MAILPRIORITY, ref prio))
            {
                Priority = prio;
            }

            string replyTo = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item, CONFIG_REPLYTO, ref replyTo))
            {
                ReplyTo = replyTo;
            }

            string pattern = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item, CONFIG_LAYOUT_PATTERN, ref pattern))
            {
                ConversionPattern = pattern;
            }
            else
            {
                ConversionPattern = DEFAULT_LAYOUT_PATTERN;
            }

            string smtpHost = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item, CONFIG_SMTP_HOST, ref smtpHost))
            {
                SmtpHost = smtpHost;
            }

            string subject = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item, CONFIG_SUBJECT, ref subject))
            {
                Subject = subject;
            }

#region Subject Encoding

            string subjectEncoding = "Default";
            if (ConfigurationAccessHelper.ParseStringValue(item, CONFIG_SUBJECTENCODING, ref subjectEncoding))
            {
                switch (bodyEncoding)
                {
                    case "Default":
                        {
                            SubjectEncoding = Encoding.Default;
                        }
                        break;

                    case "ASCII":
                        {
                            SubjectEncoding = Encoding.ASCII;
                        }
                        break;

                    case "BigEndianUnicode":
                        {
                            SubjectEncoding = Encoding.BigEndianUnicode;
                        }
                        break;

                    case "Unicode":
                        {
                            SubjectEncoding = Encoding.Unicode;
                        }
                        break;

                    case "UTF32":
                        {
                            SubjectEncoding = Encoding.UTF32;
                        }
                        break;

                    case "UTF7":
                        {
                            SubjectEncoding = Encoding.UTF7;
                        }
                        break;

                    case "UTF8":
                        {
                            SubjectEncoding = Encoding.UTF8;
                        }
                        break;

                    case "":
                    case null:
                        break;

                    default:
                        {
                            SubjectEncoding = Encoding.GetEncoding(bodyEncoding);
                        }
                        break;
                }
            }

#endregion

            string to = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item, CONFIG_TO, ref to))
            {
                To = to;
            }

            string username = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item, CONFIG_USERNAME, ref username))
            {
                Username = username;
            }

            LOGGER.Info(string.Format("{0}, Authentication: {1}, Bcc: {2}, BodyEncoding: {3}, Cc: {4}, EnableSSL: {5}, From: {6}, Password: {7}, Port: {8}, Priority: {9}, ReplyTo: {10}, SmtpHost: {11}, SubjectEncoding: {12}, To: {13}, Username: {14}",
                GetType().Name, Authentication.ToString(), Bcc, BodyEncoding.EncodingName, Cc,
                EnableSsl.ToString(), From, string.IsNullOrEmpty(Password) ? "not set" : "exist", Port.ToString(), Priority.ToString(), ReplyTo, SmtpHost, SubjectEncoding.EncodingName, To, Username));

            IsInitialized = true;
        }

        /// <summary>
        /// Processes the report package.
        /// </summary>
        /// <param name="package">The package.</param>
        public override void ProcessReportPackage(ReportPackage package)
        {
            DoInitializationCheck();
            LOGGER.Debug(string.Format("{0}, an error report package arrived. SinkId: '{0}'", GetType().Name, SinkId));

            PatternLayout layout = new PatternLayout(ConversionPattern);
            StringBuilder sb = new StringBuilder();
            sb.Append("Created (UTC): ");
            sb.AppendLine(package.ReportCreated.ToString());
            sb.Append("ApplicationId: ");
            sb.AppendLine(package.ApplicationId);
            sb.AppendLine();
            sb.AppendLine(layout.Format(package.LoggingEvent));

            using (SmtpClient client = new SmtpClient())
            {
                if (!string.IsNullOrEmpty(SmtpHost))
                {
                    client.Host = SmtpHost;
                }
                client.Port = Port;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = EnableSsl;

                if (Authentication == SmtpAppender.SmtpAuthentication.Basic)
                {
                    client.Credentials = new NetworkCredential(Username, Password);
                }
                else if (Authentication == SmtpAppender.SmtpAuthentication.Ntlm)
                {
                    client.Credentials = CredentialCache.DefaultNetworkCredentials;
                }

                using (MailMessage message = new MailMessage())
                {
                    message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    message.IsBodyHtml = false;
                    message.Body = sb.ToString();
                    message.BodyEncoding = BodyEncoding;
                    message.From = new MailAddress(From);
                    message.To.Add(To);
                    if (!string.IsNullOrEmpty(Cc))
                    {
                        message.CC.Add(Cc);
                    }
                    if (!string.IsNullOrEmpty(Bcc))
                    {
                        message.Bcc.Add(Bcc);
                    }
                    if (!string.IsNullOrEmpty(ReplyTo))
                    {
                        message.ReplyToList.Add(new MailAddress(ReplyTo));
                    }
                    message.Subject = Subject;
                    message.SubjectEncoding = SubjectEncoding;
                    message.Priority = Priority;
                    client.Send(message);
                }

            }
        }

#endregion

    }

}

#endif