/* *********************************************************************
 * Date: 24 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Configuration.Shared;

namespace Forge.ErrorReport.Filter
{

    /// <summary>
    /// Represents the base method(s) of an error report filter
    /// </summary>
    [Serializable]
    public abstract class FilterBase : IErrorReportFilter
    {

        #region Field(s)

        /// <summary>
        /// The configuration g_ negation
        /// </summary>
        protected const string CONFIG_NEGATION = "Negation";

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterBase"/> class.
        /// </summary>
        protected FilterBase()
        {
        } 

        #endregion

        #region Public properties
        
        /// <summary>
        /// Gets a value indicating whether [is initialized].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is initialized]; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [negation].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [negation]; otherwise, <c>false</c>.
        /// </value>
        public bool Negation { get; set; }

        #endregion

        #region Public method(s)
        
        /// <summary>
        /// Initializes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual void Initialize(CategoryPropertyItem item)
        {
            if (item == null)
            {
                ThrowHelper.ThrowArgumentNullException("item");
            }

            if (item.PropertyItems != null)
            {
                bool negation = false;
                if (ConfigurationAccessHelper.ParseBooleanValue(item.PropertyItems, CONFIG_NEGATION, ref negation))
                {
                    this.Negation = negation;
                }
            }
        }

        /// <summary>
        /// Filters the specified package.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <returns>
        /// True, if the filter criterias matches, otherwise False.
        /// </returns>
        public abstract bool Filter(ReportPackage package);

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Documents the initialization check.
        /// </summary>
        /// <exception cref="InitializationException"></exception>
        protected virtual void DoInitializationCheck()
        {
            if (!IsInitialized)
            {
                throw new InitializationException("Filter has not been initialized.");
            }
        }

        #endregion

    }

}
