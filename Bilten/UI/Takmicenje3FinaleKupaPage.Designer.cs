namespace Bilten.UI
{
    partial class Takmicenje3FinaleKupaPage
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
            this.ckbPostojiTak3 = new System.Windows.Forms.CheckBox();
            this.lblMaxTak = new System.Windows.Forms.Label();
            this.txtMaxTak = new System.Windows.Forms.TextBox();
            this.ckbNeogranicenBrojTak = new System.Windows.Forms.CheckBox();
            this.lblBrojFinalista = new System.Windows.Forms.Label();
            this.txtBrojFinalista = new System.Windows.Forms.TextBox();
            this.lblBrojRezervi = new System.Windows.Forms.Label();
            this.txtBrojRezervi = new System.Windows.Forms.TextBox();
            this.lblPoredakPreskok = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbtNaOsnovuPrvogIDrugogKola = new System.Windows.Forms.RadioButton();
            this.rbtOdvojenoTak3 = new System.Windows.Forms.RadioButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rbtPoredakPreskok2 = new System.Windows.Forms.RadioButton();
            this.rbtPoredakPreskok1 = new System.Windows.Forms.RadioButton();
            this.panel4 = new System.Windows.Forms.Panel();
            this.rbtDrzava = new System.Windows.Forms.RadioButton();
            this.rbtKlub = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ckbNeRacunajProsek = new System.Windows.Forms.CheckBox();
            this.rbtFinalnaOcenaJeProsek = new System.Windows.Forms.RadioButton();
            this.rbtFinalnaOcenaJeZbir = new System.Windows.Forms.RadioButton();
            this.rbtFinalnaOcenaJeMax = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ckbPostojiTak3
            // 
            this.ckbPostojiTak3.AutoSize = true;
            this.ckbPostojiTak3.Location = new System.Drawing.Point(18, 16);
            this.ckbPostojiTak3.Name = "ckbPostojiTak3";
            this.ckbPostojiTak3.Size = new System.Drawing.Size(123, 17);
            this.ckbPostojiTak3.TabIndex = 0;
            this.ckbPostojiTak3.Text = "Postoji takmicenje III";
            this.ckbPostojiTak3.UseVisualStyleBackColor = true;
            this.ckbPostojiTak3.CheckedChanged += new System.EventHandler(this.ckbPostojiTak3_CheckedChanged);
            // 
            // lblMaxTak
            // 
            this.lblMaxTak.AutoSize = true;
            this.lblMaxTak.Location = new System.Drawing.Point(15, 268);
            this.lblMaxTak.Name = "lblMaxTak";
            this.lblMaxTak.Size = new System.Drawing.Size(167, 13);
            this.lblMaxTak.TabIndex = 2;
            this.lblMaxTak.Text = "Maksimalan broj takmicara iz istog";
            // 
            // txtMaxTak
            // 
            this.txtMaxTak.Location = new System.Drawing.Point(258, 266);
            this.txtMaxTak.Name = "txtMaxTak";
            this.txtMaxTak.Size = new System.Drawing.Size(43, 20);
            this.txtMaxTak.TabIndex = 3;
            this.txtMaxTak.TextChanged += new System.EventHandler(this.txtMaxTak_TextChanged);
            // 
            // ckbNeogranicenBrojTak
            // 
            this.ckbNeogranicenBrojTak.AutoSize = true;
            this.ckbNeogranicenBrojTak.Location = new System.Drawing.Point(307, 269);
            this.ckbNeogranicenBrojTak.Name = "ckbNeogranicenBrojTak";
            this.ckbNeogranicenBrojTak.Size = new System.Drawing.Size(195, 17);
            this.ckbNeogranicenBrojTak.TabIndex = 4;
            this.ckbNeogranicenBrojTak.Text = "Nema ogranicenja za broj takmicara";
            this.ckbNeogranicenBrojTak.UseVisualStyleBackColor = true;
            this.ckbNeogranicenBrojTak.CheckedChanged += new System.EventHandler(this.ckbNeogranicenBrojTak_CheckedChanged);
            // 
            // lblBrojFinalista
            // 
            this.lblBrojFinalista.AutoSize = true;
            this.lblBrojFinalista.Location = new System.Drawing.Point(15, 319);
            this.lblBrojFinalista.Name = "lblBrojFinalista";
            this.lblBrojFinalista.Size = new System.Drawing.Size(63, 13);
            this.lblBrojFinalista.TabIndex = 5;
            this.lblBrojFinalista.Text = "Broj finalista";
            // 
            // txtBrojFinalista
            // 
            this.txtBrojFinalista.Location = new System.Drawing.Point(84, 316);
            this.txtBrojFinalista.Name = "txtBrojFinalista";
            this.txtBrojFinalista.Size = new System.Drawing.Size(42, 20);
            this.txtBrojFinalista.TabIndex = 6;
            this.txtBrojFinalista.TextChanged += new System.EventHandler(this.txtBrojFinalista_TextChanged);
            // 
            // lblBrojRezervi
            // 
            this.lblBrojRezervi.AutoSize = true;
            this.lblBrojRezervi.Location = new System.Drawing.Point(15, 345);
            this.lblBrojRezervi.Name = "lblBrojRezervi";
            this.lblBrojRezervi.Size = new System.Drawing.Size(59, 13);
            this.lblBrojRezervi.TabIndex = 7;
            this.lblBrojRezervi.Text = "Broj rezervi";
            // 
            // txtBrojRezervi
            // 
            this.txtBrojRezervi.Location = new System.Drawing.Point(84, 342);
            this.txtBrojRezervi.Name = "txtBrojRezervi";
            this.txtBrojRezervi.Size = new System.Drawing.Size(41, 20);
            this.txtBrojRezervi.TabIndex = 8;
            this.txtBrojRezervi.TextChanged += new System.EventHandler(this.txtBrojRezervi_TextChanged);
            // 
            // lblPoredakPreskok
            // 
            this.lblPoredakPreskok.AutoSize = true;
            this.lblPoredakPreskok.Location = new System.Drawing.Point(15, 390);
            this.lblPoredakPreskok.Name = "lblPoredakPreskok";
            this.lblPoredakPreskok.Size = new System.Drawing.Size(234, 13);
            this.lblPoredakPreskok.TabIndex = 12;
            this.lblPoredakPreskok.Text = "Poredak u finalu preskoka se racuna na osnovu";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbtNaOsnovuPrvogIDrugogKola);
            this.panel1.Controls.Add(this.rbtOdvojenoTak3);
            this.panel1.Location = new System.Drawing.Point(28, 48);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(220, 50);
            this.panel1.TabIndex = 15;
            // 
            // rbtNaOsnovuPrvogIDrugogKola
            // 
            this.rbtNaOsnovuPrvogIDrugogKola.AutoSize = true;
            this.rbtNaOsnovuPrvogIDrugogKola.Location = new System.Drawing.Point(3, 26);
            this.rbtNaOsnovuPrvogIDrugogKola.Name = "rbtNaOsnovuPrvogIDrugogKola";
            this.rbtNaOsnovuPrvogIDrugogKola.Size = new System.Drawing.Size(172, 17);
            this.rbtNaOsnovuPrvogIDrugogKola.TabIndex = 3;
            this.rbtNaOsnovuPrvogIDrugogKola.Text = "Na osnovu rezultata 1. i 2. kola";
            this.rbtNaOsnovuPrvogIDrugogKola.UseVisualStyleBackColor = true;
            this.rbtNaOsnovuPrvogIDrugogKola.CheckedChanged += new System.EventHandler(this.rbtNaOsnovuTak1_CheckedChanged);
            // 
            // rbtOdvojenoTak3
            // 
            this.rbtOdvojenoTak3.Location = new System.Drawing.Point(3, 3);
            this.rbtOdvojenoTak3.Name = "rbtOdvojenoTak3";
            this.rbtOdvojenoTak3.Size = new System.Drawing.Size(141, 17);
            this.rbtOdvojenoTak3.TabIndex = 2;
            this.rbtOdvojenoTak3.Text = "Posebno takmicenje III";
            this.rbtOdvojenoTak3.UseVisualStyleBackColor = true;
            this.rbtOdvojenoTak3.CheckedChanged += new System.EventHandler(this.rbtOdvojenoTak3_CheckedChanged);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.rbtPoredakPreskok2);
            this.panel3.Controls.Add(this.rbtPoredakPreskok1);
            this.panel3.Location = new System.Drawing.Point(28, 406);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(117, 49);
            this.panel3.TabIndex = 17;
            // 
            // rbtPoredakPreskok2
            // 
            this.rbtPoredakPreskok2.AutoSize = true;
            this.rbtPoredakPreskok2.Location = new System.Drawing.Point(3, 27);
            this.rbtPoredakPreskok2.Name = "rbtPoredakPreskok2";
            this.rbtPoredakPreskok2.Size = new System.Drawing.Size(92, 17);
            this.rbtPoredakPreskok2.TabIndex = 16;
            this.rbtPoredakPreskok2.TabStop = true;
            this.rbtPoredakPreskok2.Text = "Oba preskoka";
            this.rbtPoredakPreskok2.UseVisualStyleBackColor = true;
            this.rbtPoredakPreskok2.CheckedChanged += new System.EventHandler(this.rbtPoredakPreskok_CheckedChanged);
            // 
            // rbtPoredakPreskok1
            // 
            this.rbtPoredakPreskok1.AutoSize = true;
            this.rbtPoredakPreskok1.Location = new System.Drawing.Point(3, 4);
            this.rbtPoredakPreskok1.Name = "rbtPoredakPreskok1";
            this.rbtPoredakPreskok1.Size = new System.Drawing.Size(100, 17);
            this.rbtPoredakPreskok1.TabIndex = 15;
            this.rbtPoredakPreskok1.TabStop = true;
            this.rbtPoredakPreskok1.Text = "Prvog preskoka";
            this.rbtPoredakPreskok1.UseVisualStyleBackColor = true;
            this.rbtPoredakPreskok1.CheckedChanged += new System.EventHandler(this.rbtPoredakPreskok_CheckedChanged);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.rbtDrzava);
            this.panel4.Controls.Add(this.rbtKlub);
            this.panel4.Location = new System.Drawing.Point(188, 255);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(66, 42);
            this.panel4.TabIndex = 18;
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
            // panel2
            // 
            this.panel2.Controls.Add(this.ckbNeRacunajProsek);
            this.panel2.Controls.Add(this.rbtFinalnaOcenaJeProsek);
            this.panel2.Controls.Add(this.rbtFinalnaOcenaJeZbir);
            this.panel2.Controls.Add(this.rbtFinalnaOcenaJeMax);
            this.panel2.Location = new System.Drawing.Point(28, 121);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(346, 112);
            this.panel2.TabIndex = 19;
            // 
            // ckbNeRacunajProsek
            // 
            this.ckbNeRacunajProsek.AutoSize = true;
            this.ckbNeRacunajProsek.Location = new System.Drawing.Point(37, 76);
            this.ckbNeRacunajProsek.Name = "ckbNeRacunajProsek";
            this.ckbNeRacunajProsek.Size = new System.Drawing.Size(279, 17);
            this.ckbNeRacunajProsek.TabIndex = 3;
            this.ckbNeRacunajProsek.Text = "Ne racunaj prosek ako ne postoje rezultati iz oba kola";
            this.ckbNeRacunajProsek.UseVisualStyleBackColor = true;
            // 
            // rbtFinalnaOcenaJeProsek
            // 
            this.rbtFinalnaOcenaJeProsek.AutoSize = true;
            this.rbtFinalnaOcenaJeProsek.Location = new System.Drawing.Point(5, 53);
            this.rbtFinalnaOcenaJeProsek.Name = "rbtFinalnaOcenaJeProsek";
            this.rbtFinalnaOcenaJeProsek.Size = new System.Drawing.Size(190, 17);
            this.rbtFinalnaOcenaJeProsek.TabIndex = 2;
            this.rbtFinalnaOcenaJeProsek.TabStop = true;
            this.rbtFinalnaOcenaJeProsek.Text = "Finalna ocena je prosek 1. i 2. kola";
            this.rbtFinalnaOcenaJeProsek.UseVisualStyleBackColor = true;
            this.rbtFinalnaOcenaJeProsek.CheckedChanged += new System.EventHandler(this.rbtFinalnaOcenaJeProsek_CheckedChanged);
            // 
            // rbtFinalnaOcenaJeZbir
            // 
            this.rbtFinalnaOcenaJeZbir.AutoSize = true;
            this.rbtFinalnaOcenaJeZbir.Location = new System.Drawing.Point(5, 30);
            this.rbtFinalnaOcenaJeZbir.Name = "rbtFinalnaOcenaJeZbir";
            this.rbtFinalnaOcenaJeZbir.Size = new System.Drawing.Size(174, 17);
            this.rbtFinalnaOcenaJeZbir.TabIndex = 1;
            this.rbtFinalnaOcenaJeZbir.TabStop = true;
            this.rbtFinalnaOcenaJeZbir.Text = "Finalna ocena je zbir 1. i 2. kola";
            this.rbtFinalnaOcenaJeZbir.UseVisualStyleBackColor = true;
            this.rbtFinalnaOcenaJeZbir.CheckedChanged += new System.EventHandler(this.rbtFinalnaOcenaJeZbir_CheckedChanged);
            // 
            // rbtFinalnaOcenaJeMax
            // 
            this.rbtFinalnaOcenaJeMax.AutoSize = true;
            this.rbtFinalnaOcenaJeMax.Location = new System.Drawing.Point(5, 7);
            this.rbtFinalnaOcenaJeMax.Name = "rbtFinalnaOcenaJeMax";
            this.rbtFinalnaOcenaJeMax.Size = new System.Drawing.Size(223, 17);
            this.rbtFinalnaOcenaJeMax.TabIndex = 0;
            this.rbtFinalnaOcenaJeMax.TabStop = true;
            this.rbtFinalnaOcenaJeMax.Text = "Finalna ocena je bolja ocena iz 1. i 2. kola";
            this.rbtFinalnaOcenaJeMax.UseVisualStyleBackColor = true;
            this.rbtFinalnaOcenaJeMax.CheckedChanged += new System.EventHandler(this.rbtFinalnaOcenaJeMax_CheckedChanged);
            // 
            // Takmicenje3FinaleKupaPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblPoredakPreskok);
            this.Controls.Add(this.txtBrojRezervi);
            this.Controls.Add(this.lblBrojRezervi);
            this.Controls.Add(this.txtBrojFinalista);
            this.Controls.Add(this.lblBrojFinalista);
            this.Controls.Add(this.ckbNeogranicenBrojTak);
            this.Controls.Add(this.txtMaxTak);
            this.Controls.Add(this.lblMaxTak);
            this.Controls.Add(this.ckbPostojiTak3);
            this.Name = "Takmicenje3FinaleKupaPage";
            this.Size = new System.Drawing.Size(527, 466);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ckbPostojiTak3;
        private System.Windows.Forms.Label lblMaxTak;
        private System.Windows.Forms.TextBox txtMaxTak;
        private System.Windows.Forms.CheckBox ckbNeogranicenBrojTak;
        private System.Windows.Forms.Label lblBrojFinalista;
        private System.Windows.Forms.TextBox txtBrojFinalista;
        private System.Windows.Forms.Label lblBrojRezervi;
        private System.Windows.Forms.TextBox txtBrojRezervi;
        private System.Windows.Forms.Label lblPoredakPreskok;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbtNaOsnovuPrvogIDrugogKola;
        private System.Windows.Forms.RadioButton rbtOdvojenoTak3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton rbtPoredakPreskok2;
        private System.Windows.Forms.RadioButton rbtPoredakPreskok1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.RadioButton rbtDrzava;
        private System.Windows.Forms.RadioButton rbtKlub;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rbtFinalnaOcenaJeProsek;
        private System.Windows.Forms.RadioButton rbtFinalnaOcenaJeZbir;
        private System.Windows.Forms.RadioButton rbtFinalnaOcenaJeMax;
        private System.Windows.Forms.CheckBox ckbNeRacunajProsek;


    }
}
