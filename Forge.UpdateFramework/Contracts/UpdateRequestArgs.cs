/* *********************************************************************
 * Date: 16 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Shared;
using System;
using System.Collections.Generic;

namespace Forge.UpdateFramework.Contracts
{

    /// <summary>
    /// Represents a request for Update service
    /// </summary>
    [Serializable]
    public sealed class UpdateRequestArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateRequestArgs"/> class.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <param name="descriptors">The descriptors.</param>
        public UpdateRequestArgs(string productId, List<DescriptorBase> descriptors)
        {
            if (descriptors == null)
            {
                ThrowHelper.ThrowArgumentNullException("descriptors");
            }

            ProductId = productId;
            Descriptors = descriptors;
        }

        /// <summary>
        /// Gets the product id.
        /// </summary>
        public string ProductId { get; private set; }

        /// <summary>
        /// Gets the descriptors.
        /// </summary>
        public List<DescriptorBase> Descriptors { get; private set; }

    }

}
