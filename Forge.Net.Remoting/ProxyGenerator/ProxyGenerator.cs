/* *********************************************************************
 * Date: 12 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Forge.Collections;
using Forge.Net.Remoting.Proxy;
using Forge.Net.Remoting.Validators;
using Forge.Reflection;

namespace Forge.Net.Remoting.ProxyGenerator
{

    /// <summary>
    /// Generate client and server side objects
    /// </summary>
    /// <typeparam name="TContract">The type of the contract.</typeparam>
    public sealed class ProxyGenerator<TContract> where TContract : IRemoteContract
    {

        #region Field(s)

        private readonly WellKnownObjectModeEnum mWellKnownObjectMode = WellKnownObjectModeEnum.PerSession;

        private readonly List<MethodComparator> mServiceMethods = new List<MethodComparator>();

        private readonly List<MethodComparator> mClientMethods = new List<MethodComparator>();

        private readonly ListSpecialized<MethodComparator> mNeutralMethods = new ListSpecialized<MethodComparator>();

        private readonly ListSpecialized<PropertyComparator> mNeutralProperties = new ListSpecialized<PropertyComparator>();

        private readonly ListSpecialized<EventComparator> mNeutralEvents = new ListSpecialized<EventComparator>();

        private bool mIsCollected = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyGenerator&lt;TContract&gt;"/> class.
        /// </summary>
        public ProxyGenerator()
        {
            ContractValidator.ValidateContractIntegrity(ContractType);
            ContractValidator.GetWellKnownObjectMode(ContractType, out mWellKnownObjectMode);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the type of the contract.
        /// </summary>
        /// <value>
        /// The type of the contract.
        /// </value>
        public Type ContractType
        {
            get { return typeof(TContract); }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Generates this instance.
        /// </summary>
        /// <param name="outputDir">The output dir.</param>
        /// <exception cref="ProxyGenerationFailedException">Failed to generate proxy classes.</exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Generate(string outputDir)
        {
            if (outputDir == null)
            {
                ThrowHelper.ThrowArgumentNullException("outputDir");
            }

            if (!mIsCollected)
            {
                // reset fields
                mServiceMethods.Clear();
                mClientMethods.Clear();
                mNeutralMethods.Clear();
                mNeutralProperties.Clear();
                mNeutralEvents.Clear();

                // collect methods
                CollectMethods(ContractType);
                mIsCollected = true;
            }

            FileStream fs = null;
            try
            {
                TypeDescriptor descriptor = new TypeDescriptor(ContractType);
                {
                    DirectoryInfo outDir = new DirectoryInfo(outputDir);
                    if (!outDir.Exists)
                    {
                        outDir.Create();
                    }
                }

                // generate service class(es)
                bool writeOverride = false;
                if (mWellKnownObjectMode == WellKnownObjectModeEnum.PerSession && mClientMethods.Count > 0)
                {
                    // write abstract class
                    FileInfo abstractFile = new FileInfo(Path.Combine(outputDir, String.Format("{0}.cs", descriptor.TypeNameServiceAbstract)));
                    using (fs = new FileStream(abstractFile.FullName, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        GeneratorBase.WriteAbstractProxyClassHeader(ContractType, true, fs);
                        GeneratorBase.WriteEvents(mNeutralEvents, fs);
                        GeneratorBase.WriteProxyContructor(false, descriptor.TypeNameServiceAbstract, fs);
                        GeneratorBase.WriteAbstractProperties(mNeutralProperties, fs);
                        GeneratorBase.WriteAbstractMethods(mServiceMethods, fs);
                        GeneratorBase.WriteAbstractMethods(mNeutralMethods, fs);
                        IEnumerator<MethodComparator> iterator = mClientMethods.GetEnumerator();
                        while (iterator.MoveNext())
                        {
                            MethodComparator mc = iterator.Current;
                            ServiceSideGenerator.GenerateServiceMethod(ContractType, mc.Method, fs);
                        }
                        GeneratorBase.WriteEndClass(fs);
                    }
                    writeOverride = true;
                }
                {
                    FileInfo implFile = new FileInfo(Path.Combine(outputDir, String.Format("{0}.cs", descriptor.TypeNameServiceImpl)));
                    using (fs = new FileStream(implFile.FullName, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        String baseType = string.Empty;
                        if (mWellKnownObjectMode == WellKnownObjectModeEnum.PerSession)
                        {
                            if (mClientMethods.Count > 0)
                            {
                                baseType = descriptor.TypeFullNameServiceAbstract;
                            }
                            else
                            {
                                baseType = typeof(ProxyBase).FullName;
                            }
                        }
                        GeneratorBase.WriteImplementationClassHeader(ContractType, baseType, true, fs);
                        if (!writeOverride)
                        {
                            GeneratorBase.WriteEvents(mNeutralEvents, fs);
                        }
                        if (mWellKnownObjectMode == WellKnownObjectModeEnum.PerSession)
                        {
                            GeneratorBase.WriteProxyContructor(true, descriptor.TypeNameServiceImpl, fs);
                        }
                        else
                        {
                            GeneratorBase.WriteEmptyContructor(true, descriptor.TypeNameServiceImpl, fs);
                        }
                        GeneratorBase.WriteProperties(mNeutralProperties, writeOverride, fs);
                        GeneratorBase.WriteMethods(mServiceMethods, writeOverride, fs);
                        GeneratorBase.WriteMethods(mNeutralMethods, writeOverride, fs);
                        GeneratorBase.WriteEndClass(fs);
                    }
                }

                // generate client class(es)
                writeOverride = false;
                bool disposeDetected = typeof(IDisposable).IsAssignableFrom(ContractType) && mNeutralMethods.Count == 1;
                if ((mClientMethods.Count > 0 || mNeutralMethods.Count > 0) && !disposeDetected)
                {
                    // write abstract class
                    FileInfo abstractFile = new FileInfo(Path.Combine(outputDir, String.Format("{0}.cs", descriptor.TypeNameClientAbstract)));
                    using (fs = new FileStream(abstractFile.FullName, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        GeneratorBase.WriteAbstractProxyClassHeader(ContractType, false, fs);
                        GeneratorBase.WriteEvents(mNeutralEvents, fs);
                        GeneratorBase.WriteProxyContructor(false, descriptor.TypeNameClientAbstract, fs);
                        GeneratorBase.WriteAbstractProperties(mNeutralProperties, fs);
                        GeneratorBase.WriteAbstractMethods(mClientMethods, fs);
                        GeneratorBase.WriteAbstractMethods(mNeutralMethods, fs);
                        IEnumerator<MethodComparator> iterator = mServiceMethods.GetEnumerator();
                        while (iterator.MoveNext())
                        {
                            MethodComparator mc = iterator.Current;
                            ClientSideGenerator.GenerateServiceMethod(ContractType, mc.Method, fs);
                        }
                        GeneratorBase.WriteEndClass(fs);
                    }
                    writeOverride = true;
                }
                {
                    FileInfo implFile = new FileInfo(Path.Combine(outputDir, String.Format("{0}.cs", descriptor.TypeNameClientImpl)));
                    using (fs = new FileStream(implFile.FullName, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        String baseType = typeof(ProxyBase).FullName;
                        if ((mClientMethods.Count > 0 || mNeutralMethods.Count > 0) && !disposeDetected)
                        {
                            baseType = descriptor.TypeFullNameClientAbstract;
                        }
                        GeneratorBase.WriteImplementationClassHeader(ContractType, baseType, false, fs);
                        if (!writeOverride)
                        {
                            GeneratorBase.WriteEvents(mNeutralEvents, fs);
                        }
                        GeneratorBase.WriteProxyContructor(true, descriptor.TypeNameClientImpl, fs);
                        GeneratorBase.WriteProperties(mNeutralProperties, writeOverride, fs);
                        GeneratorBase.WriteMethods(mClientMethods, writeOverride, fs);

                        if (disposeDetected)
                        {
                            // ha az IDisposable interface implementálva van, akkor a dispose() metódust nem szabad generálni kliens oldalon, mert a ProxyBase-ben már benne van
                            IEnumeratorSpecialized<MethodComparator> iterator = mNeutralMethods.GetEnumerator();
                            while (iterator.MoveNext())
                            {
                                MethodComparator mc = iterator.Current;
                                if (mc.Method.Name.Equals("Dispose") && mc.Method.GetParameters().Length == 0)
                                {
                                    iterator.Remove();
                                    break;
                                }
                            }
                        }
                        GeneratorBase.WriteMethods(mNeutralMethods, writeOverride, fs);

                        if (!((mClientMethods.Count > 0 || mNeutralMethods.Count > 0) && !disposeDetected))
                        {
                            IEnumerator<MethodComparator> iterator = mServiceMethods.GetEnumerator();
                            while (iterator.MoveNext())
                            {
                                MethodComparator mc = iterator.Current;
                                ClientSideGenerator.GenerateServiceMethod(ContractType, mc.Method, fs);
                            }
                        }
                        GeneratorBase.WriteEndClass(fs);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ProxyGenerationFailedException("Failed to generate proxy classes.", ex);
            }
        }

        #endregion

        #region Private method(s)

        private void CollectMethods(Type type)
        {
            foreach (MethodInfo m in type.GetMethods())
            {
                MethodComparator cmp = new MethodComparator(m);
                if (!mServiceMethods.Contains(cmp) && !mClientMethods.Contains(cmp) && !mNeutralMethods.Contains(cmp))
                {
                    // még nem detektált metódus
                    OperationContractAttribute ocAnnotation = TypeHelper.GetAttribute<OperationContractAttribute>(m);
                    if (ocAnnotation == null)
                    {
                        if (mWellKnownObjectMode == WellKnownObjectModeEnum.PerSession && typeof(IDisposable).IsAssignableFrom(type) && m.Name.Equals("Dispose"))
                        {
                            // perSession módban IDisposable-t nem dolgozunk fel, mert már implementálva van a ProxyBase-ben
                        }
                        else if (!m.IsSpecialName)
                        {
                            mNeutralMethods.Add(cmp); // a contract-ban nem vesz részt, de az interface-en valahol rajta van
                        }
                    }
                    else if (ocAnnotation.Direction.Equals(OperationDirectionEnum.ServerSide))
                    {
                        mServiceMethods.Add(cmp);
                    }
                    else
                    {
                        mClientMethods.Add(cmp);
                    }
                }
            }
            foreach (PropertyInfo pi in type.GetProperties())
            {
                PropertyComparator pc = new PropertyComparator(pi);
                if (!mNeutralProperties.Contains(pc))
                {
                    mNeutralProperties.Add(pc);
                }
            }
            foreach (EventInfo ei in type.GetEvents())
            {
                mNeutralEvents.Add(new EventComparator(ei));
            }
            if (type.GetInterfaces().Length > 0)
            {
                foreach (Type cls in type.GetInterfaces())
                {
                    CollectMethods(cls);
                }
            }
        }

        #endregion

    }

}
