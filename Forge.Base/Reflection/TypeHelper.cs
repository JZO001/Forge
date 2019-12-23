/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Forge.Collections;
using Forge.Logging;

namespace Forge.Reflection
{

    /// <summary>
    /// Helps to resolve a type from an assembly. Supports assembly dynamic loads.
    /// </summary>
    public static class TypeHelper
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(TypeHelper));

        private static readonly Dictionary<TypeKey, Type> mTypeKeyVsType = new Dictionary<TypeKey, Type>();

        private static readonly Dictionary<string, ListSpecialized<TypeKey>> mAssemblyNameVsTypeKey = new Dictionary<string, ListSpecialized<TypeKey>>();

        private static readonly object mLockObject = new object();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes the <see cref="TypeHelper"/> class.
        /// </summary>
        static TypeHelper()
        {
            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);
        }

        #endregion

        #region Private method(s)

        private static string BeginReadGenericData(string type, TypeLookupModeEnum typeLookupMode, bool findNewestTypeVersion, bool ignoreCase, ref int index)
        {
            string typeName = string.Empty;
            string typeChar = string.Empty;
            while (index < type.Length)
            {
                typeChar = type.Substring(index, 1);
                if ("[".Equals(typeChar))
                {
                    index++;
                    typeName = string.Format("{0}[{1}]", typeName, BeginReadInnerType(type, typeLookupMode, findNewestTypeVersion, ignoreCase, ref index).AssemblyQualifiedName);
                }
                else if ("]".Equals(typeChar))
                {
                    // egy generikus paraméter beolvasása megtörtént
                    break;
                }
                else
                {
                    typeName = typeName + typeChar;
                }
                index++;
            }

            return typeName;
        }

        private static Type BeginReadInnerType(string type, TypeLookupModeEnum typeLookupMode, bool findNewestTypeVersion, bool ignoreCase, ref int index)
        {
            string typeName = string.Empty;
            string typeChar = string.Empty;
            string assembly = string.Empty;
            bool saveAssemblyInfo = false;
            while (index < type.Length)
            {
                typeChar = type.Substring(index, 1);
                if ("[".Equals(typeChar))
                {
                    index++;
                    typeName = string.Format("{0}[{1}]", typeName, BeginReadGenericData(type, typeLookupMode, findNewestTypeVersion, ignoreCase, ref index));
                    saveAssemblyInfo = true;
                    assembly = string.Empty;
                }
                else if ("]".Equals(typeChar))
                {
                    // egy generikus paraméter beolvasása megtörtént
                    break;
                }
                else
                {
                    typeName = typeName + typeChar;
                    if (saveAssemblyInfo)
                    {
                        assembly = assembly + typeChar;
                    }
                }
                index++;
            }

            Type result = null;

            if (string.IsNullOrEmpty(assembly))
            {
                result = GetTypeFromString(null, typeName, typeLookupMode, findNewestTypeVersion, ignoreCase);
            }
            else
            {
                // generikus
                AssemblyName asmName = new AssemblyName(assembly.Substring(2, assembly.Length - 2));
                string typeNameStr = typeName.Substring(0, typeName.Length - assembly.Length);
                result = GetTypeFromString(asmName, typeNameStr, typeLookupMode, findNewestTypeVersion, ignoreCase);
            }

            return result;
        }

        private static Type GetTypeFromString(AssemblyName asmName, string typeName, TypeLookupModeEnum typeLookupMode, bool findNewestTypeVersion, bool ignoreCase)
        {
            Type result = null;

            TypeKey typeKey = new TypeKey(asmName, typeName, typeLookupMode, findNewestTypeVersion);

            lock (mLockObject)
            {
                if (mTypeKeyVsType.ContainsKey(typeKey))
                {
                    result = mTypeKeyVsType[typeKey];
                }
                else
                {
                    if (asmName != null)
                    {
                        try
                        {
                            AppDomain.CurrentDomain.Load(asmName);
                        }
                        catch (Exception ex)
                        {
                            // ilyen akkor van, ha a DLL nem betölthető (hibás, nem található, security)
                            if (LOGGER.IsDebugEnabled) LOGGER.Debug(ex.Message, ex);
                        }
                    }

                    if (typeName.Contains("["))
                    {
                        // ez egy array
                        string typeNameWithoutArrayMarks = typeName.Substring(0, typeName.IndexOf("["));
                        Type elementType = GetTypeFromString(asmName, typeNameWithoutArrayMarks, typeLookupMode, findNewestTypeVersion, ignoreCase);
                        result = Type.GetType(string.Format("{0}{1}, {2}", elementType.FullName, typeName.Substring(typeName.IndexOf("[")), elementType.Assembly.GetName().FullName));
                    }
                    else
                    {
                        // pontos betöltés
                        result = Type.GetType(typeName, false, ignoreCase);

                        if (result == null && asmName != null)
                        {
                            // betöltés assembly-vel
                            try
                            {
                                result = Type.GetType(string.Format("{0}, {1}", typeName, asmName.FullName), false, ignoreCase);
                            }
                            catch (Exception ex)
                            {
                                // ilyen akkor van, ha a DLL verziója más, mint ami az AppDomain-be be van töltve
                                if (LOGGER.IsDebugEnabled) LOGGER.Debug(ex.Message, ex);
                            }
                        }

                        // ha van assembly és nem engedélyezett az azonos verzió, akkor töröljük az eredményt
                        if ((typeLookupMode & TypeLookupModeEnum.AllowExactVersions) == 0 &&
                            asmName != null && asmName.Version != null && result != null &&
                            result.Assembly.GetName().Version.Equals(asmName.Version))
                        {
                            // azonos verzió nem engedélyezett
                            result = null;
                        }

                        if (result == null || findNewestTypeVersion)
                        {
                            if (result != null && findNewestTypeVersion && result.IsArray)
                            {
                                // tömböt máshogy kezeljük
                                Type elementType = result.GetElementType();
                                while (elementType.IsArray)
                                {
                                    elementType = elementType.GetElementType();
                                }
                                Type newestType = GetTypeFromString(elementType.AssemblyQualifiedName, typeLookupMode, true, false, ignoreCase);
                                if (!result.Assembly.GetName().FullName.Equals(newestType.Assembly.GetName().FullName))
                                {
                                    // újabb típus
                                    string newTypeStr = string.Format("{0}, {1}", result.FullName, newestType.Assembly.GetName().FullName);
                                    result = GetTypeFromString(newTypeStr, TypeLookupModeEnum.AllowExactVersions, false, false, false);
                                }
                            }
                            else
                            {
                                // manuális keresés az AppDomain assembly-k között
                                // 1, ide akkor jutunk, ha a DLL nem létezik vagy a partial name nincs megadva a qualifyAssembly szekcióban
                                // 2, engedélyeztük újabb verziók használatát
                                foreach (Assembly a in new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies()))
                                {
                                    if (asmName == null || a.GetName().Name.Equals(asmName.Name))
                                    {
                                        Type[] types = null;
                                        try
                                        {
                                            // találkoztam olyan dinamikus DLL-ekkel, amelyeknek a type-jait nem lehetett kiolvasni
                                            types = a.GetTypes();
                                        }
                                        catch (Exception) { }

                                        if (types != null)
                                        {
                                            foreach (Type t in a.GetTypes())
                                            {
                                                if (t.FullName.Equals(typeName))
                                                {
                                                    if (result == null)
                                                    {
                                                        // még nincs kiválasztott típus
                                                        if (asmName != null && asmName.Version != null)
                                                        {
                                                            // van kiinduló verzió
                                                            if ((typeLookupMode & TypeLookupModeEnum.AllowExactVersions) > 0 && asmName.Version.Equals(a.GetName().Version))
                                                            {
                                                                // azonos verzió engedélyezett
                                                                result = t;
                                                            }
                                                            else if ((typeLookupMode & TypeLookupModeEnum.AllowNewerVersions) > 0 && asmName.Version < a.GetName().Version)
                                                            {
                                                                // újabb verzió engedélyezett
                                                                result = t;
                                                            }
                                                            else if ((typeLookupMode & TypeLookupModeEnum.AllowOlderVersions) > 0 && asmName.Version > a.GetName().Version)
                                                            {
                                                                // régebbi verzió engedélyezett
                                                                result = t;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            // nincs kiinduló assembly verzió, nincs mihez viszonyítani a verzió, ezért az első alkalmas típus megfelelő
                                                            result = t;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        // már van egy kiválasztott típusunk, attól keresünk megfelelőbbet (vagyis újabb verziót)
                                                        if (asmName != null && asmName.Version != null)
                                                        {
                                                            // van kiinduló verzió
                                                            if ((typeLookupMode & TypeLookupModeEnum.AllowExactVersions) > 0 && asmName.Version.Equals(a.GetName().Version))
                                                            {
                                                                // azonos verzió engedélyezett
                                                                result = t;
                                                            }
                                                            else if ((typeLookupMode & TypeLookupModeEnum.AllowNewerVersions) > 0 && asmName.Version < a.GetName().Version)
                                                            {
                                                                // újabb verzió engedélyezett
                                                                result = t;
                                                            }
                                                            else if ((typeLookupMode & TypeLookupModeEnum.AllowOlderVersions) > 0 && asmName.Version > a.GetName().Version)
                                                            {
                                                                // régebbi verzió engedélyezett
                                                                if (result.Assembly.GetName().Version < a.GetName().Version)
                                                                {
                                                                    // ha már van exact vagy újabb verziónk, akkor attól régebbire már ne térjünk vissza
                                                                    result = t;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            // nincs referencia verzió assembly, a legfrissebb type keresése
                                                            if (result.Assembly.GetName().Version < a.GetName().Version)
                                                            {
                                                                // újabb verzió
                                                                result = t;
                                                            }
                                                        }
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (result == null)
                    {
                        throw new TypeLoadException(string.Format("Type '{0}' not found.", typeName));
                    }

                    string name = result.Assembly.GetName().Name;

                    lock (mAssemblyNameVsTypeKey)
                    {
                        ListSpecialized<TypeKey> typeKeys = null;
                        mTypeKeyVsType[typeKey] = result;
                        if (mAssemblyNameVsTypeKey.ContainsKey(name))
                        {
                            typeKeys = mAssemblyNameVsTypeKey[name];
                        }
                        else
                        {
                            typeKeys = new ListSpecialized<TypeKey>();
                            mAssemblyNameVsTypeKey[name] = typeKeys;
                        }
                        typeKeys.Add(typeKey);
                    }
                }
            }

            return result;
        }

        private static Type GetTypeFromString(string assemblyFullQualifiedName, TypeLookupModeEnum typeLookupMode, bool findNewestTypeVersion, bool throwOnError, bool ignoreCase, ref int index)
        {
            Type result = null;

            int i = assemblyFullQualifiedName.IndexOf(", ");

            try
            {
                if (assemblyFullQualifiedName.Contains("`"))
                {
                    // generikus típus
                    result = BeginReadInnerType(assemblyFullQualifiedName, typeLookupMode, findNewestTypeVersion, ignoreCase, ref index);
                }
                else if (i > 0)
                {
                    // normál assembly-vel
                    string assembly = assemblyFullQualifiedName.Substring(i + 1).Trim();
                    AssemblyName asmName = new AssemblyName(assembly);
                    string typeStrName = assemblyFullQualifiedName.Substring(0, i);
                    result = GetTypeFromString(asmName, typeStrName, typeLookupMode, findNewestTypeVersion, ignoreCase);
                }
                else
                {
                    // normál assembly nélkül
                    result = GetTypeFromString(null, assemblyFullQualifiedName, typeLookupMode, findNewestTypeVersion, ignoreCase);
                }
            }
            catch (TypeLoadException ex)
            {
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("TypeLoader, unable to load assembly: {0}", assemblyFullQualifiedName), ex);
                if (throwOnError)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("TypeLoader, unable to load assembly: {0}", assemblyFullQualifiedName), ex);
                if (throwOnError)
                {
                    throw new TypeLoadException(string.Format("Type '{0}' not found.", assemblyFullQualifiedName), ex);
                }
            }

            if (result == null && throwOnError)
            {
                throw new TypeLoadException(string.Format("Type '{0}' not found.", assemblyFullQualifiedName));
            }

            return result;
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            string name = args.LoadedAssembly.GetName().Name;
            lock (mAssemblyNameVsTypeKey)
            {
                if (mAssemblyNameVsTypeKey.ContainsKey(name))
                {
                    ListSpecialized<TypeKey> list = mAssemblyNameVsTypeKey[name];
                    IEnumeratorSpecialized<TypeKey> iterator = list.GetEnumerator();
                    while (iterator.MoveNext())
                    {
                        if (iterator.Current.TypeLookupMode != TypeLookupModeEnum.AllowExactVersions)
                        {
                            mTypeKeyVsType.Remove(iterator.Current);
                            iterator.Remove();
                        }
                    }
                }
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Gets the type from string.
        /// </summary>
        /// <param name="assemblyFullQualifiedName">Name of the assembly full qualified.</param>
        /// <returns>The type if it was resolved, otherwise False.</returns>
        /// <example>
        /// <code>
        /// Type type = null;
        ///
        /// type = TypeHelper.GetTypeFromString("System.Int32", true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString("System.Collections.Generic.Dictionary`2[[System.Int32],[System.Int64]]", true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString("System.Collections.Generic.Dictionary`2[[System.Int32],[System.Int64]][,]", true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(Dictionary&lt;int, Dictionary&lt;int, string&gt;&gt;).AssemblyQualifiedName);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(Dictionary&lt;int, Dictionary&lt;int, string&gt;&gt;[]).AssemblyQualifiedName);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(HashSet&lt;int?&gt;[][,]).AssemblyQualifiedName, true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(HashSet&lt;int?&gt;[][,]).AssemblyQualifiedName, TypeLookupModeEnum.AllowAll, true, false, false);
        /// Assert.IsFalse(type == null);
        /// </code>
        /// </example>
        public static Type GetTypeFromString(string assemblyFullQualifiedName)
        {
            return GetTypeFromString(assemblyFullQualifiedName, false);
        }

        /// <summary>
        /// Gets the type from string.
        /// </summary>
        /// <param name="assemblyFullQualifiedName">Name of the assembly full qualified.</param>
        /// <param name="allowDifferentTypeVersion">if set to <c>true</c> [allow different type version].</param>
        /// <returns>
        /// The type if it was resolved, otherwise False.
        /// </returns>
        /// <example>
        /// <code>
        /// Type type = null;
        ///
        /// type = TypeHelper.GetTypeFromString("System.Int32", true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString("System.Collections.Generic.Dictionary`2[[System.Int32],[System.Int64]]", true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString("System.Collections.Generic.Dictionary`2[[System.Int32],[System.Int64]][,]", true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(Dictionary&lt;int, Dictionary&lt;int, string&gt;&gt;).AssemblyQualifiedName);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(Dictionary&lt;int, Dictionary&lt;int, string&gt;&gt;[]).AssemblyQualifiedName);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(HashSet&lt;int?&gt;[][,]).AssemblyQualifiedName, true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(HashSet&lt;int?&gt;[][,]).AssemblyQualifiedName, TypeLookupModeEnum.AllowAll, true, false, false);
        /// Assert.IsFalse(type == null);
        /// </code>
        /// </example>
        public static Type GetTypeFromString(string assemblyFullQualifiedName, bool allowDifferentTypeVersion)
        {
            if (string.IsNullOrEmpty(assemblyFullQualifiedName))
            {
                ThrowHelper.ThrowArgumentNullException("assemblyFullQualifiedName");
            }

            int startIndex = 0;
            return GetTypeFromString(assemblyFullQualifiedName,
                allowDifferentTypeVersion ? TypeLookupModeEnum.AllowAll : TypeLookupModeEnum.AllowExactVersions, false, true, false,
                ref startIndex);
        }

        /// <summary>
        /// Gets the type from string.
        /// </summary>
        /// <param name="assemblyFullQualifiedName">Name of the assembly full qualified.</param>
        /// <param name="typeLookupMode">The type lookup mode.</param>
        /// <returns>The type if it was resolved, otherwise False.</returns>
        /// <example>
        /// <code>
        /// Type type = null;
        ///
        /// type = TypeHelper.GetTypeFromString("System.Int32", true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString("System.Collections.Generic.Dictionary`2[[System.Int32],[System.Int64]]", true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString("System.Collections.Generic.Dictionary`2[[System.Int32],[System.Int64]][,]", true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(Dictionary&lt;int, Dictionary&lt;int, string&gt;&gt;).AssemblyQualifiedName);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(Dictionary&lt;int, Dictionary&lt;int, string&gt;&gt;[]).AssemblyQualifiedName);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(HashSet&lt;int?&gt;[][,]).AssemblyQualifiedName, true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(HashSet&lt;int?&gt;[][,]).AssemblyQualifiedName, TypeLookupModeEnum.AllowAll, true, false, false);
        /// Assert.IsFalse(type == null);
        /// </code>
        /// </example>
        public static Type GetTypeFromString(string assemblyFullQualifiedName, TypeLookupModeEnum typeLookupMode)
        {
            if (string.IsNullOrEmpty(assemblyFullQualifiedName))
            {
                ThrowHelper.ThrowArgumentNullException("assemblyFullQualifiedName");
            }

            int startIndex = 0;
            return GetTypeFromString(assemblyFullQualifiedName, typeLookupMode, false, true, false, ref startIndex);
        }

        /// <summary>
        /// Gets the type from string.
        /// </summary>
        /// <param name="assemblyFullQualifiedName">Name of the assembly full qualified.</param>
        /// <param name="typeLookupMode">The type lookup mode.</param>
        /// <param name="throwOnError">if set to <c>true</c> [throw on error].</param>
        /// <returns>The type if it was resolved, otherwise False.</returns>
        /// <example>
        /// <code>
        /// Type type = null;
        ///
        /// type = TypeHelper.GetTypeFromString("System.Int32", true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString("System.Collections.Generic.Dictionary`2[[System.Int32],[System.Int64]]", true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString("System.Collections.Generic.Dictionary`2[[System.Int32],[System.Int64]][,]", true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(Dictionary&lt;int, Dictionary&lt;int, string&gt;&gt;).AssemblyQualifiedName);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(Dictionary&lt;int, Dictionary&lt;int, string&gt;&gt;[]).AssemblyQualifiedName);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(HashSet&lt;int?&gt;[][,]).AssemblyQualifiedName, true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(HashSet&lt;int?&gt;[][,]).AssemblyQualifiedName, TypeLookupModeEnum.AllowAll, true, false, false);
        /// Assert.IsFalse(type == null);
        /// </code>
        /// </example>
        public static Type GetTypeFromString(string assemblyFullQualifiedName, TypeLookupModeEnum typeLookupMode, bool throwOnError)
        {
            if (string.IsNullOrEmpty(assemblyFullQualifiedName))
            {
                ThrowHelper.ThrowArgumentNullException("assemblyFullQualifiedName");
            }

            int startIndex = 0;
            return GetTypeFromString(assemblyFullQualifiedName, typeLookupMode, false, throwOnError, false, ref startIndex);
        }

        /// <summary>
        /// Gets the type from string.
        /// </summary>
        /// <param name="assemblyFullQualifiedName">Name of the assembly full qualified.</param>
        /// <param name="typeLookupMode">The type lookup mode.</param>
        /// <param name="throwOnError">if set to <c>true</c> [throw on error].</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns>The type if it was resolved, otherwise False.</returns>
        /// <example>
        /// <code>
        /// Type type = null;
        ///
        /// type = TypeHelper.GetTypeFromString("System.Int32", true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString("System.Collections.Generic.Dictionary`2[[System.Int32],[System.Int64]]", true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString("System.Collections.Generic.Dictionary`2[[System.Int32],[System.Int64]][,]", true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(Dictionary&lt;int, Dictionary&lt;int, string&gt;&gt;).AssemblyQualifiedName);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(Dictionary&lt;int, Dictionary&lt;int, string&gt;&gt;[]).AssemblyQualifiedName);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(HashSet&lt;int?&gt;[][,]).AssemblyQualifiedName, true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(HashSet&lt;int?&gt;[][,]).AssemblyQualifiedName, TypeLookupModeEnum.AllowAll, true, false, false);
        /// Assert.IsFalse(type == null);
        /// </code>
        /// </example>
        public static Type GetTypeFromString(string assemblyFullQualifiedName, TypeLookupModeEnum typeLookupMode, bool throwOnError, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(assemblyFullQualifiedName))
            {
                ThrowHelper.ThrowArgumentNullException("assemblyFullQualifiedName");
            }

            int startIndex = 0;
            return GetTypeFromString(assemblyFullQualifiedName, typeLookupMode, false, throwOnError, ignoreCase, ref startIndex);
        }

        /// <summary>
        /// Gets the type from string.
        /// </summary>
        /// <param name="assemblyFullQualifiedName">Name of the assembly full qualified.</param>
        /// <param name="typeLookupMode">The type lookup mode.</param>
        /// <param name="findNewestTypeVersion">Find the newest version from a type is mandatory or not.</param>
        /// <param name="throwOnError">if set to <c>true</c> [throw on error].</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns>The type if it was resolved, otherwise False.</returns>
        /// <example>
        /// <code>
        /// Type type = null;
        ///
        /// type = TypeHelper.GetTypeFromString("System.Int32", true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString("System.Collections.Generic.Dictionary`2[[System.Int32],[System.Int64]]", true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString("System.Collections.Generic.Dictionary`2[[System.Int32],[System.Int64]][,]", true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(Dictionary&lt;int, Dictionary&lt;int, string&gt;&gt;).AssemblyQualifiedName);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(Dictionary&lt;int, Dictionary&lt;int, string&gt;&gt;[]).AssemblyQualifiedName);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(HashSet&lt;int?&gt;[][,]).AssemblyQualifiedName, true);
        /// Assert.IsFalse(type == null);
        ///
        /// type = TypeHelper.GetTypeFromString(typeof(HashSet&lt;int?&gt;[][,]).AssemblyQualifiedName, TypeLookupModeEnum.AllowAll, true, false, false);
        /// Assert.IsFalse(type == null);
        /// </code>
        /// </example>
        public static Type GetTypeFromString(string assemblyFullQualifiedName, TypeLookupModeEnum typeLookupMode, bool findNewestTypeVersion, bool throwOnError, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(assemblyFullQualifiedName))
            {
                ThrowHelper.ThrowArgumentNullException("assemblyFullQualifiedName");
            }

            int startIndex = 0;
            return GetTypeFromString(assemblyFullQualifiedName, typeLookupMode, findNewestTypeVersion, throwOnError, ignoreCase, ref startIndex);
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <typeparam name="T">Type of the Attribute</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>The attribute or null</returns>
        public static T GetAttribute<T>(Type type) where T : Attribute
        {
            if (type == null)
            {
                ThrowHelper.ThrowArgumentNullException("type");
            }

            T result = default(T);

            object[] attrs = type.GetCustomAttributes(typeof(T), false);
            if (attrs.Length > 0 && (attrs[0] as T) != null)
            {
                result = attrs[0] as T;
            }

            return result;
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <typeparam name="T">Type of the Attribute</typeparam>
        /// <param name="mi">The mi.</param>
        /// <returns>The attribute or null</returns>
        public static T GetAttribute<T>(MethodInfo mi) where T : Attribute
        {
            if (mi == null)
            {
                ThrowHelper.ThrowArgumentNullException("mi");
            }

            T result = default(T);

            object[] attrs = mi.GetCustomAttributes(typeof(T), false);
            if (attrs.Length > 0 && (attrs[0] as T) != null)
            {
                result = attrs[0] as T;
            }

            return result;
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <typeparam name="T">Type of the Attribute</typeparam>
        /// <param name="fi">The fi.</param>
        /// <returns>The attribute or null</returns>
        public static T GetAttribute<T>(FieldInfo fi) where T : Attribute
        {
            if (fi == null)
            {
                ThrowHelper.ThrowArgumentNullException("fi");
            }

            T result = default(T);

            object[] attrs = fi.GetCustomAttributes(typeof(T), false);
            if (attrs.Length > 0 && (attrs[0] as T) != null)
            {
                result = attrs[0] as T;
            }

            return result;
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <typeparam name="T">Type of the Attribute</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>The attribute or empty collection</returns>
        public static ICollection<T> GetAttributes<T>(Type type) where T : Attribute
        {
            if (type == null)
            {
                ThrowHelper.ThrowArgumentNullException("type");
            }

            ICollection<T> result = new List<T>();

            object[] attrs = type.GetCustomAttributes(typeof(T), false);
            if (attrs.Length > 0)
            {
                foreach (object o in attrs)
                {
                    if ((o as T) != null) result.Add(o as T);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <typeparam name="T">Type of the Attribute</typeparam>
        /// <param name="mi">The mi.</param>
        /// <returns>The attribute or empty collection</returns>
        public static ICollection<T> GetAttributes<T>(MethodInfo mi) where T : Attribute
        {
            if (mi == null)
            {
                ThrowHelper.ThrowArgumentNullException("mi");
            }

            ICollection<T> result = new List<T>();

            object[] attrs = mi.GetCustomAttributes(typeof(T), false);
            if (attrs.Length > 0)
            {
                foreach (object o in attrs)
                {
                    if ((o as T) != null) result.Add(o as T);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <typeparam name="T">Type of the Attribute</typeparam>
        /// <param name="fi">The fi.</param>
        /// <returns>The attribute or empty collection</returns>
        public static ICollection<T> GetAttributes<T>(FieldInfo fi) where T : Attribute
        {
            if (fi == null)
            {
                ThrowHelper.ThrowArgumentNullException("fi");
            }

            ICollection<T> result = new List<T>();

            object[] attrs = fi.GetCustomAttributes(typeof(T), false);
            if (attrs.Length > 0)
            {
                foreach (object o in attrs)
                {
                    if ((o as T) != null) result.Add(o as T);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the field by name.
        /// </summary>
        /// <param name="o">The object</param>
        /// <param name="name">The field name</param>
        /// <exception cref="MissingFieldException">Throws when the field with the specified name in the object was not found.</exception>
        /// <returns>FieldInfo</returns>
        public static FieldInfo GetFieldByName(object o, string name)
        {
            if (o == null)
            {
                ThrowHelper.ThrowArgumentNullException("o");
            }
            if (String.IsNullOrEmpty(name))
            {
                ThrowHelper.ThrowArgumentNullException("name");
            }

            FieldInfo field = null;
            Type objectType = o.GetType();
            while (!objectType.Equals(typeof(object)))
            {
                field = objectType.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (field != null) break;
                objectType = objectType.BaseType;
            }
            if (field == null)
            {
                throw new MissingFieldException(String.Format("Field has not found. Field name: '{0}', Entity type: '{1}'.", name, o.GetType().FullName));
            }
            return field;
        }

        /// <summary>
        /// Gets the property by name.
        /// </summary>
        /// <param name="o">The object</param>
        /// <param name="name">The property name</param>
        /// <returns>The property info</returns>
        public static PropertyInfo GetPropertyByName(object o, string name)
        {
            if (o == null)
            {
                ThrowHelper.ThrowArgumentNullException("o");
            }
            if (String.IsNullOrEmpty(name))
            {
                ThrowHelper.ThrowArgumentNullException("name");
            }

            PropertyInfo prop = null;
            Type objectType = o.GetType();
            while (!objectType.Equals(typeof(object)))
            {
                prop = objectType.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (prop != null) break;
                objectType = objectType.BaseType;
            }
            if (prop == null)
            {
                throw new MissingMemberException(String.Format("Property has not found. Property name: '{0}', Entity type: '{1}'.", name, o.GetType().FullName));
            }
            return prop;
        }

        #endregion

        #region Nested classes

        [DebuggerDisplay("[{GetType().Name}, TypeLookupMode = {TypeLookupMode}, IsAsmNameNull = {AsmName == null}, TypeName = {TypeName}]")]
        private sealed class TypeKey
        {

            /// <summary>
            /// Initializes a new instance of the <see cref="TypeKey" /> class.
            /// </summary>
            /// <param name="asmName">Name of the asm.</param>
            /// <param name="typeName">Name of the type.</param>
            /// <param name="typeLookupMode">The type lookup mode.</param>
            /// <param name="findNewestTypeVersion">if set to <c>true</c> [find newest type version].</param>
            internal TypeKey(AssemblyName asmName, string typeName, TypeLookupModeEnum typeLookupMode, bool findNewestTypeVersion)
            {
                this.AsmName = asmName;
                this.TypeName = typeName;
                this.TypeLookupMode = typeLookupMode;
                this.FindNewestTypeVersion = findNewestTypeVersion;
            }

            /// <summary>
            /// Gets or sets the name of the asm.
            /// </summary>
            /// <value>
            /// The name of the asm.
            /// </value>
            internal AssemblyName AsmName { get; private set; }

            /// <summary>
            /// Gets or sets the name of the type.
            /// </summary>
            /// <value>
            /// The name of the type.
            /// </value>
            internal string TypeName { get; private set; }

            /// <summary>
            /// Gets or sets the type lookup mode.
            /// </summary>
            /// <value>
            /// The type lookup mode.
            /// </value>
            internal TypeLookupModeEnum TypeLookupMode { get; private set; }

            /// <summary>
            /// Gets or sets a value indicating whether [find newest type version].
            /// </summary>
            /// <value>
            /// <c>true</c> if [find newest type version]; otherwise, <c>false</c>.
            /// </value>
            internal bool FindNewestTypeVersion { get; private set; }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
            /// <returns>
            ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
            /// </returns>
            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (!obj.GetType().Equals(GetType())) return false;

                bool result = false;

                TypeKey other = (TypeKey)obj;
                if (other.TypeName.Equals(this.TypeName))
                {
                    int localHash = this.GetHashCode();
                    int otherHash = other.GetHashCode();
                    if (localHash == otherHash && other.AsmName == null && this.AsmName == null)
                    {
                        result = true;
                    }
                    else if (localHash == otherHash &&
                        other.AsmName != null && this.AsmName != null &&
                        other.AsmName.KeyPair != null && this.AsmName.KeyPair != null)
                    {
                        result = Arrays.DeepEquals(other.AsmName.KeyPair.PublicKey, this.AsmName.KeyPair.PublicKey);
                    }
                    else if (localHash == otherHash &&
                        other.AsmName != null && this.AsmName != null &&
                        other.AsmName.KeyPair == null && this.AsmName.KeyPair == null)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }

                return result;
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                int hash = 7;
                if (AsmName != null)
                {
                    if (AsmName.CodeBase != null)
                    {
                        hash = 18 * hash + AsmName.CodeBase.GetHashCode();
                    }
                    if (AsmName.CultureInfo != null)
                    {
                        hash = 31 * hash + AsmName.CultureInfo.GetHashCode();
                    }
                    if (AsmName.EscapedCodeBase != null)
                    {
                        hash = 24 * hash + AsmName.EscapedCodeBase.GetHashCode();
                    }
                    hash = 2 * hash + AsmName.Flags.GetHashCode();
                    if (AsmName.FullName != null)
                    {
                        hash = 9 * hash + AsmName.FullName.GetHashCode();
                    }
                    hash = 10 * hash + AsmName.HashAlgorithm.GetHashCode();
                    if (AsmName.Name != null)
                    {
                        hash = 14 * hash + AsmName.Name.GetHashCode();
                    }
                    hash = 3 * hash + AsmName.ProcessorArchitecture.GetHashCode();
                    if (AsmName.Version != null)
                    {
                        hash = 34 * hash + AsmName.Version.ToString().GetHashCode();
                    }
                    hash = 4 * hash + AsmName.VersionCompatibility.GetHashCode();
                }
                hash = 5 * hash + TypeName.GetHashCode();
                hash = 9 * hash + TypeLookupMode.GetHashCode();
                hash = 17 * hash + FindNewestTypeVersion.GetHashCode();
                return hash;
            }

        }

        #endregion

    }

}
