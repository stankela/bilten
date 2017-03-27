namespace Bilten.UI
{
    partial class Takmicenje2FinaleKupaPage
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
            this.rbtOdvojenoTak2 = new System.Windows.Forms.RadioButton();
            this.rbtNaOsnovuPrvogIDrugogKola = new System.Windows.Forms.RadioButton();
            this.lblMaxTak = new System.Windows.Forms.Label();
            this.txtMaxTak = new System.Windows.Forms.TextBox();
            this.ckbNeogranicenBrojTak = new System.Windows.Forms.CheckBox();
            this.lblBrojFinalista = new System.Windows.Forms.Label();
            this.txtBrojFinalista = new System.Windows.Forms.TextBox();
            this.lblBrojRezervi = new System.Windows.Forms.Label();
            this.txtBrojRezervi = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ckbNeRacunajProsek = new System.Windows.Forms.CheckBox();
            this.rbtFinalnaOcenaJeProsek = new System.Windows.Forms.RadioButton();
            this.rbtFinalnaOcenaJeZbir = new System.Windows.Forms.RadioButton();
            this.rbtFinalnaOcenaJeMax = new System.Windows.Forms.RadioButton();
            this.panel2.SuspendLayout();
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
            // rbtOdvojenoTak2
            // 
            this.rbtOdvojenoTak2.Location = new System.Drawing.Point(18, 58);
            this.rbtOdvojenoTak2.Name = "rbtOdvojenoTak2";
            this.rbtOdvojenoTak2.Size = new System.Drawing.Size(141, 17);
            this.rbtOdvojenoTak2.TabIndex = 1;
            this.rbtOdvojenoTak2.Text = "Posebno takmicenje II";
            this.rbtOdvojenoTak2.UseVisualStyleBackColor = true;
            this.rbtOdvojenoTak2.CheckedChanged += new System.EventHandler(this.rbtOdvojenoTak2_CheckedChanged);
            // 
            // rbtNaOsnovuPrvogIDrugogKola
            // 
            this.rbtNaOsnovuPrvogIDrugogKola.AutoSize = true;
            this.rbtNaOsnovuPrvogIDrugogKola.Location = new System.Drawing.Point(18, 81);
            this.rbtNaOsnovuPrvogIDrugogKola.Name = "rbtNaOsnovuPrvogIDrugogKola";
            this.rbtNaOsnovuPrvogIDrugogKola.Size = new System.Drawing.Size(172, 17);
            this.rbtNaOsnovuPrvogIDrugogKola.TabIndex = 2;
            this.rbtNaOsnovuPrvogIDrugogKola.Text = "Na osnovu rezultata 1. i 2. kola";
            this.rbtNaOsnovuPrvogIDrugogKola.UseVisualStyleBackColor = true;
            this.rbtNaOsnovuPrvogIDrugogKola.CheckedChanged += new System.EventHandler(this.rbtNaOsnovuTak1_CheckedChanged);
            // 
            // lblMaxTak
            // 
            this.lblMaxTak.AutoSize = true;
            this.lblMaxTak.Location = new System.Drawing.Point(15, 342);
            this.lblMaxTak.Name = "lblMaxTak";
            this.lblMaxTak.Size = new System.Drawing.Size(233, 13);
            this.lblMaxTak.TabIndex = 2;
            this.lblMaxTak.Text = "Maksimalan broj takmicara iz istog kluba/drzave";
            // 
            // txtMaxTak
            // 
            this.txtMaxTak.Location = new System.Drawing.Point(254, 339);
            this.txtMaxTak.Name = "txtMaxTak";
            this.txtMaxTak.Size = new System.Drawing.Size(43, 20);
            this.txtMaxTak.TabIndex = 10;
            this.txtMaxTak.TextChanged += new System.EventHandler(this.txtMaxTak_TextChanged);
            // 
            // ckbNeogranicenBrojTak
            // 
            this.ckbNeogranicenBrojTak.AutoSize = true;
            this.ckbNeogranicenBrojTak.Location = new System.Drawing.Point(303, 341);
            this.ckbNeogranicenBrojTak.Name = "ckbNeogranicenBrojTak";
            this.ckbNeogranicenBrojTak.Size = new System.Drawing.Size(195, 17);
            this.ckbNeogranicenBrojTak.TabIndex = 12;
            this.ckbNeogranicenBrojTak.Text = "Nema ogranicenja za broj takmicara";
            this.ckbNeogranicenBrojTak.UseVisualStyleBackColor = true;
            this.ckbNeogranicenBrojTak.CheckedChanged += new System.EventHandler(this.ckbNeogranicenBrojTak_CheckedChanged);
            // 
            // lblBrojFinalista
            // 
            this.lblBrojFinalista.AutoSize = true;
            this.lblBrojFinalista.Location = new System.Drawing.Point(17, 265);
            this.lblBrojFinalista.Name = "lblBrojFinalista";
            this.lblBrojFinalista.Size = new System.Drawing.Size(63, 13);
            this.lblBrojFinalista.TabIndex = 5;
            this.lblBrojFinalista.Text = "Broj finalista";
            // 
            // txtBrojFinalista
            // 
            this.txtBrojFinalista.Location = new System.Drawing.Point(86, 262);
            this.txtBrojFinalista.Name = "txtBrojFinalista";
            this.txtBrojFinalista.Size = new System.Drawing.Size(42, 20);
            this.txtBrojFinalista.TabIndex = 6;
            this.txtBrojFinalista.TextChanged += new System.EventHandler(this.txtBrojFinalista_TextChanged);
            // 
            // lblBrojRezervi
            // 
            this.lblBrojRezervi.AutoSize = true;
            this.lblBrojRezervi.Location = new System.Drawing.Point(17, 299);
            this.lblBrojRezervi.Name = "lblBrojRezervi";
            this.lblBrojRezervi.Size = new System.Drawing.Size(59, 13);
            this.lblBrojRezervi.TabIndex = 7;
            this.lblBrojRezervi.Text = "Broj rezervi";
            // 
            // txtBrojRezervi
            // 
            this.txtBrojRezervi.Location = new System.Drawing.Point(86, 296);
            this.txtBrojRezervi.Name = "txtBrojRezervi";
            this.txtBrojRezervi.Size = new System.Drawing.Size(41, 20);
            this.txtBrojRezervi.TabIndex = 8;
            this.txtBrojRezervi.TextChanged += new System.EventHandler(this.txtBrojRezervi_TextChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.ckbNeRacunajProsek);
            this.panel2.Controls.Add(this.rbtFinalnaOcenaJeProsek);
            this.panel2.Controls.Add(this.rbtFinalnaOcenaJeZbir);
            this.panel2.Controls.Add(this.rbtFinalnaOcenaJeMax);
            this.panel2.Location = new System.Drawing.Point(18, 130);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(346, 112);
            this.panel2.TabIndex = 4;
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
            this.ckbNeRacunajProsek.CheckedChanged += new System.EventHandler(this.ckbNeRacunajProsek_CheckedChanged);
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
            // Takmicenje2FinaleKupaPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.txtBrojRezervi);
            this.Controls.Add(this.lblBrojRezervi);
            this.Controls.Add(this.txtBrojFinalista);
            this.Controls.Add(this.lblBrojFinalista);
            this.Controls.Add(this.ckbNeogranicenBrojTak);
            this.Controls.Add(this.txtMaxTak);
            this.Controls.Add(this.lblMaxTak);
            this.Controls.Add(this.rbtNaOsnovuPrvogIDrugogKola);
            this.Controls.Add(this.rbtOdvojenoTak2);
            this.Controls.Add(this.ckbPostojiTak2);
            this.Name = "Takmicenje2FinaleKupaPage";
            this.Size = new System.Drawing.Size(501, 455);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ckbPostojiTak2;
        private System.Windows.Forms.RadioButton rbtOdvojenoTak2;
        private System.Windows.Forms.RadioButton rbtNaOsnovuPrvogIDrugogKola;
        private System.Windows.Forms.Label lblMaxTak;
        private System.Windows.Forms.TextBox txtMaxTak;
        private System.Windows.Forms.CheckBox ckbNeogranicenBrojTak;
        private System.Windows.Forms.Label lblBrojFinalista;
        private System.Windows.Forms.TextBox txtBrojFinalista;
        private System.Windows.Forms.Label lblBrojRezervi;
        private System.Windows.Forms.TextBox txtBrojRezervi;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox ckbNeRacunajProsek;
        private System.Windows.Forms.RadioButton rbtFinalnaOcenaJeProsek;
        private System.Windows.Forms.RadioButton rbtFinalnaOcenaJeZbir;
        private System.Windows.Forms.RadioButton rbtFinalnaOcenaJeMax;


    }
}
