namespace SQLUserControl
{
    partial class fieldControl
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkExclude = new System.Windows.Forms.CheckBox();
            this.chkPK = new System.Windows.Forms.CheckBox();
            this.lblName = new System.Windows.Forms.Label();
            this.chkSeed = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chkExclude
            // 
            this.chkExclude.AutoSize = true;
            this.chkExclude.Location = new System.Drawing.Point(3, 0);
            this.chkExclude.Name = "chkExclude";
            this.chkExclude.Size = new System.Drawing.Size(15, 14);
            this.chkExclude.TabIndex = 0;
            this.chkExclude.UseVisualStyleBackColor = true;
            // 
            // chkPK
            // 
            this.chkPK.AutoSize = true;
            this.chkPK.Location = new System.Drawing.Point(67, 0);
            this.chkPK.Name = "chkPK";
            this.chkPK.Size = new System.Drawing.Size(15, 14);
            this.chkPK.TabIndex = 1;
            this.chkPK.UseVisualStyleBackColor = true;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(125, 1);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(0, 13);
            this.lblName.TabIndex = 2;
            // 
            // chkSeed
            // 
            this.chkSeed.AutoSize = true;
            this.chkSeed.Location = new System.Drawing.Point(34, 0);
            this.chkSeed.Name = "chkSeed";
            this.chkSeed.Size = new System.Drawing.Size(15, 14);
            this.chkSeed.TabIndex = 3;
            this.chkSeed.UseVisualStyleBackColor = true;
            // 
            // fieldControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkSeed);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.chkPK);
            this.Controls.Add(this.chkExclude);
            this.Name = "fieldControl";
            this.Size = new System.Drawing.Size(545, 14);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox chkExclude;
        public System.Windows.Forms.CheckBox chkPK;
        public System.Windows.Forms.Label lblName;
        public System.Windows.Forms.CheckBox chkSeed;
    }
}
