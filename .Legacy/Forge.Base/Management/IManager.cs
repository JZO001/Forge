/* *********************************************************************
 * Date: 22 Jan 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Management
{

    /// <summary>
    /// Represents a manager services
    /// </summary>
    public interface IManager
    {

        #region Field(s)

        /// <summary>
        /// Occurs when [event start].
        /// </summary>
        event EventHandler<ManagerEventStateEventArgs> EventStart;

        /// <summary>
        /// Occurs when [event stop].
        /// </summary>
        event EventHandler<ManagerEventStateEventArgs> EventStop;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the state of the manager.
        /// </summary>
        /// <value>
        /// The state of the manager.
        /// </value>
        ManagerStateEnum ManagerState { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [event sync invocation].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [event sync invocation]; otherwise, <c>false</c>.
        /// </value>
        bool EventSyncInvocation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [event UI invocation].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [event UI invocation]; otherwise, <c>false</c>.
        /// </value>
        bool EventUIInvocation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [event parallel invocation].
        /// </summary>
        /// <value>
        /// <c>true</c> if [event parallel invocation]; otherwise, <c>false</c>.
        /// </value>
        bool EventParallelInvocation { get; set; }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Starts the manager asyncronously.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginStart(AsyncCallback callback, object state);

        /// <summary>
        /// Starts this manager instance.
        /// </summary>
        /// <returns>Manager State</returns>
        ManagerStateEnum Start();

        /// <summary>
        /// Ends the asynchronous start process.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Manager State</returns>
        ManagerStateEnum EndStart(IAsyncResult asyncResult);

        /// <summary>
        /// Stops the manager asyncronously.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginStop(AsyncCallback callback, object state);

        /// <summary>
        /// Stops this manager instance.
        /// </summary>
        /// <returns>Manager State</returns>
        ManagerStateEnum Stop();

        /// <summary>
        /// Ends the asynchronous stop process.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Manager State</returns>
        ManagerStateEnum EndStop(IAsyncResult asyncResult);

        #endregion

    }

}
