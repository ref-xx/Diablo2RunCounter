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
            toolStripMenuItem3 = new ToolStripSeparator();
            setInterval1500ToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripSeparator();
            viewEventLogToolStripMenuItem = new ToolStripMenuItem();
            viewDetailedFoundButtonsToolStripMenuItem = new ToolStripMenuItem();
            button8 = new Button();
            labelTurnDuration = new Label();
            contextMenuStrip2 = new ContextMenuStrip(components);
            editPauseToolStripMenuItem = new ToolStripMenuItem();
            removeToolStripMenuItem = new ToolStripMenuItem();
            markFoundToolStripMenuItem = new ToolStripMenuItem();
            removeFoundToolStripMenuItem = new ToolStripMenuItem();
            listView2 = new ListView();
            panel1 = new Panel();
            splitContainer1 = new SplitContainer();
            groupBoxFound = new GroupBox();
            checkBoxinPrevRun = new CheckBox();
            textBox1 = new TextBox();
            buttonlom = new Button();
            buttonlop = new Button();
            buttonmidm = new Button();
            buttonhim = new Button();
            buttonmidp = new Button();
            buttonhip = new Button();
            contextMenuStrip1.SuspendLayout();
            contextMenuStrip3.SuspendLayout();
            contextMenuStrip2.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            groupBoxFound.SuspendLayout();
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
            listBox1.Size = new Size(211, 109);
            listBox1.TabIndex = 0;
            listBox1.Visible = false;
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
            button5.Text = "Continue Session";
            button5.UseVisualStyleBackColor = true;
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
            listView1.Location = new Point(3, 3);
            listView1.Name = "listView1";
            listView1.Size = new Size(181, 295);
            listView1.TabIndex = 16;
            listView1.UseCompatibleStateImageBehavior = false;
            // 
            // labelRunDuration
            // 
            labelRunDuration.BorderStyle = BorderStyle.FixedSingle;
            labelRunDuration.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelRunDuration.Location = new Point(12, 38);
            labelRunDuration.Name = "labelRunDuration";
            labelRunDuration.Size = new Size(75, 23);
            labelRunDuration.TabIndex = 17;
            labelRunDuration.Text = "00:00";
            labelRunDuration.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelTurnAverage
            // 
            labelTurnAverage.BorderStyle = BorderStyle.FixedSingle;
            labelTurnAverage.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelTurnAverage.Location = new Point(12, 67);
            labelTurnAverage.Name = "labelTurnAverage";
            labelTurnAverage.Size = new Size(74, 23);
            labelTurnAverage.TabIndex = 18;
            labelTurnAverage.Text = "00:00";
            labelTurnAverage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // contextMenuStrip3
            // 
            contextMenuStrip3.Items.AddRange(new ToolStripItem[] { beepOnEndToolStripMenuItem, beepOnStartToolStripMenuItem, toolStripMenuItem3, setInterval1500ToolStripMenuItem, toolStripMenuItem2, viewEventLogToolStripMenuItem, viewDetailedFoundButtonsToolStripMenuItem });
            contextMenuStrip3.Name = "contextMenuStrip3";
            contextMenuStrip3.Size = new Size(224, 126);
            // 
            // beepOnEndToolStripMenuItem
            // 
            beepOnEndToolStripMenuItem.Name = "beepOnEndToolStripMenuItem";
            beepOnEndToolStripMenuItem.Size = new Size(223, 22);
            beepOnEndToolStripMenuItem.Text = "Beep";
            // 
            // beepOnStartToolStripMenuItem
            // 
            beepOnStartToolStripMenuItem.Name = "beepOnStartToolStripMenuItem";
            beepOnStartToolStripMenuItem.Size = new Size(223, 22);
            beepOnStartToolStripMenuItem.Text = "Try detect run start";
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new Size(220, 6);
            // 
            // setInterval1500ToolStripMenuItem
            // 
            setInterval1500ToolStripMenuItem.Name = "setInterval1500ToolStripMenuItem";
            setInterval1500ToolStripMenuItem.Size = new Size(223, 22);
            setInterval1500ToolStripMenuItem.Text = "Set Interval (1500)";
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(220, 6);
            // 
            // viewEventLogToolStripMenuItem
            // 
            viewEventLogToolStripMenuItem.Name = "viewEventLogToolStripMenuItem";
            viewEventLogToolStripMenuItem.Size = new Size(223, 22);
            viewEventLogToolStripMenuItem.Text = "View event log";
            // 
            // viewDetailedFoundButtonsToolStripMenuItem
            // 
            viewDetailedFoundButtonsToolStripMenuItem.Name = "viewDetailedFoundButtonsToolStripMenuItem";
            viewDetailedFoundButtonsToolStripMenuItem.Size = new Size(223, 22);
            viewDetailedFoundButtonsToolStripMenuItem.Text = "View detailed found buttons";
            // 
            // button8
            // 
            button8.Location = new Point(13, 175);
            button8.Name = "button8";
            button8.Size = new Size(65, 23);
            button8.TabIndex = 21;
            button8.Text = "Options";
            button8.UseVisualStyleBackColor = true;
            // 
            // labelTurnDuration
            // 
            labelTurnDuration.BorderStyle = BorderStyle.FixedSingle;
            labelTurnDuration.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelTurnDuration.Location = new Point(13, 96);
            labelTurnDuration.Name = "labelTurnDuration";
            labelTurnDuration.Size = new Size(74, 23);
            labelTurnDuration.TabIndex = 22;
            labelTurnDuration.Text = "00:00";
            labelTurnDuration.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // contextMenuStrip2
            // 
            contextMenuStrip2.Items.AddRange(new ToolStripItem[] { editPauseToolStripMenuItem, removeToolStripMenuItem, markFoundToolStripMenuItem, removeFoundToolStripMenuItem });
            contextMenuStrip2.Name = "contextMenuStrip2";
            contextMenuStrip2.Size = new Size(178, 92);
            // 
            // editPauseToolStripMenuItem
            // 
            editPauseToolStripMenuItem.Name = "editPauseToolStripMenuItem";
            editPauseToolStripMenuItem.Size = new Size(177, 22);
            editPauseToolStripMenuItem.Text = "Edit Pause Duration";
            // 
            // removeToolStripMenuItem
            // 
            removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            removeToolStripMenuItem.Size = new Size(177, 22);
            removeToolStripMenuItem.Text = "Remove Run";
            // 
            // markFoundToolStripMenuItem
            // 
            markFoundToolStripMenuItem.Name = "markFoundToolStripMenuItem";
            markFoundToolStripMenuItem.Size = new Size(177, 22);
            markFoundToolStripMenuItem.Text = "Mark Found";
            // 
            // removeFoundToolStripMenuItem
            // 
            removeFoundToolStripMenuItem.Name = "removeFoundToolStripMenuItem";
            removeFoundToolStripMenuItem.Size = new Size(177, 22);
            removeFoundToolStripMenuItem.Text = "Remove Found";
            // 
            // listView2
            // 
            listView2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listView2.Location = new Point(3, 3);
            listView2.Name = "listView2";
            listView2.Size = new Size(303, 295);
            listView2.TabIndex = 23;
            listView2.UseCompatibleStateImageBehavior = false;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(splitContainer1);
            panel1.Location = new Point(229, 11);
            panel1.Name = "panel1";
            panel1.Size = new Size(516, 303);
            panel1.TabIndex = 24;
            // 
            // splitContainer1
            // 
            splitContainer1.BorderStyle = BorderStyle.FixedSingle;
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(listView2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(listView1);
            splitContainer1.Size = new Size(516, 303);
            splitContainer1.SplitterDistance = 311;
            splitContainer1.SplitterWidth = 6;
            splitContainer1.TabIndex = 0;
            // 
            // groupBoxFound
            // 
            groupBoxFound.Controls.Add(checkBoxinPrevRun);
            groupBoxFound.Controls.Add(textBox1);
            groupBoxFound.Controls.Add(buttonlom);
            groupBoxFound.Controls.Add(buttonlop);
            groupBoxFound.Controls.Add(buttonmidm);
            groupBoxFound.Controls.Add(buttonhim);
            groupBoxFound.Controls.Add(buttonmidp);
            groupBoxFound.Controls.Add(buttonhip);
            groupBoxFound.Location = new Point(3, 204);
            groupBoxFound.Name = "groupBoxFound";
            groupBoxFound.Size = new Size(220, 110);
            groupBoxFound.TabIndex = 25;
            groupBoxFound.TabStop = false;
            groupBoxFound.Text = "Found hi, mid, lo";
            // 
            // checkBoxinPrevRun
            // 
            checkBoxinPrevRun.AutoSize = true;
            checkBoxinPrevRun.Location = new Point(117, 0);
            checkBoxinPrevRun.Name = "checkBoxinPrevRun";
            checkBoxinPrevRun.Size = new Size(86, 19);
            checkBoxinPrevRun.TabIndex = 22;
            checkBoxinPrevRun.Text = "In Prev Run";
            checkBoxinPrevRun.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(16, 23);
            textBox1.MaxLength = 512;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(189, 23);
            textBox1.TabIndex = 21;
            textBox1.Text = "Legendary, Set, Junk";
            // 
            // buttonlom
            // 
            buttonlom.BackColor = Color.CornflowerBlue;
            buttonlom.Location = new Point(146, 81);
            buttonlom.Name = "buttonlom";
            buttonlom.Size = new Size(59, 23);
            buttonlom.TabIndex = 20;
            buttonlom.Text = "-1";
            buttonlom.UseVisualStyleBackColor = false;
            // 
            // buttonlop
            // 
            buttonlop.BackColor = Color.CornflowerBlue;
            buttonlop.Location = new Point(146, 52);
            buttonlop.Name = "buttonlop";
            buttonlop.Size = new Size(59, 23);
            buttonlop.TabIndex = 19;
            buttonlop.Text = "+1";
            buttonlop.UseVisualStyleBackColor = false;
            // 
            // buttonmidm
            // 
            buttonmidm.BackColor = Color.LimeGreen;
            buttonmidm.Location = new Point(81, 81);
            buttonmidm.Name = "buttonmidm";
            buttonmidm.Size = new Size(59, 23);
            buttonmidm.TabIndex = 18;
            buttonmidm.Text = "-1";
            buttonmidm.UseVisualStyleBackColor = false;
            // 
            // buttonhim
            // 
            buttonhim.BackColor = Color.Goldenrod;
            buttonhim.Location = new Point(16, 81);
            buttonhim.Name = "buttonhim";
            buttonhim.Size = new Size(59, 23);
            buttonhim.TabIndex = 17;
            buttonhim.Text = "-1";
            buttonhim.UseVisualStyleBackColor = false;
            // 
            // buttonmidp
            // 
            buttonmidp.BackColor = Color.LimeGreen;
            buttonmidp.Location = new Point(81, 52);
            buttonmidp.Name = "buttonmidp";
            buttonmidp.Size = new Size(59, 23);
            buttonmidp.TabIndex = 16;
            buttonmidp.Text = "+1";
            buttonmidp.UseVisualStyleBackColor = false;
            // 
            // buttonhip
            // 
            buttonhip.BackColor = Color.Goldenrod;
            buttonhip.Location = new Point(16, 52);
            buttonhip.Name = "buttonhip";
            buttonhip.Size = new Size(59, 23);
            buttonhip.TabIndex = 15;
            buttonhip.Text = "+1";
            buttonhip.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(757, 323);
            Controls.Add(groupBoxFound);
            Controls.Add(panel1);
            Controls.Add(labelTurnDuration);
            Controls.Add(button8);
            Controls.Add(labelTurnAverage);
            Controls.Add(labelRunDuration);
            Controls.Add(label3);
            Controls.Add(button6);
            Controls.Add(button7);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(listBox1);
            Name = "Form1";
            Text = "Arda's D2R automatic run counter V1.0";
            contextMenuStrip1.ResumeLayout(false);
            contextMenuStrip3.ResumeLayout(false);
            contextMenuStrip2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            groupBoxFound.ResumeLayout(false);
            groupBoxFound.PerformLayout();
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
        private Label labelTurnDuration;
        private ContextMenuStrip contextMenuStrip2;
        private ToolStripMenuItem editPauseToolStripMenuItem;
        private ToolStripMenuItem removeToolStripMenuItem;
        private ToolStripMenuItem markFoundToolStripMenuItem;
        private ToolStripMenuItem removeFoundToolStripMenuItem;
        private ListView listView2;
        private Panel panel1;
        private SplitContainer splitContainer1;
        private ToolStripSeparator toolStripMenuItem3;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem viewEventLogToolStripMenuItem;
        private GroupBox groupBoxFound;
        private Button buttonlom;
        private Button buttonlop;
        private Button buttonmidm;
        private Button buttonhim;
        private Button buttonmidp;
        private Button buttonhip;
        private TextBox textBox1;
        private CheckBox checkBoxinPrevRun;
        private ToolStripMenuItem viewDetailedFoundButtonsToolStripMenuItem;
    }
}
