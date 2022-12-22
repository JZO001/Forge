/* *********************************************************************
 * Date: 11 Sep 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.IO;
using Forge.Invoker;

namespace Forge.RemoteDesktop.Client
{

    /// <summary>
    /// Extended implementation of file stream
    /// </summary>
    public sealed class ExtendedFileStream : FileStream
    {

        #region Field(s)

        private int mLastPercent = 0;

        private int mReadCounter = 0;

        /// <summary>
        /// Occurs when [event file read].
        /// </summary>
        public event EventHandler<FileStreamProgressEventArgs> EventFileRead;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedFileStream"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public ExtendedFileStream(string path)
            : base(path, FileMode.Open, FileAccess.Read, FileShare.Read)
        {
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Reads a block of bytes from the stream and writes the data in a given buffer.
        /// </summary>
        /// <param name="array">When this method returns, contains the specified byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1 /> replaced by the bytes read from the current source.</param>
        /// <param name="offset">The byte offset in <paramref name="array" /> at which the read bytes will be placed.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This might be less than the number of bytes requested if that number of bytes are not currently available, or zero if the end of the stream is reached.
        /// </returns>
        public override int Read(byte[] array, int offset, int count)
        {
            int readBytes = base.Read(array, offset, count);

            if (readBytes > 0)
            {
                OnFileRead();
            }

            return readBytes;
        }

        /// <summary>
        /// Reads a byte from the file and advances the read position one byte.
        /// </summary>
        /// <returns>
        /// The byte, cast to an <see cref="T:System.Int32" />, or -1 if the end of the stream has been reached.
        /// </returns>
        public override int ReadByte()
        {
            int count = base.ReadByte();

            if (count > 0)
            {
                OnFileRead();
            }

            return count;
        }

        #endregion

        #region Private method(s)

        private void OnFileRead()
        {
            mReadCounter++;
            int percent = Convert.ToInt32(Convert.ToDouble(Position) / (Convert.ToDouble(Length) / 100d));
            if (percent != mLastPercent || mReadCounter == 3)
            {
                mReadCounter = 0;
                mLastPercent = percent;
                Executor.Invoke(EventFileRead, this, new FileStreamProgressEventArgs(Length, Position, percent));
            }
        }

        #endregion

    }

}
