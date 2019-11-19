/* *********************************************************************
 * Date: 12 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Reflection;
using Forge.Net.Remoting.Channels;
using Forge.Net.Remoting.Proxy;
using Forge.Reflection;

namespace Forge.Net.Remoting.Validators
{

    /// <summary>
    /// Validate proxy implementation
    /// </summary>
    public static class ImplementationValidator
    {

        /// <summary>
        /// Check integrity of the provided proxy implementation
        /// </summary>
        /// <param name="implClass">The impl class.</param>
        /// <exception cref="InvalidProxyImplementationException">
        /// </exception>
        public static void ValidateProxyIntegration(Type implClass)
        {
            if (implClass == null)
            {
                ThrowHelper.ThrowArgumentNullException("implClass");
            }

            // Szabályok
            // 1, az implementációnak ki kell terjesztenie legalább egy vagy több contract-ot
            // 2, több kiterjesztett contract esetén, a contract-oknak azonos WellKnownObjectMode-ra kell lennie állítva
            // 3, az osztálynak példányosíthatónak kell lennie:
            //    a, nem lehet abstract, interface vagy enum
            //    b, A ProxyBase-ből kell származnia a megfelelő public constructor-al (Channel, String)

            // 3a. szabály ellenőrzése
            if (implClass.IsEnum || implClass.IsAbstract || implClass.IsInterface)
            {
                throw new InvalidProxyImplementationException(String.Format("Invalid proxy type: '{0}'. Proxy must be an instantiable class.", implClass.FullName));
            }

            // 2. szabály ellenőrzése
            bool foundFlag = false;
            WellKnownObjectModeEnum referenceMode = WellKnownObjectModeEnum.PerSession;
            CheckObjectMode(implClass, ref referenceMode, ref foundFlag);

            // 1. szabály ellenőrzése
            if (!foundFlag)
            {
                // ha ugyanis nem talált egy WellKnownObjectMode értéket sem az öröklési láncban, akkor ott nincs vagy hibás a contract definíció
                throw new InvalidProxyImplementationException(String.Format("Valid contract definition has not found on proxy type: {0}", implClass.FullName));
            }

            // 3b. szabály ellenőrzése
            if (!typeof(ProxyBase).IsAssignableFrom(implClass))
            {
                throw new InvalidProxyImplementationException(String.Format("Proxy '{0}' must be inherited from '{2}'.", implClass.FullName, typeof(ProxyBase).FullName));
            }

            // 3b. szabály ellenőrzése
            try
            {
                ConstructorInfo c = implClass.GetConstructor(new Type[] { typeof(Channel), typeof(String) });
                if (!c.IsPublic)
                {
                    throw new InvalidProxyImplementationException(String.Format("Required constructor with type parameters ({0}, {1}) must be public on proxy type '{2}'.", typeof(Channel).FullName, typeof(String).FullName, implClass.FullName));
                }
            }
            catch (Exception ex)
            {
                throw new InvalidProxyImplementationException(String.Format("Required constructor with type parameters ({0}, {1}) has not found on proxy type '{2}'.", typeof(Channel).FullName, typeof(String).FullName, implClass.FullName), ex);
            }
        }

        /// <summary>
        /// Check the integrity of the provided implementation
        /// </summary>
        /// <param name="implClass">The impl class.</param>
        /// <exception cref="System.ArgumentNullException">implClass</exception>
        /// <exception cref="InvalidProxyImplementationException">
        /// </exception>
        public static void ValidateImplementationIntegrity(Type implClass)
        {
            if (implClass == null)
            {
                throw new ArgumentNullException("implClass");
            }

            // Szabályok
            // 1, az implementációnak ki kell terjesztenie legalább egy vagy több contract-ot
            // 2, több kiterjesztett contract esetén, a contract-oknak azonos WellKnownObjectMode-ra kell lennie állítva
            // 3, az osztálynak példányosíthatónak kell lennie:
            //    a, nem lehet abstract, interface vagy enum
            //    b, PerSession módban a ProxyBase-ből kell származnia a megfelelő public constructor-al (Channel, String)
            //    c, A másik két módban kell lennie publikus üres constructor-ának

            // 3a. szabály ellenőrzése
            if (implClass.IsEnum || implClass.IsAbstract || implClass.IsInterface)
            {
                throw new InvalidProxyImplementationException(String.Format("Invalid implementation type: '{0}'. Implementation must be an instantiable class.", implClass.FullName));
            }

            // 2. szabály ellenőrzése
            WellKnownObjectModeEnum referenceMode = WellKnownObjectModeEnum.PerSession;
            bool foundFlag = false;
            CheckObjectMode(implClass, ref referenceMode, ref foundFlag);

            // 1. szabály ellenőrzése
            if (!foundFlag)
            {
                // ha ugyanis nem talált egy WellKnownObjectMode értéket sem az öröklési láncban, akkor ott nincs vagy hibás a contract definíció
                throw new InvalidProxyImplementationException(String.Format("Valid contract definition has not found on implementation type: {0}", implClass.FullName));
            }

            if (referenceMode == WellKnownObjectModeEnum.PerSession)
            {
                // 3b. szabály ellenőrzése
                if (!typeof(ProxyBase).IsAssignableFrom(implClass))
                {
                    throw new InvalidProxyImplementationException(String.Format("Implementation '{0}' must be inherited from '{2}' in {3} mode.", implClass.FullName, typeof(ProxyBase).FullName, WellKnownObjectModeEnum.PerSession.ToString()));
                }

                try
                {
                    ConstructorInfo c = implClass.GetConstructor(new Type[] { typeof(Channel), typeof(String) });
                    if (!c.IsPublic)
                    {
                        throw new InvalidProxyImplementationException(String.Format("Required constructor with type parameters ({0}, {1}) must be public on implementation type '{2}'.", typeof(Channel).FullName, typeof(String).FullName, implClass.FullName));
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidProxyImplementationException(String.Format("Required constructor with type parameters ({0}, {1}) has not found on implementation type '{2}'.", typeof(Channel).FullName, typeof(String).FullName, implClass.FullName), ex);
                }
            }
            else
            {
                // 3c. szabály ellenőrzése
                try
                {
                    ConstructorInfo c = implClass.GetConstructor(new Type[] { });
                    if (!c.IsPublic)
                    {
                        throw new InvalidProxyImplementationException(String.Format("Required constructor with empty type parameters must be public on implementation type '{0}'.", implClass.FullName));
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidProxyImplementationException(String.Format("Required constructor with empty type parameters has not found on implementation type '{0}'.", implClass.FullName), ex);
                }
            }
        }

        private static void CheckObjectMode(Type type, ref WellKnownObjectModeEnum targetMode, ref bool foundFlag)
        {
            if (type.GetInterfaces().Length > 0)
            {
                foreach (Type interfaceClass in type.GetInterfaces())
                {
                    ServiceContractAttribute scAnnotation = TypeHelper.GetAttribute<ServiceContractAttribute>(interfaceClass);
                    if (scAnnotation != null)
                    {
                        // van attribútum
                        if (!foundFlag)
                        {
                            // meg van az első WellKnownObjectMode, ez lesz a referencia a többi számára
                            targetMode = scAnnotation.WellKnownObjectMode;
                            foundFlag = true;
                        }
                        else if (!targetMode.Equals(scAnnotation.WellKnownObjectMode))
                        {
                            // eltérő
                            throw new InvalidProxyImplementationException(String.Format("Different {0} definitions found on type '{1}'. Contract '{2}' configured to '{3}' and previously found '{4}' mode.", typeof(WellKnownObjectModeEnum).FullName, type.FullName, interfaceClass.FullName, scAnnotation.WellKnownObjectMode, targetMode.ToString()));
                        }
                    }
                    if (!foundFlag)
                    {
                        // még nincs referencia WellKnownObjectMode
                        CheckObjectMode(interfaceClass, ref targetMode, ref foundFlag);
                    }
                    else
                    {
                        // már van
                        WellKnownObjectModeEnum interfaceMode = WellKnownObjectModeEnum.PerSession;
                        bool tempFound = false;
                        CheckObjectMode(interfaceClass, ref interfaceMode, ref tempFound);
                        if (tempFound && foundFlag && !targetMode.Equals(interfaceMode))
                        {
                            // eltérő
                            throw new InvalidProxyImplementationException(String.Format("Different {0} definitions found on type '{1}'. Contract '{2}' configured to '{3}' and previously found '{4}' mode.", typeof(WellKnownObjectModeEnum).FullName, type.FullName, interfaceClass.FullName, interfaceMode.ToString(), targetMode.ToString()));
                        }
                        if (tempFound && !foundFlag)
                        {
                            // meg van az első WellKnownObjectMode, referencia mentése
                            targetMode = interfaceMode;
                            foundFlag = true;
                        }
                    }
                }
            }
            if (type.BaseType != null && !type.BaseType.Equals(typeof(ProxyBase)) && !type.BaseType.Equals(typeof(Object)))
            {
                CheckObjectMode(type.BaseType, ref targetMode, ref foundFlag);
            }
        }

    }

}
