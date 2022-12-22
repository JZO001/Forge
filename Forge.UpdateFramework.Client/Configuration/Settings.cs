/* *********************************************************************
 * Date: 12 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Forge.Configuration;
using Forge.Configuration.Shared;
using Forge.Legacy;
using Forge.Reflection;
using Forge.Shared;
using Forge.UpdateFramework.ConfigSection;

namespace Forge.UpdateFramework.Client.Configuration
{

    /// <summary>
    /// Updater client settings
    /// </summary>
    [Serializable]
    public sealed class Settings : MBRBase, ICloneable
    {

        #region Field(s)

        private static readonly string PRODUCT_ID = "ProductId";
        private static readonly string DOWNLOAD_FOLDER = "DownloadFolder";
        private static readonly string COLLECTORS = "Collectors";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mProductId = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mDownloadFolder = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Updates").ToLower();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<IDataCollector> mDataCollectors = new List<IDataCollector>();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        internal Settings()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            if (asm != null && asm.EntryPoint != null)
            {
                object[] attribs = asm.GetCustomAttributes(typeof(GuidAttribute), true);
                if (attribs.Length > 0)
                {
                    mProductId = (attribs[0] as GuidAttribute).Value;
                }
            }

            ConfigurationAccessHelper.ParseStringValue(UpdateFrameworkConfiguration.Settings.CategoryPropertyItems, PRODUCT_ID, ref mProductId);
            ConfigurationAccessHelper.ParseStringValue(UpdateFrameworkConfiguration.Settings.CategoryPropertyItems, DOWNLOAD_FOLDER, ref mDownloadFolder);

            if (!PathHelper.IsAbsolutePath(mDownloadFolder))
            {
                mDownloadFolder = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, mDownloadFolder).ToLower();
            }
            else
            {
                mDownloadFolder = PathHelper.CutoffBackslashFromPathEnd(new DirectoryInfo(mDownloadFolder).FullName.ToLower());
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the product id.
        /// </summary>
        [DebuggerHidden]
        public string ProductId
        {
            get { return mProductId; }
        }

        /// <summary>
        /// Gets or sets the download folder.
        /// </summary>
        /// <value>
        /// The download folder.
        /// </value>
        [DebuggerHidden]
        public string DownloadFolder
        {
            get { return mDownloadFolder; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }
                if (PathHelper.IsAbsolutePath(value))
                {
                    this.mDownloadFolder = PathHelper.CutoffBackslashFromPathEnd(value.ToLower().Trim());
                }
                else
                {
                    this.mDownloadFolder = PathHelper.CutoffBackslashFromPathEnd(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, value).ToLower());
                }
            }
        }

        /// <summary>
        /// Gets the collectors.
        /// </summary>
        public List<IDataCollector> Collectors
        {
            get { return Updater.Instance.IsInitialized ? new List<IDataCollector>(mDataCollectors) : mDataCollectors; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            Settings clone = new Settings();
            clone.mProductId = this.mProductId;
            clone.mDownloadFolder = this.mDownloadFolder;
            clone.mDataCollectors.AddRange(this.mDataCollectors);
            return clone;
        }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        internal void Initialize()
        {
            IPropertyItem piCollectors = ConfigurationAccessHelper.GetPropertyByPath(UpdateFrameworkConfiguration.Settings.CategoryPropertyItems, COLLECTORS);
            if (piCollectors != null)
            {
                foreach (IPropertyItem piCol in piCollectors.Items.Values)
                {
                    Type collectorType = TypeHelper.GetTypeFromString(piCol.Id);
                    IDataCollector collector = (IDataCollector)collectorType.GetConstructor(Type.EmptyTypes).Invoke(null);
                    collector.Initialize(piCol);
                    mDataCollectors.Add(collector);
                }
            }
        }

        #endregion

    }

}
