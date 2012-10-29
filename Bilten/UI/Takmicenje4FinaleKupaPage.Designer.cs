namespace Bilten.UI
{
    partial class Takmicenje4FinaleKupaPage
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
            this.ckbPostojiTak4 = new System.Windows.Forms.CheckBox();
            this.lblBrojRezultata = new System.Windows.Forms.Label();
            this.txtBrojRezultata = new System.Windows.Forms.TextBox();
            this.lblBrojEkipa = new System.Windows.Forms.Label();
            this.txtBrojEkipa = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbtNaOsnovuPrvogIDrugogKola = new System.Windows.Forms.RadioButton();
            this.rbtOdvojenoTak4 = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbtJednoTak4ZaSveKategorije = new System.Windows.Forms.RadioButton();
            this.rbtPostojiTak4ZaSvakuKategoriju = new System.Windows.Forms.RadioButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.ckbNeRacunajProsek = new System.Windows.Forms.CheckBox();
            this.rbtFinalnaOcenaJeProsek = new System.Windows.Forms.RadioButton();
            this.rbtFinalnaOcenaJeZbir = new System.Windows.Forms.RadioButton();
            this.rbtFinalnaOcenaJeMax = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // ckbPostojiTak4
            // 
            this.ckbPostojiTak4.AutoSize = true;
            this.ckbPostojiTak4.Location = new System.Drawing.Point(18, 16);
            this.ckbPostojiTak4.Name = "ckbPostojiTak4";
            this.ckbPostojiTak4.Size = new System.Drawing.Size(124, 17);
            this.ckbPostojiTak4.TabIndex = 2;
            this.ckbPostojiTak4.Text = "Postoji takmicenje IV";
            this.ckbPostojiTak4.UseVisualStyleBackColor = true;
            this.ckbPostojiTak4.CheckedChanged += new System.EventHandler(this.ckbPostojiTak4_CheckedChanged);
            // 
            // lblBrojRezultata
            // 
            this.lblBrojRezultata.AutoSize = true;
            this.lblBrojRezultata.Location = new System.Drawing.Point(22, 335);
            this.lblBrojRezultata.Name = "lblBrojRezultata";
            this.lblBrojRezultata.Size = new System.Drawing.Size(188, 13);
            this.lblBrojRezultata.TabIndex = 7;
            this.lblBrojRezultata.Text = "Broj rezultata koji se vrednuju za ekipu";
            // 
            // txtBrojRezultata
            // 
            this.txtBrojRezultata.Location = new System.Drawing.Point(216, 332);
            this.txtBrojRezultata.Name = "txtBrojRezultata";
            this.txtBrojRezultata.Size = new System.Drawing.Size(34, 20);
            this.txtBrojRezultata.TabIndex = 8;
            this.txtBrojRezultata.TextChanged += new System.EventHandler(this.txtBrojRezultata_TextChanged);
            // 
            // lblBrojEkipa
            // 
            this.lblBrojEkipa.AutoSize = true;
            this.lblBrojEkipa.Location = new System.Drawing.Point(22, 370);
            this.lblBrojEkipa.Name = "lblBrojEkipa";
            this.lblBrojEkipa.Size = new System.Drawing.Size(91, 13);
            this.lblBrojEkipa.TabIndex = 9;
            this.lblBrojEkipa.Text = "Broj ekipa u finalu";
            // 
            // txtBrojEkipa
            // 
            this.txtBrojEkipa.Location = new System.Drawing.Point(119, 367);
            this.txtBrojEkipa.Name = "txtBrojEkipa";
            this.txtBrojEkipa.Size = new System.Drawing.Size(32, 20);
            this.txtBrojEkipa.TabIndex = 10;
            this.txtBrojEkipa.TextChanged += new System.EventHandler(this.txtBrojEkipa_TextChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbtNaOsnovuPrvogIDrugogKola);
            this.panel1.Controls.Add(this.rbtOdvojenoTak4);
            this.panel1.Location = new System.Drawing.Point(23, 50);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 51);
            this.panel1.TabIndex = 13;
            // 
            // rbtNaOsnovuPrvogIDrugogKola
            // 
            this.rbtNaOsnovuPrvogIDrugogKola.AutoSize = true;
            this.rbtNaOsnovuPrvogIDrugogKola.Location = new System.Drawing.Point(3, 26);
            this.rbtNaOsnovuPrvogIDrugogKola.Name = "rbtNaOsnovuPrvogIDrugogKola";
            this.rbtNaOsnovuPrvogIDrugogKola.Size = new System.Drawing.Size(172, 17);
            this.rbtNaOsnovuPrvogIDrugogKola.TabIndex = 6;
            this.rbtNaOsnovuPrvogIDrugogKola.TabStop = true;
            this.rbtNaOsnovuPrvogIDrugogKola.Text = "Na osnovu rezultata 1. i 2. kola";
            this.rbtNaOsnovuPrvogIDrugogKola.UseVisualStyleBackColor = true;
            this.rbtNaOsnovuPrvogIDrugogKola.CheckedChanged += new System.EventHandler(this.rbtNaOsnovuTak1_CheckedChanged);
            // 
            // rbtOdvojenoTak4
            // 
            this.rbtOdvojenoTak4.Location = new System.Drawing.Point(3, 3);
            this.rbtOdvojenoTak4.Name = "rbtOdvojenoTak4";
            this.rbtOdvojenoTak4.Size = new System.Drawing.Size(141, 17);
            this.rbtOdvojenoTak4.TabIndex = 5;
            this.rbtOdvojenoTak4.TabStop = true;
            this.rbtOdvojenoTak4.Text = "Posebno takmicenje IV";
            this.rbtOdvojenoTak4.UseVisualStyleBackColor = true;
            this.rbtOdvojenoTak4.CheckedChanged += new System.EventHandler(this.rbtOdvojenoTak4_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbtJednoTak4ZaSveKategorije);
            this.panel2.Controls.Add(this.rbtPostojiTak4ZaSvakuKategoriju);
            this.panel2.Location = new System.Drawing.Point(23, 122);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(239, 52);
            this.panel2.TabIndex = 14;
            // 
            // rbtJednoTak4ZaSveKategorije
            // 
            this.rbtJednoTak4ZaSveKategorije.AutoSize = true;
            this.rbtJednoTak4ZaSveKategorije.Location = new System.Drawing.Point(3, 31);
            this.rbtJednoTak4ZaSveKategorije.Name = "rbtJednoTak4ZaSveKategorije";
            this.rbtJednoTak4ZaSveKategorije.Size = new System.Drawing.Size(204, 17);
            this.rbtJednoTak4ZaSveKategorije.TabIndex = 14;
            this.rbtJednoTak4ZaSveKategorije.TabStop = true;
            this.rbtJednoTak4ZaSveKategorije.Text = "Jedno takmicenje IV za sve kategorije";
            this.rbtJednoTak4ZaSveKategorije.UseVisualStyleBackColor = true;
            this.rbtJednoTak4ZaSveKategorije.CheckedChanged += new System.EventHandler(this.rbtJednoTak4ZaSveKategorije_CheckedChanged);
            // 
            // rbtPostojiTak4ZaSvakuKategoriju
            // 
            this.rbtPostojiTak4ZaSvakuKategoriju.AutoSize = true;
            this.rbtPostojiTak4ZaSvakuKategoriju.Location = new System.Drawing.Point(3, 8);
            this.rbtPostojiTak4ZaSvakuKategoriju.Name = "rbtPostojiTak4ZaSvakuKategoriju";
            this.rbtPostojiTak4ZaSvakuKategoriju.Size = new System.Drawing.Size(229, 17);
            this.rbtPostojiTak4ZaSvakuKategoriju.TabIndex = 13;
            this.rbtPostojiTak4ZaSvakuKategoriju.TabStop = true;
            this.rbtPostojiTak4ZaSvakuKategoriju.Text = "Posebno takmicenje IV za svaku kategoriju";
            this.rbtPostojiTak4ZaSvakuKategoriju.UseVisualStyleBackColor = true;
            this.rbtPostojiTak4ZaSvakuKategoriju.CheckedChanged += new System.EventHandler(this.rbtPostojiTak4ZaSvakuKategoriju_CheckedChanged);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.ckbNeRacunajProsek);
            this.panel3.Controls.Add(this.rbtFinalnaOcenaJeProsek);
            this.panel3.Controls.Add(this.rbtFinalnaOcenaJeZbir);
            this.panel3.Controls.Add(this.rbtFinalnaOcenaJeMax);
            this.panel3.Location = new System.Drawing.Point(23, 195);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(346, 112);
            this.panel3.TabIndex = 20;
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
            // Takmicenje4FinaleKupaPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtBrojEkipa);
            this.Controls.Add(this.lblBrojEkipa);
            this.Controls.Add(this.txtBrojRezultata);
            this.Controls.Add(this.lblBrojRezultata);
            this.Controls.Add(this.ckbPostojiTak4);
            this.Name = "Takmicenje4FinaleKupaPage";
            this.Size = new System.Drawing.Size(436, 425);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ckbPostojiTak4;
        private System.Windows.Forms.Label lblBrojRezultata;
        private System.Windows.Forms.TextBox txtBrojRezultata;
        private System.Windows.Forms.Label lblBrojEkipa;
        private System.Windows.Forms.TextBox txtBrojEkipa;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbtNaOsnovuPrvogIDrugogKola;
        private System.Windows.Forms.RadioButton rbtOdvojenoTak4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rbtJednoTak4ZaSveKategorije;
        private System.Windows.Forms.RadioButton rbtPostojiTak4ZaSvakuKategoriju;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox ckbNeRacunajProsek;
        private System.Windows.Forms.RadioButton rbtFinalnaOcenaJeProsek;
        private System.Windows.Forms.RadioButton rbtFinalnaOcenaJeZbir;
        private System.Windows.Forms.RadioButton rbtFinalnaOcenaJeMax;

    }
}
