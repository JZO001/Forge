﻿/* *********************************************************************
 * Date: 24 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Configuration;
using Forge.Configuration.Shared;
using Forge.Shared;

namespace Forge.ErrorReport.Filter
{

    /// <summary>
    /// Filter base with member name and value
    /// </summary>
    [Serializable]
    public abstract class FilterMemberNameAndValue : FilterBase
    {

        #region Field(s)

        /// <summary>
        /// The configuration g_ membername
        /// </summary>
        protected const string CONFIG_MEMBERNAME = "MemberName";

        /// <summary>
        /// The configuration g_ value
        /// </summary>
        protected const string CONFIG_VALUE = "Value";

        #endregion

        #region Constructor(s)
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterMemberNameAndValue"/> class.
        /// </summary>
        protected FilterMemberNameAndValue()
        {
        } 

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the name of the member.
        /// </summary>
        /// <value>
        /// The name of the member.
        /// </value>
        public string MemberName { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public override void Initialize(IPropertyItem item)
        {
            base.Initialize(item);

            MemberName = null;
            Value = null;

            string memberName = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item, CONFIG_MEMBERNAME, ref memberName))
            {
                MemberName = memberName;
            }

            string value = string.Empty;
            if (ConfigurationAccessHelper.ParseStringValue(item, CONFIG_VALUE, ref value))
            {
                Value = value;
            }

            if (string.IsNullOrEmpty(MemberName))
            {
                throw new InitializationException("Member name has not been definied.");
            }
        }

        #endregion

    }

}
