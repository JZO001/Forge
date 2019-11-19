/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Forge.Net.Remoting.Proxy;
using Forge.Reflection;

namespace Forge.Net.Remoting.Validators
{

    /// <summary>
    /// Validates a contract
    /// </summary>
    public static class ContractValidator
    {

        /// <summary>
        /// Check integrity of the service contract
        /// </summary>
        /// <param name="contractInterface">The contract interface.</param>
        /// <exception cref="InvalidContractDefinitionException">
        /// </exception>
        public static void ValidateContractIntegrity(Type contractInterface)
        {
            if (contractInterface == null)
            {
                ThrowHelper.ThrowArgumentNullException("contractInterface");
            }
            if (!contractInterface.IsInterface)
            {
                ThrowHelper.ThrowArgumentException("Provided contract type must be an interface.");
            }

            // Szabályok, amiket be kell tartani a deklarációk során:
            // 1, kötelező lennie a contract-on ServiceContract annotációnak
            // 2, ha az interface öröklődik más contract interface-ből, a ServiceContract-ok WellKnownObjectMode beállításainak egyezniük kell
            // 3, WellKnownObjectMode.Singleton és WellKnownObjectMode.SingleCall contract nem tartalmazhat olyan metódust,
            //    amelyiknek az OperationContract annotációjának OperationDirection beállítása ClientSide
            // 4, Metódus visszatérési értéke vagy a paraméterlistája nem lehet a Stream leszármazottja, csakis maga a Stream.
            // 5, oneWay típusú metódusnak nem lehet visszatérési értéke
            // 6, isReliable csak akkor lehet False, ha a metódus oneWay típusú
            // 7, Azonos névvel és paraméterlistával rendelkező metódusoknak meg kell egyeznie az OperationContract-oknak. Emellett mindkét metódus
            //    deklaráción léteznie kell az annotációnak vagy pedig egyiken sem szabad. Lényeg, hogy ugyanaz legyen.

            List<MethodComparator> detectedMethods = new List<MethodComparator>();

            // 1. szabály ellenőrzése
            ServiceContractAttribute referenceServiceContract = TypeHelper.GetAttribute<ServiceContractAttribute>(contractInterface);
            if (referenceServiceContract == null)
            {
                throw new InvalidContractDefinitionException(String.Format("Annotation '{0}' not definied on interface '{1}'.", typeof(ServiceContractAttribute).FullName, contractInterface.Name));
            }

            // 2. szabály ellenőrzése
            if (contractInterface.GetInterfaces().Length > 0)
            {
                foreach (Type superInterface in contractInterface.GetInterfaces())
                {
                    ServiceContractAttribute scAnnotation = TypeHelper.GetAttribute<ServiceContractAttribute>(superInterface);
                    if (scAnnotation != null)
                    {
                        if (!(scAnnotation.WellKnownObjectMode.Equals(referenceServiceContract.WellKnownObjectMode)))
                        {
                            throw new InvalidContractDefinitionException(String.Format("Different {0} definition found on a super interface '{1}' of the contract '{2}'. Contract setting is {3}, super interface setting is {4}.", typeof(ServiceContractAttribute).Name, superInterface.FullName, contractInterface.FullName, referenceServiceContract.WellKnownObjectMode, scAnnotation.WellKnownObjectMode));
                        }

                        // 3. szabály ellenőrzése
                        if (referenceServiceContract.WellKnownObjectMode != WellKnownObjectModeEnum.PerSession)
                        {
                            CheckSingletonOrSingleCallContract(superInterface, referenceServiceContract.WellKnownObjectMode);
                        }
                        // 4. és 5. szabály ellenőrzése
                        CheckInvalidMethodDeclaration(superInterface, detectedMethods);
                    }
                }
            }

            // 3. szabály ellenőrzése
            if (referenceServiceContract.WellKnownObjectMode != WellKnownObjectModeEnum.PerSession)
            {
                CheckSingletonOrSingleCallContract(contractInterface, referenceServiceContract.WellKnownObjectMode);
            }
            // 4. és 5. szabály ellenőrzése
            CheckInvalidMethodDeclaration(contractInterface, detectedMethods);
        }

