/* *********************************************************************
 * Date: 25 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Diagnostics;
using System.Reflection;

namespace Forge.Net.Remoting.Validators
{

    /// <summary>
    /// Represents an event information
    /// </summary>
    public sealed class EventComparator : MBRBase
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly EventInfo mEventInfo = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="EventComparator"/> class.
        /// </summary>
        /// <param name="ei">The ei.</param>
        public EventComparator(EventInfo ei)
        {
            if (ei == null)
            {
                ThrowHelper.ThrowArgumentNullException("ei");
            }
            this.mEventInfo = ei;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the event info.
        /// </summary>
        /// <value>
        /// The event info.
        /// </value>
        [DebuggerHidden]
        public EventInfo EventInfo
        {
            get { return mEventInfo; }
        }  

        #endregion

        #region Public method(s)

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj != null && obj is EventComparator)
            {
                EventComparator ec = (EventComparator)obj;
                return ec.EventInfo.Name.Equals(mEventInfo.Name) && ec.EventInfo.EventHandlerType.Equals(mEventInfo.EventHandlerType);
            }
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

    }

}
