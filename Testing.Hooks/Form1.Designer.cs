namespace Testing.Hooks
{
    partial class Form1
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
            this.btCapture = new System.Windows.Forms.Button();
            this.btMouseClick = new System.Windows.Forms.Button();
            this.btMotionStart = new System.Windows.Forms.Button();
            this.btMotionStop = new System.Windows.Forms.Button();
            this.btTimer = new System.Windows.Forms.Button();
            this.nudTimer = new System.Windows.Forms.NumericUpDown();
            this.btView = new System.Windows.Forms.Button();
            this.btKeyDemo = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btMouseMoveRel = new System.Windows.Forms.Button();
            this.btMouseMoveAbs = new System.Windows.Forms.Button();
            this.btMouseLeftClick = new System.Windows.Forms.Button();
            this.btMouseRightClick = new System.Windows.Forms.Button();
            this.btClipboard = new System.Windows.Forms.Button();
            this.btPrintClipboard = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimer)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btCapture
            // 
            this.btCapture.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btCapture.Location = new System.Drawing.Point(12, 12);
            this.btCapture.Name = "btCapture";
            this.btCapture.Size = new System.Drawing.Size(75, 23);
            this.btCapture.TabIndex = 0;
            this.btCapture.Text = "Capture";
            this.btCapture.UseVisualStyleBackColor = true;
            this.btCapture.Click += new System.EventHandler(this.btCapture_Click);
            // 
            // btMouseClick
            // 
            this.btMouseClick.Location = new System.Drawing.Point(93, 12);
            this.btMouseClick.Name = "btMouseClick";
            this.btMouseClick.Size = new System.Drawing.Size(75, 23);
            this.btMouseClick.TabIndex = 1;
            this.btMouseClick.Text = "Mouse Click";
            this.btMouseClick.UseVisualStyleBackColor = true;
            this.btMouseClick.Click += new System.EventHandler(this.btMouseClick_Click);
            // 
            // btMotionStart
            // 
            this.btMotionStart.Location = new System.Drawing.Point(12, 41);
            this.btMotionStart.Name = "btMotionStart";
            this.btMotionStart.Size = new System.Drawing.Size(75, 23);
            this.btMotionStart.TabIndex = 2;
            this.btMotionStart.Text = "Motion Start";
            this.btMotionStart.UseVisualStyleBackColor = true;
            this.btMotionStart.Click += new System.EventHandler(this.btMotionStart_Click);
            // 
            // btMotionStop
            // 
            this.btMotionStop.Location = new System.Drawing.Point(93, 41);
            this.btMotionStop.Name = "btMotionStop";
            this.btMotionStop.Size = new System.Drawing.Size(75, 23);
            this.btMotionStop.TabIndex = 3;
            this.btMotionStop.Text = "Motion Stop";
            this.btMotionStop.UseVisualStyleBackColor = true;
            this.btMotionStop.Click += new System.EventHandler(this.btMotionStop_Click);
            // 
            // btTimer
            // 
            this.btTimer.Location = new System.Drawing.Point(12, 70);
            this.btTimer.Name = "btTimer";
            this.btTimer.Size = new System.Drawing.Size(75, 23);
            this.btTimer.TabIndex = 4;
            this.btTimer.Text = "Timer";
            this.btTimer.UseVisualStyleBackColor = true;
            this.btTimer.Click += new System.EventHandler(this.btTimer_Click);
            // 
            // nudTimer
            // 
            this.nudTimer.Location = new System.Drawing.Point(93, 73);
            this.nudTimer.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudTimer.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudTimer.Name = "nudTimer";
            this.nudTimer.Size = new System.Drawing.Size(75, 20);
            this.nudTimer.TabIndex = 5;
            this.nudTimer.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudTimer.ThousandsSeparator = true;
            this.nudTimer.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // btView
            // 
            this.btView.Location = new System.Drawing.Point(174, 12);
            this.btView.Name = "btView";
            this.btView.Size = new System.Drawing.Size(75, 23);
            this.btView.TabIndex = 7;
            this.btView.Text = "View";
            this.btView.UseVisualStyleBackColor = true;
            this.btView.Click += new System.EventHandler(this.btView_Click);
            // 
            // btKeyDemo
            // 
            this.btKeyDemo.Location = new System.Drawing.Point(174, 41);
            this.btKeyDemo.Name = "btKeyDemo";
            this.btKeyDemo.Size = new System.Drawing.Size(75, 23);
            this.btKeyDemo.TabIndex = 8;
            this.btKeyDemo.Text = "Key demo";
            this.btKeyDemo.UseVisualStyleBackColor = true;
            this.btKeyDemo.Click += new System.EventHandler(this.btKeyDemo_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Location = new System.Drawing.Point(12, 99);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(260, 151);
            this.panel1.TabIndex = 9;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(167, 142);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 0;
            // 
            // btMouseMoveRel
            // 
            this.btMouseMoveRel.Location = new System.Drawing.Point(255, 12);
            this.btMouseMoveRel.Name = "btMouseMoveRel";
            this.btMouseMoveRel.Size = new System.Drawing.Size(105, 23);
            this.btMouseMoveRel.TabIndex = 10;
            this.btMouseMoveRel.Text = "Mouse Move Rel";
            this.btMouseMoveRel.UseVisualStyleBackColor = true;
            this.btMouseMoveRel.Click += new System.EventHandler(this.btMouseMoveRel_Click);
            // 
            // btMouseMoveAbs
            // 
            this.btMouseMoveAbs.Location = new System.Drawing.Point(255, 41);
            this.btMouseMoveAbs.Name = "btMouseMoveAbs";
            this.btMouseMoveAbs.Size = new System.Drawing.Size(105, 23);
            this.btMouseMoveAbs.TabIndex = 11;
            this.btMouseMoveAbs.Text = "Mouse Move Abs";
            this.btMouseMoveAbs.UseVisualStyleBackColor = true;
            this.btMouseMoveAbs.Click += new System.EventHandler(this.btMouseMoveAbs_Click);
            // 
            // btMouseLeftClick
            // 
            this.btMouseLeftClick.Location = new System.Drawing.Point(366, 12);
            this.btMouseLeftClick.Name = "btMouseLeftClick";
            this.btMouseLeftClick.Size = new System.Drawing.Size(117, 23);
            this.btMouseLeftClick.TabIndex = 12;
            this.btMouseLeftClick.Text = "Mouse Left Click";
            this.btMouseLeftClick.UseVisualStyleBackColor = true;
            this.btMouseLeftClick.Click += new System.EventHandler(this.btMouseLeftClick_Click);
            // 
            // btMouseRightClick
            // 
            this.btMouseRightClick.Location = new System.Drawing.Point(366, 41);
            this.btMouseRightClick.Name = "btMouseRightClick";
            this.btMouseRightClick.Size = new System.Drawing.Size(117, 23);
            this.btMouseRightClick.TabIndex = 13;
            this.btMouseRightClick.Text = "Mouse Right Click";
            this.btMouseRightClick.UseVisualStyleBackColor = true;
            this.btMouseRightClick.Click += new System.EventHandler(this.btMouseRightClick_Click);
            // 
            // btClipboard
            // 
            this.btClipboard.Location = new System.Drawing.Point(174, 70);
            this.btClipboard.Name = "btClipboard";
            this.btClipboard.Size = new System.Drawing.Size(75, 23);
            this.btClipboard.TabIndex = 14;
            this.btClipboard.Text = "Clipboard";
            this.btClipboard.UseVisualStyleBackColor = true;
            this.btClipboard.Click += new System.EventHandler(this.btClipboard_Click);
            // 
            // btPrintClipboard
            // 
            this.btPrintClipboard.Location = new System.Drawing.Point(255, 70);
            this.btPrintClipboard.Name = "btPrintClipboard";
            this.btPrintClipboard.Size = new System.Drawing.Size(75, 23);
            this.btPrintClipboard.TabIndex = 15;
            this.btPrintClipboard.Text = "Print CB";
            this.btPrintClipboard.UseVisualStyleBackColor = true;
            this.btPrintClipboard.Click += new System.EventHandler(this.btPrintClipboard_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(336, 70);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "Mouse Move";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(417, 70);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 17;
            this.button2.Text = "Popup";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 262);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btPrintClipboard);
            this.Controls.Add(this.btClipboard);
            this.Controls.Add(this.btMouseRightClick);
            this.Controls.Add(this.btMouseLeftClick);
            this.Controls.Add(this.btMouseMoveAbs);
            this.Controls.Add(this.btMouseMoveRel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btKeyDemo);
            this.Controls.Add(this.btView);
            this.Controls.Add(this.nudTimer);
            this.Controls.Add(this.btTimer);
            this.Controls.Add(this.btMotionStop);
            this.Controls.Add(this.btMotionStart);
            this.Controls.Add(this.btMouseClick);
            this.Controls.Add(this.btCapture);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.nudTimer)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btCapture;
        private System.Windows.Forms.Button btMouseClick;
        private System.Windows.Forms.Button btMotionStart;
        private System.Windows.Forms.Button btMotionStop;
        private System.Windows.Forms.Button btTimer;
        private System.Windows.Forms.NumericUpDown nudTimer;
        private System.Windows.Forms.Button btView;
        private System.Windows.Forms.Button btKeyDemo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btMouseMoveRel;
        private System.Windows.Forms.Button btMouseMoveAbs;
        private System.Windows.Forms.Button btMouseLeftClick;
        private System.Windows.Forms.Button btMouseRightClick;
        private System.Windows.Forms.Button btClipboard;
        private System.Windows.Forms.Button btPrintClipboard;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

