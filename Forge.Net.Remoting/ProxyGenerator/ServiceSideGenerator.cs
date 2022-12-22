/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.IO;
using System.Reflection;
using System.Text;
using Forge.Reflection;
using Forge.Shared;

namespace Forge.Net.Remoting.ProxyGenerator
{

    /// <summary>
    /// Generate remote method on client side
    /// </summary>
    internal sealed class ServiceSideGenerator : GeneratorBase
    {

        /// <summary>
        /// Generate remote method on client side
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="method">The method.</param>
        /// <param name="stream">The stream.</param>
        public static void GenerateServiceMethod(Type contractType, MethodInfo method, FileStream stream)
        {
            if (contractType == null)
            {
                ThrowHelper.ThrowArgumentNullException("contractType");
            }
            if (method == null)
            {
                ThrowHelper.ThrowArgumentNullException("method");
            }
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            OperationContractAttribute ocAnnotation = TypeHelper.GetAttribute<OperationContractAttribute>(method);

            Write(stream, NEW_LINE_BYTES);
            Write(stream, CODE_DEBUGGER_STEP_THROUGH_ATTRIBUTE);
            Write(stream, CODE_PUBLIC_METHOD);

            // visszatérési érték
            if (method.ReturnType.Equals(typeof(void)))
            {
                Write(stream, VOID);
            }
            else
            {
                Write(stream, method.ReturnType.FullName);
            }

            Write(stream, SPACE);

            // metódus neve
            Write(stream, method.Name);
            Write(stream, BRACKET_LEFT);

            // paraméterlista
            if (method.GetParameters().Length > 0)
            {
                for (int paramIndex = 0; paramIndex < method.GetParameters().Length; paramIndex++)
                {
                    // ha több paraméter van, akkor vesszővel és space-el elválasztom őket
                    if (paramIndex > 0)
                    {
                        Write(stream, COMMA);
                        Write(stream, SPACE);
                    }
                    // paraméter típusa
                    Type pType = method.GetParameters()[paramIndex].ParameterType;
                    if (IsTypeFromNullable(ref pType))
                    {
                        Write(stream, string.Format("{0}?", pType.FullName));
                    }
                    else
                    {
                        Write(stream, pType.FullName);
                    }
                    Write(stream, SPACE);
                    // paraméter neve
                    //string pName = string.Format("{0}{1}", method.GetParameters()[paramIndex].ParameterType.Name.Substring(0, 1).ToLower(), method.GetParameters()[paramIndex].ParameterType.Name.Substring(1));
                    //Write(stream, string.Format("{0}{1}", pName, paramIndex));
#if NET40
                    Write(stream, string.Format("_p{0}", paramIndex));
#else
                    Write(stream, method.GetParameters()[paramIndex].Name);
#endif
                }
            }
            Write(stream, BRACKET_RIGHT);
            Write(stream, NEW_LINE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, LEFT_CURLY_BRACE); // metódustörzs kezdete

            Write(stream, NEW_LINE_BYTES);
            Write(stream, CODE_DISPOSECHECK); // doDisposeCheck
            if (!ocAnnotation.IsOneWay)
            {
                Write(stream, CODE_RESPONSEMESSAGE_VARIABLE); // ResponseMessage variable declaration
            }
            Write(stream, CODE_BEGIN_TRY_BLOCK); // try

            Write(stream, CODE_CHECK_PROXY_REGISTERED); // proxy registration check
            Write(stream, NEW_LINE_BYTES);

            Write(stream, CODE_METHOD_PARAMETER_DECLARATION); // _mps declaration
            Write(stream, NEW_LINE_BYTES);

            // paraméter lista: _mpx sorozat
            if (method.GetParameters().Length > 0)
            {
                // paraméterek átadása
                for (int i = 0; i < method.GetParameters().Length; i++)
                {
                    Type pType = method.GetParameters()[i].ParameterType;
                    if (IsTypeFromNullable(ref pType))
                    {
#if NET40
                        Write(stream, string.Format(CODE_METHOD_PARAMETER_ITEM, i, i, string.Format("{0}?", pType.FullName), string.Format("_p{0}", i))); // declaration
#else
                        Write(stream, string.Format(CODE_METHOD_PARAMETER_ITEM, i, i, string.Format("{0}?", pType.FullName), method.GetParameters()[i].Name)); // declaration
#endif
                    }
                    else
                    {
#if NET40
                        Write(stream, string.Format(CODE_METHOD_PARAMETER_ITEM, i, i, pType.FullName, string.Format("_p{0}", i))); // declaration
#else
                        Write(stream, string.Format(CODE_METHOD_PARAMETER_ITEM, i, i, pType.FullName, method.GetParameters()[i].Name)); // declaration
#endif
                    }
                }
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < method.GetParameters().Length; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append("_mp");
                    sb.Append(i);
                }
                Write(stream, string.Format(CODE_METHOD_PARAMETER_ARRAY, sb.ToString()).Replace("<REPLACELEFT>", "{").Replace("<REPLACERIGHT>", "}"));
                Write(stream, NEW_LINE_BYTES);
            }

            // message declaration
            Write(stream, string.Format(CODE_MESSAGE_DECLARATION, ocAnnotation.IsOneWay ? (ocAnnotation.IsReliable ? MessageTypeEnum.Datagram.ToString() : MessageTypeEnum.DatagramOneway.ToString()) : MessageTypeEnum.Request.ToString(), MessageInvokeModeEnum.RequestCallback.ToString(), contractType.FullName, method.Name, ocAnnotation.AllowParallelExecution.ToString().ToLower()));
            Write(stream, CODE_CONTEXT_FILL_SERVICESIDE); // fill proxyid
            Write(stream, NEW_LINE_BYTES);

            // timeout
            Write(stream, string.Format(CODE_TIMEOUT, contractType.FullName, method.Name));
            Write(stream, NEW_LINE_BYTES);

            // send message and optionally receive response
            if (ocAnnotation.IsOneWay)
            {
                Write(stream, TAB_SPACE_BYTES); // TABs
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
                Write(stream, TAB_SPACE_BYTES);
            }
            else
            {
                Write(stream, CODE_RESPONSE_MESSAGE_ASSIGN);
            }
            Write(stream, CODE_SEND_MESSAGE);

            WriteEndTryBlock(stream); // try block vége catch ággal

            if (!ocAnnotation.IsOneWay)
            {
                // return value or exception
                WriteReturnValueAndExceptions(method, stream);
            }

            // metódus törzs vége
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, TAB_SPACE_BYTES);
            Write(stream, RIGHT_CURLY_BRACE);
            Write(stream, NEW_LINE_BYTES);
        }

    }

}
