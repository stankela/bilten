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
            this.rbtOdvojenoTak2 = new System.Windows.Forms.RadioButton();
            this.rbtNaOsnovuTak1 = new System.Windows.Forms.RadioButton();
            this.lblMaxTak = new System.Windows.Forms.Label();
            this.txtMaxTak = new System.Windows.Forms.TextBox();
            this.ckbNeogranicenBrojTak = new System.Windows.Forms.CheckBox();
            this.lblBrojFinalista = new System.Windows.Forms.Label();
            this.txtBrojFinalista = new System.Windows.Forms.TextBox();
            this.lblBrojRezervi = new System.Windows.Forms.Label();
            this.txtBrojRezervi = new System.Windows.Forms.TextBox();
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
            // rbtNaOsnovuTak1
            // 
            this.rbtNaOsnovuTak1.AutoSize = true;
            this.rbtNaOsnovuTak1.Location = new System.Drawing.Point(18, 81);
            this.rbtNaOsnovuTak1.Name = "rbtNaOsnovuTak1";
            this.rbtNaOsnovuTak1.Size = new System.Drawing.Size(190, 17);
            this.rbtNaOsnovuTak1.TabIndex = 2;
            this.rbtNaOsnovuTak1.Text = "Na osnovu rezultata iz takmicenja I";
            this.rbtNaOsnovuTak1.UseVisualStyleBackColor = true;
            this.rbtNaOsnovuTak1.CheckedChanged += new System.EventHandler(this.rbtNaOsnovuTak1_CheckedChanged);
            // 
            // lblMaxTak
            // 
            this.lblMaxTak.AutoSize = true;
            this.lblMaxTak.Location = new System.Drawing.Point(15, 199);
            this.lblMaxTak.Name = "lblMaxTak";
            this.lblMaxTak.Size = new System.Drawing.Size(233, 13);
            this.lblMaxTak.TabIndex = 2;
            this.lblMaxTak.Text = "Maksimalan broj takmicara iz istog kluba/drzave";
            // 
            // txtMaxTak
            // 
            this.txtMaxTak.Location = new System.Drawing.Point(254, 196);
            this.txtMaxTak.Name = "txtMaxTak";
            this.txtMaxTak.Size = new System.Drawing.Size(43, 20);
            this.txtMaxTak.TabIndex = 9;
            this.txtMaxTak.TextChanged += new System.EventHandler(this.txtMaxTak_TextChanged);
            // 
            // ckbNeogranicenBrojTak
            // 
            this.ckbNeogranicenBrojTak.AutoSize = true;
            this.ckbNeogranicenBrojTak.Location = new System.Drawing.Point(303, 198);
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
            this.lblBrojFinalista.Location = new System.Drawing.Point(15, 124);
            this.lblBrojFinalista.Name = "lblBrojFinalista";
            this.lblBrojFinalista.Size = new System.Drawing.Size(63, 13);
            this.lblBrojFinalista.TabIndex = 5;
            this.lblBrojFinalista.Text = "Broj finalista";
            // 
            // txtBrojFinalista
            // 
            this.txtBrojFinalista.Location = new System.Drawing.Point(84, 121);
            this.txtBrojFinalista.Name = "txtBrojFinalista";
            this.txtBrojFinalista.Size = new System.Drawing.Size(42, 20);
            this.txtBrojFinalista.TabIndex = 6;
            this.txtBrojFinalista.TextChanged += new System.EventHandler(this.txtBrojFinalista_TextChanged);
            // 
            // lblBrojRezervi
            // 
            this.lblBrojRezervi.AutoSize = true;
            this.lblBrojRezervi.Location = new System.Drawing.Point(15, 158);
            this.lblBrojRezervi.Name = "lblBrojRezervi";
            this.lblBrojRezervi.Size = new System.Drawing.Size(59, 13);
            this.lblBrojRezervi.TabIndex = 7;
            this.lblBrojRezervi.Text = "Broj rezervi";
            // 
            // txtBrojRezervi
            // 
            this.txtBrojRezervi.Location = new System.Drawing.Point(84, 155);
            this.txtBrojRezervi.Name = "txtBrojRezervi";
            this.txtBrojRezervi.Size = new System.Drawing.Size(41, 20);
            this.txtBrojRezervi.TabIndex = 8;
            this.txtBrojRezervi.TextChanged += new System.EventHandler(this.txtBrojRezervi_TextChanged);
            // 
            // Takmicenje2Page
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.txtBrojRezervi);
            this.Controls.Add(this.lblBrojRezervi);
            this.Controls.Add(this.txtBrojFinalista);
            this.Controls.Add(this.lblBrojFinalista);
            this.Controls.Add(this.ckbNeogranicenBrojTak);
            this.Controls.Add(this.txtMaxTak);
            this.Controls.Add(this.lblMaxTak);
            this.Controls.Add(this.rbtNaOsnovuTak1);
            this.Controls.Add(this.rbtOdvojenoTak2);
            this.Controls.Add(this.ckbPostojiTak2);
            this.Name = "Takmicenje2Page";
            this.Size = new System.Drawing.Size(501, 230);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ckbPostojiTak2;
        private System.Windows.Forms.RadioButton rbtOdvojenoTak2;
        private System.Windows.Forms.RadioButton rbtNaOsnovuTak1;
        private System.Windows.Forms.Label lblMaxTak;
        private System.Windows.Forms.TextBox txtMaxTak;
        private System.Windows.Forms.CheckBox ckbNeogranicenBrojTak;
        private System.Windows.Forms.Label lblBrojFinalista;
        private System.Windows.Forms.TextBox txtBrojFinalista;
        private System.Windows.Forms.Label lblBrojRezervi;
        private System.Windows.Forms.TextBox txtBrojRezervi;


    }
}
