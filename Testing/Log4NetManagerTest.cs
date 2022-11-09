using Forge.Logging;
using Forge.Logging.Log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.Testing
{

    [TestClass]
    public class Log4NetManagerTest
    {

        private static readonly ILog LOGGER;

        static Log4NetManagerTest()
        {
            Forge.Logging.Log4net.Log4NetManager.InitializeFromAppConfig();
            LogManager.LOGGER = Log4NetManager.Instance;
            LOGGER = LogManager.GetLogger(typeof(Log4NetManagerTest));
        }

        [TestMethod]
        public void TestMethod1()
        {
            LOGGER.Info("Test");
        }

    }

}
