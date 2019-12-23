/* *********************************************************************
 * Date: 08 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Forge.Configuration.Shared;
using Forge.Logging;
using Forge.Net.TerraGraf.ConfigSection;

namespace Forge.Net.TerraGraf.Configuration
{

    /// <summary>
    /// Represent the settings of the network contexts
    /// </summary>
    [Serializable]
    public sealed class NetworkContextSettings
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(NetworkContextSettings));

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mName = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mSeparation = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<ContextRule> mWhiteList = new List<ContextRule>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<ContextRule> mBlackList = new List<ContextRule>();

        //private bool mInitialized = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkContextSettings"/> class.
        /// </summary>
        internal NetworkContextSettings()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DebuggerHidden]
        public string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NetworkContextSettings"/> is separation.
        /// </summary>
        /// <value>
        ///   <c>true</c> if separation; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool Separation
        {
            get { return mSeparation; }
            internal set { mSeparation = value; }
        }

        /// <summary>
        /// Gets or sets the white list.
        /// </summary>
        /// <value>
        /// The white list.
        /// </value>
        public List<ContextRule> WhiteList
        {
            get { return new List<ContextRule>(mWhiteList); }
            internal set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }

                mWhiteList.Clear();
                mWhiteList.AddRange(value);
            }
        }

        /// <summary>
        /// Gets or sets the black list.
        /// </summary>
        /// <value>
        /// The black list.
        /// </value>
        public List<ContextRule> BlackList
        {
            get { return new List<ContextRule>(mBlackList); }
            internal set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }

                mBlackList.Clear();
                mBlackList.AddRange(value);
            }
        }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Logs the configuration.
        /// </summary>
        internal void LogConfiguration()
        {
            if (LOGGER.IsInfoEnabled)
            {
                LOGGER.Info(string.Format("TERRAGRAF, Network Context Name: {0}", this.mName));
                LOGGER.Info(string.Format("TERRAGRAF, Separation: {0}", this.mSeparation));

                StringBuilder sb = new StringBuilder();
                foreach (ContextRule ep in mWhiteList)
                {
                    sb.AppendLine(ep.ToString());
                }
                LOGGER.Info(string.Format("TERRAGRAF, White list definition(s): {0}{1}", Environment.NewLine, sb.ToString()));

                sb.Clear();
                foreach (ContextRule ep in mBlackList)
                {
                    sb.AppendLine(ep.ToString());
                }
                LOGGER.Info(string.Format("TERRAGRAF, Black list definition(s): {0}{1}", Environment.NewLine, sb.ToString()));
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        internal void Initialize()
        {
            SectionHandler_OnConfigurationChanged(null, null);
            //this.mInitialized = true;
        }

        #endregion

        #region Private method(s)

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Boolean.TryParse(System.String,System.Boolean@)")]
        private void SectionHandler_OnConfigurationChanged(object sender, EventArgs e)
        {
            string value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "NetworkContext");
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(value.Trim()))
            {
                throw new InitializationException("Network context name not configured.");
            }
            this.mName = value.Trim();

            value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "NetworkContext/Separation");
            bool boolValue = false;
            if (!string.IsNullOrEmpty(value))
            {
                bool.TryParse(value, out boolValue);
            }
            Separation = boolValue;

            CategoryPropertyItem piRoot = ConfigurationAccessHelper.GetCategoryPropertyByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "NetworkContext/Separation/WhiteList");
            List<ContextRule> list = new List<ContextRule>();
            if (piRoot != null)
            {
                foreach (CategoryPropertyItem pi in piRoot)
                {
                    if (string.IsNullOrEmpty(pi.EntryName) || string.IsNullOrEmpty(pi.EntryName.Trim()))
                    {
                        throw new InitializationException("Invalid context rule.");
                    }
                    if (string.IsNullOrEmpty(pi.EntryValue) || string.IsNullOrEmpty(pi.EntryValue.Trim()))
                    {
                        throw new InitializationException("Invalid context rule.");
                    }
                    list.Add(new ContextRule(pi.EntryName, pi.EntryValue));
                }
            }
            WhiteList = list;

            piRoot = ConfigurationAccessHelper.GetCategoryPropertyByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "NetworkContext/Separation/BlackList");
            list = new List<ContextRule>();
            if (piRoot != null)
            {
                foreach (CategoryPropertyItem pi in piRoot)
                {
                    if (string.IsNullOrEmpty(pi.EntryName) || string.IsNullOrEmpty(pi.EntryName.Trim()))
                    {
                        throw new InitializationException("Invalid context rule.");
                    }
                    if (string.IsNullOrEmpty(pi.EntryValue) || string.IsNullOrEmpty(pi.EntryValue.Trim()))
                    {
                        throw new InitializationException("Invalid context rule.");
                    }
                    list.Add(new ContextRule(pi.EntryName, pi.EntryValue));
                }
            }
            BlackList = list;
        }

        #endregion

    }

}
