/* *********************************************************************
 * Date: 20 Feb 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;

namespace Forge.Configuration.Shared
{

    /// <summary>
    /// Configuration Section Standard
    /// </summary>
    [Serializable]
    public abstract class ConfigurationSectionStandard : ConfigurationSectionBase
    {

        #region Field(s)

        private static readonly List<ConfigurationSectionStandard> CONFIG_SECTIONS = new List<ConfigurationSectionStandard>();

        [NonSerialized]
        private ConfigurationProperty mCommonConfiguration;

        // The LoggerCategories
        //protected ConfigurationProperty mLoggerCategories;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationSectionStandard" /> class.
        /// </summary>
        protected ConfigurationSectionStandard()
            : base()
        {
            mCommonConfiguration = new ConfigurationProperty("CategoryPropertyItems", typeof(CategoryPropertyItems));
            //mLoggerCategories = new ConfigurationProperty( "LoggerCategories", typeof( LoggerCategories ) );
            Properties.Add(mCommonConfiguration);
            //mProperties.Add( mLoggerCategories );
            lock (CONFIG_SECTIONS)
            {
                CONFIG_SECTIONS.Add(this);
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Gets the known sections.
        /// </summary>
        /// <value>
        /// The known sections.
        /// </value>
        public static List<ConfigurationSectionStandard> KnownSections
        {
            get { return new List<ConfigurationSectionStandard>(CONFIG_SECTIONS); }
        }

        /// <summary>
        /// Encrypts the section.
        /// </summary>
        public void EncryptSection()
        {
            foreach (CategoryPropertyItem item in CategoryPropertyItems)
            {
                Encrypt(item);
            }
        }

        /// <summary>
        /// Decrypts the section.
        /// </summary>
        public void DecryptSection()
        {
            foreach (CategoryPropertyItem item in CategoryPropertyItems)
            {
                Decrypt(item);
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the category property items.
        /// </summary>
        /// <value>
        /// The category property items.
        /// </value>
        [DebuggerHidden]
        [ConfigurationProperty("CategoryPropertyItems", IsRequired = false)]
        public CategoryPropertyItems CategoryPropertyItems
        {
            get
            {
                return (CategoryPropertyItems)this["CategoryPropertyItems"];
            }
            set
            {
                this["CategoryPropertyItems"] = value;
            }
        }

        //[DebuggerHidden]
        //[ConfigurationProperty( "LoggerCategories", IsRequired = false )]
        //public virtual LoggerCategories LoggerCategories
        //{
        //    get
        //    {
        //        return ( LoggerCategories ) this[ "LoggerCategories" ];
        //    }
        //    set
        //    {
        //        this[ "LoggerCategories" ] = value;
        //    }
        //}

        #endregion

        #region Private method(s)

        private void Encrypt(CategoryPropertyItem item)
        {
            item.Encrypted = true;
            foreach (CategoryPropertyItem i in item.PropertyItems)
            {
                Encrypt(i);
            }
        }

        private void Decrypt(CategoryPropertyItem item)
        {
            item.Encrypted = false;
            foreach (CategoryPropertyItem i in item.PropertyItems)
            {
                Decrypt(i);
            }
        }

        #endregion

    }

}
