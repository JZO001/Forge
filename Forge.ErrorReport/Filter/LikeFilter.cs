/* *********************************************************************
 * Date: 24 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Configuration.Shared;
using Forge.Reflection;

namespace Forge.ErrorReport.Filter
{

    /// <summary>
    /// Like filter
    /// </summary>
    [Serializable]
    public class LikeFilter : FilterMemberNameAndValue
    {

        #region Field(s)

        private const string CONFIG_MATCHMODE = "MatchMode";

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="LikeFilter"/> class.
        /// </summary>
        public LikeFilter()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the match mode.
        /// </summary>
        /// <value>
        /// The match mode.
        /// </value>
        public LikeMatchModeFilterEnum MatchMode { get; set; }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public override void Initialize(CategoryPropertyItem item)
        {
            base.Initialize(item);

            this.MatchMode = LikeMatchModeFilterEnum.Exact;
            if (item.PropertyItems != null)
            {
                LikeMatchModeFilterEnum matchMode = LikeMatchModeFilterEnum.Exact;
                if (ConfigurationAccessHelper.ParseEnumValue<LikeMatchModeFilterEnum>(item.PropertyItems, CONFIG_MATCHMODE, ref matchMode))
                {
                    this.MatchMode = matchMode;
                }
            }

            this.IsInitialized = true;
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

            string value = ExtractObjectData.Create(this.MemberName).GetValue(package).ToString();
            if (value != null)
            {
                switch (MatchMode)
                {
                    case LikeMatchModeFilterEnum.Exact:
                        result = value.ToLower().Equals(Value.ToLower());
                        break;

                    case LikeMatchModeFilterEnum.Start:
                        result = value.ToLower().StartsWith(Value.ToLower());
                        break;

                    case LikeMatchModeFilterEnum.End:
                        result = value.ToLower().EndsWith(Value.ToLower());
                        break;

                    case LikeMatchModeFilterEnum.Anywhere:
                        result = value.ToLower().Contains(Value.ToLower());
                        break;
                }
            }

            if (Negation)
            {
                result = !result;
            }

            return result;
        }

        #endregion

    }

}
