/* *********************************************************************
 * Date: 8 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Forge.Native.Hooks
{

    /// <summary>
    /// Represents a clipboard change event argument
    /// </summary>
    public class ClipboardChangedEventArgs : EventArgs
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardChangedEventArgs"/> class.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        public ClipboardChangedEventArgs(IDataObject dataObject)
        {
            this.DataObject = dataObject;
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Gets the data object.
        /// </summary>
        /// <value>
        /// The data object.
        /// </value>
        public IDataObject DataObject { get; private set; }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Determines whether this instance contains audio.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance contains audio; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsAudio()
        {
            return ((DataObject != null) && DataObject.GetDataPresent(DataFormats.WaveAudio, false));
        }

        /// <summary>
        /// Determines whether the specified format contains data.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        ///   <c>true</c> if the specified format contains data; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsData(string format)
        {
            return ((DataObject != null) && DataObject.GetDataPresent(format, false));
        }

        /// <summary>
        /// Determines whether [contains file drop list].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [contains file drop list]; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsFileDropList()
        {
            return ((DataObject != null) && DataObject.GetDataPresent(DataFormats.FileDrop, true));
        }

        /// <summary>
        /// Determines whether this instance contains image.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance contains image; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsImage()
        {
            return ((DataObject != null) && DataObject.GetDataPresent(DataFormats.Bitmap, true));
        }

        /// <summary>
        /// Determines whether this instance contains text.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance contains text; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsText()
        {
            if ((Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major >= 5))
            {
                return ContainsText(TextDataFormat.UnicodeText);
            }
            return ContainsText(TextDataFormat.Text);
        }

        /// <summary>
        /// Determines whether the specified format contains text.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        ///   <c>true</c> if the specified format contains text; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">format</exception>
        public bool ContainsText(TextDataFormat format)
        {
            if (!ClientUtils.IsEnumValid(format, (int)format, 0, 4))
            {
                throw new InvalidEnumArgumentException("format", (int)format, typeof(TextDataFormat));
            }
            return ((DataObject != null) && DataObject.GetDataPresent(ConvertToDataFormats(format), false));
        }

        /// <summary>
        /// Gets the audio stream.
        /// </summary>
        /// <returns></returns>
        public Stream GetAudioStream()
        {
            if (DataObject != null)
            {
                return (DataObject.GetData(DataFormats.WaveAudio, false) as Stream);
            }
            return null;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public object GetData(string format)
        {
            if (DataObject != null)
            {
                return DataObject.GetData(format);
            }
            return null;
        }

        /// <summary>
        /// Gets the file drop list.
        /// </summary>
        /// <returns></returns>
        public StringCollection GetFileDropList()
        {
            StringCollection strings = new StringCollection();
            if (DataObject != null)
            {
                string[] data = DataObject.GetData(DataFormats.FileDrop, true) as string[];
                if (data != null)
                {
                    strings.AddRange(data);
                }
            }
            return strings;
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <returns></returns>
        public Image GetImage()
        {
            if (DataObject != null)
            {
                return (DataObject.GetData(DataFormats.Bitmap, true) as Image);
            }
            return null;
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            if ((Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major >= 5))
            {
                return GetText(TextDataFormat.UnicodeText);
            }
            return GetText(TextDataFormat.Text);
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">format</exception>
        public string GetText(TextDataFormat format)
        {
            if (!ClientUtils.IsEnumValid(format, (int)format, 0, 4))
            {
                throw new InvalidEnumArgumentException("format", (int)format, typeof(TextDataFormat));
            }
            if (DataObject != null)
            {
                string data = DataObject.GetData(ConvertToDataFormats(format), false) as string;
                if (data != null)
                {
                    return data;
                }
            }
            return string.Empty;
        }

        #endregion

        #region Private method(s)

        private static string ConvertToDataFormats(TextDataFormat format)
        {
            switch (format)
            {
                case TextDataFormat.Text:
                    return DataFormats.Text;

                case TextDataFormat.UnicodeText:
                    return DataFormats.UnicodeText;

                case TextDataFormat.Rtf:
                    return DataFormats.Rtf;

                case TextDataFormat.Html:
                    return DataFormats.Html;

                case TextDataFormat.CommaSeparatedValue:
                    return DataFormats.CommaSeparatedValue;
            }

            return DataFormats.UnicodeText;
        }

        #endregion

    }

}
