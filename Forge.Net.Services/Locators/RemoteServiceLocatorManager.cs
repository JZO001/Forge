/* *********************************************************************
 * Date: 11 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Forge.Net.Remoting;

namespace Forge.Net.Services.Locators
{

    /// <summary>
    /// Managing the remote service locators
    /// </summary>
    public static class RemoteServiceLocatorManager
    {

        private static readonly Dictionary<Type, object> mServiceLocators = new Dictionary<Type, object>();

        /// <summary>
        /// Gets the service locator.
        /// </summary>
        /// <typeparam name="TIProxyType">The type of the I proxy type.</typeparam>
        /// <typeparam name="TLocatorType">The type of the locator type.</typeparam>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IRemoteServiceLocator<TIProxyType> GetServiceLocator<TIProxyType, TLocatorType>()
            where TIProxyType : IRemoteContract
            where TLocatorType : IRemoteServiceLocator<TIProxyType>, new()
        {
            IRemoteServiceLocator<TIProxyType> result = null;

            if (mServiceLocators.ContainsKey(typeof(TLocatorType)))
            {
                result = (IRemoteServiceLocator<TIProxyType>)mServiceLocators[typeof(TLocatorType)];
            }
            else
            {
                result = (IRemoteServiceLocator<TIProxyType>)typeof(TLocatorType).GetConstructor(Type.EmptyTypes).Invoke(null);
                mServiceLocators[typeof(TLocatorType)] = result;
            }

            return result;
        }

    }

}
