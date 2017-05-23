namespace SQLGenerator
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSelectScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelFields = new System.Windows.Forms.Panel();
            this.btnGenerateSQL = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panelDisp = new System.Windows.Forms.Panel();
            this.chkDateLogic = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTempSeed = new System.Windows.Forms.TextBox();
            this.chkTemp = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.panelDisp.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(547, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSelectScriptToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openSelectScriptToolStripMenuItem
            // 
            this.openSelectScriptToolStripMenuItem.Name = "openSelectScriptToolStripMenuItem";
            this.openSelectScriptToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.openSelectScriptToolStripMenuItem.Text = "Open Select Script";
            this.openSelectScriptToolStripMenuItem.Click += new System.EventHandler(this.openSelectScriptToolStripMenuItem_Click);
            // 
            // panelFields
            // 
            this.panelFields.AutoScroll = true;
            this.panelFields.Location = new System.Drawing.Point(36, 153);
            this.panelFields.Name = "panelFields";
            this.panelFields.Size = new System.Drawing.Size(404, 324);
            this.panelFields.TabIndex = 1;
            this.panelFields.Visible = false;
            // 
            // btnGenerateSQL
            // 
            this.btnGenerateSQL.Location = new System.Drawing.Point(456, 27);
            this.btnGenerateSQL.Name = "btnGenerateSQL";
            this.btnGenerateSQL.Size = new System.Drawing.Size(75, 40);
            this.btnGenerateSQL.TabIndex = 2;
            this.btnGenerateSQL.Text = "Generate SQL";
            this.btnGenerateSQL.UseVisualStyleBackColor = true;
            this.btnGenerateSQL.Visible = false;
            this.btnGenerateSQL.Click += new System.EventHandler(this.btnGenerateSQL_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Table:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(47, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(0, 13);
            this.label5.TabIndex = 7;
            // 
            // panelDisp
            // 
            this.panelDisp.Controls.Add(this.label11);
            this.panelDisp.Controls.Add(this.label10);
            this.panelDisp.Controls.Add(this.label9);
            this.panelDisp.Controls.Add(this.label8);
            this.panelDisp.Controls.Add(this.chkDateLogic);
            this.panelDisp.Controls.Add(this.label7);
            this.panelDisp.Controls.Add(this.label6);
            this.panelDisp.Controls.Add(this.txtTempSeed);
            this.panelDisp.Controls.Add(this.chkTemp);
            this.panelDisp.Controls.Add(this.label2);
            this.panelDisp.Controls.Add(this.label1);
            this.panelDisp.Controls.Add(this.label5);
            this.panelDisp.Controls.Add(this.label3);
            this.panelDisp.Controls.Add(this.label4);
            this.panelDisp.Location = new System.Drawing.Point(36, 27);
            this.panelDisp.Name = "panelDisp";
            this.panelDisp.Size = new System.Drawing.Size(404, 120);
            this.panelDisp.TabIndex = 9;
            this.panelDisp.Visible = false;
            // 
            // chkDateLogic
            // 
            this.chkDateLogic.AutoSize = true;
            this.chkDateLogic.Location = new System.Drawing.Point(6, 42);
            this.chkDateLogic.Name = "chkDateLogic";
            this.chkDateLogic.Size = new System.Drawing.Size(353, 17);
            this.chkDateLogic.TabIndex = 13;
            this.chkDateLogic.Text = "Apply Date Logic(Exclude TimeStamps, Load Orders, and Load Keys)";
            this.chkDateLogic.UseVisualStyleBackColor = true;
            this.chkDateLogic.CheckedChanged += new System.EventHandler(this.chkDateLogic_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(-3, 98);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Exc";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(95, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Sample Seed Value";
            this.label6.Visible = false;
            // 
            // txtTempSeed
            // 
            this.txtTempSeed.Location = new System.Drawing.Point(195, 20);
            this.txtTempSeed.Name = "txtTempSeed";
            this.txtTempSeed.Size = new System.Drawing.Size(73, 20);
            this.txtTempSeed.TabIndex = 10;
            this.txtTempSeed.Visible = false;
            // 
            // chkTemp
            // 
            this.chkTemp.AutoSize = true;
            this.chkTemp.Location = new System.Drawing.Point(6, 23);
            this.chkTemp.Name = "chkTemp";
            this.chkTemp.Size = new System.Drawing.Size(83, 17);
            this.chkTemp.TabIndex = 9;
            this.chkTemp.Text = "Use Sample";
            this.chkTemp.UseVisualStyleBackColor = true;
            this.chkTemp.CheckedChanged += new System.EventHandler(this.chkTemp_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(84, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Pk";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Seed";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(133, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Name";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(28, 62);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Start";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(28, 82);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(26, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "End";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(64, 62);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(0, 13);
            this.label10.TabIndex = 16;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(64, 82);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(0, 13);
            this.label11.TabIndex = 17;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 506);
            this.Controls.Add(this.panelDisp);
            this.Controls.Add(this.btnGenerateSQL);
            this.Controls.Add(this.panelFields);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Comparison SQL Generator";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelDisp.ResumeLayout(false);
            this.panelDisp.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSelectScriptToolStripMenuItem;
        private System.Windows.Forms.Panel panelFields;
        private System.Windows.Forms.Button btnGenerateSQL;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panelDisp;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtTempSeed;
        private System.Windows.Forms.CheckBox chkTemp;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkDateLogic;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
    }
}

