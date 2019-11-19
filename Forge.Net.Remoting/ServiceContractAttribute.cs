/* *********************************************************************
 * Date: 08 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Net.Remoting
{

    /// <summary>
    /// Represent that the marked interface is a service contract
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class ServiceContractAttribute : Attribute
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceContractAttribute"/> class.
        /// </summary>
        public ServiceContractAttribute()
        {
            WellKnownObjectMode = WellKnownObjectModeEnum.PerSession;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceContractAttribute"/> class.
        /// </summary>
        /// <param name="wellKnownObjectMode">The well known object mode.</param>
        public ServiceContractAttribute(WellKnownObjectModeEnum wellKnownObjectMode)
        {
            WellKnownObjectMode = wellKnownObjectMode;
        } 

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the well known object mode.
        /// </summary>
        /// <value>
        /// The well known object mode.
        /// </value>
        public WellKnownObjectModeEnum WellKnownObjectMode
        {
            get;
            set;
        } 

        #endregion

    }

}
