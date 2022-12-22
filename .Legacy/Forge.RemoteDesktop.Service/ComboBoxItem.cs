/* *********************************************************************
 * Date: 9 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.RemoteDesktop.Service.Properties;

namespace Forge.RemoteDesktop.Service
{

    /// <summary>
    /// ComboBox item
    /// </summary>
    /// <typeparam name="T">Type of the inner item</typeparam>
    [Serializable]
    public class ComboBoxItem<T>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ComboBoxItem{T}"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public ComboBoxItem(T item)
        {
            this.Item = item;
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        public T Item { get; private set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public object Tag { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Item == null ? Resources.NotSelected : Item.ToString();
        }

    }

}
