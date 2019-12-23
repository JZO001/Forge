using System;
using System.Reflection;

namespace Forge.Reflection
{

    internal static class UIReflectionHelper
    {

        internal static bool IsObjectWinFormsControl(object obj)
        {
            bool result = false;
            Type type = obj.GetType();
            while (!result && type != null)
            {
                result = Consts.WINDOWS_FORMS_CONTROL.Equals(type.FullName);
                type = type.BaseType;
            }
            return result;
        }

        internal static object InvokeOnWinFormsControl(object control, Delegate d, object[] parameters)
        {
            MethodInfo miInvoke = control.GetType().GetMethod("Invoke", new Type[] { typeof(Delegate), typeof(object[]) });
            return miInvoke.Invoke(control, new object[] { d, parameters });
        }

        internal static bool IsObjectWPFDependency(object obj)
        {
            bool result = false;
            Type type = obj.GetType();
            while (!result && type != null)
            {
                result = Consts.WINDOWS_DEPENDENCY_OBJECT.Equals(type.FullName);
                type = type.BaseType;
            }
            return result;
        }

        internal static object InvokeOnWPFDependency(object control, Delegate d, object[] parameters)
        {
            MethodInfo miDispatcher = control.GetType().GetProperty("Dispatcher").GetGetMethod();
            object dispatcherObj = miDispatcher.Invoke(control, null);
            MethodInfo miInvoke = dispatcherObj.GetType().GetMethod("Invoke", new Type[] { typeof(Delegate), typeof(object[]) });
            return miInvoke.Invoke(dispatcherObj, new object[] { d, parameters });
        }

    }

}
