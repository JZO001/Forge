using System;
using System.Windows.Forms;
using Forge.Native.Hooks;

namespace Testing.Hooks
{
    public partial class PopupForm : Form
    {
        public PopupForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MouseEventHookManager.Instance.Start();
        }
    }
}
