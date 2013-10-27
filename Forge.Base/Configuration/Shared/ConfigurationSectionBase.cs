/* *********************************************************************
 * Date: 20 Feb 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Configuration;

namespace Forge.Configuration.Shared
{

    /// <summary>
    /// Base configuration section for local configuration handler classes
    /// </summary>
    [Serializable]
    public abstract class ConfigurationSectionBase : ConfigurationSection
    {

        #region Field(s)

        // The collection (property bag) that conatains 
        // the section properties.
        [NonSerialized]
        private ConfigurationPropertyCollection mProperties = new ConfigurationPropertyCollection();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationSectionBase"/> class.
        /// </summary>
        protected ConfigurationSectionBase()
        {
        }

        #endregion

        #region Protected properties

        /// <summary>
        /// This is a key customization.
        /// It returns the initialized property bag.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Configuration.ConfigurationPropertyCollection" /> of properties for the element.
        ///   </returns>
        protected override ConfigurationPropertyCollection Properties
        {
            get { return mProperties; }
        }

        #endregion

    }

}
