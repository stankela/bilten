namespace Bilten.UI
{
    partial class GimnasticarForm
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
            this.txtIme = new System.Windows.Forms.TextBox();
            this.lblIme = new System.Windows.Forms.Label();
            this.lblPrezime = new System.Windows.Forms.Label();
            this.txtPrezime = new System.Windows.Forms.TextBox();
            this.lblDatRodj = new System.Windows.Forms.Label();
            this.txtDatRodj = new System.Windows.Forms.TextBox();
            this.lblRegBroj = new System.Windows.Forms.Label();
            this.txtRegBroj = new System.Windows.Forms.TextBox();
            this.lblDatumPoslReg = new System.Windows.Forms.Label();
            this.txtDatumPoslReg = new System.Windows.Forms.TextBox();
            this.lblGimnastika = new System.Windows.Forms.Label();
            this.cmbGimnastika = new System.Windows.Forms.ComboBox();
            this.lblKategorija = new System.Windows.Forms.Label();
            this.cmbKategorija = new System.Windows.Forms.ComboBox();
            this.lblKlub = new System.Windows.Forms.Label();
            this.cmbKlub = new System.Windows.Forms.ComboBox();
            this.lblDrzava = new System.Windows.Forms.Label();
            this.cmbDrzava = new System.Windows.Forms.ComboBox();
            this.btnAddDrzava = new System.Windows.Forms.Button();
            this.btnAddKlub = new System.Windows.Forms.Button();
            this.btnAddKategorija = new System.Windows.Forms.Button();
            this.lblSrednjeIme = new System.Windows.Forms.Label();
            this.txtSrednjeIme = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(388, 323);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(480, 323);
            // 
            // txtIme
            // 
            this.txtIme.Location = new System.Drawing.Point(24, 36);
            this.txtIme.Name = "txtIme";
            this.txtIme.Size = new System.Drawing.Size(100, 20);
            this.txtIme.TabIndex = 3;
            // 
            // lblIme
            // 
            this.lblIme.AutoSize = true;
            this.lblIme.Location = new System.Drawing.Point(21, 20);
            this.lblIme.Name = "lblIme";
            this.lblIme.Size = new System.Drawing.Size(34, 13);
            this.lblIme.TabIndex = 2;
            this.lblIme.Text = "Ime  *";
            // 
            // lblPrezime
            // 
            this.lblPrezime.AutoSize = true;
            this.lblPrezime.Location = new System.Drawing.Point(224, 20);
            this.lblPrezime.Name = "lblPrezime";
            this.lblPrezime.Size = new System.Drawing.Size(54, 13);
            this.lblPrezime.TabIndex = 4;
            this.lblPrezime.Text = "Prezime  *";
            // 
            // txtPrezime
            // 
            this.txtPrezime.Location = new System.Drawing.Point(227, 36);
            this.txtPrezime.Name = "txtPrezime";
            this.txtPrezime.Size = new System.Drawing.Size(167, 20);
            this.txtPrezime.TabIndex = 5;
            this.txtPrezime.Enter += new System.EventHandler(this.txtPrezime_Enter);
            this.txtPrezime.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPrezime_KeyDown);
            // 
            // lblDatRodj
            // 
            this.lblDatRodj.AutoSize = true;
            this.lblDatRodj.Location = new System.Drawing.Point(21, 69);
            this.lblDatRodj.Name = "lblDatRodj";
            this.lblDatRodj.Size = new System.Drawing.Size(122, 13);
            this.lblDatRodj.TabIndex = 8;
            this.lblDatRodj.Text = "Datum ili godina rodjenja";
            // 
            // txtDatRodj
            // 
            this.txtDatRodj.Location = new System.Drawing.Point(24, 85);
            this.txtDatRodj.Name = "txtDatRodj";
            this.txtDatRodj.Size = new System.Drawing.Size(100, 20);
            this.txtDatRodj.TabIndex = 9;
            // 
            // lblRegBroj
            // 
            this.lblRegBroj.AutoSize = true;
            this.lblRegBroj.Location = new System.Drawing.Point(21, 173);
            this.lblRegBroj.Name = "lblRegBroj";
            this.lblRegBroj.Size = new System.Drawing.Size(79, 13);
            this.lblRegBroj.TabIndex = 12;
            this.lblRegBroj.Text = "Registarski broj";
            // 
            // txtRegBroj
            // 
            this.txtRegBroj.Location = new System.Drawing.Point(24, 189);
            this.txtRegBroj.Name = "txtRegBroj";
            this.txtRegBroj.Size = new System.Drawing.Size(85, 20);
            this.txtRegBroj.TabIndex = 13;
            // 
            // lblDatumPoslReg
            // 
            this.lblDatumPoslReg.AutoSize = true;
            this.lblDatumPoslReg.Location = new System.Drawing.Point(128, 173);
            this.lblDatumPoslReg.Name = "lblDatumPoslReg";
            this.lblDatumPoslReg.Size = new System.Drawing.Size(183, 13);
            this.lblDatumPoslReg.TabIndex = 16;
            this.lblDatumPoslReg.Text = "Datum ili godina poslednje registracije";
            // 
            // txtDatumPoslReg
            // 
            this.txtDatumPoslReg.Location = new System.Drawing.Point(131, 189);
            this.txtDatumPoslReg.Name = "txtDatumPoslReg";
            this.txtDatumPoslReg.Size = new System.Drawing.Size(100, 20);
            this.txtDatumPoslReg.TabIndex = 17;
            // 
            // lblGimnastika
            // 
            this.lblGimnastika.AutoSize = true;
            this.lblGimnastika.Location = new System.Drawing.Point(412, 20);
            this.lblGimnastika.Name = "lblGimnastika";
            this.lblGimnastika.Size = new System.Drawing.Size(69, 13);
            this.lblGimnastika.TabIndex = 6;
            this.lblGimnastika.Text = "Gimnastika  *";
            // 
            // cmbGimnastika
            // 
            this.cmbGimnastika.FormattingEnabled = true;
            this.cmbGimnastika.Location = new System.Drawing.Point(415, 36);
            this.cmbGimnastika.Name = "cmbGimnastika";
            this.cmbGimnastika.Size = new System.Drawing.Size(79, 21);
            this.cmbGimnastika.TabIndex = 7;
            this.cmbGimnastika.SelectedIndexChanged += new System.EventHandler(this.cmbGimnastika_SelectedIndexChanged);
            // 
            // lblKategorija
            // 
            this.lblKategorija.AutoSize = true;
            this.lblKategorija.Location = new System.Drawing.Point(21, 279);
            this.lblKategorija.Name = "lblKategorija";
            this.lblKategorija.Size = new System.Drawing.Size(54, 13);
            this.lblKategorija.TabIndex = 20;
            this.lblKategorija.Text = "Kategorija";
            // 
            // cmbKategorija
            // 
            this.cmbKategorija.FormattingEnabled = true;
            this.cmbKategorija.Location = new System.Drawing.Point(24, 295);
            this.cmbKategorija.Name = "cmbKategorija";
            this.cmbKategorija.Size = new System.Drawing.Size(207, 21);
            this.cmbKategorija.TabIndex = 21;
            this.cmbKategorija.DropDown += new System.EventHandler(this.cmbKategorija_DropDown);
            // 
            // lblKlub
            // 
            this.lblKlub.AutoSize = true;
            this.lblKlub.Location = new System.Drawing.Point(21, 226);
            this.lblKlub.Name = "lblKlub";
            this.lblKlub.Size = new System.Drawing.Size(28, 13);
            this.lblKlub.TabIndex = 18;
            this.lblKlub.Text = "Klub";
            // 
            // cmbKlub
            // 
            this.cmbKlub.FormattingEnabled = true;
            this.cmbKlub.Location = new System.Drawing.Point(24, 242);
            this.cmbKlub.Name = "cmbKlub";
            this.cmbKlub.Size = new System.Drawing.Size(207, 21);
            this.cmbKlub.TabIndex = 19;
            // 
            // lblDrzava
            // 
            this.lblDrzava.AutoSize = true;
            this.lblDrzava.Location = new System.Drawing.Point(21, 118);
            this.lblDrzava.Name = "lblDrzava";
            this.lblDrzava.Size = new System.Drawing.Size(51, 13);
            this.lblDrzava.TabIndex = 10;
            this.lblDrzava.Text = "Drzava  *";
            // 
            // cmbDrzava
            // 
            this.cmbDrzava.FormattingEnabled = true;
            this.cmbDrzava.Location = new System.Drawing.Point(24, 134);
            this.cmbDrzava.Name = "cmbDrzava";
            this.cmbDrzava.Size = new System.Drawing.Size(121, 21);
            this.cmbDrzava.TabIndex = 11;
            // 
            // btnAddDrzava
            // 
            this.btnAddDrzava.Location = new System.Drawing.Point(151, 133);
            this.btnAddDrzava.Name = "btnAddDrzava";
            this.btnAddDrzava.Size = new System.Drawing.Size(25, 23);
            this.btnAddDrzava.TabIndex = 23;
            this.btnAddDrzava.Text = "...";
            this.btnAddDrzava.UseVisualStyleBackColor = true;
            this.btnAddDrzava.Click += new System.EventHandler(this.btnAddDrzava_Click);
            // 
            // btnAddKlub
            // 
            this.btnAddKlub.Location = new System.Drawing.Point(237, 241);
            this.btnAddKlub.Name = "btnAddKlub";
            this.btnAddKlub.Size = new System.Drawing.Size(24, 23);
            this.btnAddKlub.TabIndex = 24;
            this.btnAddKlub.Text = "...";
            this.btnAddKlub.UseVisualStyleBackColor = true;
            this.btnAddKlub.Click += new System.EventHandler(this.btnAddKlub_Click);
            // 
            // btnAddKategorija
            // 
            this.btnAddKategorija.Location = new System.Drawing.Point(237, 294);
            this.btnAddKategorija.Name = "btnAddKategorija";
            this.btnAddKategorija.Size = new System.Drawing.Size(25, 23);
            this.btnAddKategorija.TabIndex = 25;
            this.btnAddKategorija.Text = "...";
            this.btnAddKategorija.UseVisualStyleBackColor = true;
            this.btnAddKategorija.Click += new System.EventHandler(this.btnAddKategorija_Click);
            // 
            // lblSrednjeIme
            // 
            this.lblSrednjeIme.AutoSize = true;
            this.lblSrednjeIme.Location = new System.Drawing.Point(141, 21);
            this.lblSrednjeIme.Name = "lblSrednjeIme";
            this.lblSrednjeIme.Size = new System.Drawing.Size(62, 13);
            this.lblSrednjeIme.TabIndex = 26;
            this.lblSrednjeIme.Text = "Srednje ime";
            // 
            // txtSrednjeIme
            // 
            this.txtSrednjeIme.Location = new System.Drawing.Point(144, 37);
            this.txtSrednjeIme.Name = "txtSrednjeIme";
            this.txtSrednjeIme.Size = new System.Drawing.Size(59, 20);
            this.txtSrednjeIme.TabIndex = 27;
            // 
            // GimnasticarForm
            // 
            this.AcceptButton = null;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 368);
            this.Controls.Add(this.btnAddKategorija);
            this.Controls.Add(this.btnAddKlub);
            this.Controls.Add(this.txtSrednjeIme);
            this.Controls.Add(this.lblSrednjeIme);
            this.Controls.Add(this.btnAddDrzava);
            this.Controls.Add(this.cmbDrzava);
            this.Controls.Add(this.lblDrzava);
            this.Controls.Add(this.cmbKlub);
            this.Controls.Add(this.lblKlub);
            this.Controls.Add(this.cmbKategorija);
            this.Controls.Add(this.lblKategorija);
            this.Controls.Add(this.cmbGimnastika);
            this.Controls.Add(this.lblGimnastika);
            this.Controls.Add(this.txtDatumPoslReg);
            this.Controls.Add(this.lblDatumPoslReg);
            this.Controls.Add(this.txtRegBroj);
            this.Controls.Add(this.lblRegBroj);
            this.Controls.Add(this.txtDatRodj);
            this.Controls.Add(this.lblDatRodj);
            this.Controls.Add(this.txtIme);
            this.Controls.Add(this.txtPrezime);
            this.Controls.Add(this.lblIme);
            this.Controls.Add(this.lblPrezime);
            this.Name = "GimnasticarForm";
            this.Text = "GimnasticarForm";
            this.Shown += new System.EventHandler(this.GimnasticarForm_Shown);
            this.Controls.SetChildIndex(this.lblPrezime, 0);
            this.Controls.SetChildIndex(this.lblIme, 0);
            this.Controls.SetChildIndex(this.txtPrezime, 0);
            this.Controls.SetChildIndex(this.txtIme, 0);
            this.Controls.SetChildIndex(this.lblDatRodj, 0);
            this.Controls.SetChildIndex(this.txtDatRodj, 0);
            this.Controls.SetChildIndex(this.lblRegBroj, 0);
            this.Controls.SetChildIndex(this.txtRegBroj, 0);
            this.Controls.SetChildIndex(this.lblDatumPoslReg, 0);
            this.Controls.SetChildIndex(this.txtDatumPoslReg, 0);
            this.Controls.SetChildIndex(this.lblGimnastika, 0);
            this.Controls.SetChildIndex(this.cmbGimnastika, 0);
            this.Controls.SetChildIndex(this.lblKategorija, 0);
            this.Controls.SetChildIndex(this.cmbKategorija, 0);
            this.Controls.SetChildIndex(this.lblKlub, 0);
            this.Controls.SetChildIndex(this.cmbKlub, 0);
            this.Controls.SetChildIndex(this.lblDrzava, 0);
            this.Controls.SetChildIndex(this.cmbDrzava, 0);
            this.Controls.SetChildIndex(this.btnAddDrzava, 0);
            this.Controls.SetChildIndex(this.lblSrednjeIme, 0);
            this.Controls.SetChildIndex(this.txtSrednjeIme, 0);
            this.Controls.SetChildIndex(this.btnAddKlub, 0);
            this.Controls.SetChildIndex(this.btnAddKategorija, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtIme;
        private System.Windows.Forms.Label lblIme;
        private System.Windows.Forms.Label lblPrezime;
        private System.Windows.Forms.TextBox txtPrezime;
        private System.Windows.Forms.Label lblDatRodj;
        private System.Windows.Forms.TextBox txtDatRodj;
        private System.Windows.Forms.Label lblRegBroj;
        private System.Windows.Forms.TextBox txtRegBroj;
        private System.Windows.Forms.Label lblDatumPoslReg;
        private System.Windows.Forms.TextBox txtDatumPoslReg;
        private System.Windows.Forms.Label lblGimnastika;
        private System.Windows.Forms.ComboBox cmbGimnastika;
        private System.Windows.Forms.Label lblKategorija;
        private System.Windows.Forms.ComboBox cmbKategorija;
        private System.Windows.Forms.Label lblKlub;
        private System.Windows.Forms.ComboBox cmbKlub;
        private System.Windows.Forms.Label lblDrzava;
        private System.Windows.Forms.ComboBox cmbDrzava;
        private System.Windows.Forms.Button btnAddDrzava;
        private System.Windows.Forms.Button btnAddKlub;
        private System.Windows.Forms.Button btnAddKategorija;
        private System.Windows.Forms.Label lblSrednjeIme;
        private System.Windows.Forms.TextBox txtSrednjeIme;
    }
}