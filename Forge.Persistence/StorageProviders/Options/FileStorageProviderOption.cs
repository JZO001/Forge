namespace Forge.Persistence.StorageProviders.Options
{

    /// <summary>File storage provider options</summary>
    public class FileStorageProviderOption : StorageProviderBaseOption
    {

        /// <summary>Gets or sets the base URL.</summary>
        /// <value>The base URL.</value>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>Gets or sets a value indicating whether [compress content].</summary>
        /// <value>
        ///   <c>true</c> if [compress content]; otherwise, <c>false</c>.</value>
        public bool CompressContent { get; set; }

    }

}
