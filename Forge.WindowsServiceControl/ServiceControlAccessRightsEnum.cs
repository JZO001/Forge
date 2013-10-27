/* *********************************************************************
 * Date: 26 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.WindowsServiceControl
{

    /// <summary>
    /// Service Control Access Rights
    /// </summary>
    [Flags]
    internal enum ServiceControlAccessRightsEnum : int
    {

        /// <summary>
        /// Required to connect to the service control manager
        /// </summary>
        SC_MANAGER_CONNECT = 0x0001,

        /// <summary>
        /// Required to call the CreateService function to create a service object and add it to the database
        /// </summary>
        SC_MANAGER_CREATE_SERVICE = 0x0002,

        /// <summary>
        /// Required to call the EnumServicesStatusEx function to list the services that are in the database
        /// </summary>
        SC_MANAGER_ENUMERATE_SERVICE = 0x0004,

        /// <summary>
        /// Required to call the LockServiceDatabase function to acquire a lock on the database
        /// </summary>
        SC_MANAGER_LOCK = 0x0008,

        /// <summary>
        /// Required to call the QueryServiceLockStatus function to retrieve the lock status information for the database
        /// </summary>
        SC_MANAGER_QUERY_LOCK_STATUS = 0x0010,

        /// <summary>
        /// Required to call the NotifyBootConfigStatus function
        /// </summary>
        SC_MANAGER_MODIFY_BOOT_CONFIG = 0x0020,

        /// <summary>
        /// Includes STANDARD_RIGHTS_REQUIRED, in addition to all access rights in this table
        /// </summary>
        SC_MANAGER_ALL_ACCESS = 0xF003F

    }

}
