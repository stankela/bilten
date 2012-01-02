namespace Bilten.UI
{
    partial class SablonRasporedaNastupaTakm1ItemForm
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
            this.lblSprava = new System.Windows.Forms.Label();
            this.cmbSprava = new System.Windows.Forms.ComboBox();
            this.lblEkipa = new System.Windows.Forms.Label();
            this.cmbEkipa = new System.Windows.Forms.ComboBox();
            this.lblBrojUcesnika = new System.Windows.Forms.Label();
            this.txtBrojUcesnika = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(11, 176);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(103, 176);
            // 
            // lblSprava
            // 
            this.lblSprava.AutoSize = true;
            this.lblSprava.Location = new System.Drawing.Point(9, 13);
            this.lblSprava.Name = "lblSprava";
            this.lblSprava.Size = new System.Drawing.Size(41, 13);
            this.lblSprava.TabIndex = 0;
            this.lblSprava.Text = "Sprava";
            // 
            // cmbSprava
            // 
            this.cmbSprava.FormattingEnabled = true;
            this.cmbSprava.Location = new System.Drawing.Point(12, 29);
            this.cmbSprava.Name = "cmbSprava";
            this.cmbSprava.Size = new System.Drawing.Size(121, 21);
            this.cmbSprava.TabIndex = 1;
            // 
            // lblEkipa
            // 
            this.lblEkipa.AutoSize = true;
            this.lblEkipa.Location = new System.Drawing.Point(9, 64);
            this.lblEkipa.Name = "lblEkipa";
            this.lblEkipa.Size = new System.Drawing.Size(34, 13);
            this.lblEkipa.TabIndex = 2;
            this.lblEkipa.Text = "Ekipa";
            // 
            // cmbEkipa
            // 
            this.cmbEkipa.FormattingEnabled = true;
            this.cmbEkipa.Location = new System.Drawing.Point(12, 80);
            this.cmbEkipa.Name = "cmbEkipa";
            this.cmbEkipa.Size = new System.Drawing.Size(121, 21);
            this.cmbEkipa.TabIndex = 3;
            // 
            // lblBrojUcesnika
            // 
            this.lblBrojUcesnika.AutoSize = true;
            this.lblBrojUcesnika.Location = new System.Drawing.Point(9, 113);
            this.lblBrojUcesnika.Name = "lblBrojUcesnika";
            this.lblBrojUcesnika.Size = new System.Drawing.Size(71, 13);
            this.lblBrojUcesnika.TabIndex = 4;
            this.lblBrojUcesnika.Text = "Broj ucesnika";
            // 
            // txtBrojUcesnika
            // 
            this.txtBrojUcesnika.Location = new System.Drawing.Point(12, 129);
            this.txtBrojUcesnika.Name = "txtBrojUcesnika";
            this.txtBrojUcesnika.Size = new System.Drawing.Size(100, 20);
            this.txtBrojUcesnika.TabIndex = 5;
            // 
            // SablonRasporedaNastupaTakm1ItemForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(200, 217);
            this.Controls.Add(this.txtBrojUcesnika);
            this.Controls.Add(this.lblBrojUcesnika);
            this.Controls.Add(this.cmbEkipa);
            this.Controls.Add(this.lblEkipa);
            this.Controls.Add(this.cmbSprava);
            this.Controls.Add(this.lblSprava);
            this.Name = "SablonRasporedaNastupaTakm1ItemForm";
            this.Text = "SablonRasporedaNastupaTakm1ItemForm";
            this.Controls.SetChildIndex(this.lblSprava, 0);
            this.Controls.SetChildIndex(this.cmbSprava, 0);
            this.Controls.SetChildIndex(this.lblEkipa, 0);
            this.Controls.SetChildIndex(this.cmbEkipa, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.lblBrojUcesnika, 0);
            this.Controls.SetChildIndex(this.txtBrojUcesnika, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSprava;
        private System.Windows.Forms.ComboBox cmbSprava;
        private System.Windows.Forms.Label lblEkipa;
        private System.Windows.Forms.ComboBox cmbEkipa;
        private System.Windows.Forms.Label lblBrojUcesnika;
        private System.Windows.Forms.TextBox txtBrojUcesnika;

    }
}