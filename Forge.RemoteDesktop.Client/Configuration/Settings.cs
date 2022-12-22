/* *********************************************************************
 * Date: 9 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Configuration;
using System.Runtime.CompilerServices;
using Forge.Configuration.Shared;
using Forge.Invoker;
using Forge.RemoteDesktop.ConfigSection;
using Forge.Shared;

namespace Forge.RemoteDesktop.Client.Configuration
{

    /// <summary>
    /// Represents the configuration settings for a service
    /// </summary>
    public static class Settings
    {

        #region Field(s)

        private const string CONFIG_ROOT = "Client";

        private const string CONFIG_MOUSE_MOVE_SEND_INTERVAL = "MouseMoveSendInterval";

        private static int mMouseMoveSendInterval = Consts.DEFAULT_MOUSE_MOVE_SEND_INTERVAL;

        /// <summary>
        /// Occurs when [event configuration changed].
        /// </summary>
        public static event EventHandler<EventArgs> EventConfigurationChanged;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes the <see cref="Settings"/> class.
        /// </summary>
        static Settings()
        {
            SectionHandler_OnConfigurationChanged(null, EventArgs.Empty);
            RemoteDesktopConfiguration.SectionHandler.OnConfigurationChanged += new EventHandler<EventArgs>(SectionHandler_OnConfigurationChanged);
        }

        #endregion

        #region Public properties

         /// <summary>
        /// Gets or sets the mouse move send interval.
        /// </summary>
        /// <value>
        /// The mouse move send interval.
        /// </value>
        public static int MouseMoveSendInterval
        {
            get { return mMouseMoveSendInterval; }
            set
            {
                if (value < 0)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }

                mMouseMoveSendInterval = value;
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        /// <param name="saveMode">The save mode.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Save(ConfigurationSaveMode saveMode)
        {
            CategoryPropertyItem rootItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(RemoteDesktopConfiguration.Settings.CategoryPropertyItems, CONFIG_ROOT);
            if (rootItem == null)
            {
                rootItem = new CategoryPropertyItem();
                rootItem.Id = CONFIG_ROOT;
                RemoteDesktopConfiguration.Settings.CategoryPropertyItems.Add(rootItem);
            }

            CategoryPropertyItem piMouseMoveSendingInterval = ConfigurationAccessHelper.GetCategoryPropertyByPath(rootItem.PropertyItems, CONFIG_MOUSE_MOVE_SEND_INTERVAL);
            if (piMouseMoveSendingInterval == null)
            {
                piMouseMoveSendingInterval = new CategoryPropertyItem();
                piMouseMoveSendingInterval.Id = CONFIG_MOUSE_MOVE_SEND_INTERVAL;
                rootItem.PropertyItems.Add(piMouseMoveSendingInterval);
            }
            piMouseMoveSendingInterval.EntryValue = MouseMoveSendInterval.ToString();

            RemoteDesktopConfiguration.Save(saveMode);
        }

        #endregion

        #region Private method(s)

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void SectionHandler_OnConfigurationChanged(object sender, EventArgs e)
        {
            CategoryPropertyItem rootItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(RemoteDesktopConfiguration.Settings.CategoryPropertyItems, CONFIG_ROOT);
            if (rootItem != null)
            {
                int mouseMoveSendingInterval = Consts.DEFAULT_MOUSE_MOVE_SEND_INTERVAL;
                if (ConfigurationAccessHelper.ParseIntValue(rootItem.PropertyItems, CONFIG_MOUSE_MOVE_SEND_INTERVAL, 0, int.MaxValue, ref mouseMoveSendingInterval))
                {
                    MouseMoveSendInterval = mouseMoveSendingInterval;
                }

            }

            Executor.Invoke(EventConfigurationChanged, null, EventArgs.Empty);
        }

        #endregion

    }

}
