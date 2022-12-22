using System.Windows.Forms;
using Forge.Native.Hooks;

namespace Testing.Hooks
{
    partial class KeyDemoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                KeyboardEventHookManager.Instance.KeyDown -= new KeyEventHandler(Event_KeyDown);
                KeyboardEventHookManager.Instance.KeyPress -= new KeyPressEventHandler(Event_KeyPress);
                KeyboardEventHookManager.Instance.KeyUp -= new KeyEventHandler(Event_KeyUp);
            }
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // KeyDemoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 134);
            this.Name = "KeyDemoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "KeyDemoForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Event_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Event_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Event_KeyUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
    }
}