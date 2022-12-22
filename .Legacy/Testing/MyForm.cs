using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using log4net;
using HVS.Forge.Net.TerraGraf.Contexts;
using HVS.Forge.Net.TerraGraf.NetworkPeers;

namespace HVS.Forge.Testing
{
    public partial class MyForm : Form
    {
        private static readonly ILog LOGGER =  LogManager.GetLogger(typeof(MyForm));

        private Dictionary<string, PublicTestPoxy> mPublicTestPoxy = new Dictionary<string, PublicTestPoxy>();
        private Dictionary<string, Dictionary<string, TreeNode>> mNodes = new Dictionary<string, Dictionary<string, TreeNode>>();

        public MyForm()
        {
            InitializeComponent();
        }

        private void CreateMyProxy(object sender, EventArgs e)
        {
          
            

                AppDomainSetup domainInfo = new AppDomainSetup();
                domainInfo.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

                domainInfo.ConfigurationFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyApp.config");
                domainInfo.ApplicationName = AppDomain.CurrentDomain.SetupInformation.ApplicationName;

                AppDomain domain = AppDomain.CreateDomain("", AppDomain.CurrentDomain.Evidence, domainInfo);

                PublicTestPoxy publicTestPoxy = (PublicTestPoxy)domain.CreateInstanceAndUnwrap(typeof(PublicTestPoxy).Assembly.FullName, typeof(PublicTestPoxy).FullName);

                
           
            
        }
    }
}
