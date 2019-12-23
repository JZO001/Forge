using Forge.EventRaiser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Forge.Testing.WinForms
{

    public partial class Form1 : Form
    {

        EVTest test = new EVTest();

        public Form1()
        {
            this.Shown += Form1_Shown;
            InitializeComponent();
            Button btn = new Button();
            btn.Text = "Press";
            btn.Click += Btn_Click;
            btn.Location = new Point(10, 10);
            this.Controls.Add(btn);
            test.TestEvent += Test_TestEvent;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Console.WriteLine(string.Format("IsUI thread: {0}", ApplicationHelper.IsUIThread().ToString()));
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            test.Raise();
        }

        private void Test_TestEvent(object sender, EventArgs e)
        {
            this.Text = "Event arrived!";
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
