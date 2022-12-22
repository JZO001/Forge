/* *********************************************************************
 * Date: 20 Feb 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/


namespace Forge.Configuration.Shared.Interfaces
{

    /// <summary>
    /// Configuration section handler interface for local configuration handler classes
    /// </summary>
    public interface IConfigurationSectionHandler
    {

        /// <summary>
        /// Get category property items
        /// </summary>
        /// <value>
        /// The category property items.
        /// </value>
        CategoryPropertyItems CategoryPropertyItems { get; set; }

        ///// <summary>
        ///// Get logger listeners which the current component using
        ///// </summary>
        //LoggerCategories LoggerCategories { get; set; }

    }

}
