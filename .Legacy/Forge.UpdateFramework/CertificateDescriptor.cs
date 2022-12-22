/* *********************************************************************
 * Date: 15 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Forge.UpdateFramework
{

    /// <summary>
    /// Represents the meta data of a certificate
    /// </summary>
    [Serializable]
    public class CertificateDescriptor : FileWithHashCodeDescriptorBase
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateDescriptor"/> class.
        /// </summary>
        /// <param name="fileInfo">The fi.</param>
        public CertificateDescriptor(FileInfo fileInfo)
            : base(Guid.NewGuid().ToString(), DescriptorTypeEnum.Certificate, fileInfo)
        {
            try
            {
                this.X509ContentType = X509Certificate2.GetCertContentType(fileInfo.FullName);
            }
            catch (Exception)
            {
                this.FileLoadResult = FileLoadResultEnum.BadFormat;
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the type of the X509 content.
        /// </summary>
        /// <value>
        /// The type of the X509 content.
        /// </value>
        public X509ContentType X509ContentType { get; private set; }

        #endregion

    }

}
