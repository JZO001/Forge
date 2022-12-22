using System;

namespace Forge.Persistence.StorageProviders.Options
{

    /// <summary>Base options for a storage provider</summary>
    public abstract class StorageProviderBaseOption
    {

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the data formatter.
        /// This is the full qualified name of the type. If not specified, 'DataFormatterType' will be used.
        /// Using order: 1. 'DataFormatterType'; 2. 'DataFormatter'
        /// </summary>
        /// <value>The data formatter.</value>
        public string DataFormatter { get; set; }

        /// <summary>
        /// Gets or sets the type of the data formatter.
        /// If it is not provided, 'DataFormatter' property will be used.
        /// Using order: 1. 'DataFormatterType'; 2. 'DataFormatter'
        /// </summary>
        /// <value>The type of the data formatter.</value>
        public Type DataFormatterType { get; set; }

    }

}
