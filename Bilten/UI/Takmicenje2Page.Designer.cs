namespace Bilten.UI
{
    partial class Takmicenje2Page
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
            this.ckbPostojiTak2 = new System.Windows.Forms.CheckBox();
            this.lblMaxTak = new System.Windows.Forms.Label();
            this.txtMaxTak = new System.Windows.Forms.TextBox();
            this.ckbNeogranicenBrojTak = new System.Windows.Forms.CheckBox();
            this.lblBrojFinalista = new System.Windows.Forms.Label();
            this.txtBrojFinalista = new System.Windows.Forms.TextBox();
            this.lblBrojRezervi = new System.Windows.Forms.Label();
            this.txtBrojRezervi = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbtNaOsnovuTak1 = new System.Windows.Forms.RadioButton();
            this.rbtOdvojenoTak2 = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbtBoljiPreskok = new System.Windows.Forms.RadioButton();
            this.rbtPrviPreskok = new System.Windows.Forms.RadioButton();
            this.lblPreskokViseboj = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.rbtDrzava = new System.Windows.Forms.RadioButton();
            this.rbtKlub = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // ckbPostojiTak2
            // 
            this.ckbPostojiTak2.AutoSize = true;
            this.ckbPostojiTak2.Location = new System.Drawing.Point(18, 16);
            this.ckbPostojiTak2.Name = "ckbPostojiTak2";
            this.ckbPostojiTak2.Size = new System.Drawing.Size(120, 17);
            this.ckbPostojiTak2.TabIndex = 0;
            this.ckbPostojiTak2.Text = "Postoji takmicenje II";
            this.ckbPostojiTak2.UseVisualStyleBackColor = true;
            this.ckbPostojiTak2.CheckedChanged += new System.EventHandler(this.ckbPostojiTak2_CheckedChanged);
            // 
            // lblMaxTak
            // 
            this.lblMaxTak.AutoSize = true;
            this.lblMaxTak.Location = new System.Drawing.Point(15, 289);
            this.lblMaxTak.Name = "lblMaxTak";
            this.lblMaxTak.Size = new System.Drawing.Size(167, 13);
            this.lblMaxTak.TabIndex = 2;
            this.lblMaxTak.Text = "Maksimalan broj takmicara iz istog";
            // 
            // txtMaxTak
            // 
            this.txtMaxTak.Location = new System.Drawing.Point(254, 286);
            this.txtMaxTak.Name = "txtMaxTak";
            this.txtMaxTak.Size = new System.Drawing.Size(43, 20);
            this.txtMaxTak.TabIndex = 9;
            this.txtMaxTak.TextChanged += new System.EventHandler(this.txtMaxTak_TextChanged);
            // 
            // ckbNeogranicenBrojTak
            // 
            this.ckbNeogranicenBrojTak.AutoSize = true;
            this.ckbNeogranicenBrojTak.Location = new System.Drawing.Point(303, 288);
            this.ckbNeogranicenBrojTak.Name = "ckbNeogranicenBrojTak";
            this.ckbNeogranicenBrojTak.Size = new System.Drawing.Size(195, 17);
            this.ckbNeogranicenBrojTak.TabIndex = 10;
            this.ckbNeogranicenBrojTak.Text = "Nema ogranicenja za broj takmicara";
            this.ckbNeogranicenBrojTak.UseVisualStyleBackColor = true;
            this.ckbNeogranicenBrojTak.CheckedChanged += new System.EventHandler(this.ckbNeogranicenBrojTak_CheckedChanged);
            // 
            // lblBrojFinalista
            // 
            this.lblBrojFinalista.AutoSize = true;
            this.lblBrojFinalista.Location = new System.Drawing.Point(15, 214);
            this.lblBrojFinalista.Name = "lblBrojFinalista";
            this.lblBrojFinalista.Size = new System.Drawing.Size(63, 13);
            this.lblBrojFinalista.TabIndex = 5;
            this.lblBrojFinalista.Text = "Broj finalista";
            // 
            // txtBrojFinalista
            // 
            this.txtBrojFinalista.Location = new System.Drawing.Point(84, 211);
            this.txtBrojFinalista.Name = "txtBrojFinalista";
            this.txtBrojFinalista.Size = new System.Drawing.Size(42, 20);
            this.txtBrojFinalista.TabIndex = 6;
            this.txtBrojFinalista.TextChanged += new System.EventHandler(this.txtBrojFinalista_TextChanged);
            // 
            // lblBrojRezervi
            // 
            this.lblBrojRezervi.AutoSize = true;
            this.lblBrojRezervi.Location = new System.Drawing.Point(15, 248);
            this.lblBrojRezervi.Name = "lblBrojRezervi";
            this.lblBrojRezervi.Size = new System.Drawing.Size(59, 13);
            this.lblBrojRezervi.TabIndex = 7;
            this.lblBrojRezervi.Text = "Broj rezervi";
            // 
            // txtBrojRezervi
            // 
            this.txtBrojRezervi.Location = new System.Drawing.Point(84, 245);
            this.txtBrojRezervi.Name = "txtBrojRezervi";
            this.txtBrojRezervi.Size = new System.Drawing.Size(41, 20);
            this.txtBrojRezervi.TabIndex = 8;
            this.txtBrojRezervi.TextChanged += new System.EventHandler(this.txtBrojRezervi_TextChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbtNaOsnovuTak1);
            this.panel1.Controls.Add(this.rbtOdvojenoTak2);
            this.panel1.Location = new System.Drawing.Point(18, 51);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 53);
            this.panel1.TabIndex = 12;
            // 
            // rbtNaOsnovuTak1
            // 
            this.rbtNaOsnovuTak1.AutoSize = true;
            this.rbtNaOsnovuTak1.Location = new System.Drawing.Point(3, 26);
            this.rbtNaOsnovuTak1.Name = "rbtNaOsnovuTak1";
            this.rbtNaOsnovuTak1.Size = new System.Drawing.Size(190, 17);
            this.rbtNaOsnovuTak1.TabIndex = 3;
            this.rbtNaOsnovuTak1.Text = "Na osnovu rezultata iz takmicenja I";
            this.rbtNaOsnovuTak1.UseVisualStyleBackColor = true;
            this.rbtNaOsnovuTak1.CheckedChanged += new System.EventHandler(this.rbtNaOsnovuTak1_CheckedChanged);
            // 
            // rbtOdvojenoTak2
            // 
            this.rbtOdvojenoTak2.Location = new System.Drawing.Point(3, 3);
            this.rbtOdvojenoTak2.Name = "rbtOdvojenoTak2";
            this.rbtOdvojenoTak2.Size = new System.Drawing.Size(141, 17);
            this.rbtOdvojenoTak2.TabIndex = 2;
            this.rbtOdvojenoTak2.Text = "Posebno takmicenje II";
            this.rbtOdvojenoTak2.UseVisualStyleBackColor = true;
            this.rbtOdvojenoTak2.CheckedChanged += new System.EventHandler(this.rbtOdvojenoTak2_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbtBoljiPreskok);
            this.panel2.Controls.Add(this.rbtPrviPreskok);
            this.panel2.Controls.Add(this.lblPreskokViseboj);
            this.panel2.Location = new System.Drawing.Point(18, 110);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 80);
            this.panel2.TabIndex = 13;
            // 
            // rbtBoljiPreskok
            // 
            this.rbtBoljiPreskok.AutoSize = true;
            this.rbtBoljiPreskok.Location = new System.Drawing.Point(3, 54);
            this.rbtBoljiPreskok.Name = "rbtBoljiPreskok";
            this.rbtBoljiPreskok.Size = new System.Drawing.Size(85, 17);
            this.rbtBoljiPreskok.TabIndex = 2;
            this.rbtBoljiPreskok.TabStop = true;
            this.rbtBoljiPreskok.Text = "Bolji preskok";
            this.rbtBoljiPreskok.UseVisualStyleBackColor = true;
            this.rbtBoljiPreskok.CheckedChanged += new System.EventHandler(this.rbtBoljiPreskok_CheckedChanged);
            // 
            // rbtPrviPreskok
            // 
            this.rbtPrviPreskok.AutoSize = true;
            this.rbtPrviPreskok.Location = new System.Drawing.Point(3, 31);
            this.rbtPrviPreskok.Name = "rbtPrviPreskok";
            this.rbtPrviPreskok.Size = new System.Drawing.Size(84, 17);
            this.rbtPrviPreskok.TabIndex = 1;
            this.rbtPrviPreskok.TabStop = true;
            this.rbtPrviPreskok.Text = "Prvi preskok";
            this.rbtPrviPreskok.UseVisualStyleBackColor = true;
            this.rbtPrviPreskok.CheckedChanged += new System.EventHandler(this.rbtPrviPreskok_CheckedChanged);
            // 
            // lblPreskokViseboj
            // 
            this.lblPreskokViseboj.AutoSize = true;
            this.lblPreskokViseboj.Location = new System.Drawing.Point(3, 10);
            this.lblPreskokViseboj.Name = "lblPreskokViseboj";
            this.lblPreskokViseboj.Size = new System.Drawing.Size(153, 13);
            this.lblPreskokViseboj.TabIndex = 0;
            this.lblPreskokViseboj.Text = "Za preskok viseboja racuna se";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.rbtDrzava);
            this.panel4.Controls.Add(this.rbtKlub);
            this.panel4.Location = new System.Drawing.Point(184, 277);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(66, 42);
            this.panel4.TabIndex = 14;
            // 
            // rbtDrzava
            // 
            this.rbtDrzava.AutoSize = true;
            this.rbtDrzava.Location = new System.Drawing.Point(3, 21);
            this.rbtDrzava.Name = "rbtDrzava";
            this.rbtDrzava.Size = new System.Drawing.Size(59, 17);
            this.rbtDrzava.TabIndex = 1;
            this.rbtDrzava.TabStop = true;
            this.rbtDrzava.Text = "Drzave";
            this.rbtDrzava.UseVisualStyleBackColor = true;
            this.rbtDrzava.CheckedChanged += new System.EventHandler(this.rbtDrzava_CheckedChanged);
            // 
            // rbtKlub
            // 
            this.rbtKlub.AutoSize = true;
            this.rbtKlub.Location = new System.Drawing.Point(3, 3);
            this.rbtKlub.Name = "rbtKlub";
            this.rbtKlub.Size = new System.Drawing.Size(52, 17);
            this.rbtKlub.TabIndex = 0;
            this.rbtKlub.TabStop = true;
            this.rbtKlub.Text = "Kluba";
            this.rbtKlub.UseVisualStyleBackColor = true;
            this.rbtKlub.CheckedChanged += new System.EventHandler(this.rbtKlub_CheckedChanged);
            // 
            // Takmicenje2Page
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtBrojRezervi);
            this.Controls.Add(this.lblBrojRezervi);
            this.Controls.Add(this.txtBrojFinalista);
            this.Controls.Add(this.lblBrojFinalista);
            this.Controls.Add(this.ckbNeogranicenBrojTak);
            this.Controls.Add(this.txtMaxTak);
            this.Controls.Add(this.lblMaxTak);
            this.Controls.Add(this.ckbPostojiTak2);
            this.Name = "Takmicenje2Page";
            this.Size = new System.Drawing.Size(501, 448);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ckbPostojiTak2;
        private System.Windows.Forms.Label lblMaxTak;
        private System.Windows.Forms.TextBox txtMaxTak;
        private System.Windows.Forms.CheckBox ckbNeogranicenBrojTak;
        private System.Windows.Forms.Label lblBrojFinalista;
        private System.Windows.Forms.TextBox txtBrojFinalista;
        private System.Windows.Forms.Label lblBrojRezervi;
        private System.Windows.Forms.TextBox txtBrojRezervi;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbtNaOsnovuTak1;
        private System.Windows.Forms.RadioButton rbtOdvojenoTak2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rbtBoljiPreskok;
        private System.Windows.Forms.RadioButton rbtPrviPreskok;
        private System.Windows.Forms.Label lblPreskokViseboj;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.RadioButton rbtDrzava;
        private System.Windows.Forms.RadioButton rbtKlub;


    }
}
