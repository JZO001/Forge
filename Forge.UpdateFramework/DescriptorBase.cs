/* *********************************************************************
 * Date: 15 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.UpdateFramework
{

    /// <summary>
    /// Descriptor base
    /// </summary>
    [Serializable]
    public abstract class DescriptorBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptorBase"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="descriptorType">Type of the descriptor.</param>
        protected DescriptorBase(string id, DescriptorTypeEnum descriptorType)
        {
            if (string.IsNullOrEmpty(id))
            {
                ThrowHelper.ThrowArgumentNullException("id");
            }

            this.Id = id;
            this.DescriptorType = descriptorType;
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        public string Id { get; protected set; }

        /// <summary>
        /// Gets or sets the type of the descriptor.
        /// </summary>
        /// <value>
        /// The type of the descriptor.
        /// </value>
        public DescriptorTypeEnum DescriptorType
        {
            get;
            private set;
        }

    }

}
