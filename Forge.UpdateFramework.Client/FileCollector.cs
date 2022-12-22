/* *********************************************************************
 * Date: 16 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using Forge.Configuration;
using Forge.Configuration.Shared;
using Forge.Legacy;
using Forge.Logging.Abstraction;
using Forge.Reflection;
using Forge.Shared;
using Forge.UpdateFramework.Client.Configuration;

namespace Forge.UpdateFramework.Client
{

    /// <summary>
    /// Collects file data
    /// </summary>
    public class FileCollector : MBRBase, IDataCollector
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger<FileCollector>();

        private static readonly string CODEBASE_FOLDER = "CodeBaseFolder";
        private static readonly string CODEBASE_FOLDER_EXCLUSIONS = "CodeBaseFolderExclusions";
        private static readonly string EXTERNAL_FOLDERS = "ExternalFolders";
        private static readonly string EXTERNAL_FILES = "ExternalFiles";
        private static readonly string PERFORM_SEC_TEST = "PerformSecurityTestOnFolders";
        private static readonly string EXTENSIONS = "Extensions";

        private static readonly string JOKER_EXTENSION = "*";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mCodeBaseFolder = PathHelper.CutoffBackslashFromPathEnd(AppDomain.CurrentDomain.SetupInformation.ApplicationBase).ToLower();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly HashSet<DirectoryEntry> mFolderExclusions = new HashSet<DirectoryEntry>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly HashSet<DirectoryEntry> mExternalFolders = new HashSet<DirectoryEntry>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly HashSet<string> mExternalFiles = new HashSet<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mPerformSecurityTestOnFolders = true;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<string, Type> mExtensionVsDescriptor = new Dictionary<string, Type>();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCollector"/> class.
        /// </summary>
        public FileCollector()
        {
            mExtensionVsDescriptor["exe"] = typeof(AssemblyDescriptor);
            mExtensionVsDescriptor["dll"] = typeof(AssemblyDescriptor);
            mExtensionVsDescriptor["pfx"] = typeof(CertificateDescriptor);
            mExtensionVsDescriptor["snk"] = typeof(SimpleFileHashDescriptor);
            mExtensionVsDescriptor["cer"] = typeof(CertificateDescriptor);
            mExtensionVsDescriptor["config"] = typeof(AppConfigDescriptor);
            mExtensionVsDescriptor[JOKER_EXTENSION] = typeof(SimpleFileDescriptor);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets or sets the work folder.
        /// </summary>
        /// <value>
        /// The work folder.
        /// </value>
        [DebuggerHidden]
        public string CodeBaseFolder
        {
            get { return mCodeBaseFolder; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }
                if (PathHelper.IsAbsolutePath(value))
                {
                    mCodeBaseFolder = PathHelper.CutoffBackslashFromPathEnd(value.Trim().ToLower());
                }
                else
                {
                    mCodeBaseFolder = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, value.Trim().ToLower());
                }
            }
        }

        /// <summary>
        /// Gets or sets the code base folders exclude.
        /// </summary>
        /// <value>
        /// The code base folders exclude.
        /// </value>
        [DebuggerHidden]
        public HashSet<DirectoryEntry> FolderExclusions
        {
            get { return mFolderExclusions; }
            set
            {
                mFolderExclusions.Clear();

                if (value != null)
                {
                    foreach (DirectoryEntry path in value)
                    {
                        mFolderExclusions.Add(path);
                    }
                }

                mFolderExclusions.Add(new DirectoryEntry(Updater.Instance.Settings.DownloadFolder, true));
            }
        }

        /// <summary>
        /// Gets or sets the external folders.
        /// </summary>
        /// <value>
        /// The external folders.
        /// </value>
        [DebuggerHidden]
        public HashSet<DirectoryEntry> ExternalFolders
        {
            get { return mExternalFolders; }
            set
            {
                mExternalFolders.Clear();

                if (value != null)
                {
                    foreach (DirectoryEntry path in value)
                    {
                        mExternalFolders.Add(path);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the external files.
        /// </summary>
        /// <value>
        /// The external files.
        /// </value>
        [DebuggerHidden]
        public HashSet<string> ExternalFiles
        {
            get { return mExternalFiles; }
            set
            {
                if (value == null)
                {
                    mExternalFiles.Clear();
                }
                else
                {
                    foreach (string path in value)
                    {
                        if (!PathHelper.IsAbsolutePath(path))
                        {
                            ThrowHelper.ThrowArgumentException("One of the item in the collection is not an absolute path.", "value");
                        }
                    }

                    mExternalFiles.Clear();
                    foreach (string path in value)
                    {
                        mExternalFiles.Add(path);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [perform security test on folders].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [perform security test on folders]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool PerformSecurityTestOnFolders
        {
            get { return mPerformSecurityTestOnFolders; }
            set { mPerformSecurityTestOnFolders = value; }
        }

        /// <summary>
        /// Gets the extension vs descriptor.
        /// </summary>
        [DebuggerHidden]
        public Dictionary<string, Type> ExtensionVsDescriptor
        {
            get { return new Dictionary<string, Type>(mExtensionVsDescriptor); }
            set
            {
                if (value == null)
                {
                    mExtensionVsDescriptor.Clear();
                }
                else
                {
                    foreach (KeyValuePair<string, Type> kv in value)
                    {
                        if (!typeof(DescriptorBase).IsAssignableFrom(kv.Value))
                        {
                            ThrowHelper.ThrowArgumentException(string.Format("Provided type '{0}' is not assignable from '{1}' type. Key: {2}", kv.Value.AssemblyQualifiedName, typeof(DescriptorBase).FullName, kv.Key));
                        }
                    }
                    foreach (KeyValuePair<string, Type> kv in value)
                    {
                        mExtensionVsDescriptor.Add(kv.Key, kv.Value);
                    }
                }
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes the specified configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Initialize(IPropertyItem configuration)
        {
            if (configuration == null)
            {
                ThrowHelper.ThrowArgumentNullException("configuration");
            }

            if (!this.IsInitialized)
            {
                ConfigurationAccessHelper.ParseStringValue(configuration, CODEBASE_FOLDER, ref mCodeBaseFolder);
                if (!PathHelper.IsAbsolutePath(mCodeBaseFolder))
                {
                    throw new InitializationException(string.Format("Provided code base folder for file collector is not an absolute path: '{0}'", mCodeBaseFolder));
                }
                mCodeBaseFolder = PathHelper.CutoffBackslashFromPathEnd(new DirectoryInfo(mCodeBaseFolder).FullName.ToLower());

                IPropertyItem piExclusions = ConfigurationAccessHelper.GetPropertyByPath(configuration, CODEBASE_FOLDER_EXCLUSIONS);
                if (piExclusions != null)
                {
                    foreach (IPropertyItem pi in piExclusions.Items.Values)
                    {
                        mFolderExclusions.Add(new DirectoryEntry(mCodeBaseFolder, pi));
                    }
                }
                mFolderExclusions.Add(new DirectoryEntry(Updater.Instance.Settings.DownloadFolder, true));

                IPropertyItem piExternals = ConfigurationAccessHelper.GetPropertyByPath(configuration, EXTERNAL_FOLDERS);
                if (piExternals != null)
                {
                    foreach (IPropertyItem pi in piExternals.Items.Values)
                    {
                        mExternalFolders.Add(new DirectoryEntry(pi.Id, pi));
                    }
                }

                IPropertyItem piExternalFiles = ConfigurationAccessHelper.GetPropertyByPath(configuration, EXTERNAL_FILES);
                if (piExternalFiles != null)
                {
                    foreach (IPropertyItem pi in piExternalFiles.Items.Values)
                    {
                        if (PathHelper.IsAbsolutePath(pi.Id))
                        {
                            mExternalFiles.Add(pi.Id);
                        }
                        else
                        {
                            mExternalFiles.Add(Path.Combine(mCodeBaseFolder, pi.Id));
                        }
                    }
                }

                ConfigurationAccessHelper.ParseBooleanValue(configuration, PERFORM_SEC_TEST, ref mPerformSecurityTestOnFolders);

                IPropertyItem piExtensions = ConfigurationAccessHelper.GetPropertyByPath(configuration, EXTENSIONS);
                if (piExtensions != null)
                {
                    foreach (IPropertyItem pi in piExtensions.Items.Values)
                    {
                        string ext = pi.Id.ToLower().Trim().Replace(".", string.Empty);
                        if (!string.IsNullOrEmpty(ext))
                        {
                            if (string.IsNullOrEmpty(pi.Value))
                            {
                                mExtensionVsDescriptor[ext] = null;
                            }
                            else
                            {
                                mExtensionVsDescriptor[ext] = TypeHelper.GetTypeFromString(pi.Value);
                            }
                        }
                    }
                }

                this.IsInitialized = true;
            }
        }

        /// <summary>
        /// Collects the specified update data.
        /// </summary>
        /// <param name="updateData">The update data.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Collect(Dictionary<string, DescriptorBase> updateData)
        {
            if (updateData == null)
            {
                ThrowHelper.ThrowArgumentNullException("updateData");
            }
            if (!IsInitialized)
            {
                throw new InitializationException("Collector has not been initialized.");
            }

            // collects paths
            List<string> paths = new List<string>();
            ScanFolder(paths, CodeBaseFolder, true);

            foreach (DirectoryEntry de in mExternalFolders)
            {
                ScanFolder(paths, de.FolderName, de.IncludeSubFolders);
            }

            if (PerformSecurityTestOnFolders)
            {
                foreach (string path in paths)
                {
                    if (!PathHelper.PerformFolderSecurityTest(path))
                    {
                        throw new SecurityException(string.Format("Security test failed on directory '{0}'. Please grant read/write rights to the current user.", path));
                    }
                }
            }

            // collects files from folders
            foreach (string path in paths)
            {
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo fi in di.GetFiles())
                {
                    Type descriptorType = null;
                    string ext = fi.Extension.ToLower().Replace(".", string.Empty);
                    if (mExtensionVsDescriptor.ContainsKey(ext))
                    {
                        descriptorType = mExtensionVsDescriptor[ext];
                    }
                    else if (mExtensionVsDescriptor.ContainsKey(JOKER_EXTENSION))
                    {
                        descriptorType = mExtensionVsDescriptor[JOKER_EXTENSION];
                    }
                    if (descriptorType != null)
                    {
                        try
                        {
                            DescriptorBase desc = (DescriptorBase)descriptorType.GetConstructor(new Type[] { typeof(FileInfo) }).Invoke(new object[] { fi });
                            updateData[desc.Id] = desc;
                        }
                        catch (Exception ex)
                        {
                            if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("FILE_COLLECTOR, failed to create descriptor object for file '{0}'.", fi.FullName), ex);
                        }
                    }
                }
            }

            foreach (string file in mExternalFiles)
            {
                FileInfo fi = new FileInfo(file);
                Type descriptorType = null;
                string ext = fi.Extension.ToLower().Replace(".", string.Empty);
                if (mExtensionVsDescriptor.ContainsKey(ext))
                {
                    descriptorType = mExtensionVsDescriptor[ext];
                }
                else if (mExtensionVsDescriptor.ContainsKey(JOKER_EXTENSION))
                {
                    descriptorType = mExtensionVsDescriptor[JOKER_EXTENSION];
                }

                if (descriptorType != null)
                {
                    try
                    {
                        DescriptorBase desc = (DescriptorBase)descriptorType.GetConstructor(new Type[] { typeof(FileInfo) }).Invoke(new object[] { fi });
                        updateData[desc.Id] = desc;
                    }
                    catch (Exception ex)
                    {
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("FILE_COLLECTOR, failed to create descriptor object for file '{0}'.", fi.FullName), ex);
                    }
                }
                else
                {
                    if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("FILE_COLLECTOR, no registered descriptor type found for this file extension: {0}", ext));
                }
            }
        }

        #endregion

        #region Private method(s)

        private void ScanFolder(List<string> allowedPaths, string path, bool includeSubFolders)
        {
            if (!IsFolderExcluded(path))
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    allowedPaths.Add(path);
                    if (includeSubFolders)
                    {
                        foreach (DirectoryInfo dirInfo in di.GetDirectories())
                        {
                            ScanFolder(allowedPaths, dirInfo.FullName.ToLower(), includeSubFolders);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("FILE_COLLECTOR, failed to examine directory: '{0}'.", path), ex);
                }
            }
        }

        private bool IsFolderExcluded(string path)
        {
            bool result = false;

            foreach (DirectoryEntry entry in mFolderExclusions)
            {
                if (path.Equals(entry.FolderName))
                {
                    result = true;
                    break; // ki van tiltva
                }
                else if (path.Length > entry.FolderName.Length &&
                    entry.IncludeSubFolders &&
                    path.StartsWith(entry.FolderName))
                {
                    result = true;
                    break; // ki van tiltva
                }
            }

            return result;
        }

        #endregion

    }

}
