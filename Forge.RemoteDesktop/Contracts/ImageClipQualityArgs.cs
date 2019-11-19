/* *********************************************************************
 * Date: 07 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.RemoteDesktop.Contracts
{

    /// <summary>
    /// Represents the image clip quality argument
    /// </summary>
    [Serializable]
    public class ImageClipQualityArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageClipQualityArgs"/> class.
        /// </summary>
        /// <param name="qualityPercent">The quality percent. -1 means auto quality control.</param>
        public ImageClipQualityArgs(int qualityPercent)
        {
            if (qualityPercent < -1 || qualityPercent > 100)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("qualityPercent");
            }

            this.QualityPercent = qualityPercent;
        }

        /// <summary>
        /// Gets the quality percent. -1 means auto quality control.
        /// </summary>
        /// <value>
        /// The quality percent.
        /// </value>
        public int QualityPercent { get; private set; }

    }

}
