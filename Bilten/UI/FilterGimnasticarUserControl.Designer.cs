namespace Bilten.UI
{
    partial class FilterGimnasticarUserControl
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
            this.lblRegBroj = new System.Windows.Forms.Label();
            this.lblIme = new System.Windows.Forms.Label();
            this.lblPrezime = new System.Windows.Forms.Label();
            this.txtRegBroj = new System.Windows.Forms.TextBox();
            this.lblGodRodj = new System.Windows.Forms.Label();
            this.cmbGimnastika = new System.Windows.Forms.ComboBox();
            this.lblGimnastika = new System.Windows.Forms.Label();
            this.cmbDrzava = new System.Windows.Forms.ComboBox();
            this.lblDrzava = new System.Windows.Forms.Label();
            this.cmbKategorija = new System.Windows.Forms.ComboBox();
            this.lblKategorija = new System.Windows.Forms.Label();
            this.cmbKlub = new System.Windows.Forms.ComboBox();
            this.lblKlub = new System.Windows.Forms.Label();
            this.txtIme = new System.Windows.Forms.TextBox();
            this.txtPrezime = new System.Windows.Forms.TextBox();
            this.txtGodRodj = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblRegBroj
            // 
            this.lblRegBroj.AutoSize = true;
            this.lblRegBroj.Location = new System.Drawing.Point(0, 0);
            this.lblRegBroj.Name = "lblRegBroj";
            this.lblRegBroj.Size = new System.Drawing.Size(294, 13);
            this.lblRegBroj.TabIndex = 0;
            this.lblRegBroj.Text = "Registarski broj  (ako ga unesete, ostali kriterijumi se ignorisu)";
            // 
            // lblIme
            // 
            this.lblIme.AutoSize = true;
            this.lblIme.Location = new System.Drawing.Point(0, 42);
            this.lblIme.Name = "lblIme";
            this.lblIme.Size = new System.Drawing.Size(24, 13);
            this.lblIme.TabIndex = 2;
            this.lblIme.Text = "Ime";
            // 
            // lblPrezime
            // 
            this.lblPrezime.AutoSize = true;
            this.lblPrezime.Location = new System.Drawing.Point(115, 42);
            this.lblPrezime.Name = "lblPrezime";
            this.lblPrezime.Size = new System.Drawing.Size(44, 13);
            this.lblPrezime.TabIndex = 4;
            this.lblPrezime.Text = "Prezime";
            // 
            // txtRegBroj
            // 
            this.txtRegBroj.Location = new System.Drawing.Point(0, 16);
            this.txtRegBroj.Name = "txtRegBroj";
            this.txtRegBroj.Size = new System.Drawing.Size(100, 20);
            this.txtRegBroj.TabIndex = 1;
            // 
            // lblGodRodj
            // 
            this.lblGodRodj.AutoSize = true;
            this.lblGodRodj.Location = new System.Drawing.Point(251, 41);
            this.lblGodRodj.Name = "lblGodRodj";
            this.lblGodRodj.Size = new System.Drawing.Size(81, 13);
            this.lblGodRodj.TabIndex = 6;
            this.lblGodRodj.Text = "Godina rodjenja";
            // 
            // cmbGimnastika
            // 
            this.cmbGimnastika.FormattingEnabled = true;
            this.cmbGimnastika.Location = new System.Drawing.Point(372, 102);
            this.cmbGimnastika.Name = "cmbGimnastika";
            this.cmbGimnastika.Size = new System.Drawing.Size(77, 21);
            this.cmbGimnastika.TabIndex = 15;
            this.cmbGimnastika.SelectedIndexChanged += new System.EventHandler(this.cmbGimnastika_SelectedIndexChanged);
            // 
            // lblGimnastika
            // 
            this.lblGimnastika.AutoSize = true;
            this.lblGimnastika.Location = new System.Drawing.Point(369, 87);
            this.lblGimnastika.Name = "lblGimnastika";
            this.lblGimnastika.Size = new System.Drawing.Size(59, 13);
            this.lblGimnastika.TabIndex = 14;
            this.lblGimnastika.Text = "Gimnastika";
            // 
            // cmbDrzava
            // 
            this.cmbDrzava.FormattingEnabled = true;
            this.cmbDrzava.Location = new System.Drawing.Point(254, 103);
            this.cmbDrzava.Name = "cmbDrzava";
            this.cmbDrzava.Size = new System.Drawing.Size(100, 21);
            this.cmbDrzava.TabIndex = 13;
            // 
            // lblDrzava
            // 
            this.lblDrzava.AutoSize = true;
            this.lblDrzava.Location = new System.Drawing.Point(251, 87);
            this.lblDrzava.Name = "lblDrzava";
            this.lblDrzava.Size = new System.Drawing.Size(41, 13);
            this.lblDrzava.TabIndex = 12;
            this.lblDrzava.Text = "Drzava";
            // 
            // cmbKategorija
            // 
            this.cmbKategorija.FormattingEnabled = true;
            this.cmbKategorija.Location = new System.Drawing.Point(349, 57);
            this.cmbKategorija.Name = "cmbKategorija";
            this.cmbKategorija.Size = new System.Drawing.Size(100, 21);
            this.cmbKategorija.TabIndex = 9;
            // 
            // lblKategorija
            // 
            this.lblKategorija.AutoSize = true;
            this.lblKategorija.Location = new System.Drawing.Point(346, 41);
            this.lblKategorija.Name = "lblKategorija";
            this.lblKategorija.Size = new System.Drawing.Size(54, 13);
            this.lblKategorija.TabIndex = 8;
            this.lblKategorija.Text = "Kategorija";
            // 
            // cmbKlub
            // 
            this.cmbKlub.FormattingEnabled = true;
            this.cmbKlub.Location = new System.Drawing.Point(0, 102);
            this.cmbKlub.Name = "cmbKlub";
            this.cmbKlub.Size = new System.Drawing.Size(240, 21);
            this.cmbKlub.TabIndex = 11;
            // 
            // lblKlub
            // 
            this.lblKlub.AutoSize = true;
            this.lblKlub.Location = new System.Drawing.Point(0, 87);
            this.lblKlub.Name = "lblKlub";
            this.lblKlub.Size = new System.Drawing.Size(28, 13);
            this.lblKlub.TabIndex = 10;
            this.lblKlub.Text = "Klub";
            // 
            // txtIme
            // 
            this.txtIme.Location = new System.Drawing.Point(0, 58);
            this.txtIme.Name = "txtIme";
            this.txtIme.Size = new System.Drawing.Size(100, 20);
            this.txtIme.TabIndex = 3;
            // 
            // txtPrezime
            // 
            this.txtPrezime.Location = new System.Drawing.Point(118, 58);
            this.txtPrezime.Name = "txtPrezime";
            this.txtPrezime.Size = new System.Drawing.Size(122, 20);
            this.txtPrezime.TabIndex = 4;
            // 
            // txtGodRodj
            // 
            this.txtGodRodj.Location = new System.Drawing.Point(254, 57);
            this.txtGodRodj.Name = "txtGodRodj";
            this.txtGodRodj.Size = new System.Drawing.Size(78, 20);
            this.txtGodRodj.TabIndex = 7;
            // 
            // FilterGimnasticarUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtGodRodj);
            this.Controls.Add(this.txtPrezime);
            this.Controls.Add(this.txtIme);
            this.Controls.Add(this.lblKlub);
            this.Controls.Add(this.cmbKlub);
            this.Controls.Add(this.lblKategorija);
            this.Controls.Add(this.cmbKategorija);
            this.Controls.Add(this.lblDrzava);
            this.Controls.Add(this.cmbDrzava);
            this.Controls.Add(this.lblGimnastika);
            this.Controls.Add(this.cmbGimnastika);
            this.Controls.Add(this.lblGodRodj);
            this.Controls.Add(this.txtRegBroj);
            this.Controls.Add(this.lblPrezime);
            this.Controls.Add(this.lblIme);
            this.Controls.Add(this.lblRegBroj);
            this.Name = "FilterGimnasticarUserControl";
            this.Size = new System.Drawing.Size(456, 127);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblRegBroj;
        private System.Windows.Forms.Label lblIme;
        private System.Windows.Forms.Label lblPrezime;
        private System.Windows.Forms.TextBox txtRegBroj;
        private System.Windows.Forms.Label lblGodRodj;
        private System.Windows.Forms.ComboBox cmbGimnastika;
        private System.Windows.Forms.Label lblGimnastika;
        private System.Windows.Forms.ComboBox cmbDrzava;
        private System.Windows.Forms.Label lblDrzava;
        private System.Windows.Forms.ComboBox cmbKategorija;
        private System.Windows.Forms.Label lblKategorija;
        private System.Windows.Forms.ComboBox cmbKlub;
        private System.Windows.Forms.Label lblKlub;
        private System.Windows.Forms.TextBox txtIme;
        private System.Windows.Forms.TextBox txtPrezime;
        private System.Windows.Forms.TextBox txtGodRodj;
    }
}
