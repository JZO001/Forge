/* *********************************************************************
 * Date: 12 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Forge.Legacy;
using Forge.Net.Remoting.Channels;
using Forge.Net.Remoting.Proxy;
using Forge.Net.Remoting.Validators;
using Forge.Shared;

namespace Forge.Net.Remoting.ProxyGenerator
{

    /// <summary>
    /// Helper constants and methods for generators
    /// </summary>
    internal abstract class GeneratorBase
    {

        protected static readonly string TAB_SPACE = "    ";

        protected static readonly byte[] TAB_SPACE_BYTES = GetBytes("    ");

        protected static readonly string NEW_LINE = Environment.NewLine;

        protected static readonly byte[] NEW_LINE_BYTES = GetBytes(NEW_LINE);

        protected static readonly byte[] COMMENT_STREAM_NOTICE = GetBytes(TAB_SPACE + TAB_SPACE + TAB_SPACE + "// NOTICE: your responsibility to call Dispose() method on the given Stream(s) before you return from this statement or later on an other thread." + NEW_LINE);

        protected static readonly byte[] COMMENT_PATTERN = GetBytes(TAB_SPACE + TAB_SPACE + TAB_SPACE + TAB_SPACE + "// you may use this try-finally pattern and take your code here" + NEW_LINE);

        protected static readonly byte[] CODE_DISPOSECHECK = GetBytes(TAB_SPACE + TAB_SPACE + TAB_SPACE + "DoDisposeCheck();" + NEW_LINE);

        protected static readonly byte[] CODE_RESPONSEMESSAGE_VARIABLE = GetBytes(TAB_SPACE + TAB_SPACE + TAB_SPACE + "Forge.Net.Remoting.Messaging.ResponseMessage _response = null;" + NEW_LINE);

        protected static readonly byte[] CODE_DEBUGGER_STEP_THROUGH_ATTRIBUTE = GetBytes(TAB_SPACE + TAB_SPACE + "[" + typeof(DebuggerStepThroughAttribute).FullName + "]" + NEW_LINE);

        protected static readonly byte[] CODE_PUBLIC_METHOD = GetBytes(TAB_SPACE + TAB_SPACE + "public ");

        protected static readonly byte[] CODE_PUBLIC_CLASS = GetBytes("public ");

        protected static readonly byte[] CODE_PROTECTED_METHOD = GetBytes(TAB_SPACE + TAB_SPACE + "protected ");

        protected static readonly byte[] CODE_ABSTRACT_METHOD = GetBytes("abstract ");

        protected static readonly byte[] CODE_CLASS = GetBytes("class ");

        protected static readonly byte[] CODE_BEGIN_TRY_BLOCK = GetBytes(TAB_SPACE + TAB_SPACE + TAB_SPACE + "try" + NEW_LINE + TAB_SPACE + TAB_SPACE + TAB_SPACE + "{" + NEW_LINE);

        protected static readonly byte[] CODE_METHOD_PARAMETER_DECLARATION = GetBytes(TAB_SPACE + TAB_SPACE + TAB_SPACE + TAB_SPACE + "Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;" + NEW_LINE);

        protected static readonly string CODE_METHOD_PARAMETER_ITEM = TAB_SPACE + TAB_SPACE + TAB_SPACE + TAB_SPACE + "Forge.Net.Remoting.Messaging.MethodParameter _mp{0} = new Forge.Net.Remoting.Messaging.MethodParameter({1}, typeof({2}).FullName + \", \" + new System.Reflection.AssemblyName(typeof({2}).Assembly.FullName).Name, {3});" + NEW_LINE;

        protected static readonly string CODE_METHOD_PARAMETER_ARRAY = TAB_SPACE + TAB_SPACE + TAB_SPACE + TAB_SPACE + "_mps = new Forge.Net.Remoting.Messaging.MethodParameter[] <REPLACELEFT> {0} <REPLACERIGHT>;" + NEW_LINE;

        protected static readonly string CODE_MESSAGE_DECLARATION = TAB_SPACE + TAB_SPACE + TAB_SPACE + TAB_SPACE + "Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.{0}, Forge.Net.Remoting.MessageInvokeModeEnum.{1}, typeof({2}), \"{3}\", _mps, {4});" + NEW_LINE;

        protected static readonly byte[] CODE_CONTEXT_FILL_SERVICESIDE = GetBytes(TAB_SPACE + TAB_SPACE + TAB_SPACE + TAB_SPACE + "_message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, Forge.Net.Remoting.ServiceBase.GetPeerProxyId(this));" + NEW_LINE);

        protected static readonly byte[] CODE_CONTEXT_FILL_PROXYSIDE = GetBytes(TAB_SPACE + TAB_SPACE + TAB_SPACE + TAB_SPACE + "_message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, this.ProxyId);" + NEW_LINE);

        protected static readonly string CODE_TIMEOUT = TAB_SPACE + TAB_SPACE + TAB_SPACE + TAB_SPACE + "long _timeout = GetTimeoutByMethod(typeof({0}), \"{1}\", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);" + NEW_LINE;

        protected static readonly byte[] CODE_RESPONSE_MESSAGE_ASSIGN = GetBytes(TAB_SPACE + TAB_SPACE + TAB_SPACE + TAB_SPACE + "_response = (Forge.Net.Remoting.Messaging.ResponseMessage)");

        protected static readonly byte[] CODE_SEND_MESSAGE = GetBytes("this.mChannel.SendMessage(this.mSessionId, _message, _timeout);" + NEW_LINE);

        protected static readonly byte[] CODE_CHECK_PROXY_REGISTERED = GetBytes(TAB_SPACE + TAB_SPACE + TAB_SPACE + TAB_SPACE + "Forge.Net.Remoting.ServiceBase.CheckProxyRegistered(this);" + NEW_LINE);

        protected static readonly byte[] CODE_NAMESPACE = GetBytes("namespace ");

        protected static readonly byte[] CODE_EXTENDS = GetBytes(": ");

        protected static readonly byte[] CODE_NOT_IMPLEMENTED_EXCEPTION = GetBytes(TAB_SPACE + TAB_SPACE + TAB_SPACE + "throw new System.NotImplementedException();" + NEW_LINE);

        protected static readonly byte[] CODE_TRY_FINALLY = GetBytes(TAB_SPACE + TAB_SPACE + TAB_SPACE + "}" + NEW_LINE + TAB_SPACE + TAB_SPACE + TAB_SPACE + "finally" + NEW_LINE + TAB_SPACE + TAB_SPACE + TAB_SPACE + "{" + NEW_LINE);

        protected static readonly string CODE_RETURN_VALUE = "return ({0})_response.ReturnValue.Value;" + NEW_LINE;

        protected static readonly byte[] VOID = GetBytes("void");

        protected static readonly byte[] SPACE = GetBytes(" ");

        protected static readonly byte[] BRACKET_LEFT = GetBytes("(");

        protected static readonly byte[] BRACKET_RIGHT = GetBytes(")");

        protected static readonly byte[] COMMA = GetBytes(",");

        protected static readonly byte[] LEFT_CURLY_BRACE = GetBytes("{");

        protected static readonly byte[] RIGHT_CURLY_BRACE = GetBytes("}");

        protected static readonly byte[] SEMICOLON = GetBytes(";");

        protected GeneratorBase()
        {
        }

        protected static byte[] GetBytes(string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }

        protected static void Write(Stream stream, string str)
        {
            byte[] b = Encoding.UTF8.GetBytes(str);
            stream.Write(b, 0, b.Length);
        }

        protected static void Write(Stream stream, byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Writes the abstract proxy class header.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="isServiceProxy">if set to <c>true</c> [is service proxy].</param>
        /// <param name="stream">The stream.</param>
        public static void WriteAbstractProxyClassHeader(Type contractType, bool isServiceProxy, Stream stream)
        {
            if (contractType == null)
            {
                ThrowHelper.ThrowArgumentNullException("contractType");
            }
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            // construct type name
            TypeDescriptor descriptor = new TypeDescriptor(contractType);

            // write namespace
            stream.Write(CODE_NAMESPACE, 0, CODE_NAMESPACE.Length);
            Write(stream, isServiceProxy ? GetBytes(descriptor.ServicePackage) : GetBytes(descriptor.ClientPackage));
            Write(stream, NEW_LINE_BYTES);
            Write(stream, LEFT_CURLY_BRACE);
            Write(stream, NEW_LINE_BYTES);
            Write(stream, NEW_LINE_BYTES);

            // write class header
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, CODE_PUBLIC_CLASS);
            Write(stream, CODE_ABSTRACT_METHOD);
            Write(stream, CODE_CLASS);
            Write(stream, isServiceProxy ? GetBytes(descriptor.TypeNameServiceAbstract) : GetBytes(descriptor.TypeNameClientAbstract));
            Write(stream, SPACE);
            Write(stream, CODE_EXTENDS);
            Write(stream, typeof(ProxyBase).FullName);
            Write(stream, COMMA);
            Write(stream, SPACE);
            Write(stream, contractType.FullName);
            Write(stream, NEW_LINE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, LEFT_CURLY_BRACE);
            Write(stream, NEW_LINE_BYTES);
        }

        /// <summary>
        /// Writes the implementation class header.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="superClassType">Type of the super class.</param>
        /// <param name="isServiceProxy">if set to <c>true</c> [is service proxy].</param>
        /// <param name="stream">The stream.</param>
        public static void WriteImplementationClassHeader(Type contractType, string superClassType, bool isServiceProxy, Stream stream)
        {
            if (contractType == null)
            {
                ThrowHelper.ThrowArgumentNullException("contractType");
            }
            if (!isServiceProxy)
            {
                if (string.IsNullOrEmpty(superClassType))
                {
                    ThrowHelper.ThrowArgumentNullException("superClassType");
                }
            }
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            // Service oldal:
            // superClassType lehet egy abstract class vagy proxyBase vagy üres
            // ha ProxyBase-ből származtatjuk, akkor implements-et kell írni a contract típusával

            // Client oldal:
            // superClassType lehet egy abstract class vagy proxyBase, kötelező
            // ha ProxyBase-ből származtatjuk, akkor implements-et kell írni a contract típusával

            // construct type name
            TypeDescriptor descriptor = new TypeDescriptor(contractType);

            // write package
            Write(stream, CODE_NAMESPACE);
            Write(stream, isServiceProxy ? GetBytes(descriptor.ServicePackage) : GetBytes(descriptor.ClientPackage));
            Write(stream, NEW_LINE_BYTES);
            Write(stream, LEFT_CURLY_BRACE);
            Write(stream, NEW_LINE_BYTES);
            Write(stream, NEW_LINE_BYTES);

            // write class header
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, CODE_PUBLIC_CLASS);
            Write(stream, CODE_CLASS);
            Write(stream, isServiceProxy ? GetBytes(descriptor.TypeNameServiceImpl) : GetBytes(descriptor.TypeNameClientImpl));
            Write(stream, SPACE);
            Write(stream, CODE_EXTENDS);

            if (!string.IsNullOrEmpty(superClassType))
            {
                Write(stream, superClassType);
            }
            else
            {
                Write(stream, typeof(MBRBase).FullName);
            }

            if (typeof(ProxyBase).FullName.Equals(superClassType) || string.IsNullOrEmpty(superClassType))
            {
                Write(stream, COMMA);
                Write(stream, SPACE);
                Write(stream, contractType.FullName);
            }

            Write(stream, NEW_LINE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, LEFT_CURLY_BRACE);
            Write(stream, NEW_LINE_BYTES);
        }

        /// <summary>
        /// Writes the empty contructor.
        /// </summary>
        /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="stream">The stream.</param>
        public static void WriteEmptyContructor(bool isVisible, string typeName, Stream stream)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                ThrowHelper.ThrowArgumentNullException("typeName");
            }
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            Write(stream, NEW_LINE_BYTES);
            if (isVisible)
            {
                Write(stream, CODE_PUBLIC_METHOD);
            }
            else
            {
                Write(stream, CODE_PROTECTED_METHOD);
            }
            Write(stream, typeName);
            Write(stream, BRACKET_LEFT);
            Write(stream, BRACKET_RIGHT);
            Write(stream, NEW_LINE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, LEFT_CURLY_BRACE);
            Write(stream, NEW_LINE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, RIGHT_CURLY_BRACE);
            Write(stream, NEW_LINE_BYTES);
        }

        /// <summary>
        /// Writes the proxy contructor.
        /// </summary>
        /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="stream">The stream.</param>
        public static void WriteProxyContructor(bool isVisible, string typeName, Stream stream)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                ThrowHelper.ThrowArgumentNullException("typeName");
            }
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            Write(stream, NEW_LINE_BYTES);
            if (isVisible)
            {
                Write(stream, CODE_PUBLIC_METHOD);
            }
            else
            {
                Write(stream, CODE_PROTECTED_METHOD);
            }
            Write(stream, typeName);
            Write(stream, BRACKET_LEFT);
            Write(stream, typeof(Channel).FullName);
            Write(stream, " channel, ");
            Write(stream, typeof(string).FullName);
            Write(stream, " sessionId) : base(channel, sessionId) { }");
            Write(stream, NEW_LINE_BYTES);
        }

        /// <summary>
        /// Writes the abstract methods.
        /// </summary>
        /// <param name="methods">The methods.</param>
        /// <param name="stream">The stream.</param>
        public static void WriteAbstractMethods(IList<MethodComparator> methods, Stream stream)
        {
            if (methods == null)
            {
                ThrowHelper.ThrowArgumentNullException("methods");
            }
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            IEnumerator<MethodComparator> iterator = methods.GetEnumerator();
            while (iterator.MoveNext())
            {
                MethodComparator mc = iterator.Current;
                Write(stream, NEW_LINE_BYTES);
                Write(stream, CODE_PUBLIC_METHOD);
                Write(stream, CODE_ABSTRACT_METHOD);
                if (mc.Method.ReturnType.Equals(typeof(void)))
                {
                    Write(stream, VOID);
                }
                else
                {
                    Write(stream, mc.Method.ReturnType.FullName);
                }
                Write(stream, SPACE);
                Write(stream, mc.Method.Name);
                Write(stream, BRACKET_LEFT);

                if (mc.Method.GetParameters().Length > 0)
                {
                    for (int i = 0; i < mc.Method.GetParameters().Length; i++)
                    {
                        if (i > 0)
                        {
                            Write(stream, ", ");
                        }
                        Type pType = mc.Method.GetParameters()[i].ParameterType;
                        if (IsTypeFromNullable(ref pType))
                        {
                            Write(stream, string.Format("{0}?", pType.FullName));
                        }
                        else
                        {
                            Write(stream, pType.FullName);
                        }
#if NET40
                        Write(stream, string.Format(" _p{0}", i));
#else
                        Write(stream, string.Format(" {0}", mc.Method.GetParameters()[i].Name));
#endif
                    }
                }
                Write(stream, BRACKET_RIGHT);
                Write(stream, SEMICOLON);
                Write(stream, NEW_LINE_BYTES);
            }
        }

        /// <summary>
        /// Writes the methods.
        /// </summary>
        /// <param name="methods">The methods.</param>
        /// <param name="writeOverride">if set to <c>true</c> [write override].</param>
        /// <param name="stream">The stream.</param>
        public static void WriteMethods(IList<MethodComparator> methods, bool writeOverride, Stream stream)
        {
            if (methods == null)
            {
                ThrowHelper.ThrowArgumentNullException("methods");
            }
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            IEnumerator<MethodComparator> iterator = methods.GetEnumerator();
            while (iterator.MoveNext())
            {
                MethodComparator mc = iterator.Current;
                Write(stream, NEW_LINE_BYTES);
                Write(stream, CODE_PUBLIC_METHOD);
                if (writeOverride)
                {
                    Write(stream, "override ");
                }
                if (mc.Method.ReturnType.Equals(typeof(void)))
                {
                    Write(stream, VOID);
                }
                else
                {
                    Write(stream, mc.Method.ReturnType.FullName);
                }
                Write(stream, SPACE);
                Write(stream, mc.Method.Name);
                Write(stream, BRACKET_LEFT);

                if (mc.Method.GetParameters().Length > 0)
                {
                    for (int i = 0; i < mc.Method.GetParameters().Length; i++)
                    {
                        if (i > 0)
                        {
                            Write(stream, ", ");
                        }
                        Type pType = mc.Method.GetParameters()[i].ParameterType;
                        if (IsTypeFromNullable(ref pType))
                        {
                            Write(stream, string.Format("{0}?", pType.FullName));
                        }
                        else
                        {
                            Write(stream, pType.FullName);
                        }
#if NET40
                        Write(stream, string.Format(" _p{0}", i));
#else
                        Write(stream, string.Format(" {0}", mc.Method.GetParameters()[i].Name));
#endif
                    }
                }
                Write(stream, BRACKET_RIGHT);
                Write(stream, NEW_LINE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, LEFT_CURLY_BRACE);
                Write(stream, NEW_LINE_BYTES);

                int index = 0;
                bool commentWrote = false;
                foreach (ParameterInfo cls in mc.Method.GetParameters())
                {
                    if (typeof(Stream).IsAssignableFrom(cls.ParameterType))
                    {
                        if (!commentWrote)
                        {
                            // stream detected, write notice and code pattern
                            Write(stream, COMMENT_STREAM_NOTICE);
                            Write(stream, CODE_BEGIN_TRY_BLOCK);
                            Write(stream, COMMENT_PATTERN);
                            Write(stream, NEW_LINE_BYTES);
                            Write(stream, CODE_TRY_FINALLY);
                            commentWrote = true;
                        }

                        Write(stream, TAB_SPACE_BYTES);
                        Write(stream, TAB_SPACE_BYTES);
                        Write(stream, TAB_SPACE_BYTES);
                        Write(stream, TAB_SPACE_BYTES);
#if NET40
                        Write(stream, string.Format("if (_p{0} != null)", index));
#else
                        Write(stream, string.Format("if ({0} != null)", cls.Name));
#endif
                        Write(stream, NEW_LINE_BYTES);
                        Write(stream, TAB_SPACE_BYTES);
                        Write(stream, TAB_SPACE_BYTES);
                        Write(stream, TAB_SPACE_BYTES);
                        Write(stream, TAB_SPACE_BYTES);
                        Write(stream, "{");
                        Write(stream, NEW_LINE_BYTES);
                        Write(stream, TAB_SPACE_BYTES);
                        Write(stream, TAB_SPACE_BYTES);
                        Write(stream, TAB_SPACE_BYTES);
                        Write(stream, TAB_SPACE_BYTES);
                        Write(stream, TAB_SPACE_BYTES);
#if NET40
                        Write(stream, string.Format("_p{0}.Dispose();", index));
#else
                        Write(stream, string.Format("{0}.Dispose();", cls.Name));
#endif
                        Write(stream, NEW_LINE_BYTES);
                        Write(stream, TAB_SPACE_BYTES);
                        Write(stream, TAB_SPACE_BYTES);
                        Write(stream, TAB_SPACE_BYTES);
                        Write(stream, TAB_SPACE_BYTES);
                        Write(stream, RIGHT_CURLY_BRACE);
                        Write(stream, NEW_LINE_BYTES);
                    }
                    index++;
                }
                if (commentWrote)
                {
                    Write(stream, TAB_SPACE_BYTES);
                    Write(stream, TAB_SPACE_BYTES);
                    Write(stream, TAB_SPACE_BYTES);
                    Write(stream, RIGHT_CURLY_BRACE);
                    Write(stream, NEW_LINE_BYTES);
                }

                Write(stream, CODE_NOT_IMPLEMENTED_EXCEPTION);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, RIGHT_CURLY_BRACE);
                Write(stream, NEW_LINE_BYTES);
            }
        }

        /// <summary>
        /// Writes the abstract properties.
        /// </summary>
        /// <param name="prs">The property comparators.</param>
        /// <param name="stream">The stream.</param>
        public static void WriteAbstractProperties(IList<PropertyComparator> prs, Stream stream)
        {
            if (prs == null)
            {
                ThrowHelper.ThrowArgumentNullException("prs");
            }
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            IEnumerator<PropertyComparator> iterator = prs.GetEnumerator();
            while (iterator.MoveNext())
            {
                PropertyComparator pc = iterator.Current;
                Write(stream, NEW_LINE_BYTES);
                Write(stream, CODE_PUBLIC_METHOD);
                Write(stream, CODE_ABSTRACT_METHOD);
                Write(stream, pc.PropertyInfo.PropertyType.FullName);
                Write(stream, SPACE);
                Write(stream, pc.PropertyInfo.Name);
                Write(stream, NEW_LINE_BYTES);
                Write(stream, TAB_SPACE);
                Write(stream, TAB_SPACE);
                Write(stream, LEFT_CURLY_BRACE);
                if (pc.GetMethod != null)
                {
                    Write(stream, NEW_LINE_BYTES);
                    Write(stream, TAB_SPACE);
                    Write(stream, TAB_SPACE);
                    Write(stream, TAB_SPACE);
                    Write(stream, "get;");
                }
                if (pc.SetMethod != null)
                {
                    Write(stream, NEW_LINE_BYTES);
                    Write(stream, TAB_SPACE);
                    Write(stream, TAB_SPACE);
                    Write(stream, TAB_SPACE);
                    Write(stream, "set;");
                }
                Write(stream, NEW_LINE_BYTES);
                Write(stream, TAB_SPACE);
                Write(stream, TAB_SPACE);
                Write(stream, RIGHT_CURLY_BRACE);
                Write(stream, NEW_LINE_BYTES);
            }
        }

        /// <summary>
        /// Writes the properties.
        /// </summary>
        /// <param name="prs">The property comparators.</param>
        /// <param name="writeOverride">if set to <c>true</c> [write override].</param>
        /// <param name="stream">The stream.</param>
        public static void WriteProperties(IList<PropertyComparator> prs, bool writeOverride, Stream stream)
        {
            if (prs == null)
            {
                ThrowHelper.ThrowArgumentNullException("prs");
            }
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            IEnumerator<PropertyComparator> iterator = prs.GetEnumerator();
            while (iterator.MoveNext())
            {
                PropertyComparator pc = iterator.Current;
                Write(stream, NEW_LINE_BYTES);
                Write(stream, CODE_PUBLIC_METHOD);
                if (writeOverride)
                {
                    Write(stream, "override ");
                }
                Write(stream, pc.PropertyInfo.PropertyType.FullName);
                Write(stream, SPACE);
                Write(stream, pc.PropertyInfo.Name);
                Write(stream, NEW_LINE_BYTES);
                Write(stream, TAB_SPACE);
                Write(stream, TAB_SPACE);
                Write(stream, LEFT_CURLY_BRACE);
                if (pc.GetMethod != null)
                {
                    Write(stream, NEW_LINE_BYTES);
                    Write(stream, TAB_SPACE);
                    Write(stream, TAB_SPACE);
                    Write(stream, TAB_SPACE);
                    Write(stream, "get");
                    Write(stream, NEW_LINE_BYTES);
                    Write(stream, TAB_SPACE);
                    Write(stream, TAB_SPACE);
                    Write(stream, TAB_SPACE);
                    Write(stream, LEFT_CURLY_BRACE);
                    Write(stream, NEW_LINE_BYTES);
                    Write(stream, TAB_SPACE);
                    Write(stream, CODE_NOT_IMPLEMENTED_EXCEPTION);
                    Write(stream, TAB_SPACE);
                    Write(stream, TAB_SPACE);
                    Write(stream, TAB_SPACE);
                    Write(stream, RIGHT_CURLY_BRACE);
                }
                if (pc.SetMethod != null)
                {
                    Write(stream, NEW_LINE_BYTES);
                    Write(stream, TAB_SPACE);
                    Write(stream, TAB_SPACE);
                    Write(stream, TAB_SPACE);
                    Write(stream, "set");
                    Write(stream, NEW_LINE_BYTES);
                    Write(stream, TAB_SPACE);
                    Write(stream, TAB_SPACE);
                    Write(stream, TAB_SPACE);
                    Write(stream, LEFT_CURLY_BRACE);
                    Write(stream, NEW_LINE_BYTES);
                    Write(stream, TAB_SPACE);
                    Write(stream, CODE_NOT_IMPLEMENTED_EXCEPTION);
                    Write(stream, TAB_SPACE);
                    Write(stream, TAB_SPACE);
                    Write(stream, TAB_SPACE);
                    Write(stream, RIGHT_CURLY_BRACE);
                }
                Write(stream, NEW_LINE_BYTES);
                Write(stream, TAB_SPACE);
                Write(stream, TAB_SPACE);
                Write(stream, RIGHT_CURLY_BRACE);
                Write(stream, NEW_LINE_BYTES);
            }
        }

        /// <summary>
        /// Writes the events.
        /// </summary>
        /// <param name="evs">The event comparators.</param>
        /// <param name="stream">The stream.</param>
        public static void WriteEvents(IList<EventComparator> evs, Stream stream)
        {
            if (evs == null)
            {
                ThrowHelper.ThrowArgumentNullException("prs");
            }
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            IEnumerator<EventComparator> iterator = evs.GetEnumerator();
            while (iterator.MoveNext())
            {
                EventComparator ec = iterator.Current;
                Write(stream, NEW_LINE_BYTES);
                Write(stream, CODE_PUBLIC_METHOD);
                Write(stream, "event ");
                if (ec.EventInfo.EventHandlerType.IsGenericType)
                {
                    Write(stream, ec.EventInfo.EventHandlerType.FullName.Substring(0, ec.EventInfo.EventHandlerType.FullName.IndexOf("`1")));
                    Write(stream, "<");
                    Write(stream, ec.EventInfo.EventHandlerType.GetGenericArguments()[0].FullName);
                    Write(stream, ">");
                }
                else
                {
                    Write(stream, ec.EventInfo.EventHandlerType.FullName);
                }
                Write(stream, SPACE);
                Write(stream, ec.EventInfo.Name);
                Write(stream, ";");
                Write(stream, NEW_LINE_BYTES);
            }
        }

        /// <summary>
        /// Writes the end class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public static void WriteEndClass(Stream stream)
        {
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            Write(stream, NEW_LINE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, RIGHT_CURLY_BRACE);
            Write(stream, NEW_LINE_BYTES);
            Write(stream, NEW_LINE_BYTES);
            Write(stream, RIGHT_CURLY_BRACE);
            Write(stream, NEW_LINE_BYTES);
        }

        protected static void WriteEndTryBlock(Stream stream)
        {
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, "}");
            Write(stream, NEW_LINE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, "catch (System.Exception ex)");
            Write(stream, NEW_LINE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, "{");
            Write(stream, NEW_LINE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, "throw new Forge.Net.Remoting.RemoteMethodInvocationException(\"Unable to call remote method. See inner exception for details.\", ex);");
            Write(stream, NEW_LINE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            stream.Write(RIGHT_CURLY_BRACE, 0, RIGHT_CURLY_BRACE.Length);
            Write(stream, NEW_LINE_BYTES);
        }

        protected static void WriteReturnValueAndExceptions(MethodInfo method, Stream stream)
        {
            Write(stream, NEW_LINE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            if (method.ReturnType.Equals(typeof(void)))
            {
                Write(stream, "if (_response.MethodInvocationException != null)");
                Write(stream, NEW_LINE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, "{");
                Write(stream, NEW_LINE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, "throw new Forge.Net.Remoting.RemoteMethodInvocationException(\"An exception arrived from the remote side. See inner exception for details.\", _response.MethodInvocationException);");
                Write(stream, NEW_LINE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                stream.Write(RIGHT_CURLY_BRACE, 0, RIGHT_CURLY_BRACE.Length);
                Write(stream, NEW_LINE_BYTES);
            }
            else
            {
                Write(stream, "if (_response.MethodInvocationException == null)");
                Write(stream, NEW_LINE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, "{");
                Write(stream, NEW_LINE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, string.Format(CODE_RETURN_VALUE, method.ReturnType.FullName));
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, "}");
                Write(stream, NEW_LINE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, "else");
                Write(stream, NEW_LINE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, "{");
                Write(stream, NEW_LINE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, "throw new Forge.Net.Remoting.RemoteMethodInvocationException(\"An exception arrived from the remote side. See inner exception for details.\", _response.MethodInvocationException);");
                Write(stream, NEW_LINE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                stream.Write(RIGHT_CURLY_BRACE, 0, RIGHT_CURLY_BRACE.Length);
                Write(stream, NEW_LINE_BYTES);
            }
        }

        protected static bool IsTypeFromNullable(ref Type type)
        {
            bool result = false;
            
            if (type.IsGenericType)
            {
                // mc.Method.GetParameters()[i].ParameterType.GenericTypeArguments
                if (type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    Type[] types = type.GetGenericArguments();
                    type = types[0];
                    result = true;
                }
            }

            return result;
        }

    }

}
