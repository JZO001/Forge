/* *********************************************************************
 * Date: 10 Dec 2010
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Configuration;
using System.Security.Permissions;

namespace Forge.Threading.ConfigSection
{

    /// <summary>
    /// Configuration access helper class for ThreadPools
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Unrestricted = true)]
    public class ThreadPoolConfiguration : Forge.Configuration.Shared.SharedConfigSettings<ThreadPoolSection, ThreadPoolConfiguration>
    {

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="ThreadPoolConfiguration"/> class.
        /// </summary>
        static ThreadPoolConfiguration()
        {
            LOG_PREFIX = "THREADPOOL";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadPoolConfiguration"/> class.
        /// </summary>
        public ThreadPoolConfiguration()
            : base()
        {
        }

        #endregion

        #region Protected helpers

        /// <summary>
        /// Loads the factory defaults.
        /// </summary>
        /// <returns></returns>
        protected override ThreadPoolSection LoadFactoryDefaults()
        {
            mSettings = base.LoadFactoryDefaults();
            mSettings.ThreadPools = new ThreadPools();
            return mSettings;
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        protected override void Validate()
        {
            try
            {
                foreach (ThreadPoolItem item in mSettings.ThreadPools)
                {
                    if (item.MinThreadNumber < 1)
                    {
                        throw new ConfigurationErrorsException("MinThreadNumber");
                    }
                    if (item.MaxThreadNumber < 1)
                    {
                        throw new ConfigurationErrorsException("MaxThreadNumber");
                    }
                    if (item.ShutDownIdleThreadTime < 1)
                    {
                        throw new ConfigurationErrorsException("ShutDownIdleThreadTime");
                    }
                    item.Name.ToString();
                    item.SetReadOnlyFlag.ToString();
                }
                mLastKnownGoodSettings = mSettings;
            }
            catch (Exception ex)
            {
                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("THREADPOOL: validation of the configuration has failed. Reason: {0}", ex.Message));
                if (mLastKnownGoodSettings == null)
                {
                    mLastKnownGoodSettings = LoadFactoryDefaults();
                }
                mSettings = mLastKnownGoodSettings;
            }
        }

        #endregion

    }

}
