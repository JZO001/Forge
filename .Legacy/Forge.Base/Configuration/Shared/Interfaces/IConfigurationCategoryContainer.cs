/* *********************************************************************
 * Date: 20 Feb 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

namespace Forge.Configuration.Shared.Interfaces
{

    /// <summary>
    /// Configuration Category Container interface
    /// </summary>
    public interface IConfigurationCategoryContainer
    {

        /// <summary>
        /// Gets or sets CategoryProperty Items collection
        /// </summary>
        /// <value>
        /// The category property items.
        /// </value>
        CategoryPropertyItems CategoryPropertyItems { get; set; }

    }

}
