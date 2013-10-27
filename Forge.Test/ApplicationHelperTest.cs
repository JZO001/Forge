using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Test
{
    [TestClass]
    public class ApplicationHelperTest
    {

        [TestMethod]
        public void TestSimple()
        {
            string machineName = Environment.MachineName;
            string userName = Environment.UserName;

            string appId = string.Format("{0}_{1}_ForgeTest", machineName, userName);

            Assert.IsTrue(appId.Equals(ApplicationHelper.ApplicationId));
        }

    }
}
