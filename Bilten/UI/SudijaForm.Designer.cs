namespace Bilten.UI
{
    partial class SudijaForm
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
            this.lblIme = new System.Windows.Forms.Label();
            this.txtIme = new System.Windows.Forms.TextBox();
            this.lblPrezime = new System.Windows.Forms.Label();
            this.txtPrezime = new System.Windows.Forms.TextBox();
            this.lblPol = new System.Windows.Forms.Label();
            this.cmbDrzava = new System.Windows.Forms.ComboBox();
            this.lblDrzava = new System.Windows.Forms.Label();
            this.cmbPol = new System.Windows.Forms.ComboBox();
            this.btnAddDrzava = new System.Windows.Forms.Button();
            this.btnAddKlub = new System.Windows.Forms.Button();
            this.cmbKlub = new System.Windows.Forms.ComboBox();
            this.lblKlub = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(214, 212);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(306, 212);
            // 
            // lblIme
            // 
            this.lblIme.AutoSize = true;
            this.lblIme.Location = new System.Drawing.Point(12, 20);
            this.lblIme.Name = "lblIme";
            this.lblIme.Size = new System.Drawing.Size(34, 13);
            this.lblIme.TabIndex = 2;
            this.lblIme.Text = "Ime  *";
            // 
            // txtIme
            // 
            this.txtIme.Location = new System.Drawing.Point(12, 36);
            this.txtIme.Name = "txtIme";
            this.txtIme.Size = new System.Drawing.Size(100, 20);
            this.txtIme.TabIndex = 3;
            // 
            // lblPrezime
            // 
            this.lblPrezime.AutoSize = true;
            this.lblPrezime.Location = new System.Drawing.Point(129, 20);
            this.lblPrezime.Name = "lblPrezime";
            this.lblPrezime.Size = new System.Drawing.Size(54, 13);
            this.lblPrezime.TabIndex = 4;
            this.lblPrezime.Text = "Prezime  *";
            // 
            // txtPrezime
            // 
            this.txtPrezime.Location = new System.Drawing.Point(132, 36);
            this.txtPrezime.Name = "txtPrezime";
            this.txtPrezime.Size = new System.Drawing.Size(100, 20);
            this.txtPrezime.TabIndex = 5;
            // 
            // lblPol
            // 
            this.lblPol.AutoSize = true;
            this.lblPol.Location = new System.Drawing.Point(255, 20);
            this.lblPol.Name = "lblPol";
            this.lblPol.Size = new System.Drawing.Size(32, 13);
            this.lblPol.TabIndex = 6;
            this.lblPol.Text = "Pol  *";
            // 
            // cmbDrzava
            // 
            this.cmbDrzava.FormattingEnabled = true;
            this.cmbDrzava.Location = new System.Drawing.Point(12, 94);
            this.cmbDrzava.Name = "cmbDrzava";
            this.cmbDrzava.Size = new System.Drawing.Size(100, 21);
            this.cmbDrzava.TabIndex = 9;
            // 
            // lblDrzava
            // 
            this.lblDrzava.AutoSize = true;
            this.lblDrzava.Location = new System.Drawing.Point(12, 78);
            this.lblDrzava.Name = "lblDrzava";
            this.lblDrzava.Size = new System.Drawing.Size(51, 13);
            this.lblDrzava.TabIndex = 8;
            this.lblDrzava.Text = "Drzava  *";
            // 
            // cmbPol
            // 
            this.cmbPol.FormattingEnabled = true;
            this.cmbPol.Location = new System.Drawing.Point(255, 36);
            this.cmbPol.Name = "cmbPol";
            this.cmbPol.Size = new System.Drawing.Size(68, 21);
            this.cmbPol.TabIndex = 7;
            // 
            // btnAddDrzava
            // 
            this.btnAddDrzava.Location = new System.Drawing.Point(118, 93);
            this.btnAddDrzava.Name = "btnAddDrzava";
            this.btnAddDrzava.Size = new System.Drawing.Size(25, 23);
            this.btnAddDrzava.TabIndex = 10;
            this.btnAddDrzava.TabStop = false;
            this.btnAddDrzava.Text = "...";
            this.btnAddDrzava.UseVisualStyleBackColor = true;
            this.btnAddDrzava.Click += new System.EventHandler(this.btnAddDrzava_Click);
            // 
            // btnAddKlub
            // 
            this.btnAddKlub.Location = new System.Drawing.Point(228, 146);
            this.btnAddKlub.Name = "btnAddKlub";
            this.btnAddKlub.Size = new System.Drawing.Size(24, 23);
            this.btnAddKlub.TabIndex = 27;
            this.btnAddKlub.TabStop = false;
            this.btnAddKlub.Text = "...";
            this.btnAddKlub.UseVisualStyleBackColor = true;
            this.btnAddKlub.Click += new System.EventHandler(this.btnAddKlub_Click);
            // 
            // cmbKlub
            // 
            this.cmbKlub.FormattingEnabled = true;
            this.cmbKlub.Location = new System.Drawing.Point(15, 147);
            this.cmbKlub.Name = "cmbKlub";
            this.cmbKlub.Size = new System.Drawing.Size(207, 21);
            this.cmbKlub.TabIndex = 26;
            // 
            // lblKlub
            // 
            this.lblKlub.AutoSize = true;
            this.lblKlub.Location = new System.Drawing.Point(12, 131);
            this.lblKlub.Name = "lblKlub";
            this.lblKlub.Size = new System.Drawing.Size(28, 13);
            this.lblKlub.TabIndex = 25;
            this.lblKlub.Text = "Klub";
            // 
            // SudijaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 256);
            this.Controls.Add(this.btnAddKlub);
            this.Controls.Add(this.cmbKlub);
            this.Controls.Add(this.lblKlub);
            this.Controls.Add(this.btnAddDrzava);
            this.Controls.Add(this.cmbPol);
            this.Controls.Add(this.lblDrzava);
            this.Controls.Add(this.cmbDrzava);
            this.Controls.Add(this.lblIme);
            this.Controls.Add(this.txtIme);
            this.Controls.Add(this.lblPol);
            this.Controls.Add(this.txtPrezime);
            this.Controls.Add(this.lblPrezime);
            this.Name = "SudijaForm";
            this.Text = "SudijaForm";
            this.Shown += new System.EventHandler(this.SudijaForm_Shown);
            this.Controls.SetChildIndex(this.lblPrezime, 0);
            this.Controls.SetChildIndex(this.txtPrezime, 0);
            this.Controls.SetChildIndex(this.lblPol, 0);
            this.Controls.SetChildIndex(this.txtIme, 0);
            this.Controls.SetChildIndex(this.lblIme, 0);
            this.Controls.SetChildIndex(this.cmbDrzava, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.lblDrzava, 0);
            this.Controls.SetChildIndex(this.cmbPol, 0);
            this.Controls.SetChildIndex(this.btnAddDrzava, 0);
            this.Controls.SetChildIndex(this.lblKlub, 0);
            this.Controls.SetChildIndex(this.cmbKlub, 0);
            this.Controls.SetChildIndex(this.btnAddKlub, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblIme;
        private System.Windows.Forms.TextBox txtIme;
        private System.Windows.Forms.Label lblPrezime;
        private System.Windows.Forms.TextBox txtPrezime;
        private System.Windows.Forms.Label lblPol;
        private System.Windows.Forms.ComboBox cmbDrzava;
        private System.Windows.Forms.Label lblDrzava;
        private System.Windows.Forms.ComboBox cmbPol;
        private System.Windows.Forms.Button btnAddDrzava;
        private System.Windows.Forms.Button btnAddKlub;
        private System.Windows.Forms.ComboBox cmbKlub;
        private System.Windows.Forms.Label lblKlub;
    }
}