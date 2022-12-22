/* *********************************************************************
 * Date: 07 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.RemoteDesktop.Contracts
{

    /// <summary>
    /// Represents a mouse wheel event argument
    /// </summary>
    [Serializable]
    public class MouseWheelEventArgs
    {

        #region Constructor(s)
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MouseWheelEventArgs"/> class.
        /// </summary>
        /// <param name="wheelType">Type of the wheel.</param>
        /// <param name="amount">The amount.</param>
        public MouseWheelEventArgs(MouseWheelTypeEnum wheelType, int amount)
        {
            WheelType = wheelType;
            Amount = amount;
        }

        #endregion

        #region Public properties
        
        /// <summary>
        /// Gets the type of the wheel.
        /// </summary>
        /// <value>
        /// The type of the wheel.
        /// </value>
        public MouseWheelTypeEnum WheelType { get; private set; }

        /// <summary>
        /// Gets the amount.
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        public int Amount { get; private set; }

        #endregion

    }

}
