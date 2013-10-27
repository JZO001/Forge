/***********************************************************************
 * Date: 30 Jul 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Native.Hooks
{

    /// <summary>
    /// Represents the hook event device types
    /// </summary>
    [Serializable]
    public enum HookEventEnum
    {

        /// <summary>
        /// Windows NT/2000/XP: Installs a hook procedure that monitors low-level mouse input events.
        /// </summary>
        WH_MOUSE_LL = 14,

        /// <summary>
        /// Windows NT/2000/XP: Installs a hook procedure that monitors low-level keyboard  input events.
        /// </summary>
        WH_KEYBOARD_LL = 13,

        /// <summary>
        /// Installs a hook procedure that monitors mouse messages. For more information, see the MouseProc hook procedure. 
        /// </summary>
        WH_MOUSE = 7,

        /// <summary>
        /// Installs a hook procedure that monitors keystroke messages. For more information, see the KeyboardProc hook procedure. 
        /// </summary>
        WH_KEYBOARD = 2

    }

}
