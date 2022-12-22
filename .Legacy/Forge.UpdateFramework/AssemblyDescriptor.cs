/* *********************************************************************
 * Date: 15 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Forge.UpdateFramework
{

    /// <summary>
    /// Represents an assembly data descriptor
    /// </summary>
    [Serializable]
    public class AssemblyDescriptor : FileWithHashCodeDescriptorBase
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyDescriptor"/> class.
        /// </summary>
        /// <param name="assemblyFileInfo">The assembly file info.</param>
        public AssemblyDescriptor(FileInfo assemblyFileInfo)
            : base(Guid.NewGuid().ToString(), DescriptorTypeEnum.Assembly, assemblyFileInfo)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.ReflectionOnlyLoadFrom(assemblyFileInfo.FullName);
            }
            catch (BadImageFormatException)
            {
                this.FileLoadResult = FileLoadResultEnum.BadFormat;
            }
            catch (FileLoadException)
            {
                this.FileLoadResult = FileLoadResultEnum.UnspecifiedError;
            }
            catch (Exception)
            {
            }
            if (assembly == null)
            {
                this.AssemblyType = AssemblyTypeEnum.Native;
            }
            else
            {
                this.AssemblyType = AssemblyTypeEnum.Managed;
                this.AssemblyQualifiedName = assembly.FullName;
                foreach (CustomAttributeData data in assembly.GetCustomAttributesData())
                {
                    if (typeof(GuidAttribute).Equals(data.Constructor.DeclaringType))
                    {
                        this.AssemblyGuid = data.ConstructorArguments[0].Value.ToString();
                        this.Id = this.AssemblyGuid;
                        break;
                    }
                }
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the type of the assembly.
        /// </summary>
        /// <value>
        /// The type of the assembly.
        /// </value>
        public AssemblyTypeEnum AssemblyType { get; private set; }

        /// <summary>
        /// Gets or sets the name of the assembly qualified.
        /// </summary>
        /// <value>
        /// The name of the assembly qualified.
        /// </value>
        public string AssemblyQualifiedName { get; private set; }

        /// <summary>
        /// Gets or sets the assembly GUID.
        /// </summary>
        /// <value>
        /// The assembly GUID.
        /// </value>
        public string AssemblyGuid { get; private set; }

        #endregion

    }

}
