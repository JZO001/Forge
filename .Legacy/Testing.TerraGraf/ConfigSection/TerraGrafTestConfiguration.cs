using System.Security.Permissions;

namespace Testing.TerraGraf.ConfigSection
{

    [SecurityPermission(SecurityAction.Demand, Unrestricted = true)]
    public class TerraGrafTestConfiguration : Forge.Configuration.Shared.SharedConfigSettings<TerraGrafTestSection, TerraGrafTestConfiguration>
    {

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="TerraGrafTestConfiguration"/> class.
        /// </summary>
        static TerraGrafTestConfiguration()
        {
            LOG_PREFIX = "TERRAGRAF_TEST";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerraGrafTestConfiguration"/> class.
        /// </summary>
        public TerraGrafTestConfiguration()
            : base()
        {
        }

        #endregion

    }

}
