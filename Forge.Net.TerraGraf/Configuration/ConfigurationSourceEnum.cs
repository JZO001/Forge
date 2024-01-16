namespace Forge.Net.TerraGraf.Configuration
{

    /// <summary>Configuration source</summary>
    public enum ConfigurationSourceEnum
    {
        /// <summary>Configuration provided by the configuration manager
        /// (XML)</summary>
        ConfigurationManager,

#if NETCOREAPP3_1_OR_GREATER
        /// <summary>Configuration source is from external provider</summary>
        External
#endif
    }

}
