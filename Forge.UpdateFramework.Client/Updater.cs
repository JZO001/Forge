/* *********************************************************************
 * Date: 12 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Forge.Legacy;
using Forge.Net.Services.Locators;
using Forge.UpdateFramework.Client.Configuration;

namespace Forge.UpdateFramework.Client
{

    /// <summary>
    /// Update Manager implementation for the clients
    /// </summary>
    public sealed class Updater : MBRBase
    {

        #region Field(s)

        private static Updater mSingletonInstance = null;

        private readonly Settings mSettings = new Settings();

        private UpdateServiceLocator mServiceLocator = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mInitialized = false;

        private Thread mWorkerThread = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Prevents a default instance of the <see cref="Updater"/> class from being created.
        /// </summary>
        private Updater()
        {
            this.Status = UpdaterStatusEnum.UpdateNotFound;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static Updater Instance
        {
            get 
            {
                if (mSingletonInstance == null)
                {
                    mSingletonInstance = new Updater();
                    mSingletonInstance.mSettings.Initialize();
                }
                return mSingletonInstance; 
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized
        {
            get { return mInitialized; }
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        public Settings Settings
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return mInitialized ? mSettings.Clone() as Settings : mSettings; }
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        public UpdaterStatusEnum Status { get; private set; }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes the updater.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Initialize()
        {
            if (!mInitialized)
            {
                mServiceLocator = new UpdateServiceLocator();
                mServiceLocator.EventServiceStateChanged += new EventHandler<ServiceStateChangedEventArgs>(ServiceLocatorStateChangedEventHandler);
                mServiceLocator.Start();

                mInitialized = true;
            }
        }

        /// <summary>
        /// Checks for updates immediatelly.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CheckUpdate()
        {

            mWorkerThread = new Thread(new ThreadStart(WorkerThreadMain));
            mWorkerThread.Name = "UpdaterWorkerThread";
            mWorkerThread.IsBackground = true;
            mWorkerThread.Start();

        }

        #endregion

        #region Private method(s)

        private void ServiceLocatorStateChangedEventHandler(object sender, ServiceStateChangedEventArgs e)
        {

        }

        private void WorkerThreadMain()
        {
            Dictionary<string, DescriptorBase> collectedDescriptors = new Dictionary<string, DescriptorBase>();
            foreach (IDataCollector collector in this.Settings.Collectors)
            {
                collector.Collect(collectedDescriptors);
            }

        }

        #endregion

    }

}
