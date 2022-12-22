/* *********************************************************************
 * Date: 11 Sep 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.RemoteDesktop.Client
{

    /// <summary>
    /// Represents the Extended File Stream read progress state
    /// </summary>
    public class FileStreamProgressEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStreamProgressEventArgs"/> class.
        /// </summary>
        /// <param name="length">The total bytes.</param>
        /// <param name="position">The available bytes.</param>
        /// <param name="percent">The percent.</param>
        public FileStreamProgressEventArgs(long length, long position, double percent)
        {
            this.Length = length;
            this.Position = position;
            this.Percent = percent;
        }

        /// <summary>
        /// Gets the total bytes.
        /// </summary>
        /// <value>
        /// The total bytes.
        /// </value>
        public long Length { get; private set; }

        /// <summary>
        /// Gets the available bytes.
        /// </summary>
        /// <value>
        /// The available bytes.
        /// </value>
        public long Position { get; private set; }

        /// <summary>
        /// Gets the percent.
        /// </summary>
        /// <value>
        /// The percent.
        /// </value>
        public double Percent { get; private set; }

    }

}
