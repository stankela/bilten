namespace Bilten.UI
{
    partial class KategorijaGimnasticaraForm
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
            this.lblNaziv = new System.Windows.Forms.Label();
            this.txtNaziv = new System.Windows.Forms.TextBox();
            this.lblGimnastika = new System.Windows.Forms.Label();
            this.cmbGimnastika = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(20, 124);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(112, 124);
            // 
            // lblNaziv
            // 
            this.lblNaziv.AutoSize = true;
            this.lblNaziv.Location = new System.Drawing.Point(14, 16);
            this.lblNaziv.Name = "lblNaziv";
            this.lblNaziv.Size = new System.Drawing.Size(93, 13);
            this.lblNaziv.TabIndex = 2;
            this.lblNaziv.Text = "Naziv kategorije  *";
            // 
            // txtNaziv
            // 
            this.txtNaziv.Location = new System.Drawing.Point(17, 32);
            this.txtNaziv.Name = "txtNaziv";
            this.txtNaziv.Size = new System.Drawing.Size(100, 20);
            this.txtNaziv.TabIndex = 3;
            // 
            // lblGimnastika
            // 
            this.lblGimnastika.AutoSize = true;
            this.lblGimnastika.Location = new System.Drawing.Point(17, 66);
            this.lblGimnastika.Name = "lblGimnastika";
            this.lblGimnastika.Size = new System.Drawing.Size(69, 13);
            this.lblGimnastika.TabIndex = 4;
            this.lblGimnastika.Text = "Gimnastika  *";
            // 
            // cmbGimnastika
            // 
            this.cmbGimnastika.FormattingEnabled = true;
            this.cmbGimnastika.Location = new System.Drawing.Point(20, 82);
            this.cmbGimnastika.Name = "cmbGimnastika";
            this.cmbGimnastika.Size = new System.Drawing.Size(97, 21);
            this.cmbGimnastika.TabIndex = 5;
            // 
            // KategorijaGimnasticaraForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(208, 170);
            this.Controls.Add(this.txtNaziv);
            this.Controls.Add(this.lblGimnastika);
            this.Controls.Add(this.cmbGimnastika);
            this.Controls.Add(this.lblNaziv);
            this.Name = "KategorijaGimnasticaraForm";
            this.Text = "KategorijaGimnasticaraForm";
            this.Controls.SetChildIndex(this.lblNaziv, 0);
            this.Controls.SetChildIndex(this.cmbGimnastika, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.lblGimnastika, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.txtNaziv, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblNaziv;
        private System.Windows.Forms.TextBox txtNaziv;
        private System.Windows.Forms.Label lblGimnastika;
        private System.Windows.Forms.ComboBox cmbGimnastika;
    }
}