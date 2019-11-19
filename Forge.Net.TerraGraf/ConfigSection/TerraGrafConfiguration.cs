/* *********************************************************************
 * Date: 07 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/


namespace Forge.Net.TerraGraf.ConfigSection
{

    /// <summary>
    /// Configuration access helper class for TerraGraf
    /// </summary>
    public class TerraGrafConfiguration : Forge.Configuration.Shared.SharedConfigSettings<TerraGrafSection, TerraGrafConfiguration>
    {

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="TerraGrafConfiguration"/> class.
        /// </summary>
        static TerraGrafConfiguration()
        {
            LOG_PREFIX = "TERRAGRAF_CONFIGURATION";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerraGrafConfiguration"/> class.
        /// </summary>
        public TerraGrafConfiguration()
            : base()
        {
        }

        #endregion

    }

}