        /// <summary>
        /// Query and gets back the WellKnownObjectMode type of the provided contract
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="result">The result.</param>
        /// <returns>True, if the provided type found, otherwise False.</returns>
        public static bool GetWellKnownObjectMode(Type type, out WellKnownObjectModeEnum result)
        {
            if (type == null)
            {
                ThrowHelper.ThrowArgumentNullException("type");
            }

            bool found = false;
            result = WellKnownObjectModeEnum.PerSession;

            ServiceContractAttribute scAnnotation = TypeHelper.GetAttribute<ServiceContractAttribute>(type);
            if (scAnnotation != null)
            {
                result = scAnnotation.WellKnownObjectMode;
                found = true;
            }

            if (scAnnotation == null && type.GetInterfaces().Length > 0)
            {
                foreach (Type interfaceClass in type.GetInterfaces())
                {
                    scAnnotation = TypeHelper.GetAttribute<ServiceContractAttribute>(interfaceClass);
                    if (scAnnotation != null)
                    {
                        result = scAnnotation.WellKnownObjectMode;
                        found = true;
                    }
                    else
                    {
                        found = GetWellKnownObjectMode(interfaceClass, out result);
                    }
                    if (found)
                    {
                        break;
                    }
                }
            }
            if (!found && type.BaseType != null && !type.BaseType.Equals(typeof(ProxyBase)) && !type.BaseType.Equals(typeof(Object)))
            {
                found = GetWellKnownObjectMode(type.BaseType, out result);
            }

            return found;
        }

        private static void CheckSingletonOrSingleCallContract(Type contractInterface, WellKnownObjectModeEnum referenceMode)
        {
            foreach (MethodInfo m in contractInterface.GetMethods())
            {
                OperationContractAttribute ocAnnotation = TypeHelper.GetAttribute<OperationContractAttribute>(m);
                if (ocAnnotation != null && ocAnnotation.Direction == OperationDirectionEnum.ClientSide)
                {
                    throw new InvalidContractDefinitionException(String.Format("Invalid {0} on contract '{1}'. Current setting '{2}' is supported only on {3} with {4}.{5}. Current mode is {6}.",
                            typeof(OperationDirectionEnum).Name, contractInterface.Name, ocAnnotation.Direction.ToString(), typeof(ServiceContractAttribute).Name, typeof(WellKnownObjectModeEnum).Name, WellKnownObjectModeEnum.PerSession.ToString(), referenceMode.ToString()));
                }
            }
        }

