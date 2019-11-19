/* *********************************************************************
 * Date: 9 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Forge.IO;
using Forge.Persistence.Formatters;
using Forge.RemoteDesktop.Configuration;
using Forge.RemoteDesktop.Contracts;
using Forge.RemoteDesktop.Service.Configuration;
using log4net;

namespace Forge.RemoteDesktop.Service
{

    /// <summary>
    /// Handles the authentication information
    /// </summary>
    public static class AuthenticationHandlerModule
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(AuthenticationHandlerModule));

        private static SecurityStore mSecurityStore = new SecurityStore();

        private static string mStoreFileName = string.Empty;

        private static byte[] mIV = new byte[] { 56, 74, 12, 44, 34, 12, 67, 98, 232, 123, 35, 34, 12, 56, 45, 67 };

        private static byte[] mKey = new byte[] { 87, 23, 156, 222, 167, 32, 45, 156, 45, 98, 67, 42, 38, 87, 57, 26,
                                                    45, 51, 76, 66, 43, 94, 56, 43, 32, 11, 99, 176, 145, 210, 245, 132 };

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes the <see cref="AuthenticationHandlerModule"/> class.
        /// </summary>
        static AuthenticationHandlerModule()
        {
            ApplyConfiguration();
            Settings.EventConfigurationChanged += new EventHandler<EventArgs>(Settings_EventConfigurationChanged);
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Sets the password for a user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void SetPassword(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                ThrowHelper.ThrowArgumentNullException("username");
            }
            if (password == null)
            {
                ThrowHelper.ThrowArgumentNullException("password");
            }

            mSecurityStore.UsersWithPassword[username] = password;
            Save();
        }

        /// <summary>
        /// Sets the global password.
        /// </summary>
        /// <param name="password">The password.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void SetPassword(string password)
        {
            if (password == null)
            {
                ThrowHelper.ThrowArgumentNullException("password");
            }

            mSecurityStore.GlobalPassword = password;
            Save();
        }

        /// <summary>
        /// Removes the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public static bool RemoveUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                ThrowHelper.ThrowArgumentNullException("username");
            }

            if (mSecurityStore.UsersWithPassword.Remove(username))
            {
                Save();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static string[] GetUsers()
        {
            return mSecurityStore.UsersWithPassword.Keys.ToList<string>().ToArray();
        }

        /// <summary>
        /// Checks the authentication info.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static bool CheckAuthenticationInfo(string username, string password)
        {
            if (username == null)
            {
                ThrowHelper.ThrowArgumentNullException("username");
            }
            if (password == null)
            {
                ThrowHelper.ThrowArgumentNullException("password");
            }

            bool result = false;

            switch (Settings.AuthenticationMode)
            {
                case AuthenticationModeEnum.OnlyPassword:
                    {
                        result = password.Equals(mSecurityStore.GlobalPassword);
                    }
                    break;

                case AuthenticationModeEnum.UsernameAndPassword:
                    {
                        if (mSecurityStore.UsersWithPassword.ContainsKey(username))
                        {
                            result = password.Equals(mSecurityStore.UsersWithPassword[username]);
                        }
                    }
                    break;

                case AuthenticationModeEnum.Off:
                    {
                        result = true;
                    }
                    break;
            }

            return result;
        }

        #endregion

        #region Private method(s)

        private static void Settings_EventConfigurationChanged(object sender, EventArgs e)
        {
            ApplyConfiguration();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void ApplyConfiguration()
        {
            if (LOGGER.IsDebugEnabled) LOGGER.Debug("REMOTE_DESKTOP_SERVICE: loading configuration...");
            try
            {
                CreateFileName();
                Load();
            }
            catch (Exception ex)
            {
                if (LOGGER.IsErrorEnabled) LOGGER.Error("REMOTE_DESKTOP_SERVICE: failed to apply configuration.", ex);
            }
        }

        private static void Load()
        {
            try
            {
                if (File.Exists(mStoreFileName))
                {
                    using (FileStream fs = new FileStream(mStoreFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        RijndaelFormatter<SecurityStore> formatter = new RijndaelFormatter<SecurityStore>(mIV, mKey, new BinarySerializerFormatter<SecurityStore>());
                        mSecurityStore = formatter.Read(fs);
                        if (mSecurityStore == null)
                        {
                            mSecurityStore = new SecurityStore();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (LOGGER.IsErrorEnabled) LOGGER.Error("REMOTE_DESKTOP_SERVICE: failed to load security information.", ex);
            }
        }

        private static void Save()
        {
            try
            {
                using (FileStream fs = new FileStream(mStoreFileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    RijndaelFormatter<SecurityStore> formatter = new RijndaelFormatter<SecurityStore>(mIV, mKey, new BinarySerializerFormatter<SecurityStore>());
                    formatter.Write(fs, mSecurityStore);
                }
            }
            catch (Exception ex)
            {
                if (LOGGER.IsErrorEnabled) LOGGER.Error("REMOTE_DESKTOP_SERVICE: failed to save security information.", ex);
                throw;
            }
        }

        private static void CreateFileName()
        {
            mStoreFileName = Settings.AuthenticationModuleStore;
            if (PathHelper.IsAbsolutePath(mStoreFileName))
            {
                mStoreFileName = PathHelper.ResolveEnvironmentSpecialFolder(mStoreFileName);
            }
            else
            {
                mStoreFileName = AppDomain.CurrentDomain.BaseDirectory;
                if (string.IsNullOrEmpty(mStoreFileName))
                {
                    mStoreFileName = string.Empty;
                }
            }
            mStoreFileName = Path.Combine(mStoreFileName, "RemoteDesktopData.bin");
        }

        #endregion

        #region Nested type(s)

        [Serializable]
        private sealed class SecurityStore
        {

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="SecurityStore"/> class.
            /// </summary>
            public SecurityStore()
            {
                this.GlobalPassword = string.Empty;
                this.UsersWithPassword = new Dictionary<string, string>();
            }

            #endregion

            #region Public properties

            /// <summary>
            /// Gets or sets the global password.
            /// </summary>
            /// <value>
            /// The global password.
            /// </value>
            public string GlobalPassword { get; set; }

            /// <summary>
            /// Gets or sets the users with password.
            /// </summary>
            /// <value>
            /// The users with password.
            /// </value>
            public Dictionary<string, string> UsersWithPassword { get; set; }

            #endregion

        }

        #endregion

    }

}
