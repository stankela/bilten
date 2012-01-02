namespace Bilten.UI
{
    partial class TakmicariKategorijeUserControl
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
            this.dataGridViewUserControl1 = new Bilten.UI.DataGridViewUserControl();
            this.SuspendLayout();
            // 
            // dataGridViewUserControl1
            // 
            this.dataGridViewUserControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewUserControl1.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewUserControl1.Name = "dataGridViewUserControl1";
            this.dataGridViewUserControl1.Size = new System.Drawing.Size(338, 222);
            this.dataGridViewUserControl1.TabIndex = 0;
            // 
            // TakmicariKategorijeUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewUserControl1);
            this.Name = "TakmicariKategorijeUserControl";
            this.Size = new System.Drawing.Size(338, 222);
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridViewUserControl dataGridViewUserControl1;

    }
}
