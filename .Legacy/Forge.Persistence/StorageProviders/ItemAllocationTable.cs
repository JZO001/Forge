/* *********************************************************************
 * Date: 18 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;

namespace Forge.Persistence.StorageProviders
{

    [Serializable]
    internal class ItemAllocationTable
    {

        private readonly List<string> mFileItemNames = new List<string>();

        internal ItemAllocationTable()
        {
        }

        internal List<String> FileItemNames { get { return mFileItemNames; } }

        internal int FileUid { get; set; }

    }

}
