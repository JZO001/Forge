/* *********************************************************************
 * Date: 12 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Shared;
using System;
using System.Diagnostics;
using System.Text;

namespace Forge.Net.Remoting.ProxyGenerator
{

    /// <summary>
    /// Type descriptor for generator
    /// </summary>
    internal class TypeDescriptor
    {

        #region Field(s)

        private static readonly string PackageServiceBaseName = "RemotingService";

        private static readonly string PackageClientBaseName = "RemotingClient";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mTypeNameServiceImpl = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mTypeNameClientImpl = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mTypeNameServiceAbstract = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mTypeNameClientAbstract = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mServicePackage = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mClientPackage = string.Empty;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeDescriptor"/> class.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        internal TypeDescriptor(Type contractType)
        {
            if (contractType == null)
            {
                ThrowHelper.ThrowArgumentNullException("contractType");
            }

            string typeName = contractType.FullName;
            string[] slices = typeName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            typeName = slices[slices.Length - 1];
            if (typeName.StartsWith("I") && typeName.Length > 1)
            {
                typeName = typeName.Substring(1);
            }
            mTypeNameServiceImpl = string.Format("{0}ServiceImpl", typeName);
            mTypeNameClientImpl = string.Format("{0}ClientImpl", typeName);
            mTypeNameServiceAbstract = string.Format("{0}AbstractServiceProxy", typeName);
            mTypeNameClientAbstract = string.Format("{0}AbstractClientProxy", typeName);

            StringBuilder sbForServicePackage = new StringBuilder();
            StringBuilder sbForClientPackage = new StringBuilder();
            if (slices.Length == 1)
            {
                // csak typeName létezik, nincs package
                sbForServicePackage.Append(PackageServiceBaseName);
                sbForClientPackage.Append(PackageClientBaseName);
            }
            else if (slices.Length == 2)
            {
                // package végére odaszúrom a packagenév kiegészítést
                sbForServicePackage.Append(slices[0]);
                sbForServicePackage.Append(PackageServiceBaseName);

                sbForClientPackage.Append(slices[0]);
                sbForClientPackage.Append(PackageClientBaseName);
            }
            else
            {
                // a package végéről leszedem az utolsót és kicserélem
                for (int i = 0; i < slices.Length - 2; i++)
                {
                    sbForServicePackage.Append(slices[i]);
                    sbForServicePackage.Append(".");

                    sbForClientPackage.Append(slices[i]);
                    sbForClientPackage.Append(".");
                }

                sbForServicePackage.Append(PackageServiceBaseName);
                sbForClientPackage.Append(PackageClientBaseName);
            }
            mServicePackage = sbForServicePackage.ToString();
            mClientPackage = sbForClientPackage.ToString();
        }

        #endregion

        #region Internal properties

        [DebuggerHidden]
        internal string TypeNameServiceImpl
        {
            get { return mTypeNameServiceImpl; }
        }

        [DebuggerHidden]
        internal string TypeNameClientImpl
        {
            get { return mTypeNameClientImpl; }
        }

        [DebuggerHidden]
        internal string TypeNameServiceAbstract
        {
            get { return mTypeNameServiceAbstract; }
        }

        [DebuggerHidden]
        internal string TypeNameClientAbstract
        {
            get { return mTypeNameClientAbstract; }
        }

        [DebuggerHidden]
        internal string ServicePackage
        {
            get { return mServicePackage; }
        }

        [DebuggerHidden]
        internal string ClientPackage
        {
            get { return mClientPackage; }
        }

        [DebuggerHidden]
        internal string TypeFullNameServiceAbstract
        {
            get { return string.Format("{0}.{1}", mServicePackage, mTypeNameServiceAbstract); }
        }

        [DebuggerHidden]
        internal string TypeFullNameClientAbstract
        {
            get { return string.Format("{0}.{1}", mClientPackage, mTypeNameClientAbstract); }
        }

        [DebuggerHidden]
        internal string TypeFullNameServiceImpl
        {
            get { return string.Format("{0}.{1}", mServicePackage, mTypeNameServiceImpl); }
        }

        [DebuggerHidden]
        internal string TypeFullNameClientImpl
        {
            get { return string.Format("{0}.{1}", mClientPackage, mTypeNameClientImpl); }
        }

        #endregion

    }

}
