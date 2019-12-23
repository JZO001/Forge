using Forge.EventRaiser;
using Forge.Logging.Log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Forge.Testing.WPF
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        EVTest test = new EVTest();

        public MainWindow()
        {
            Log4NetManager.InitializeFromAppConfig();
            InitializeComponent();
            test.TestEvent += Test_TestEvent;
        }

        private void Test_TestEvent(object sender, EventArgs e)
        {
            label.Content = "Event arrived!";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            test.Raise();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(string.Format("IsUI thread: {0}", ApplicationHelper.IsUIThread().ToString()));
        }
    }

    public class EVTest
    {

        public event EventHandler<EventArgs> TestEvent;

        public EVTest()
        {
        }

        public void Raise()
        {
            Raiser.CallDelegatorByAsync(TestEvent, new object[] { this, EventArgs.Empty }, true);
        }

    }

}
