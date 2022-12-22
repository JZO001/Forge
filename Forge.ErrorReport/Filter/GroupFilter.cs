/* *********************************************************************
 * Date: 24 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using Forge.Configuration;
using Forge.Configuration.Shared;
using Forge.Logging.Abstraction;
using Forge.Reflection;
using Forge.Shared;

namespace Forge.ErrorReport.Filter
{

    /// <summary>
    /// Represents a group filter
    /// </summary>
    [Serializable]
    public class GroupFilter : FilterBase
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger<GroupFilter>();

        private const string CONFIG_FILTER_LOGIC = "FilterLogic";

        private const string CONFIG_FILTERS = "Filters";

        private readonly List<IErrorReportFilter> mFilters = new List<IErrorReportFilter>();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupFilter"/> class.
        /// </summary>
        public GroupFilter()
        {
            IsInitialized = true;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the filter logic.
        /// </summary>
        /// <value>
        /// The filter logic.
        /// </value>
        public GroupFilterLoginEnum FilterLogic { get; set; }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <value>
        /// The filters.
        /// </value>
        public List<IErrorReportFilter> Filters
        {
            get { return mFilters; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="propertyItem">The property item.</param>
        public override void Initialize(IPropertyItem item)
        {
            base.Initialize(item);

            FilterLogic = GroupFilterLoginEnum.And;
            mFilters.Clear();
            
            GroupFilterLoginEnum logic = GroupFilterLoginEnum.And;
            if (ConfigurationAccessHelper.ParseEnumValue<GroupFilterLoginEnum>(item, CONFIG_FILTER_LOGIC, ref logic))
            {
                FilterLogic = logic;
            }

            IPropertyItem filterItems = ConfigurationAccessHelper.GetPropertyByPath(item, CONFIG_FILTERS);
            if (filterItems != null && filterItems.Items.Count > 0)
            {
                foreach (IPropertyItem filterItem in filterItems.Items.Values)
                {
                    try
                    {
                        Type filterType = TypeHelper.GetTypeFromString(filterItem.Value, TypeLookupModeEnum.AllowAll, true, true, true);
                        IErrorReportFilter filter = (IErrorReportFilter)filterType.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
                        filter.Initialize(filterItem);
                        mFilters.Add(filter);
                    }
                    catch (Exception ex)
                    {
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("GROUP_FILTER, failed to create error report filter. Type: '{0}'", filterItem.Value), ex);
                    }
                }
            }

            IsInitialized = true;
        }

        /// <summary>
        /// Filters the specified package.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <returns>
        /// True, if the filter criterias matches, otherwise False.
        /// </returns>
        public override bool Filter(ReportPackage package)
        {
            DoInitializationCheck();

            bool result = false;

            if (FilterLogic == GroupFilterLoginEnum.And)
            {
                result = true;
                foreach (IErrorReportFilter filter in mFilters)
                {
                    result = result && filter.Filter(package);
                    if (!result) break;
                }
            }
            else
            {
                foreach (IErrorReportFilter filter in mFilters)
                {
                    result = filter.Filter(package);
                    if (result) break;
                }
            }

            if (Negation)
            {
                result = !result;
            }

            return result;
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Documents the initialization check.
        /// </summary>
        /// <exception cref="InitializationException">No filter(s) definied.</exception>
        protected override void DoInitializationCheck()
        {
            base.DoInitializationCheck();
            if (mFilters.Count == 0)
            {
                throw new InitializationException("No filter(s) definied.");
            }
        }

        #endregion

    }

}
