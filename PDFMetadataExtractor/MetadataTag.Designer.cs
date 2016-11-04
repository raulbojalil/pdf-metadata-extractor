namespace PDFMetadataExtractor
{
    partial class MetadataTag
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
            this.lblMetadataName = new System.Windows.Forms.Label();
            this.txtMetadataName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblMetadataName
            // 
            this.lblMetadataName.AutoSize = true;
            this.lblMetadataName.ForeColor = System.Drawing.Color.White;
            this.lblMetadataName.Location = new System.Drawing.Point(3, 6);
            this.lblMetadataName.Name = "lblMetadataName";
            this.lblMetadataName.Size = new System.Drawing.Size(54, 13);
            this.lblMetadataName.TabIndex = 0;
            this.lblMetadataName.Text = "NOMBRE";
            this.lblMetadataName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblMetadataName.Click += new System.EventHandler(this.lblMetadataName_Click);
            // 
            // txtMetadataName
            // 
            this.txtMetadataName.Location = new System.Drawing.Point(2, 1);
            this.txtMetadataName.Name = "txtMetadataName";
            this.txtMetadataName.Size = new System.Drawing.Size(100, 20);
            this.txtMetadataName.TabIndex = 1;
            this.txtMetadataName.Visible = false;
            this.txtMetadataName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtMetadataName_KeyUp);
            this.txtMetadataName.Leave += new System.EventHandler(this.txtMetadataName_Leave);
            // 
            // MetadataTag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.Controls.Add(this.txtMetadataName);
            this.Controls.Add(this.lblMetadataName);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Name = "MetadataTag";
            this.Size = new System.Drawing.Size(104, 24);
            this.Click += new System.EventHandler(this.MetadataTag_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMetadataName;
        private System.Windows.Forms.TextBox txtMetadataName;
    }
}
