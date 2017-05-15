namespace Bilten.UI
{
    partial class EkipaForm
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
            this.lblTakmicenje = new System.Windows.Forms.Label();
            this.txtRezTakmicenje = new System.Windows.Forms.TextBox();
            this.cmbKlub = new System.Windows.Forms.ComboBox();
            this.cmbDrzava = new System.Windows.Forms.ComboBox();
            this.lblNaziv = new System.Windows.Forms.Label();
            this.txtNaziv = new System.Windows.Forms.TextBox();
            this.lblKod = new System.Windows.Forms.Label();
            this.txtKod = new System.Windows.Forms.TextBox();
            this.lblClanovi = new System.Windows.Forms.Label();
            this.dgwUserControlClanovi = new Bilten.UI.DataGridViewUserControl();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnBrisi = new System.Windows.Forms.Button();
            this.rbtKlub = new System.Windows.Forms.RadioButton();
            this.rbtDrzava = new System.Windows.Forms.RadioButton();
            this.chbSkola = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(343, 401);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(424, 401);
            // 
            // lblTakmicenje
            // 
            this.lblTakmicenje.AutoSize = true;
            this.lblTakmicenje.Location = new System.Drawing.Point(15, 15);
            this.lblTakmicenje.Name = "lblTakmicenje";
            this.lblTakmicenje.Size = new System.Drawing.Size(62, 13);
            this.lblTakmicenje.TabIndex = 2;
            this.lblTakmicenje.Text = "Takmicenje";
            // 
            // txtRezTakmicenje
            // 
            this.txtRezTakmicenje.Location = new System.Drawing.Point(82, 12);
            this.txtRezTakmicenje.Name = "txtRezTakmicenje";
            this.txtRezTakmicenje.ReadOnly = true;
            this.txtRezTakmicenje.Size = new System.Drawing.Size(287, 20);
            this.txtRezTakmicenje.TabIndex = 3;
            // 
            // cmbKlub
            // 
            this.cmbKlub.FormattingEnabled = true;
            this.cmbKlub.Location = new System.Drawing.Point(142, 56);
            this.cmbKlub.Name = "cmbKlub";
            this.cmbKlub.Size = new System.Drawing.Size(220, 21);
            this.cmbKlub.TabIndex = 5;
            this.cmbKlub.SelectedIndexChanged += new System.EventHandler(this.cmbKlub_SelectedIndexChanged);
            // 
            // cmbDrzava
            // 
            this.cmbDrzava.FormattingEnabled = true;
            this.cmbDrzava.Location = new System.Drawing.Point(142, 83);
            this.cmbDrzava.Name = "cmbDrzava";
            this.cmbDrzava.Size = new System.Drawing.Size(121, 21);
            this.cmbDrzava.TabIndex = 7;
            this.cmbDrzava.SelectedIndexChanged += new System.EventHandler(this.cmbDrzava_SelectedIndexChanged);
            // 
            // lblNaziv
            // 
            this.lblNaziv.AutoSize = true;
            this.lblNaziv.Location = new System.Drawing.Point(12, 123);
            this.lblNaziv.Name = "lblNaziv";
            this.lblNaziv.Size = new System.Drawing.Size(63, 13);
            this.lblNaziv.TabIndex = 8;
            this.lblNaziv.Text = "Naziv ekipe";
            // 
            // txtNaziv
            // 
            this.txtNaziv.Location = new System.Drawing.Point(15, 140);
            this.txtNaziv.Name = "txtNaziv";
            this.txtNaziv.Size = new System.Drawing.Size(276, 20);
            this.txtNaziv.TabIndex = 9;
            // 
            // lblKod
            // 
            this.lblKod.AutoSize = true;
            this.lblKod.Location = new System.Drawing.Point(306, 123);
            this.lblKod.Name = "lblKod";
            this.lblKod.Size = new System.Drawing.Size(70, 13);
            this.lblKod.TabIndex = 10;
            this.lblKod.Text = "Skraceni kod";
            // 
            // txtKod
            // 
            this.txtKod.Location = new System.Drawing.Point(309, 140);
            this.txtKod.Name = "txtKod";
            this.txtKod.Size = new System.Drawing.Size(60, 20);
            this.txtKod.TabIndex = 11;
            // 
            // lblClanovi
            // 
            this.lblClanovi.AutoSize = true;
            this.lblClanovi.Location = new System.Drawing.Point(12, 183);
            this.lblClanovi.Name = "lblClanovi";
            this.lblClanovi.Size = new System.Drawing.Size(71, 13);
            this.lblClanovi.TabIndex = 12;
            this.lblClanovi.Text = "Clanovi ekipe";
            // 
            // dgwUserControlClanovi
            // 
            this.dgwUserControlClanovi.ColumnHeaderSorting = true;
            this.dgwUserControlClanovi.Location = new System.Drawing.Point(12, 199);
            this.dgwUserControlClanovi.Name = "dgwUserControlClanovi";
            this.dgwUserControlClanovi.Size = new System.Drawing.Size(486, 189);
            this.dgwUserControlClanovi.TabIndex = 13;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(11, 401);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 14;
            this.btnAdd.Text = "Dodaj clana";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnBrisi
            // 
            this.btnBrisi.Location = new System.Drawing.Point(92, 401);
            this.btnBrisi.Name = "btnBrisi";
            this.btnBrisi.Size = new System.Drawing.Size(75, 23);
            this.btnBrisi.TabIndex = 16;
            this.btnBrisi.Text = "Brisi clana";
            this.btnBrisi.UseVisualStyleBackColor = true;
            this.btnBrisi.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // rbtKlub
            // 
            this.rbtKlub.AutoSize = true;
            this.rbtKlub.Location = new System.Drawing.Point(18, 56);
            this.rbtKlub.Name = "rbtKlub";
            this.rbtKlub.Size = new System.Drawing.Size(92, 17);
            this.rbtKlub.TabIndex = 17;
            this.rbtKlub.TabStop = true;
            this.rbtKlub.Text = "Klubska ekipa";
            this.rbtKlub.UseVisualStyleBackColor = true;
            this.rbtKlub.CheckedChanged += new System.EventHandler(this.rbtKlub_CheckedChanged);
            // 
            // rbtDrzava
            // 
            this.rbtDrzava.AutoSize = true;
            this.rbtDrzava.Location = new System.Drawing.Point(18, 84);
            this.rbtDrzava.Name = "rbtDrzava";
            this.rbtDrzava.Size = new System.Drawing.Size(94, 17);
            this.rbtDrzava.TabIndex = 18;
            this.rbtDrzava.TabStop = true;
            this.rbtDrzava.Text = "Drzavna ekipa";
            this.rbtDrzava.UseVisualStyleBackColor = true;
            this.rbtDrzava.CheckedChanged += new System.EventHandler(this.rbtDrzava_CheckedChanged);
            // 
            // chbSkola
            // 
            this.chbSkola.AutoSize = true;
            this.chbSkola.Location = new System.Drawing.Point(380, 56);
            this.chbSkola.Name = "chbSkola";
            this.chbSkola.Size = new System.Drawing.Size(85, 17);
            this.chbSkola.TabIndex = 19;
            this.chbSkola.Text = "Prikazi skole";
            this.chbSkola.UseVisualStyleBackColor = true;
            // 
            // EkipaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 436);
            this.Controls.Add(this.dgwUserControlClanovi);
            this.Controls.Add(this.chbSkola);
            this.Controls.Add(this.btnBrisi);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lblClanovi);
            this.Controls.Add(this.rbtKlub);
            this.Controls.Add(this.cmbKlub);
            this.Controls.Add(this.txtNaziv);
            this.Controls.Add(this.rbtDrzava);
            this.Controls.Add(this.cmbDrzava);
            this.Controls.Add(this.txtKod);
            this.Controls.Add(this.lblTakmicenje);
            this.Controls.Add(this.lblNaziv);
            this.Controls.Add(this.txtRezTakmicenje);
            this.Controls.Add(this.lblKod);
            this.Name = "EkipaForm";
            this.Text = "EkipaForm";
            this.Shown += new System.EventHandler(this.EkipaForm_Shown);
            this.Controls.SetChildIndex(this.lblKod, 0);
            this.Controls.SetChildIndex(this.txtRezTakmicenje, 0);
            this.Controls.SetChildIndex(this.lblNaziv, 0);
            this.Controls.SetChildIndex(this.lblTakmicenje, 0);
            this.Controls.SetChildIndex(this.txtKod, 0);
            this.Controls.SetChildIndex(this.cmbDrzava, 0);
            this.Controls.SetChildIndex(this.rbtDrzava, 0);
            this.Controls.SetChildIndex(this.txtNaziv, 0);
            this.Controls.SetChildIndex(this.cmbKlub, 0);
            this.Controls.SetChildIndex(this.rbtKlub, 0);
            this.Controls.SetChildIndex(this.lblClanovi, 0);
            this.Controls.SetChildIndex(this.btnAdd, 0);
            this.Controls.SetChildIndex(this.btnBrisi, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.chbSkola, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.dgwUserControlClanovi, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTakmicenje;
        private System.Windows.Forms.TextBox txtRezTakmicenje;
        private System.Windows.Forms.ComboBox cmbKlub;
        private System.Windows.Forms.ComboBox cmbDrzava;
        private System.Windows.Forms.Label lblNaziv;
        private System.Windows.Forms.TextBox txtNaziv;
        private System.Windows.Forms.Label lblKod;
        private System.Windows.Forms.TextBox txtKod;
        private System.Windows.Forms.Label lblClanovi;
        private DataGridViewUserControl dgwUserControlClanovi;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnBrisi;
        private System.Windows.Forms.RadioButton rbtKlub;
        private System.Windows.Forms.RadioButton rbtDrzava;
        private System.Windows.Forms.CheckBox chbSkola;
    }
}