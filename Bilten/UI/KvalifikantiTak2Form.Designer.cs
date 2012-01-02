namespace Bilten.UI
{
    partial class KvalifikantiTak2Form
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
            this.cmbTakmicenje = new System.Windows.Forms.ComboBox();
            this.lblRezerve = new System.Windows.Forms.Label();
            this.dataGridViewUserControl2 = new Bilten.UI.DataGridViewUserControl();
            this.dataGridViewUserControl1 = new Bilten.UI.DataGridViewUserControl();
            this.SuspendLayout();
            // 
            // cmbTakmicenje
            // 
            this.cmbTakmicenje.FormattingEnabled = true;
            this.cmbTakmicenje.Location = new System.Drawing.Point(12, 12);
            this.cmbTakmicenje.Name = "cmbTakmicenje";
            this.cmbTakmicenje.Size = new System.Drawing.Size(330, 21);
            this.cmbTakmicenje.TabIndex = 1;
            this.cmbTakmicenje.DropDownClosed += new System.EventHandler(this.cmbTakmicenje_DropDownClosed);
            // 
            // lblRezerve
            // 
            this.lblRezerve.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRezerve.AutoSize = true;
            this.lblRezerve.Location = new System.Drawing.Point(12, 203);
            this.lblRezerve.Name = "lblRezerve";
            this.lblRezerve.Size = new System.Drawing.Size(47, 13);
            this.lblRezerve.TabIndex = 2;
            this.lblRezerve.Text = "Rezerve";
            // 
            // dataGridViewUserControl2
            // 
            this.dataGridViewUserControl2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewUserControl2.ColumnHeaderSorting = true;
            this.dataGridViewUserControl2.Location = new System.Drawing.Point(12, 219);
            this.dataGridViewUserControl2.Name = "dataGridViewUserControl2";
            this.dataGridViewUserControl2.Size = new System.Drawing.Size(690, 128);
            this.dataGridViewUserControl2.TabIndex = 3;
            // 
            // dataGridViewUserControl1
            // 
            this.dataGridViewUserControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewUserControl1.ColumnHeaderSorting = true;
            this.dataGridViewUserControl1.Location = new System.Drawing.Point(12, 54);
            this.dataGridViewUserControl1.Name = "dataGridViewUserControl1";
            this.dataGridViewUserControl1.Size = new System.Drawing.Size(690, 131);
            this.dataGridViewUserControl1.TabIndex = 0;
            // 
            // KvalifikantiTak2Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 359);
            this.Controls.Add(this.dataGridViewUserControl2);
            this.Controls.Add(this.lblRezerve);
            this.Controls.Add(this.cmbTakmicenje);
            this.Controls.Add(this.dataGridViewUserControl1);
            this.Name = "KvalifikantiTak2Form";
            this.Text = "KvalifikantiTak2Form";
            this.Shown += new System.EventHandler(this.KvalifikantiTak2Form_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DataGridViewUserControl dataGridViewUserControl1;
        private System.Windows.Forms.ComboBox cmbTakmicenje;
        private System.Windows.Forms.Label lblRezerve;
        private DataGridViewUserControl dataGridViewUserControl2;
    }
}