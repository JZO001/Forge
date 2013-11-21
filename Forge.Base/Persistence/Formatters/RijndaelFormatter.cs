/* *********************************************************************
 * Date: 9 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Forge.Persistence.Formatters
{

    /// <summary>
    /// X509 Binary serializer formatter
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    public sealed class RijndaelFormatter<T> : IDataFormatter<T>
    {

        #region Field(s)

        private IDataFormatter<T> mInternalFormatter = new BinaryFormatter<T>();

        private X509Certificate2 mCertificate = null;

        private byte[] IV = new byte[16];

        private byte[] Key = new byte[32];

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="RijndaelFormatter{T}" /> class.
        /// </summary>
        /// <param name="certificate">The certificate.</param>
        public RijndaelFormatter(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                ThrowHelper.ThrowArgumentNullException("certificate");
            }

            this.mCertificate = certificate;

            Buffer.BlockCopy(certificate.PublicKey.EncodedKeyValue.RawData, 0, IV, 0, IV.Length);
            Buffer.BlockCopy(certificate.PublicKey.EncodedKeyValue.RawData, certificate.PublicKey.EncodedKeyValue.RawData.Length - Key.Length, Key, 0, Key.Length);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RijndaelFormatter{T}" /> class.
        /// </summary>
        /// <param name="certificate">The certificate.</param>
        /// <param name="internalFormatter">The internal formatter.</param>
        public RijndaelFormatter(X509Certificate2 certificate, IDataFormatter<T> internalFormatter)
            : this(certificate)
        {
            this.mInternalFormatter = internalFormatter;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the certificate.
        /// </summary>
        /// <value>
        /// The certificate.
        /// </value>
        public X509Certificate2 Certificate
        {
            get { return mCertificate; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }

                mCertificate = value;
            }
        }

        /// <summary>
        /// Gets or sets the internal formatter.
        /// </summary>
        /// <value>
        /// The internal formatter.
        /// </value>
        public IDataFormatter<T> InternalFormatter
        {
            get { return mInternalFormatter; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }

                mInternalFormatter = value;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Determines whether this instance can read the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>
        ///   <c>true</c> if this instance can read the specified stream; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">stream</exception>
        public bool CanRead(Stream stream)
        {
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            bool result = true;
            try
            {
                long pos = stream.Position;
                try
                {
                    using (RijndaelManaged r = new RijndaelManaged())
                    {
                        r.IV = IV;
                        r.Key = Key;
                        using (ICryptoTransform decryptor = r.CreateDecryptor())
                        {
                            CryptoStream csDecrypt = new CryptoStream(stream, decryptor, CryptoStreamMode.Read);
                            this.mInternalFormatter.Read(csDecrypt);
                        }
                    }
                }
                catch (Exception)
                {
                    result = false;
                }
                finally
                {
                    stream.Position = pos;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        /// <summary>
        /// Determines whether this instance can write the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if this instance can write the specified item; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">item</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public bool CanWrite(T item)
        {
            if (item == null)
            {
                ThrowHelper.ThrowArgumentNullException("item");
            }

            bool result = false;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    this.mInternalFormatter.Write(ms, item);
                    ms.Position = 0;

                    using (RijndaelManaged r = new RijndaelManaged())
                    {
                        r.IV = IV;
                        r.Key = Key;
                        using (ICryptoTransform encryptor = r.CreateEncryptor())
                        {
                            using (MemoryStream temp = new MemoryStream())
                            {
                                using (CryptoStream csEncrypt = new CryptoStream(temp, encryptor, CryptoStreamMode.Write))
                                {
                                    csEncrypt.Write(ms.ToArray(), 0, (int)ms.Length);
                                    csEncrypt.FlushFinalBlock();
                                    temp.SetLength(0);
                                }
                            }
                        }
                    }

                    result = true;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">stream</exception>
        /// <exception cref="System.FormatException"></exception>
        public T Read(Stream stream)
        {
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            try
            {
                using (RijndaelManaged r = new RijndaelManaged())
                {
                    r.IV = IV;
                    r.Key = Key;
                    using (ICryptoTransform decryptor = r.CreateDecryptor())
                    {
                        CryptoStream csDecrypt = new CryptoStream(stream, decryptor, CryptoStreamMode.Read);
                        return (T)this.mInternalFormatter.Read(csDecrypt);
                    }
                }
            }
            catch (FormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FormatException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="data">The data.</param>
        /// <exception cref="System.ArgumentNullException">
        /// stream
        /// or
        /// data
        /// </exception>
        /// <exception cref="System.FormatException"></exception>
        public void Write(Stream stream, T data)
        {
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }
            if (data == null)
            {
                ThrowHelper.ThrowArgumentNullException("data");
            }

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    this.mInternalFormatter.Write(ms, data);
                    ms.Position = 0;

                    using (RijndaelManaged r = new RijndaelManaged())
                    {
                        r.IV = IV;
                        r.Key = Key;
                        using (ICryptoTransform encryptor = r.CreateEncryptor())
                        {
                            CryptoStream csEncrypt = new CryptoStream(stream, encryptor, CryptoStreamMode.Write);
                            csEncrypt.Write(ms.ToArray(), 0, (int)ms.Length);
                            csEncrypt.FlushFinalBlock();
                        }
                    }
                }
            }
            catch (FormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FormatException(ex.Message, ex);
            }
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new RijndaelFormatter<T>(this.mCertificate, this.mInternalFormatter);
        }

        #endregion

    }

}
