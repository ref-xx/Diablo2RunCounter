namespace ArdasD2RunCounter
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            listBox1 = new ListBox();
            button1 = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            label1 = new Label();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            listBox2 = new ListBox();
            contextMenuStrip1 = new ContextMenuStrip(components);
            copyToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripSeparator();
            renameToolStripMenuItem = new ToolStripMenuItem();
            duplicateToolStripMenuItem = new ToolStripMenuItem();
            modifyCountToolStripMenuItem = new ToolStripMenuItem();
            deleteToolStripMenuItem = new ToolStripMenuItem();
            button6 = new Button();
            button7 = new Button();
            label3 = new Label();
            listView1 = new ListView();
            labelRunDuration = new Label();
            labelTurnAverage = new Label();
            contextMenuStrip3 = new ContextMenuStrip(components);
            beepOnEndToolStripMenuItem = new ToolStripMenuItem();
            beepOnStartToolStripMenuItem = new ToolStripMenuItem();
            setInterval1500ToolStripMenuItem = new ToolStripMenuItem();
            button8 = new Button();
            contextMenuStrip1.SuspendLayout();
            contextMenuStrip3.SuspendLayout();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(12, 204);
            listBox1.Name = "listBox1";
            listBox1.ScrollAlwaysVisible = true;
            listBox1.Size = new Size(211, 64);
            listBox1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(93, 175);
            button1.Name = "button1";
            button1.Size = new Size(130, 23);
            button1.TabIndex = 1;
            button1.Text = "Pause Run";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // timer1
            // 
            timer1.Interval = 1500;
            timer1.Tick += timer1_Tick;
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
            label1.Location = new Point(93, 38);
            label1.Name = "label1";
            label1.Size = new Size(130, 73);
            label1.TabIndex = 3;
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // button2
            // 
            button2.Location = new Point(12, 12);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 4;
            button2.Text = "New";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(93, 117);
            button3.Name = "button3";
            button3.Size = new Size(65, 23);
            button3.TabIndex = 5;
            button3.Text = "+1";
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Location = new Point(164, 117);
            button4.Name = "button4";
            button4.Size = new Size(59, 23);
            button4.TabIndex = 6;
            button4.Text = "-1";
            button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.Location = new Point(93, 12);
            button5.Name = "button5";
            button5.Size = new Size(130, 23);
            button5.TabIndex = 7;
            button5.Text = "Continue Run";
            button5.UseVisualStyleBackColor = true;
            // 
            // listBox2
            // 
            listBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listBox2.FormattingEnabled = true;
            listBox2.HorizontalScrollbar = true;
            listBox2.ItemHeight = 15;
            listBox2.Location = new Point(229, 12);
            listBox2.Name = "listBox2";
            listBox2.ScrollAlwaysVisible = true;
            listBox2.Size = new Size(253, 259);
            listBox2.TabIndex = 8;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { copyToolStripMenuItem, toolStripMenuItem1, renameToolStripMenuItem, duplicateToolStripMenuItem, modifyCountToolStripMenuItem, deleteToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(149, 120);
            contextMenuStrip1.Opening += contextMenuStrip1_Opening;
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.Size = new Size(148, 22);
            copyToolStripMenuItem.Text = "Copy";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(145, 6);
            // 
            // renameToolStripMenuItem
            // 
            renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            renameToolStripMenuItem.Size = new Size(148, 22);
            renameToolStripMenuItem.Text = "Rename";
            // 
            // duplicateToolStripMenuItem
            // 
            duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
            duplicateToolStripMenuItem.Size = new Size(148, 22);
            duplicateToolStripMenuItem.Text = "Duplicate";
            // 
            // modifyCountToolStripMenuItem
            // 
            modifyCountToolStripMenuItem.Name = "modifyCountToolStripMenuItem";
            modifyCountToolStripMenuItem.Size = new Size(148, 22);
            modifyCountToolStripMenuItem.Text = "Modify Count";
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new Size(148, 22);
            deleteToolStripMenuItem.Text = "Delete";
            // 
            // button6
            // 
            button6.Location = new Point(164, 146);
            button6.Name = "button6";
            button6.Size = new Size(59, 23);
            button6.TabIndex = 14;
            button6.Text = "this run";
            button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            button7.Location = new Point(93, 146);
            button7.Name = "button7";
            button7.Size = new Size(65, 23);
            button7.TabIndex = 13;
            button7.Text = "prev run";
            button7.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(13, 150);
            label3.Name = "label3";
            label3.Size = new Size(74, 15);
            label3.TabIndex = 15;
            label3.Text = "Mark Found:";
            // 
            // listView1
            // 
            listView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listView1.Location = new Point(488, 12);
            listView1.Name = "listView1";
            listView1.Size = new Size(249, 259);
            listView1.TabIndex = 16;
            listView1.UseCompatibleStateImageBehavior = false;
            // 
            // labelRunDuration
            // 
            labelRunDuration.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelRunDuration.Location = new Point(12, 38);
            labelRunDuration.Name = "labelRunDuration";
            labelRunDuration.Size = new Size(75, 31);
            labelRunDuration.TabIndex = 17;
            labelRunDuration.Text = "00:00";
            labelRunDuration.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelTurnAverage
            // 
            labelTurnAverage.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelTurnAverage.Location = new Point(12, 80);
            labelTurnAverage.Name = "labelTurnAverage";
            labelTurnAverage.Size = new Size(74, 31);
            labelTurnAverage.TabIndex = 18;
            labelTurnAverage.Text = "00:00";
            labelTurnAverage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // contextMenuStrip3
            // 
            contextMenuStrip3.Items.AddRange(new ToolStripItem[] { beepOnEndToolStripMenuItem, beepOnStartToolStripMenuItem, setInterval1500ToolStripMenuItem });
            contextMenuStrip3.Name = "contextMenuStrip3";
            contextMenuStrip3.Size = new Size(192, 70);
            // 
            // beepOnEndToolStripMenuItem
            // 
            beepOnEndToolStripMenuItem.Name = "beepOnEndToolStripMenuItem";
            beepOnEndToolStripMenuItem.Size = new Size(191, 22);
            beepOnEndToolStripMenuItem.Text = "Beep";
            // 
            // beepOnStartToolStripMenuItem
            // 
            beepOnStartToolStripMenuItem.Name = "beepOnStartToolStripMenuItem";
            beepOnStartToolStripMenuItem.Size = new Size(191, 22);
            beepOnStartToolStripMenuItem.Text = "Try detect start of turn";
            // 
            // setInterval1500ToolStripMenuItem
            // 
            setInterval1500ToolStripMenuItem.Name = "setInterval1500ToolStripMenuItem";
            setInterval1500ToolStripMenuItem.Size = new Size(191, 22);
            setInterval1500ToolStripMenuItem.Text = "Set Interval (1500)";
            // 
            // button8
            // 
            button8.Location = new Point(13, 117);
            button8.Name = "button8";
            button8.Size = new Size(65, 23);
            button8.TabIndex = 21;
            button8.Text = "Options";
            button8.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(749, 283);
            Controls.Add(button8);
            Controls.Add(labelTurnAverage);
            Controls.Add(labelRunDuration);
            Controls.Add(listView1);
            Controls.Add(label3);
            Controls.Add(button6);
            Controls.Add(button7);
            Controls.Add(listBox2);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(listBox1);
            Name = "Form1";
            Text = "Arda's D2R automatic run counter";
            contextMenuStrip1.ResumeLayout(false);
            contextMenuStrip3.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listBox1;
        private Button button1;
        private System.Windows.Forms.Timer timer1;
        private Label label1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private ListBox listBox2;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem renameToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem duplicateToolStripMenuItem;
        private ToolStripMenuItem modifyCountToolStripMenuItem;
        private Button button6;
        private Button button7;
        private Label label3;
        private ListView listView1;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private Label labelRunDuration;
        private Label labelTurnAverage;
        private ContextMenuStrip contextMenuStrip3;
        private ToolStripMenuItem beepOnEndToolStripMenuItem;
        private ToolStripMenuItem beepOnStartToolStripMenuItem;
        private Button button8;
        private ToolStripMenuItem setInterval1500ToolStripMenuItem;
    }
}
