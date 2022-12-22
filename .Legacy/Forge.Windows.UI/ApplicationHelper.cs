/* *********************************************************************
 * Date: 17 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Threading;
using System.Web;

namespace Forge.Windows.UI
{

    /// <summary>
    /// Extension methods for ISet
    /// </summary>
    public static class ApplicationHelper
    {

        /// <summary>
        /// Gets a value indicating whether the thread is the UI thread.
        /// </summary>
        /// <value>
        /// <c>true</c> if [is unique identifier thread]; otherwise, <c>false</c>.
        /// </value>
#if NETCOREAPP3_1_OR_GREATER
        public static bool IsUIThread()
        {
            return System.Windows.Forms.Application.MessageLoop ||
                System.Windows.Threading.Dispatcher.FromThread(Thread.CurrentThread) != null;
        }
#else
        public static bool IsUIThread()
        {
            return System.Windows.Forms.Application.MessageLoop ||
                (HttpRuntime.AppDomainAppId == null ?
                    System.Windows.Threading.Dispatcher.FromThread(Thread.CurrentThread) != null
                    :
                    false);
        }
#endif

    }

}