        private static void CheckInvalidMethodDeclaration(Type contractInterface, List<MethodComparator> methods)
        {
            foreach (MethodInfo m in contractInterface.GetMethods())
            {
                {
                    OperationContractAttribute ocAnnotation = TypeHelper.GetAttribute<OperationContractAttribute>(m);
                    if (ocAnnotation != null)
                    {
                        // 4. szabály ellenőrzése
                        if (!m.ReturnType.Equals(typeof(Stream)) && typeof(Stream).IsAssignableFrom(m.ReturnType))
                        {
                            throw new InvalidContractDefinitionException(String.Format("Return type '{0}' of the method '{1}' of contract '{2}' is not allowed. Use the base class instead.", m.ReturnType.FullName, m.Name, contractInterface.Name));
                        }
                        if (m.GetParameters().Length > 0)
                        {
                            foreach (ParameterInfo pt in m.GetParameters())
                            {
                                if (!pt.ParameterType.Equals(typeof(Stream)) && typeof(Stream).IsAssignableFrom(pt.ParameterType))
                                {
                                    throw new InvalidContractDefinitionException(String.Format("Parameter type '{0}' of the method '{1}' of contract '{2}' is not allowed. Use the base class instead.", pt.ParameterType.FullName, m.Name, contractInterface.Name));
                                }
                            }
                        }

                        // 5. szabály ellenőrzése
                        if (ocAnnotation.IsOneWay && !m.ReturnType.Equals(typeof(void)))
                        {
                            throw new InvalidContractDefinitionException(String.Format("Return type '{0}' of the method '{1}' of contract '{2}' is not allowed for OneWay mode.", m.ReturnType.FullName, m.Name, contractInterface.Name));
                        }
                        // 6. szabály ellenőrzése
                        if (!ocAnnotation.IsOneWay && !ocAnnotation.IsReliable)
                        {
                            throw new InvalidContractDefinitionException(String.Format("Invalid reliable mode setting for method '{0}' of the contract type '{1}'. Non reliable communication allowed only for OneWay mode.", m.Name, contractInterface.Name));
                        }
                    }
                    // 7. szabály ellenőrzése
                    MethodComparator cmp = new MethodComparator(m);
                    if (methods.Contains(cmp))
                    {
                        MethodComparator otherCmp = null;
                        foreach (MethodComparator mc in methods)
                        {
                            if (mc.Equals(cmp))
                            {
                                otherCmp = mc;
                                break;
                            }
                        }
                        OperationContractAttribute otherOcAnnotation = TypeHelper.GetAttribute<OperationContractAttribute>(otherCmp.Method);
                        if (ocAnnotation == null && otherOcAnnotation == null)
                        {
                            // do nothing
                        }
                        else if (ocAnnotation != null && otherOcAnnotation != null)
                        {
                            if (ocAnnotation.Direction != otherOcAnnotation.Direction)
                            {
                                throw new InvalidContractDefinitionException(String.Format("Different {0} definition found on method {1}. Contract interfaces are '{2}' and '{3}'. Setting 'direction' is different.", typeof(OperationContractAttribute).Name, m.Name, contractInterface.Name, otherCmp.Method.DeclaringType.FullName));
                            }
                            if (ocAnnotation.IsOneWay != otherOcAnnotation.IsOneWay)
                            {
                                throw new InvalidContractDefinitionException(String.Format("Different {0} definition found on method {1}. Contract interfaces are '{2}' and '{3}'. Setting 'isOneWay' is different.", typeof(OperationContractAttribute).Name, m.Name, contractInterface.Name, otherCmp.Method.DeclaringType.FullName));
                            }
                            if (ocAnnotation.IsReliable != otherOcAnnotation.IsReliable)
                            {
                                throw new InvalidContractDefinitionException(String.Format("Different {0} definition found on method {1}. Contract interfaces are '{2}' and '{3}'. Setting 'isReliable' is different.", typeof(OperationContractAttribute).Name, m.Name, contractInterface.Name, otherCmp.Method.DeclaringType.FullName));
                            }
                            if (ocAnnotation.CallTimeout != otherOcAnnotation.CallTimeout)
                            {
                                throw new InvalidContractDefinitionException(String.Format("Different {0} definition found on method {1}. Contract interfaces are '{2}' and '{3}'. Setting 'timeout' is different.", typeof(OperationContractAttribute).Name, m.Name, contractInterface.Name, otherCmp.Method.DeclaringType.FullName));
                            }
                        }
                        else
                        {
                            // az egyiken van annotáció, a másikon meg nincs
                            throw new InvalidContractDefinitionException(String.Format("Different {0} definition found on method {1}. Contract interfaces are '{2}' and '{3}'. Same {4} must be exist on methods.", typeof(OperationContractAttribute).Name, m.Name, contractInterface.Name, otherCmp.Method.DeclaringType.FullName, typeof(OperationContractAttribute).Name));
                        }
                    }
                    else
                    {
                        methods.Add(cmp);
                    }
                }
            }
        }

    }

}
