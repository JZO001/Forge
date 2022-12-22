using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Forge.Net.TerraGraf;

namespace Testing.TerraGraf
{
    public partial class SocketListForm : Form
    {

        private Wrapper mWrapper = null;

        public SocketListForm()
        {
            InitializeComponent();
        }

        public SocketListForm(Wrapper wrapper)
            : this()
        {
            this.mWrapper = wrapper;
        }

        private void SocketListForm_Shown(object sender, EventArgs e)
        {
            FillList();
        }

        private void btRefresh_Click(object sender, EventArgs e)
        {
            FillList();
        }

        private void FillList()
        {
            while (lvSockets.Items.Count > 0)
            {
                lvSockets.Items.RemoveAt(0);
            }
            ICollection<ISocketSafeHandle> sockets = mWrapper.NetworkManager.ActiveSockets;
            foreach (ISocketSafeHandle handle in sockets)
            {
                ListViewItem item = new ListViewItem(handle.AddressFamily.ToString());
                item.Tag = handle;

                ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem(item, handle.LocalEndPoint == null ? "<none>" : handle.LocalEndPoint.ToString());
                item.SubItems.Add(subItem);

                subItem = new ListViewItem.ListViewSubItem(item, handle.RemoteEndPoint == null ? "<none>" : handle.RemoteEndPoint.ToString());
                item.SubItems.Add(subItem);

                lvSockets.Items.Add(item);
            }
            btClose.Enabled = false;
        }

        private void lvSockets_SelectedIndexChanged(object sender, EventArgs e)
        {
            btClose.Enabled = lvSockets.SelectedIndices.Count > 0;
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            ISocketSafeHandle handle = (ISocketSafeHandle)lvSockets.SelectedItems[0].Tag;
            handle.Close();
            FillList();
        }

    }
}
