/* *********************************************************************
 * Date: 20 Feb 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;

namespace Forge.Configuration.Shared
{

    /// <summary>
    /// Validators for built-in configuration structure
    /// </summary>
    public static class Validators
    {

        /// <summary>
        /// Categories the property items validator.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        /// Id
        /// or
        /// </exception>
        public static void CategoryPropertyItemsValidator(CategoryPropertyItems items)
        {
            if (items != null && items.Count > 0)
            {
                List<string> ids = new List<string>();
                foreach (CategoryPropertyItem catItem in items)
                {
                    catItem.Id.ToString();
                    if (catItem.Id.Length == 0)
                    {
                        throw new ConfigurationErrorsException("Id");
                    }
                    if (ids.Contains(catItem.Id))
                    {
                        throw new ConfigurationErrorsException(String.Format("Duplicated Id: {0}", catItem.Id));
                    }
                    //catItem.EntryName.ToString();
                    catItem.EntryValue.ToString();
                    CategoryPropertyItemsValidator(catItem.PropertyItems);
                    ids.Add(catItem.Id);
                }
                ids.Clear();
            }
        }

        //public static void LoggerCategoryValidator( LoggerCategories categories )
        //{
        //    foreach ( LoggerCategoryItem item in categories )
        //    {
        //        if ( String.IsNullOrEmpty( item.Name ) )
        //        {
        //            throw new ConfigurationErrorsException( "LoggerCategoryItem.Name" );
        //        }
        //    }
        //}

    }

}
