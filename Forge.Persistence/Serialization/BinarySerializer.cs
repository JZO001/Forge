/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using Forge.Reflection;

namespace Forge.Persistence.Serialization
{

    /// <summary>
    /// Binary serializer
    /// </summary>
    [ComVisible(true)]
    public sealed class BinarySerializer
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySerializer"/> class.
        /// </summary>
        public BinarySerializer()
        {
            this.TypeLookupMode = TypeLookupModeEnum.AllowExactVersions;
            this.Context = new StreamingContext();
            SurrogateSelector ss = new SurrogateSelector();
            this.Selector = ss;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySerializer" /> class.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="context">The context.</param>
        public BinarySerializer(ISurrogateSelector selector, StreamingContext context)
        {
            this.TypeLookupMode = TypeLookupModeEnum.AllowExactVersions; ;
            this.Context = context;
            this.Selector = selector;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public StreamingContext Context { get; set; }

        /// <summary>
        /// Gets or sets the selector.
        /// </summary>
        /// <value>
        /// The selector.
        /// </value>
        public ISurrogateSelector Selector { get; set; }

        /// <summary>
        /// Gets or sets the serializer behavior.
        /// </summary>
        /// <value>
        /// The serializer behavior.
        /// </value>
        public BinarySerializerBehaviorEnum SerializerBehavior
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type lookup mode.
        /// </summary>
        /// <value>
        /// The type lookup mode.
        /// </value>
        public TypeLookupModeEnum TypeLookupMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [find newest type version].
        /// </summary>
        /// <value>
        /// <c>true</c> if [find newest type version]; otherwise, <c>false</c>.
        /// </value>
        public bool FindNewestTypeVersion { get; set; }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Serializes the specified serialization stream.
        /// </summary>
        /// <param name="serializationStream">The serialization stream.</param>
        /// <param name="graph">The graph.</param>
        [SecurityCritical]
        public void Serialize(Stream serializationStream, object graph)
        {
            if (serializationStream == null)
            {
                ThrowHelper.ThrowArgumentNullException("serializationStream");
            }

            new SerializationContext(this.Selector, this.Context).Serialize(serializationStream, graph);
        }

        /// <summary>
        /// Deserializes the specified serialization stream.
        /// </summary>
        /// <param name="serializationStream">The serialization stream.</param>
        /// <returns>The object</returns>
        /// <exception cref="System.Runtime.Serialization.SerializationException">Stream content is invalid or the object version changed.</exception>
        [SecurityCritical]
        public object Deserialize(Stream serializationStream)
        {
            if (serializationStream == null)
            {
                ThrowHelper.ThrowArgumentNullException("serializationStream");
            }

            try
            {
                return new DeserializationContext(this.Selector, this.Context, this.SerializerBehavior, this.TypeLookupMode, this.FindNewestTypeVersion).Deserialize(serializationStream);
            }
            catch (MissingFieldException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Stream content is invalid or the object version changed.", ex);
            }
        }

        #endregion

        #region Nested classes

        internal abstract class ContextBase
        {

            protected static readonly HashSet<Type> mSystemByteTypes = new HashSet<Type>();

            /// <summary>
            /// Initializes the <see cref="ContextBase"/> class.
            /// </summary>
            static ContextBase()
            {
                mSystemByteTypes.Add(typeof(bool?));
                mSystemByteTypes.Add(typeof(byte?));
                mSystemByteTypes.Add(typeof(bool));
                mSystemByteTypes.Add(typeof(byte));
            }

        }

        internal sealed class SerializationContext : ContextBase
        {

            #region Field(s)

            private static readonly HashSet<Type> mSystemTypes = new HashSet<Type>();

            /// <summary>
            /// Referencia azonosító vs referencia leíró
            /// </summary>
            private readonly Dictionary<long, STypeInstanceProxy> mReferenceVsTypeInstances = new Dictionary<long, STypeInstanceProxy>();

            /// <summary>
            /// The assemblies
            /// </summary>
            private readonly Dictionary<Assembly, int> mAssemblyVsAssemblyId = new Dictionary<Assembly, int>();

            /// <summary>
            /// The assembly id vs assembly
            /// </summary>
            private readonly Dictionary<int, Assembly> mAssemblyIdVsAssembly = new Dictionary<int, Assembly>();

            /// <summary>
            /// Típus azonosító vs típus info
            /// </summary>
            private readonly Dictionary<int, STypeInfo> mTypeIdVsTypeInfo = new Dictionary<int, STypeInfo>();

            /// <summary>
            /// Típus vs típus info
            /// </summary>
            private readonly Dictionary<Type, STypeInfo> mTypeVsTypeInfo = new Dictionary<Type, STypeInfo>();

            private readonly ObjectIDGenerator mGenerator = new ObjectIDGenerator();

            private int mTypeIdGenerator = -1;

            private int mAssemblyIdGenerator = -1;

            private STypeInstanceProxy mNullProxy = null;

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Initializes the <see cref="SerializationContext"/> class.
            /// </summary>
            static SerializationContext()
            {
                mSystemTypes.Add(typeof(bool));
                mSystemTypes.Add(typeof(byte));
                mSystemTypes.Add(typeof(char));
                mSystemTypes.Add(typeof(short));
                mSystemTypes.Add(typeof(int));
                mSystemTypes.Add(typeof(long));
                mSystemTypes.Add(typeof(double));
                mSystemTypes.Add(typeof(decimal));
                mSystemTypes.Add(typeof(string));
                mSystemTypes.Add(typeof(bool?));
                mSystemTypes.Add(typeof(byte?));
                mSystemTypes.Add(typeof(char?));
                mSystemTypes.Add(typeof(short?));
                mSystemTypes.Add(typeof(int?));
                mSystemTypes.Add(typeof(long?));
                mSystemTypes.Add(typeof(double?));
                mSystemTypes.Add(typeof(decimal?));
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SerializationContext" /> class.
            /// </summary>
            /// <param name="selector">The selector.</param>
            /// <param name="context">The context.</param>
            internal SerializationContext(ISurrogateSelector selector, StreamingContext context)
            {
                this.Context = context;
                this.Selector = selector;
            }

            #endregion

            #region Internal properties

            /// <summary>
            /// Gets the context.
            /// </summary>
            /// <value>
            /// The context.
            /// </value>
            internal StreamingContext Context { get; private set; }

            /// <summary>
            /// Gets or sets the selector.
            /// </summary>
            /// <value>
            /// The selector.
            /// </value>
            internal ISurrogateSelector Selector { get; set; }

            #endregion

            #region Internal method(s)

            /// <summary>
            /// Checks the type info.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <exception cref="System.InvalidOperationException"></exception>
            internal STypeInfo GetTypeInfo(Type type)
            {
                // tömböket nem nézünk, nested típusokat sem (nem menne a HashSet)
                if (!type.IsArray && type.DeclaringType == null && !type.IsSerializable && !type.IsEnum && type.IsClass)
                {
                    if (!(type.IsAbstract && type.IsSealed))
                    {
                        // static type-okat kihagyjuk
                        throw new InvalidOperationException(string.Format("Type '{0}' does not marked with Serializable attribute.", type.AssemblyQualifiedName));
                    }
                }

                STypeInfo result = null;

                if (!mTypeVsTypeInfo.ContainsKey(type))
                {
                    result = new STypeInfo() { Type = type };
                    mTypeIdGenerator++;
                    result.TypeId = mTypeIdGenerator;
                    mTypeIdVsTypeInfo.Add(mTypeIdGenerator, result);
                    mTypeVsTypeInfo.Add(type, result);

                    if (!type.Assembly.Equals(typeof(bool).Assembly))
                    {
                        if (mAssemblyVsAssemblyId.ContainsKey(type.Assembly))
                        {
                            result.AssemblyId = mAssemblyVsAssemblyId[type.Assembly];
                        }
                        else
                        {
                            mAssemblyIdGenerator++;
                            mAssemblyIdVsAssembly.Add(mAssemblyIdGenerator, type.Assembly);
                            mAssemblyVsAssemblyId.Add(type.Assembly, mAssemblyIdGenerator);
                            result.AssemblyId = mAssemblyIdGenerator;
                        }
                    }

                    if (type.IsGenericType)
                    {
                        foreach (Type genericType in type.GetGenericArguments())
                        {
                            if (!genericType.IsGenericParameter)
                            {
                                GetTypeInfo(genericType);
                            }
                        }
                        GetTypeInfo(type.GetGenericTypeDefinition());
                    }

                    if (type.IsNested)
                    {
                        // ha nested, akkor biztosítani kell, hogy a parent type is benne legyen a serializációban
                        // deserializációkor pukkan, ha nincs
                        GetTypeInfo(type.DeclaringType);
                    }
                }
                else
                {
                    result = mTypeVsTypeInfo[type];
                }

                return result;
            }

            /// <summary>
            /// Serializes the specified serialization stream.
            /// </summary>
            /// <param name="serializationStream">The serialization stream.</param>
            /// <param name="graph">The graph.</param>
            [SecurityCritical]
            internal void Serialize(Stream serializationStream, object graph)
            {
                if (graph == null)
                {
                    // null
                    serializationStream.WriteByte(1);
                }
                else
                {
                    try
                    {
                        GetTypeInstanceProxy(graph, graph.GetType());

                        // not null
                        serializationStream.WriteByte(0);

                        // number of assemblies
                        byte[] bytes = Encoding.UTF8.GetBytes(mAssemblyIdVsAssembly.Count.ToString());
                        serializationStream.Write(bytes, 0, bytes.Length);
                        serializationStream.WriteByte(0);

                        // write assemblies
                        for (int i = 0; i < mAssemblyIdVsAssembly.Count; i++)
                        {
                            bytes = Encoding.UTF8.GetBytes(i.ToString());
                            serializationStream.Write(bytes, 0, bytes.Length);
                            serializationStream.WriteByte(0);

                            Assembly asm = mAssemblyIdVsAssembly[i];
                            bytes = Encoding.UTF8.GetBytes(asm.FullName);
                            serializationStream.Write(bytes, 0, bytes.Length);
                            serializationStream.WriteByte(0);
                        }

                        // number of types
                        bytes = Encoding.UTF8.GetBytes(mTypeIdVsTypeInfo.Count.ToString());
                        serializationStream.Write(bytes, 0, bytes.Length);
                        serializationStream.WriteByte(0);

                        // write types
                        for (int i = 0; i < mTypeIdVsTypeInfo.Count; i++)
                        {
                            STypeInfo ti = mTypeIdVsTypeInfo[i];
                            ti.SerializeType(serializationStream, this);
                        }

                        // number of items
                        bytes = Encoding.UTF8.GetBytes(mReferenceVsTypeInstances.Count.ToString());
                        serializationStream.Write(bytes, 0, bytes.Length);
                        serializationStream.WriteByte(0);

                        // write items
                        for (int i = 1; i <= mReferenceVsTypeInstances.Count; i++)
                        {
                            STypeInstanceProxy proxy = mReferenceVsTypeInstances[i];
                            proxy.SerializeContent(serializationStream, this);
                        }

                        // has null item
                        if (mNullProxy == null)
                        {
                            serializationStream.WriteByte(0); // false
                        }
                        else
                        {
                            serializationStream.WriteByte(1); // true
                            // kiírom az azonosítóját
                            bytes = Encoding.UTF8.GetBytes(mNullProxy.InstanceId.ToString());
                            serializationStream.Write(bytes, 0, bytes.Length);
                            serializationStream.WriteByte(0);
                        }
                    }
                    finally
                    {
                        mReferenceVsTypeInstances.Clear();
                        mTypeIdVsTypeInfo.Clear();
                        mTypeVsTypeInfo.Clear();
                        mAssemblyIdVsAssembly.Clear();
                        mAssemblyVsAssemblyId.Clear();
                    }
                }
            }

            #endregion

            #region Private method(s)

            [SecurityCritical]
            private STypeInstanceProxy GetTypeInstanceProxy(object o, Type nullableType)
            {
                STypeInstanceProxy result = null;

                if (o != null)
                {
                    Type objectType = nullableType == null ? o.GetType() : nullableType;
                    if (!objectType.IsSerializable && !objectType.IsNested && !objectType.IsEnum)
                    {
                        throw new SerializationException("Type has not been marked as serializable.");
                    }

                    STypeInfo ti = GetTypeInfo(objectType);

                    bool firstTime = false;
                    long instanceId = mGenerator.GetId(o, out firstTime);
                    if (!firstTime)
                    {
                        result = mReferenceVsTypeInstances[instanceId];
                    }

                    if (result == null)
                    {
                        result = (STypeInstanceProxy)ti.Clone();

                        result.InstanceId = instanceId; //mProxyIdGenerator;
                        result.InstanceValue = o;
                        mReferenceVsTypeInstances.Add(result.InstanceId, result);

                        if (mSystemTypes.Contains(objectType))
                        {
                        }
                        else if (objectType.IsEnum)
                        {
                        }
                        else if (o is ISerializable)
                        {
                            // serializable type
                            ISurrogateSelector selector = null;
                            ISerializationSurrogate surrogate = (Selector == null ? null : Selector.GetSurrogate(objectType, this.Context, out selector));

                            SerializationInfo info = new SerializationInfo(ti.Type, new FormatterConverter());
                            if (surrogate == null)
                            {
                                ISerializable serializable = (ISerializable)o;
                                serializable.GetObjectData(info, this.Context);
                            }
                            else
                            {
                                surrogate.GetObjectData(o, info, this.Context);
                            }

                            result.SerializationInfo = info;
                            if (info.MemberCount > 0)
                            {
                                result.ArrayKeys = new List<STypeInstanceProxy>();
                                result.ArrayItems = new List<STypeInstanceProxy>();
                                string[] memberNames = (string[])info.GetType().GetProperty("MemberNames", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).GetGetMethod(true).Invoke(info, null);
                                object[] memberValues = (object[])info.GetType().GetProperty("MemberValues", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).GetGetMethod(true).Invoke(info, null);

                                for (int i = 0; i < memberNames.Length; i++)
                                {
                                    string memberName = memberNames[i];
                                    if (memberName != null)
                                    {
                                        object memberValue = memberValues[i];
                                        result.ArrayKeys.Add(GetTypeInstanceProxy(memberName, null));
                                        result.ArrayItems.Add(GetTypeInstanceProxy(memberValue, null));
                                    }
                                }
                            }
                        }
                        else if (objectType.IsGenericType && objectType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                        {
                            // a reflector nem viszi a Nullable típust
                            dynamic dn = o;
                            result.FieldVsProxy.Add("value", GetTypeInstanceProxy(dn, dn.GetType()));
                        }
                        else if (objectType.IsArray && objectType.GetArrayRank() == 1 && objectType.Equals(typeof(byte[])))
                        {
                            // a byte tömböt máshogy serializálom
                        }
                        else if (objectType.IsArray && objectType.GetArrayRank() == 1)
                        {
                            Array array = o as Array;
                            List<STypeInstanceProxy> items = new List<STypeInstanceProxy>();
                            result.ArrayItems = items;
                            CollectArrayItems(items, array, new long[objectType.GetArrayRank()]);
                        }
                        else if (objectType.IsArray && objectType.GetArrayRank() > 1)
                        {
                            // multidim array
                            Array array = o as Array;
                            List<STypeInstanceProxy> items = new List<STypeInstanceProxy>();
                            result.ArrayItems = items;
                            CollectMultiArrayItems(items, array, 0, new long[objectType.GetArrayRank()]);
                        }
                        else
                        {
                            while (objectType != typeof(object) && objectType != typeof(ValueType) &&
                                objectType != typeof(MarshalByRefObject) && objectType != typeof(MBRBase))
                            {
                                foreach (FieldInfo fi in objectType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                                {
                                    if (!fi.IsNotSerialized)
                                    {
                                        if (objectType.IsValueType && objectType.IsAssignableFrom(fi.FieldType))
                                        {
                                            // ez a Boolean struktúránál tapasztalt végtelen ciklus kivédésére szolgál
                                            // a Boolean-ban van egy bool mező, amiben ismét van egy és ismét. Örökös körforgást el kell kerülni.
                                        }
                                        else if (fi.FieldType.IsGenericType && fi.FieldType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                                        {
                                            string name = string.Format("{0}.{1}", GetTypeInfo(objectType).TypeId.ToString(), fi.Name);
                                            result.FieldVsProxy.Add(name, GetTypeInstanceProxy(fi.GetValue(o), fi.FieldType));
                                        }
                                        else
                                        {
                                            string name = string.Format("{0}.{1}", GetTypeInfo(objectType).TypeId.ToString(), fi.Name);
                                            result.FieldVsProxy.Add(name, GetTypeInstanceProxy(fi.GetValue(o), null));
                                        }
                                    }
                                }
                                objectType = objectType.BaseType;
                            }
                            ti.SerializableFieldCounter = result.FieldVsProxy.Count;
                        }
                    }
                }
                else
                {
                    // null érték
                    if (mNullProxy == null)
                    {
                        STypeInfo ti = GetTypeInfo(typeof(Object));
                        result = (STypeInstanceProxy)ti.Clone();

                        bool firstTime = false;
                        result.InstanceId = mGenerator.GetId(result, out firstTime);
                        mReferenceVsTypeInstances.Add(result.InstanceId, result);
                        mNullProxy = result;
                    }
                    else
                    {
                        result = mNullProxy;
                    }
                }

                return result;
            }

            [SecurityCritical]
            private void CollectArrayItems(List<STypeInstanceProxy> items, Array array, params long[] indices)
            {
                long length = array.GetLongLength(0);
                for (long i = 0; i < length; i++)
                {
                    indices[0] = i;
                    object arrayItem = array.GetValue(indices);
                    if (arrayItem == null)
                    {
                        items.Add(null);
                    }
                    else if (arrayItem.GetType().IsGenericType && arrayItem.GetType().GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        items.Add(GetTypeInstanceProxy(arrayItem, arrayItem.GetType()));
                    }
                    else
                    {
                        items.Add(GetTypeInstanceProxy(arrayItem, null));
                    }
                }
            }

            [SecurityCritical]
            private void CollectMultiArrayItems(List<STypeInstanceProxy> items, Array array, int dimension, params long[] indices)
            {
                // visszaállítási adatok mentése
                long originalIndicesValueOnDimension = 0;
                if (dimension + 1 < array.GetType().GetArrayRank())
                {
                    originalIndicesValueOnDimension = indices[dimension + 1];
                }

                for (long i = 0; i < array.GetLongLength(dimension); i++)
                {
                    STypeInstanceProxy proxy = null;
                    indices[dimension] = i;

                    if (indices.Length - 1 == dimension)
                    {
                        object value = array.GetValue(indices);
                        if (value != null)
                        {
                            proxy = GetTypeInstanceProxy(value, value.GetType()); // array.GetType().GetElementType()
                        }
                    }
                    else
                    {
                        proxy = (STypeInstanceProxy)GetTypeInfo(array.GetType().GetElementType()).Clone();
                        bool firstTime = false;
                        proxy.InstanceId = mGenerator.GetId(proxy, out firstTime); //mProxyIdGenerator;
                        proxy.InstanceValue = dimension; // level of the array
                        proxy.IsArrayDimensionRepresentation = true;
                        mReferenceVsTypeInstances.Add(proxy.InstanceId, proxy);
                    }
                    items.Add(proxy);

                    if (dimension + 1 < array.GetType().GetArrayRank())
                    {
                        proxy.ArrayItems = new List<STypeInstanceProxy>();
                        CollectMultiArrayItems(proxy.ArrayItems, array, dimension + 1, indices);
                        indices[dimension + 1] = originalIndicesValueOnDimension;
                    }
                }
            }

            #endregion

        }

        internal sealed class DeserializationContext : ContextBase
        {

            #region Field(s)

            private static readonly HashSet<string> mNumbers = new HashSet<string>();

            private readonly BinarySerializerBehaviorEnum mSerializerBehavior = BinarySerializerBehaviorEnum.DoNotThrowExceptionOnMissingField;

            private readonly TypeLookupModeEnum mTypeLookupMode = TypeLookupModeEnum.AllowExactVersions;

            private readonly bool mFindNewestTypeVersion = false;

            /// <summary>
            /// Assembly azonosító, assembly név
            /// </summary>
            private readonly Dictionary<int, AssemblyName> mAsmIdVsAssemblyName = new Dictionary<int, AssemblyName>();

            /// <summary>
            /// Típus azonosító vs típus info
            /// </summary>
            private readonly Dictionary<int, DTypeInfo> mTypeIdVsTypeInfo = new Dictionary<int, DTypeInfo>();

            /// <summary>
            /// Példány azonosító és példány leíró
            /// </summary>
            private readonly Dictionary<long, DTypeInstanceProxy> mInstanceIdVsInstanceProxy = new Dictionary<long, DTypeInstanceProxy>();

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Initializes the <see cref="DeserializationContext"/> class.
            /// </summary>
            static DeserializationContext()
            {
                mNumbers.Add("0");
                mNumbers.Add("1");
                mNumbers.Add("2");
                mNumbers.Add("3");
                mNumbers.Add("4");
                mNumbers.Add("5");
                mNumbers.Add("6");
                mNumbers.Add("7");
                mNumbers.Add("8");
                mNumbers.Add("9");
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DeserializationContext" /> class.
            /// </summary>
            /// <param name="selector">The selector.</param>
            /// <param name="context">The context.</param>
            /// <param name="serializerBehavior">The serializer behavior.</param>
            /// <param name="typeLookupMode">The type lookup mode.</param>
            /// <param name="findNewestTypeVersion">if set to <c>true</c> [find newest type version].</param>
            internal DeserializationContext(ISurrogateSelector selector, StreamingContext context, BinarySerializerBehaviorEnum serializerBehavior, TypeLookupModeEnum typeLookupMode, bool findNewestTypeVersion)
            {
                this.Selector = selector;
                this.Context = context;
                this.mSerializerBehavior = serializerBehavior;
                this.mTypeLookupMode = typeLookupMode;
                this.mFindNewestTypeVersion = findNewestTypeVersion;
            }

            #endregion

            #region Internal properties

            /// <summary>
            /// Gets the context.
            /// </summary>
            /// <value>
            /// The context.
            /// </value>
            internal StreamingContext Context { get; private set; }

            /// <summary>
            /// Gets or sets the selector.
            /// </summary>
            /// <value>
            /// The selector.
            /// </value>
            internal ISurrogateSelector Selector { get; set; }

            /// <summary>
            /// Gets the serializer behavior.
            /// </summary>
            /// <value>
            /// The serializer behavior.
            /// </value>
            internal BinarySerializerBehaviorEnum SerializerBehavior
            {
                get { return mSerializerBehavior; }
            }

            /// <summary>
            /// Gets the type lookup mode.
            /// </summary>
            /// <value>
            /// The type lookup mode.
            /// </value>
            internal TypeLookupModeEnum TypeLookupMode
            {
                get { return mTypeLookupMode; }
            }

            /// <summary>
            /// Gets a value indicating whether [find newest type version].
            /// </summary>
            /// <value>
            /// <c>true</c> if [find newest type version]; otherwise, <c>false</c>.
            /// </value>
            internal bool FindNewestTypeVersion
            {
                get { return mFindNewestTypeVersion; }
            }

            /// <summary>
            /// Gets the name of the asm id vs assembly.
            /// </summary>
            /// <value>
            /// The name of the asm id vs assembly.
            /// </value>
            internal Dictionary<int, AssemblyName> AsmIdVsAssemblyName
            {
                get { return mAsmIdVsAssemblyName; }
            }

            /// <summary>
            /// Gets the type id vs type info.
            /// </summary>
            /// <value>
            /// The type id vs type info.
            /// </value>
            internal Dictionary<int, DTypeInfo> TypeIdVsTypeInfo
            {
                get { return mTypeIdVsTypeInfo; }
            }

            /// <summary>
            /// Gets the instance id vs instance proxy.
            /// </summary>
            /// <value>
            /// The instance id vs instance proxy.
            /// </value>
            internal Dictionary<long, DTypeInstanceProxy> InstanceIdVsInstanceProxy
            {
                get { return mInstanceIdVsInstanceProxy; }
            }

            #endregion

            #region Internal method(s)

            /// <summary>
            /// Deserializes the specified serialization stream.
            /// </summary>
            /// <param name="serializationStream">The serialization stream.</param>
            /// <returns></returns>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
            [SecurityCritical]
            internal object Deserialize(Stream serializationStream)
            {
                object result = null;

                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        int b = serializationStream.ReadByte();
                        if (b == 0)
                        {

                            #region Get assemblies

                            // get number of assemblies
                            b = serializationStream.ReadByte();
                            while (b > 0)
                            {
                                ms.WriteByte((byte)b);
                                b = serializationStream.ReadByte();
                            }
                            int asmCounter = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                            ms.SetLength(0);

                            for (int i = 0; i < asmCounter; i++)
                            {
                                // asm id
                                b = serializationStream.ReadByte();
                                while (b > 0)
                                {
                                    ms.WriteByte((byte)b);
                                    b = serializationStream.ReadByte();
                                }
                                int asmId = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                ms.SetLength(0);

                                // asm name
                                b = serializationStream.ReadByte();
                                while (b > 0)
                                {
                                    ms.WriteByte((byte)b);
                                    b = serializationStream.ReadByte();
                                }
                                string asm = Encoding.UTF8.GetString(ms.ToArray());
                                ms.SetLength(0);
                                mAsmIdVsAssemblyName.Add(asmId, new AssemblyName(asm));
                            }

                            #endregion

                            #region Proceed types

                            // get number of types
                            b = serializationStream.ReadByte();
                            while (b > 0)
                            {
                                ms.WriteByte((byte)b);
                                b = serializationStream.ReadByte();
                            }
                            int typeCounter = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                            ms.SetLength(0);

                            for (int i = 0; i < typeCounter; i++)
                            {
                                DTypeInfo typeInfo = new DTypeInfo();

                                // typeId
                                b = serializationStream.ReadByte();
                                while (b > 0)
                                {
                                    ms.WriteByte((byte)b);
                                    b = serializationStream.ReadByte();
                                }
                                typeInfo.TypeId = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                mTypeIdVsTypeInfo.Add(typeInfo.TypeId, typeInfo);
                                ms.SetLength(0);

                                // type name
                                b = serializationStream.ReadByte();
                                while (b > 0)
                                {
                                    ms.WriteByte((byte)b);
                                    b = serializationStream.ReadByte();
                                }
                                typeInfo.TypeName = Encoding.UTF8.GetString(ms.ToArray());
                                ms.SetLength(0);

                                // read asmId
                                {
                                    int index1 = typeInfo.TypeName.IndexOf("{");
                                    int index2 = typeInfo.TypeName.IndexOf("}");
                                    if (index1 >= 0)
                                    {
                                        index1++;
                                        typeInfo.AssemblyId = int.Parse(typeInfo.TypeName.Substring(index1, index2 - index1));
                                        index1--;
                                        typeInfo.TypeName = typeInfo.TypeName.Substring(0, index1);
                                    }
                                }

                                b = serializationStream.ReadByte();
                                if (b == 1)
                                {
                                    typeInfo.IsArray = true;
                                }

                                typeInfo.IsNested = typeInfo.TypeName.Contains("+");

                                // read generic info
                                b = serializationStream.ReadByte();
                                if (b == 1)
                                {
                                    typeInfo.IsGenericType = true;
                                    // IsGenericDeclaration?
                                    b = serializationStream.ReadByte();
                                    if (b == 1)
                                    {
                                        typeInfo.IsGenericDeclaration = true;
                                    }
                                    if (!typeInfo.IsGenericDeclaration)
                                    {
                                        // típus azonosítók kiolvasása
                                        List<int> genericParameterTypes = new List<int>();
                                        int indexOfChar = typeInfo.TypeName.IndexOf("[") + 1;
                                        int origIndexOfChar = indexOfChar - 1;
                                        string idChar = typeInfo.TypeName.Substring(indexOfChar, 1);
                                        string idRefNumber = string.Empty;
                                        while (!idChar.Equals("]"))
                                        {
                                            if (mNumbers.Contains(idChar))
                                            {
                                                idRefNumber = idRefNumber + idChar;
                                            }
                                            else if (idChar.Equals(";"))
                                            {
                                                genericParameterTypes.Add(int.Parse(idRefNumber));
                                                idRefNumber = string.Empty;
                                            }

                                            indexOfChar++;
                                            idChar = typeInfo.TypeName.Substring(indexOfChar, 1);
                                        }
                                        genericParameterTypes.Add(int.Parse(idRefNumber));
                                        typeInfo.GenericParameterTypeIds = genericParameterTypes;
                                        typeInfo.TypeName = string.Format("{0}*{1}", typeInfo.TypeName.Substring(0, origIndexOfChar), typeInfo.TypeName.Substring(indexOfChar + 1));
                                    }
                                }

                                // number of deserializable fields
                                b = serializationStream.ReadByte();
                                while (b > 0)
                                {
                                    ms.WriteByte((byte)b);
                                    b = serializationStream.ReadByte();
                                }
                                typeInfo.DeserializableFieldNumber = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                ms.SetLength(0);

                                if (typeInfo.IsNested)
                                {
                                    int index = 1;
                                    string refIdChar = typeInfo.TypeName.Substring(index, 1);
                                    string refId = string.Empty;
                                    while (mNumbers.Contains(refIdChar))
                                    {
                                        refId = refId + refIdChar;
                                        index++;
                                        refIdChar = typeInfo.TypeName.Substring(index, 1);
                                    }
                                    typeInfo.DeclaredTypeId = int.Parse(refId);
                                    typeInfo.TypeName = typeInfo.TypeName.Substring(index + 2); // levágom: )+
                                }
                            }

                            // resolve types
                            for (int i = 0; i < typeCounter; i++)
                            {
                                DTypeInfo ti = mTypeIdVsTypeInfo[i];
                                if (ti.Type == null)
                                {
                                    ti.ResolveType(this);
                                }
                            }

                            #endregion

                            #region Build instances

                            // build instances, number of items
                            b = serializationStream.ReadByte();
                            while (b > 0)
                            {
                                ms.WriteByte((byte)b);
                                b = serializationStream.ReadByte();
                            }
                            int numberOfItems = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                            ms.SetLength(0);

                            // create proxies
                            for (int i = 1; i <= numberOfItems; i++)
                            {

                                #region InstanceId

                                // instanceId
                                b = serializationStream.ReadByte();
                                while (b > 0)
                                {
                                    ms.WriteByte((byte)b);
                                    b = serializationStream.ReadByte();
                                }
                                long instanceId = long.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                ms.SetLength(0);

                                #endregion

                                #region TypeId

                                // typeId
                                b = serializationStream.ReadByte();
                                while (b > 0)
                                {
                                    ms.WriteByte((byte)b);
                                    b = serializationStream.ReadByte();
                                }
                                int typeId = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                ms.SetLength(0);

                                #endregion

                                #region Create proxy

                                // create proxy
                                DTypeInstanceProxy proxy = mTypeIdVsTypeInfo[typeId].Clone() as DTypeInstanceProxy;
                                proxy.InstanceId = instanceId;
                                mInstanceIdVsInstanceProxy.Add(instanceId, proxy);

                                #region ArrayKeys

                                b = serializationStream.ReadByte();
                                if (b == 1)
                                {
                                    // has array keys
                                    b = serializationStream.ReadByte();
                                    while (b > 0)
                                    {
                                        ms.WriteByte((byte)b);
                                        b = serializationStream.ReadByte();
                                    }
                                    int arrayKeyLen = ms.Length == 0 ? -1 : int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                    ms.SetLength(0);

                                    proxy.ArrayKeysRefIds = new List<int>(arrayKeyLen);
                                    for (int index = 0; index < arrayKeyLen; index++)
                                    {
                                        b = serializationStream.ReadByte();
                                        while (b > 0)
                                        {
                                            ms.WriteByte((byte)b);
                                            b = serializationStream.ReadByte();
                                        }
                                        int arrayKeyProxyId = ms.Length == 0 ? -1 : int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                        ms.SetLength(0);
                                        proxy.ArrayKeysRefIds.Add(arrayKeyProxyId);
                                    }
                                }

                                #endregion

                                b = serializationStream.ReadByte();
                                proxy.IsArrayDimensionRepresentation = (b == 1 ? true : false);

                                #region Array

                                int arrayItemNumber = 0;
                                if (proxy.Type.IsArray || proxy.IsArrayDimensionRepresentation || typeof(ISerializable).IsAssignableFrom(proxy.Type))
                                {
                                    // array type

                                    // number of array items
                                    b = serializationStream.ReadByte();
                                    while (b > 0)
                                    {
                                        ms.WriteByte((byte)b);
                                        b = serializationStream.ReadByte();
                                    }
                                    arrayItemNumber = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                    ms.SetLength(0);

                                    if (!proxy.Type.Equals(typeof(byte[])))
                                    {
                                        // byte tömböt máshogy kezelem
                                        for (int index = 0; index < arrayItemNumber; index++)
                                        {
                                            b = serializationStream.ReadByte();
                                            if (b == 0)
                                            {
                                                proxy.ArrayItemsRefIds.Add(-1);
                                            }
                                            else
                                            {
                                                while (b > 0)
                                                {
                                                    ms.WriteByte((byte)b);
                                                    b = serializationStream.ReadByte();
                                                }
                                                proxy.ArrayItemsRefIds.Add(int.Parse(Encoding.UTF8.GetString(ms.ToArray())));
                                                ms.SetLength(0);
                                            }
                                        }
                                    }
                                }

                                #endregion

                                #region Field items

                                // has field items?
                                b = serializationStream.ReadByte();
                                while (b > 0)
                                {
                                    ms.WriteByte((byte)b);
                                    b = serializationStream.ReadByte();
                                }
                                int fieldItemNumber = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                ms.SetLength(0);

                                for (int index = 0; index < fieldItemNumber; index++)
                                {
                                    // read field name
                                    b = serializationStream.ReadByte();
                                    while (b > 0)
                                    {
                                        ms.WriteByte((byte)b);
                                        b = serializationStream.ReadByte();
                                    }
                                    string fieldName = Encoding.UTF8.GetString(ms.ToArray());
                                    ms.SetLength(0);

                                    // read reference value (instanceId)
                                    b = serializationStream.ReadByte();
                                    if (b == 0)
                                    {
                                        proxy.FieldsVsInstanceIds.Add(fieldName, null);
                                    }
                                    else
                                    {
                                        while (b > 0)
                                        {
                                            ms.WriteByte((byte)b);
                                            b = serializationStream.ReadByte();
                                        }
                                        int fieldRefId = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                        ms.SetLength(0);
                                        proxy.FieldsVsInstanceIds.Add(fieldName, fieldRefId);
                                    }
                                }

                                #endregion

                                #region Instance values

                                // read instance value
                                Type reflectionType = proxy.Type;
                                if (mSystemByteTypes.Contains(reflectionType))
                                {
                                    ms.WriteByte((byte)serializationStream.ReadByte());
                                    if (reflectionType.Equals(typeof(byte)))
                                    {
                                        proxy.InstanceValue = ms.ToArray()[0];
                                        proxy.IsConstructed = true;
                                    }
                                    else if (reflectionType.Equals(typeof(byte?)))
                                    {
                                        proxy.InstanceValue = new Nullable<byte>(ms.ToArray()[0]);
                                        proxy.IsConstructed = true;
                                    }
                                    else if (reflectionType.Equals(typeof(bool?)) || reflectionType.Equals(typeof(bool)))
                                    {
                                        if (ms.ToArray()[0] == 1)
                                        {
                                            proxy.InstanceValue = true;
                                        }
                                        else
                                        {
                                            proxy.InstanceValue = false;
                                        }
                                        proxy.IsConstructed = true;
                                    }
                                }
                                else if (proxy.Type.Equals(typeof(byte[])))
                                {
                                    byte[] bytes = null;
                                    if (arrayItemNumber == 0)
                                    {
                                        bytes = new byte[0];
                                    }
                                    else
                                    {
                                        bytes = new byte[arrayItemNumber];
                                        serializationStream.Read(bytes, 0, bytes.Length);
                                    }
                                    proxy.InstanceValue = bytes;
                                    proxy.IsConstructed = true;
                                }
                                else
                                {
                                    b = serializationStream.ReadByte();
                                    if (b != 0)
                                    {
                                        while (b > 0)
                                        {
                                            ms.WriteByte((byte)b);
                                            b = serializationStream.ReadByte();
                                        }

                                        if (reflectionType.IsEnum)
                                        {
                                            proxy.InstanceValue = Enum.ToObject(reflectionType, Convert.ChangeType(Encoding.UTF8.GetString(ms.ToArray()), reflectionType.GetEnumUnderlyingType()));
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(short)))
                                        {
                                            proxy.InstanceValue = short.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(ushort)))
                                        {
                                            proxy.InstanceValue = ushort.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(int)))
                                        {
                                            proxy.InstanceValue = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(uint)))
                                        {
                                            proxy.InstanceValue = uint.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(long)))
                                        {
                                            proxy.InstanceValue = long.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(ulong)))
                                        {
                                            proxy.InstanceValue = ulong.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(short?)))
                                        {
                                            proxy.InstanceValue = new Nullable<short>(short.Parse(Encoding.UTF8.GetString(ms.ToArray())));
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(ushort?)))
                                        {
                                            proxy.InstanceValue = new Nullable<ushort>(ushort.Parse(Encoding.UTF8.GetString(ms.ToArray())));
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(int?)))
                                        {
                                            proxy.InstanceValue = new Nullable<int>(int.Parse(Encoding.UTF8.GetString(ms.ToArray())));
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(uint?)))
                                        {
                                            proxy.InstanceValue = new Nullable<uint>(uint.Parse(Encoding.UTF8.GetString(ms.ToArray())));
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(long?)))
                                        {
                                            proxy.InstanceValue = new Nullable<long>(long.Parse(Encoding.UTF8.GetString(ms.ToArray())));
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(ulong?)))
                                        {
                                            proxy.InstanceValue = new Nullable<ulong>(ulong.Parse(Encoding.UTF8.GetString(ms.ToArray())));
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(float)))
                                        {
                                            int length = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                            byte[] bytes = new byte[length];
                                            serializationStream.Read(bytes, 0, bytes.Length);
                                            proxy.InstanceValue = float.Parse(Encoding.UTF32.GetString(bytes), System.Globalization.CultureInfo.InvariantCulture);
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(double)))
                                        {
                                            int length = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                            byte[] bytes = new byte[length];
                                            serializationStream.Read(bytes, 0, bytes.Length);
                                            proxy.InstanceValue = double.Parse(Encoding.UTF32.GetString(bytes), System.Globalization.CultureInfo.InvariantCulture);
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(decimal)))
                                        {
                                            int length = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                            byte[] bytes = new byte[length];
                                            serializationStream.Read(bytes, 0, bytes.Length);
                                            proxy.InstanceValue = decimal.Parse(Encoding.UTF32.GetString(bytes), System.Globalization.CultureInfo.InvariantCulture);
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(float?)))
                                        {
                                            int length = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                            byte[] bytes = new byte[length];
                                            serializationStream.Read(bytes, 0, bytes.Length);
                                            proxy.InstanceValue = new Nullable<float>(float.Parse(Encoding.UTF32.GetString(bytes), System.Globalization.CultureInfo.InvariantCulture));
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(double?)))
                                        {
                                            int length = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                            byte[] bytes = new byte[length];
                                            serializationStream.Read(bytes, 0, bytes.Length);
                                            proxy.InstanceValue = new Nullable<double>(double.Parse(Encoding.UTF32.GetString(bytes), System.Globalization.CultureInfo.InvariantCulture));
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(decimal?)))
                                        {
                                            int length = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                            byte[] bytes = new byte[length];
                                            serializationStream.Read(bytes, 0, bytes.Length);
                                            proxy.InstanceValue = new Nullable<decimal>(decimal.Parse(Encoding.UTF32.GetString(bytes), System.Globalization.CultureInfo.InvariantCulture));
                                            proxy.IsConstructed = true;
                                        }
                                        else if (typeof(Type).IsAssignableFrom(reflectionType))
                                        {
                                            int length = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                            byte[] bytes = new byte[length];
                                            serializationStream.Read(bytes, 0, bytes.Length);
                                            proxy.InstanceValue = TypeHelper.GetTypeFromString(Encoding.UTF32.GetString(bytes), this.TypeLookupMode, this.FindNewestTypeVersion, true, false);
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(string)))
                                        {
                                            int length = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                            byte[] bytes = new byte[length];
                                            serializationStream.Read(bytes, 0, bytes.Length);
                                            proxy.InstanceValue = Encoding.UTF32.GetString(bytes);
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(char)))
                                        {
                                            int length = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                            byte[] bytes = new byte[length];
                                            serializationStream.Read(bytes, 0, bytes.Length);
                                            proxy.InstanceValue = Encoding.UTF32.GetChars(bytes)[0];
                                            proxy.IsConstructed = true;
                                        }
                                        else if (reflectionType.Equals(typeof(char?)))
                                        {
                                            int length = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                            byte[] bytes = new byte[length];
                                            serializationStream.Read(bytes, 0, bytes.Length);
                                            proxy.InstanceValue = new Nullable<char>(Encoding.UTF32.GetChars(bytes)[0]);
                                            proxy.IsConstructed = true;
                                        }
                                    }
                                }
                                ms.SetLength(0);

                                #endregion

                                #endregion

                            }

                            // read null representation proxy
                            b = serializationStream.ReadByte();
                            if (b == 1)
                            {
                                // van null
                                b = serializationStream.ReadByte();
                                while (b > 0)
                                {
                                    ms.WriteByte((byte)b);
                                    b = serializationStream.ReadByte();
                                }
                                int nullProxyId = int.Parse(Encoding.UTF8.GetString(ms.ToArray()));
                                ms.SetLength(0);
                                mInstanceIdVsInstanceProxy[nullProxyId].IsConstructed = true;
                            }

                            // construct values
                            result = mInstanceIdVsInstanceProxy[1].Construct(this);

                            #endregion

                        }
                        else if (b != 1)
                        {
                            throw new FormatException("Invalid first char in content stream.");
                        }
                    }
                }
                finally
                {
                    mAsmIdVsAssemblyName.Clear();
                    mTypeIdVsTypeInfo.Clear();
                    mInstanceIdVsInstanceProxy.Clear();
                }

                return result;
            }

            #endregion

        }

        [DebuggerDisplay("[{GetType().Name}, TypeId = {TypeId}, TypeName = '{Type.FullName}', IsGeneric = {Type.IsGenericType}, IsArray = {Type.IsArray}]")]
        internal class STypeInfo : ContextBase, ICloneable
        {

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="STypeInfo"/> class.
            /// </summary>
            internal STypeInfo()
            {
                this.AssemblyId = -1;
            }

            #endregion

            #region Internal properties

            /// <summary>
            /// Gets or sets the type id.
            /// </summary>
            /// <value>
            /// The type id.
            /// </value>
            internal int TypeId { get; set; }

            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            /// <value>
            /// The type.
            /// </value>
            internal Type Type { get; set; }

            /// <summary>
            /// Gets or sets the assembly id.
            /// </summary>
            /// <value>
            /// The assembly id.
            /// </value>
            internal int AssemblyId { get; set; }

            /// <summary>
            /// Gets or sets the serializable field counter.
            /// </summary>
            /// <value>
            /// The serializable field counter.
            /// </value>
            internal int SerializableFieldCounter { get; set; }

            /// <summary>
            /// Gets the array rank.
            /// </summary>
            /// <value>
            /// The array rank.
            /// </value>
            internal virtual int ArrayRank
            {
                get
                {
                    return this.Type.IsArray ? this.Type.GetArrayRank() : 0;
                }
            }

            #endregion

            #region Internal method(s)

            /// <summary>
            /// Serializes the type.
            /// </summary>
            /// <param name="serializationStream">The serialization stream.</param>
            /// <param name="context">The context.</param>
            internal void SerializeType(Stream serializationStream, SerializationContext context)
            {
                // typeId
                byte[] bytes = Encoding.UTF8.GetBytes(this.TypeId.ToString());
                serializationStream.Write(bytes, 0, bytes.Length);
                serializationStream.WriteByte(0);

                // Full type name
                StringBuilder sb = GetTypeString(this.Type, context);
                bytes = Encoding.UTF8.GetBytes(sb.ToString());
                serializationStream.Write(bytes, 0, bytes.Length);
                serializationStream.WriteByte(0);

                // IsArray
                if (this.Type.IsArray)
                {
                    serializationStream.WriteByte(1); // true flag
                }
                else
                {
                    serializationStream.WriteByte(0); // false flag
                }

                // normál generikus típus vagy generikus tömb
                if (this.Type.IsGenericType || this.Type.GetGenericArguments().Length > 0)
                {
                    serializationStream.WriteByte(1); // true flag
                    if (this.Type.IsGenericTypeDefinition)
                    {
                        serializationStream.WriteByte(1); // true flag
                    }
                    else
                    {
                        serializationStream.WriteByte(0); // true flag
                    }
                }
                else
                {
                    serializationStream.WriteByte(0); // false flag
                }

                // number of fields
                bytes = Encoding.UTF8.GetBytes(this.SerializableFieldCounter.ToString());
                serializationStream.Write(bytes, 0, bytes.Length);
                serializationStream.WriteByte(0);
            }

            #endregion

            #region Public method(s)

            /// <summary>
            /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
            /// <returns>
            ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
            /// </returns>
            public override bool Equals(object obj)
            {
                STypeInfo other = (STypeInfo)obj;
                return other.Type.Equals(this.Type);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                return this.Type.GetHashCode();
            }

            /// <summary>
            /// Creates a new object that is a copy of the current instance.
            /// </summary>
            /// <returns>
            /// A new object that is a copy of this instance.
            /// </returns>
            public object Clone()
            {
                return new STypeInstanceProxy() { Type = this.Type, TypeId = this.TypeId, SerializableFieldCounter = this.SerializableFieldCounter, AssemblyId = this.AssemblyId };
            }

            #endregion

            #region Private method(s)

            private bool IsNestedType(Type type)
            {
                bool result = type.IsNested;

                while (type.HasElementType)
                {
                    type = type.GetElementType();
                    result = IsNestedType(type);
                }

                return result;
            }

            private Type GetDeclaringType(Type type)
            {
                while (type.HasElementType)
                {
                    type = type.GetElementType();
                }

                return type.DeclaringType;
            }

            private string GetTypeNameWithoutArrayMarks(Type type, bool useFullname, out string arrayMarks)
            {
                arrayMarks = string.Empty;
                string result = useFullname ? type.FullName : type.Name;

                if (type.IsArray)
                {
                    while (result.EndsWith("[") || result.EndsWith("]") || result.EndsWith(","))
                    {
                        arrayMarks = result[result.Length - 1] + arrayMarks;
                        result = result.Substring(0, result.Length - 1);
                    }
                }

                return result;
            }

            private StringBuilder GetTypeString(Type type, SerializationContext context)
            {
                StringBuilder sb = null;
                if (type.GetGenericArguments().Length > 0)
                {
                    // generikus típusok
                    string arrayMarks = string.Empty;
                    if (IsNestedType(type))
                    {
                        sb = new StringBuilder(GetTypeNameWithoutArrayMarks(type, false, out arrayMarks));
                    }
                    else
                    {
                        sb = new StringBuilder(string.Format("{0}.{1}", type.Namespace, GetTypeNameWithoutArrayMarks(type, false, out arrayMarks)));
                    }
                    if (!type.IsGenericTypeDefinition)
                    {
                        // nyers generikus típusok paraméterlistáját nem serializáljuk
                        sb.Append("[");
                        bool writeComma = false;
                        foreach (Type arrayType in type.GetGenericArguments())
                        {
                            if (writeComma)
                            {
                                sb.Append(";");
                            }
                            else
                            {
                                writeComma = true;
                            }
                            sb.Append(context.GetTypeInfo(arrayType).TypeId.ToString());
                        }
                        sb.Append("]");
                    }
                    sb.Append(arrayMarks);
                }
                else
                {
                    // nem generikus típusok
                    if (IsNestedType(type))
                    {
                        sb = new StringBuilder(type.Name);
                    }
                    else
                    {
                        sb = new StringBuilder(type.FullName);
                    }
                }

                if (IsNestedType(type))
                {
                    Type declaringType = GetDeclaringType(type);
                    sb.Insert(0, string.Format("({0})+", context.GetTypeInfo(declaringType).TypeId.ToString()));
                }

                if (AssemblyId != -1)
                {
                    sb.Append("{");
                    sb.Append(AssemblyId.ToString());
                    sb.Append("}");
                }

                return sb;
            }

            #endregion

        }

        [DebuggerDisplay("[{GetType().Name}, Id = {InstanceId}, TypeId = {TypeId}, TypeName = '{Type.FullName}', IsGeneric = {Type.IsGenericType}, IsArray = {Type.IsArray}]")]
        internal sealed class STypeInstanceProxy : STypeInfo
        {

            #region Field(s)

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly Dictionary<string, STypeInstanceProxy> mFieldVsProxy = new Dictionary<string, STypeInstanceProxy>();

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="STypeInstanceProxy"/> class.
            /// </summary>
            internal STypeInstanceProxy()
            {
            }

            #endregion

            #region Internal properties

            /// <summary>
            /// Gets or sets the instance id.
            /// </summary>
            /// <value>
            /// The instance id.
            /// </value>
            internal long InstanceId { get; set; }

            /// <summary>
            /// Gets or sets the instance value.
            /// </summary>
            /// <value>
            /// The instance value.
            /// </value>
            internal object InstanceValue { get; set; }

            /// <summary>
            /// Gets or sets the array keys.
            /// </summary>
            /// <value>
            /// The array keys.
            /// </value>
            internal List<STypeInstanceProxy> ArrayKeys { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is array dimension representation.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance is array dimension representation; otherwise, <c>false</c>.
            /// </value>
            internal bool IsArrayDimensionRepresentation { get; set; }

            /// <summary>
            /// Gets or sets the array items.
            /// </summary>
            /// <value>
            /// The array items.
            /// </value>
            internal List<STypeInstanceProxy> ArrayItems { get; set; }

            /// <summary>
            /// Gets the array rank.
            /// </summary>
            /// <value>
            /// The array rank.
            /// </value>
            internal override int ArrayRank
            {
                get
                {
                    return base.ArrayRank;
                }
            }

            /// <summary>
            /// Gets the field vs proxy.
            /// </summary>
            /// <value>
            /// The field vs proxy.
            /// </value>
            internal Dictionary<string, STypeInstanceProxy> FieldVsProxy
            {
                get { return mFieldVsProxy; }
            }

            /// <summary>
            /// Gets or sets the serialization info.
            /// </summary>
            /// <value>
            /// The serialization info.
            /// </value>
            internal SerializationInfo SerializationInfo { get; set; }

            #endregion

            #region Public method(s)

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

                return this.GetHashCode().Equals(obj.GetHashCode());
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                int hash = 21;
                hash = hash ^ 23 + this.Type.GetHashCode();
                hash = hash ^ 33 + this.IsArrayDimensionRepresentation.GetHashCode();
                if (this.InstanceValue == null)
                {
                    hash = hash ^ 34511;
                }
                else
                {
                    hash = hash ^ 144 + RuntimeHelpers.GetHashCode(this.InstanceValue);
                }
                return hash;
            }

            #endregion

            #region Internal method(s)

            /// <summary>
            /// Serializes the content.
            /// </summary>
            /// <param name="serializationStream">The serialization stream.</param>
            /// <param name="context">The context.</param>
            internal void SerializeContent(Stream serializationStream, SerializationContext context)
            {
                // InstanceId
                byte[] bytes = Encoding.UTF8.GetBytes(this.InstanceId.ToString());
                serializationStream.Write(bytes, 0, bytes.Length);
                serializationStream.WriteByte(0);

                // typeId
                bytes = Encoding.UTF8.GetBytes(this.TypeId.ToString());
                serializationStream.Write(bytes, 0, bytes.Length);
                serializationStream.WriteByte(0);

                // ArrayKeys
                if (this.ArrayKeys != null && this.ArrayKeys.Count > 0)
                {
                    serializationStream.WriteByte(1); // has array keys

                    // number of array keys
                    bytes = Encoding.UTF8.GetBytes(ArrayKeys.Count.ToString());
                    serializationStream.Write(bytes, 0, bytes.Length);
                    serializationStream.WriteByte(0);

                    // the array keys
                    foreach (STypeInstanceProxy proxy in ArrayKeys)
                    {
                        bytes = Encoding.UTF8.GetBytes(proxy.InstanceId.ToString()); // proxy id
                        serializationStream.Write(bytes, 0, bytes.Length);
                        serializationStream.WriteByte(0);
                    }
                }
                else
                {
                    // array keys null
                    serializationStream.WriteByte(0);
                }

                // IsArrayDimensionRepresentation?
                serializationStream.WriteByte(IsArrayDimensionRepresentation ? (byte)1 : (byte)0);

                // IsArray?
                if (this.Type.IsArray || IsArrayDimensionRepresentation || SerializationInfo != null)
                {
                    // number of array items
                    if (this.Type.Equals(typeof(byte[])))
                    {
                        bytes = Encoding.UTF8.GetBytes(((byte[])this.InstanceValue).Length.ToString());
                        serializationStream.Write(bytes, 0, bytes.Length);
                        serializationStream.WriteByte(0);
                    }
                    else
                    {
                        bytes = Encoding.UTF8.GetBytes(ArrayItems.Count.ToString());
                        serializationStream.Write(bytes, 0, bytes.Length);
                        serializationStream.WriteByte(0);

                        // the array items
                        foreach (STypeInstanceProxy proxy in ArrayItems)
                        {
                            if (proxy == null)
                            {
                                serializationStream.WriteByte(0); // null
                            }
                            else
                            {
                                bytes = Encoding.UTF8.GetBytes(proxy.InstanceId.ToString()); // proxy id
                                serializationStream.Write(bytes, 0, bytes.Length);
                                serializationStream.WriteByte(0);
                            }
                        }
                    }
                }

                // fields
                // number of field items
                bytes = Encoding.UTF8.GetBytes(mFieldVsProxy.Count.ToString());
                serializationStream.Write(bytes, 0, bytes.Length);
                serializationStream.WriteByte(0);

                if (mFieldVsProxy.Count > 0)
                {
                    foreach (KeyValuePair<string, STypeInstanceProxy> kv in mFieldVsProxy)
                    {
                        bytes = Encoding.UTF8.GetBytes(kv.Key.ToString()); // instanceId
                        serializationStream.Write(bytes, 0, bytes.Length);
                        serializationStream.WriteByte(0);
                        if (kv.Value == null)
                        {
                            serializationStream.WriteByte(0); // null
                        }
                        else
                        {
                            bytes = Encoding.UTF8.GetBytes(kv.Value.InstanceId.ToString()); // instanceId
                            serializationStream.Write(bytes, 0, bytes.Length);
                            serializationStream.WriteByte(0);
                        }
                    }
                }

                if (this.InstanceValue == null)
                {
                    serializationStream.WriteByte(0); // null
                }
                else if (mSystemByteTypes.Contains(this.InstanceValue.GetType()))
                {
                    Type reflectionType = this.InstanceValue.GetType();
                    if (reflectionType.Equals(typeof(bool?)))
                    {
                        bool? c = (bool?)InstanceValue;
                        serializationStream.WriteByte(c.Value ? (byte)1 : (byte)0);
                    }
                    else if (reflectionType.Equals(typeof(bool)))
                    {
                        bool c = (bool)InstanceValue;
                        serializationStream.WriteByte(c ? (byte)1 : (byte)0);
                    }
                    else if (reflectionType.Equals(typeof(byte)))
                    {
                        byte b = (byte)InstanceValue;
                        serializationStream.WriteByte(b);
                    }
                    else if (reflectionType.Equals(typeof(byte?)))
                    {
                        byte? c = (byte?)InstanceValue;
                        serializationStream.WriteByte(c.Value);
                    }
                }
                else
                {
                    bytes = null;
                    Type reflectionType = this.InstanceValue.GetType();
                    if (reflectionType.IsEnum)
                    {
                        bytes = Encoding.UTF8.GetBytes(Convert.ChangeType(this.InstanceValue, reflectionType.GetEnumUnderlyingType()).ToString());
                        serializationStream.Write(bytes, 0, bytes.Length);
                        serializationStream.WriteByte(0);
                    }
                    else if (reflectionType.Equals(typeof(short)) ||
                        reflectionType.Equals(typeof(ushort)) ||
                        reflectionType.Equals(typeof(int)) ||
                        reflectionType.Equals(typeof(uint)) ||
                        reflectionType.Equals(typeof(long)) ||
                        reflectionType.Equals(typeof(ulong)))
                    {
                        bytes = Encoding.UTF8.GetBytes(InstanceValue.ToString());
                        serializationStream.Write(bytes, 0, bytes.Length);
                        serializationStream.WriteByte(0);
                    }
                    else if (reflectionType.Equals(typeof(short?)))
                    {
                        short? c = (short?)InstanceValue;
                        bytes = Encoding.UTF8.GetBytes(c.Value.ToString());
                        serializationStream.Write(bytes, 0, bytes.Length);
                        serializationStream.WriteByte(0);
                    }
                    else if (reflectionType.Equals(typeof(ushort?)))
                    {
                        ushort? c = (ushort?)InstanceValue;
                        bytes = Encoding.UTF8.GetBytes(c.Value.ToString());
                        serializationStream.Write(bytes, 0, bytes.Length);
                        serializationStream.WriteByte(0);
                    }
                    else if (reflectionType.Equals(typeof(int?)))
                    {
                        int? c = (int?)InstanceValue;
                        bytes = Encoding.UTF8.GetBytes(c.Value.ToString());
                        serializationStream.Write(bytes, 0, bytes.Length);
                        serializationStream.WriteByte(0);
                    }
                    else if (reflectionType.Equals(typeof(uint?)))
                    {
                        uint? c = (uint?)InstanceValue;
                        bytes = Encoding.UTF8.GetBytes(c.Value.ToString());
                        serializationStream.Write(bytes, 0, bytes.Length);
                        serializationStream.WriteByte(0);
                    }
                    else if (reflectionType.Equals(typeof(long?)))
                    {
                        long? c = (long?)InstanceValue;
                        bytes = Encoding.UTF8.GetBytes(c.Value.ToString());
                        serializationStream.Write(bytes, 0, bytes.Length);
                        serializationStream.WriteByte(0);
                    }
                    else if (reflectionType.Equals(typeof(ulong?)))
                    {
                        ulong? c = (ulong?)InstanceValue;
                        bytes = Encoding.UTF8.GetBytes(c.Value.ToString());
                        serializationStream.Write(bytes, 0, bytes.Length);
                        serializationStream.WriteByte(0);
                    }
                    else if (reflectionType.Equals(typeof(float)))
                    {
                        float d = (float)InstanceValue;
                        if (float.MinValue.Equals(d))
                        {
                            bytes = Encoding.UTF32.GetBytes(float.MinValue.ToString("r", System.Globalization.CultureInfo.InvariantCulture));
                        }
                        else if (float.MaxValue.Equals(d))
                        {
                            bytes = Encoding.UTF32.GetBytes(float.MaxValue.ToString("r", System.Globalization.CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            bytes = Encoding.UTF32.GetBytes(d.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        }

                        byte[] temp = Encoding.UTF8.GetBytes(bytes.Length.ToString());
                        serializationStream.Write(temp, 0, temp.Length); // beírom a hosszt
                        serializationStream.WriteByte(0);

                        serializationStream.Write(bytes, 0, bytes.Length);
                    }
                    else if (reflectionType.Equals(typeof(double)))
                    {
                        double d = (double)InstanceValue;
                        if (double.MinValue.Equals(d))
                        {
                            bytes = Encoding.UTF32.GetBytes(double.MinValue.ToString("r", System.Globalization.CultureInfo.InvariantCulture));
                        }
                        else if (double.MaxValue.Equals(d))
                        {
                            bytes = Encoding.UTF32.GetBytes(double.MaxValue.ToString("r", System.Globalization.CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            bytes = Encoding.UTF32.GetBytes(d.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        }

                        byte[] temp = Encoding.UTF8.GetBytes(bytes.Length.ToString());
                        serializationStream.Write(temp, 0, temp.Length); // beírom a hosszt
                        serializationStream.WriteByte(0);

                        serializationStream.Write(bytes, 0, bytes.Length);
                    }
                    else if (reflectionType.Equals(typeof(decimal)))
                    {
                        bytes = Encoding.UTF32.GetBytes(((decimal)InstanceValue).ToString(System.Globalization.CultureInfo.InvariantCulture));

                        byte[] temp = Encoding.UTF8.GetBytes(bytes.Length.ToString());
                        serializationStream.Write(temp, 0, temp.Length); // beírom a hosszt
                        serializationStream.WriteByte(0);

                        serializationStream.Write(bytes, 0, bytes.Length);
                    }
                    else if (reflectionType.Equals(typeof(float?)))
                    {
                        float? d = (float?)InstanceValue;
                        if (float.MinValue.Equals(d.Value))
                        {
                            bytes = Encoding.UTF32.GetBytes(float.MinValue.ToString("r", System.Globalization.CultureInfo.InvariantCulture));
                        }
                        else if (float.MaxValue.Equals(d))
                        {
                            bytes = Encoding.UTF32.GetBytes(float.MaxValue.ToString("r", System.Globalization.CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            bytes = Encoding.UTF32.GetBytes(d.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        }

                        byte[] temp = Encoding.UTF8.GetBytes(bytes.Length.ToString());
                        serializationStream.Write(temp, 0, temp.Length); // beírom a hosszt
                        serializationStream.WriteByte(0);

                        serializationStream.Write(bytes, 0, bytes.Length);
                    }
                    else if (reflectionType.Equals(typeof(double?)))
                    {
                        double? c = (double?)InstanceValue;
                        if (double.MinValue.Equals(c.Value))
                        {
                            bytes = Encoding.UTF32.GetBytes(double.MinValue.ToString("r", System.Globalization.CultureInfo.InvariantCulture));
                        }
                        else if (double.MaxValue.Equals(c.Value))
                        {
                            bytes = Encoding.UTF32.GetBytes(double.MaxValue.ToString("r", System.Globalization.CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            bytes = Encoding.UTF32.GetBytes(c.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        }

                        byte[] temp = Encoding.UTF8.GetBytes(bytes.Length.ToString());
                        serializationStream.Write(temp, 0, temp.Length); // beírom a hosszt
                        serializationStream.WriteByte(0);

                        serializationStream.Write(bytes, 0, bytes.Length);
                    }
                    else if (reflectionType.Equals(typeof(decimal?)))
                    {
                        decimal? c = (decimal?)InstanceValue;
                        bytes = Encoding.UTF32.GetBytes(c.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));

                        byte[] temp = Encoding.UTF8.GetBytes(bytes.Length.ToString());
                        serializationStream.Write(temp, 0, temp.Length); // beírom a hosszt
                        serializationStream.WriteByte(0);

                        serializationStream.Write(bytes, 0, bytes.Length);
                    }
                    else if (typeof(Type).IsAssignableFrom(reflectionType))
                    {
                        Type s = (Type)InstanceValue;
                        bytes = Encoding.UTF32.GetBytes(s.AssemblyQualifiedName);

                        byte[] temp = Encoding.UTF8.GetBytes(bytes.Length.ToString());
                        serializationStream.Write(temp, 0, temp.Length); // beírom a hosszt
                        serializationStream.WriteByte(0);

                        serializationStream.Write(bytes, 0, bytes.Length); // majd a tartalmat
                    }
                    else if (reflectionType.Equals(typeof(string)))
                    {
                        string s = (string)InstanceValue;
                        bytes = Encoding.UTF32.GetBytes(s);

                        byte[] temp = Encoding.UTF8.GetBytes(bytes.Length.ToString());
                        serializationStream.Write(temp, 0, temp.Length); // beírom a hosszt
                        serializationStream.WriteByte(0);

                        serializationStream.Write(bytes, 0, bytes.Length); // majd a tartalmat
                    }
                    else if (reflectionType.Equals(typeof(char)))
                    {
                        char c = (char)InstanceValue;
                        bytes = Encoding.UTF32.GetBytes(new char[] { c });

                        byte[] temp = Encoding.UTF8.GetBytes(bytes.Length.ToString());
                        serializationStream.Write(temp, 0, temp.Length); // beírom a hosszt
                        serializationStream.WriteByte(0);

                        serializationStream.Write(bytes, 0, bytes.Length);
                    }
                    else if (reflectionType.Equals(typeof(char?)))
                    {
                        char? c = (char?)InstanceValue;
                        bytes = Encoding.UTF32.GetBytes(new char[] { c.Value });

                        byte[] temp = Encoding.UTF8.GetBytes(bytes.Length.ToString());
                        serializationStream.Write(temp, 0, temp.Length); // beírom a hosszt
                        serializationStream.WriteByte(0);

                        serializationStream.Write(bytes, 0, bytes.Length);
                    }
                    else if (reflectionType.Equals(typeof(byte[])))
                    {
                        bytes = (byte[])this.InstanceValue;
                        serializationStream.Write(bytes, 0, bytes.Length);
                    }
                    else
                    {
                        serializationStream.WriteByte(0);
                    }
                }
            }

            #endregion

        }

        [DebuggerDisplay("[{GetType().Name}, AssemblyId = {AssemblyId}, TypeId = {TypeId}, TypeName = '{TypeName}', IsArray = {IsArray}, IsGenericType = {IsGenericType}, IsGenericDeclaration = {IsGenericDeclaration}, IsNested = {IsNested}]")]
        internal class DTypeInfo : ICloneable
        {

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="DTypeInfo"/> class.
            /// </summary>
            internal DTypeInfo()
            {
                this.AssemblyId = -1;
                this.DeclaredTypeId = -1;
            }

            #endregion

            #region Internal properties

            /// <summary>
            /// Gets or sets the assembly id.
            /// </summary>
            /// <value>
            /// The assembly id.
            /// </value>
            internal int AssemblyId { get; set; }

            /// <summary>
            /// Gets or sets the type id.
            /// </summary>
            /// <value>
            /// The type id.
            /// </value>
            internal int TypeId { get; set; }

            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            /// <value>
            /// The type.
            /// </value>
            internal Type Type { get; set; }

            /// <summary>
            /// Gets or sets the name of the type.
            /// </summary>
            /// <value>
            /// The name of the type.
            /// </value>
            internal string TypeName { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is array.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is array; otherwise, <c>false</c>.
            /// </value>
            internal bool IsArray { get; set; }

            /// <summary>
            /// Gets the array rank.
            /// </summary>
            /// <value>
            /// The array rank.
            /// </value>
            internal int ArrayRank
            {
                get
                {
                    return this.Type.IsArray ? this.Type.GetArrayRank() : 0;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is generic type.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance is generic type; otherwise, <c>false</c>.
            /// </value>
            internal bool IsGenericType { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is generic declaration.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance is generic declaration; otherwise, <c>false</c>.
            /// </value>
            internal bool IsGenericDeclaration { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is nested.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is nested; otherwise, <c>false</c>.
            /// </value>
            internal bool IsNested { get; set; }

            /// <summary>
            /// Gets or sets the declared type id.
            /// </summary>
            /// <value>
            /// The declared type id.
            /// </value>
            internal int DeclaredTypeId { get; set; }

            /// <summary>
            /// Gets or sets the generic parameter type ids.
            /// ez akkor is null, ha a IsGenericDeclaration == true
            /// </summary>
            /// <value>
            /// The generic parameter type ids.
            /// </value>
            internal List<int> GenericParameterTypeIds { get; set; }

            /// <summary>
            /// Gets or sets the deserializable field number.
            /// </summary>
            /// <value>
            /// The deserializable field number.
            /// </value>
            internal int DeserializableFieldNumber { get; set; }

            #endregion

            #region Internal method(s)

            /// <summary>
            /// Resolves the type.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <returns></returns>
            internal Type ResolveType(DeserializationContext context)
            {
                if (this.Type == null)
                {
                    string type = TypeName;

                    if (IsNested)
                    {
                        type = context.TypeIdVsTypeInfo[DeclaredTypeId].AppendNestedType(context, type);
                    }

                    if (IsGenericType && !IsGenericDeclaration)
                    {
                        StringBuilder sb = new StringBuilder("[");
                        bool appendComma = false;
                        foreach (int genericTypeId in GenericParameterTypeIds)
                        {
                            if (appendComma)
                            {
                                sb.Append(",");
                            }
                            else
                            {
                                appendComma = true;
                            }
                            sb.Append("[");
                            sb.Append(context.TypeIdVsTypeInfo[genericTypeId].ResolveType(context).AssemblyQualifiedName);
                            sb.Append("]");
                        }
                        sb.Append("]");
                        type = type.Replace("*", sb.ToString());
                    }

                    if (AssemblyId != -1)
                    {
                        type = string.Format("{0}, {1}", type, context.AsmIdVsAssemblyName[AssemblyId].FullName);
                    }

                    this.Type = TypeHelper.GetTypeFromString(type, context.TypeLookupMode, context.FindNewestTypeVersion, true, false);
                }
                return this.Type;
            }

            /// <summary>
            /// Appends the type of the nested.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="typeName">Name of the type.</param>
            /// <returns></returns>
            internal string AppendNestedType(DeserializationContext context, string typeName)
            {
                if (IsNested)
                {
                    return context.TypeIdVsTypeInfo[DeclaredTypeId].AppendNestedType(context, string.Format("{0}+{1}", this.TypeName, typeName));
                }
                else
                {
                    return string.Format("{0}+{1}", this.TypeName, typeName);
                }
            }

            #endregion

            #region Public method(s)

            /// <summary>
            /// Creates a new object that is a copy of the current instance.
            /// </summary>
            /// <returns>
            /// A new object that is a copy of this instance.
            /// </returns>
            public object Clone()
            {
                DTypeInstanceProxy proxy = new DTypeInstanceProxy();
                proxy.AssemblyId = this.AssemblyId;
                proxy.TypeId = this.TypeId;
                proxy.Type = this.Type;
                proxy.TypeName = this.TypeName;
                proxy.IsArray = this.IsArray;
                proxy.IsGenericType = this.IsGenericType;
                proxy.IsGenericDeclaration = this.IsGenericDeclaration;
                proxy.IsNested = this.IsNested;
                proxy.DeclaredTypeId = this.DeclaredTypeId;
                if (this.GenericParameterTypeIds != null)
                {
                    proxy.GenericParameterTypeIds = new List<int>(this.GenericParameterTypeIds);
                }
                proxy.DeserializableFieldNumber = this.DeserializableFieldNumber;
                return proxy;
            }

            #endregion

        }

        [DebuggerDisplay("[{GetType().Name}, InstanceId = {InstanceId}, AssemblyId = {AssemblyId}, TypeId = {TypeId}, TypeName = '{TypeName}', IsArray = {IsArray}, IsGenericType = {IsGenericType}, IsGenericDeclaration = {IsGenericDeclaration}, IsNested = {IsNested}]")]
        internal sealed class DTypeInstanceProxy : DTypeInfo
        {

            #region Field(s)

            private readonly Dictionary<string, int?> mFieldsVsInstanceIds = new Dictionary<string, int?>();

            private readonly List<int> mArrayItemsRefIds = new List<int>();

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="DTypeInstanceProxy"/> class.
            /// </summary>
            internal DTypeInstanceProxy()
            {
            }

            #endregion

            #region Internal properties

            /// <summary>
            /// Gets or sets the instance id.
            /// </summary>
            /// <value>
            /// The instance id.
            /// </value>
            internal long InstanceId { get; set; }

            /// <summary>
            /// Gets or sets the instance value.
            /// </summary>
            /// <value>
            /// The instance value.
            /// </value>
            internal object InstanceValue { get; set; }

            /// <summary>
            /// Gets or sets the array key.
            /// </summary>
            /// <value>
            /// The array key.
            /// </value>
            internal List<int> ArrayKeysRefIds { get; set; }

            /// <summary>
            /// Gets the array items ref ids.
            /// </summary>
            /// <value>
            /// The array items ref ids.
            /// </value>
            internal List<int> ArrayItemsRefIds
            {
                get { return mArrayItemsRefIds; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is array dimension representation.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance is array dimension representation; otherwise, <c>false</c>.
            /// </value>
            internal bool IsArrayDimensionRepresentation { get; set; }

            /// <summary>
            /// Gets the fields vs instance ids.
            /// </summary>
            /// <value>
            /// The fields vs instance ids.
            /// </value>
            internal Dictionary<string, int?> FieldsVsInstanceIds
            {
                get { return mFieldsVsInstanceIds; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is constructed.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance is constructed; otherwise, <c>false</c>.
            /// </value>
            internal bool IsConstructed { get; set; }

            #endregion

            #region Internal method(s)

            /// <summary>
            /// Constructs the specified context.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <returns></returns>
            /// <exception cref="System.MissingFieldException"></exception>
            [SecurityCritical]
            internal object Construct(DeserializationContext context)
            {
                if (!IsConstructed)
                {
                    if (this.IsArrayDimensionRepresentation)
                    {
                    }
                    else if (typeof(ISerializable).IsAssignableFrom(this.Type))
                    {
                        ISurrogateSelector selector = null;
                        ISerializationSurrogate surrogate = (context.Selector == null ? null : context.Selector.GetSurrogate(this.Type, context.Context, out selector));
                        SerializationInfo info = new SerializationInfo(this.Type, new FormatterConverter());
                        for (int i = 0; i < this.ArrayKeysRefIds.Count; i++)
                        {
                            DTypeInstanceProxy proxyKey = context.InstanceIdVsInstanceProxy[this.ArrayKeysRefIds[i]];
                            DTypeInstanceProxy proxyItem = context.InstanceIdVsInstanceProxy[this.ArrayItemsRefIds[i]];
                            info.AddValue((string)proxyKey.Construct(context), proxyItem.Construct(context));
                        }

                        if (surrogate == null)
                        {
                            bool found = false;

                            foreach (ConstructorInfo ci in this.Type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                            {
                                ParameterInfo[] pis = ci.GetParameters();
                                if (pis != null && pis.Length == 2 && pis[0].ParameterType.Equals(typeof(SerializationInfo)) && pis[1].ParameterType.Equals(typeof(StreamingContext)))
                                {
                                    found = true;
                                    this.InstanceValue = ci.Invoke(new object[] { info, context.Context });
                                    this.IsConstructed = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                throw new SerializationException(string.Format("The constructor to deserialize an object type '{0}' was not found.", this.Type.FullName));
                            }

                            if (this.InstanceValue is IDeserializationCallback)
                            {
                                ((IDeserializationCallback)this.InstanceValue).OnDeserialization(context.Context);
                            }
                        }
                        else
                        {
                            this.InstanceValue = surrogate.SetObjectData(FormatterServices.GetUninitializedObject(this.Type), info, context.Context, selector);
                            this.IsConstructed = true;
                        }
                    }
                    else if (this.Type.IsArray)
                    {
                        // a dimenziók méretének megállapítása
                        List<int> dimSizes = new List<int>();
                        DTypeInstanceProxy proxy = this;
                        for (int i = 0; i < this.ArrayRank; i++)
                        {
                            dimSizes.Add(proxy.ArrayItemsRefIds.Count);
                            if (proxy.ArrayItemsRefIds.Count > 0)
                            {
                                foreach (int id in proxy.ArrayItemsRefIds)
                                {
                                    if (id > -1)
                                    {
                                        proxy = context.InstanceIdVsInstanceProxy[id];
                                        break;
                                    }
                                }
                            }
                        }

                        Array array = Array.CreateInstance(this.Type.GetElementType(), dimSizes.ToArray());
                        this.InstanceValue = array;
                        this.IsConstructed = true;

                        int[] indices = new int[dimSizes.Count];
                        SetArrayItems(context, array, 0, indices);
                    }
                    else
                    {

                        #region Restore object

                        Type currentType = this.Type;
                        if (!(currentType.IsGenericType && currentType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))))
                        {
                            this.InstanceValue = FormatterServices.GetUninitializedObject(this.Type);
                            this.IsConstructed = true;
                        }

                        foreach (KeyValuePair<string, int?> kv in mFieldsVsInstanceIds)
                        {
                            currentType = this.Type;
                            if (currentType.IsGenericType && currentType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                            {
                                // nullable típust nem viszi a reflector
                                object obj = context.InstanceIdVsInstanceProxy[kv.Value.Value].Construct(context);
                                this.InstanceValue = currentType.GetConstructors()[0].Invoke(new object[] { obj });
                                this.IsConstructed = true;
                            }
                            else
                            {
                                string fieldName = kv.Key.Substring(kv.Key.IndexOf(".") + 1);
                                int typeId = int.Parse(kv.Key.Substring(0, kv.Key.IndexOf(".")));
                                Type targetType = context.TypeIdVsTypeInfo[typeId].Type;
                                bool found = false;

                                while (currentType != typeof(object) && currentType != typeof(ValueType) &&
                                    currentType != typeof(MarshalByRefObject) && currentType != typeof(MBRBase))
                                {
                                    if (currentType.Equals(targetType))
                                    {
                                        foreach (FieldInfo fi in currentType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                                        {
                                            if (fi.Name.Equals(fieldName))
                                            {
                                                found = true;
                                                if (!fi.IsNotSerialized)
                                                {
                                                    int? v = kv.Value;
                                                    if (v == null || !v.HasValue)
                                                    {
                                                        fi.SetValue(this.InstanceValue, null);
                                                    }
                                                    else
                                                    {
                                                        fi.SetValue(this.InstanceValue, context.InstanceIdVsInstanceProxy[v.Value].Construct(context));
                                                    }
                                                }
                                                break;
                                            }
                                        }
                                        if (found)
                                        {
                                            break;
                                        }
                                    }
                                    currentType = currentType.BaseType;
                                }

                                if (!found && context.SerializerBehavior == BinarySerializerBehaviorEnum.ThrowExceptionOnMissingField)
                                {
                                    throw new MissingFieldException(this.Type.FullName, fieldName);
                                }
                            }
                        }

                        #endregion

                    }
                }

                return this.InstanceValue;
            }

            #endregion

            #region Private method(s)

            [SecurityCritical]
            private void SetArrayItems(DeserializationContext context, Array array, int dimension, int[] indices)
            {
                for (int i = 0; i < mArrayItemsRefIds.Count; i++)
                {
                    indices[dimension] = i;
                    int instanceId = mArrayItemsRefIds[i];
                    // ha -1 az érték, az null-t jelent
                    if (instanceId == -1)
                    {
                        array.SetValue(null, indices);
                    }
                    else
                    {
                        DTypeInstanceProxy proxy = context.InstanceIdVsInstanceProxy[instanceId];
                        if (proxy.IsArrayDimensionRepresentation)
                        {
                            // dimension level
                            proxy.SetArrayItems(context, array, dimension + 1, indices);
                        }
                        else
                        {
                            // value/reference proxy
                            array.SetValue(proxy.Construct(context), indices);
                        }
                    }
                }
                indices[dimension] = 0;
            }

            #endregion

        }

        #endregion

    }

}
