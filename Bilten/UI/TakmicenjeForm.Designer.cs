namespace Bilten.UI
{
    partial class TakmicenjeForm
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
            this.lblDatum = new System.Windows.Forms.Label();
            this.txtDatum = new System.Windows.Forms.TextBox();
            this.lblMesto = new System.Windows.Forms.Label();
            this.txtMesto = new System.Windows.Forms.TextBox();
            this.lblGimnastika = new System.Windows.Forms.Label();
            this.cmbGimnastika = new System.Windows.Forms.ComboBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnIzaberiPrvaDvaKola = new System.Windows.Forms.Button();
            this.lblTipTakmicenja = new System.Windows.Forms.Label();
            this.cmbTipTakmicenja = new System.Windows.Forms.ComboBox();
            this.ckbKopirajPrethTak = new System.Windows.Forms.CheckBox();
            this.txtPrethTak = new System.Windows.Forms.TextBox();
            this.btnIzaberiPrethTak = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(927, 483);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(1019, 483);
            // 
            // lblNaziv
            // 
            this.lblNaziv.AutoSize = true;
            this.lblNaziv.Location = new System.Drawing.Point(21, 18);
            this.lblNaziv.Name = "lblNaziv";
            this.lblNaziv.Size = new System.Drawing.Size(98, 13);
            this.lblNaziv.TabIndex = 2;
            this.lblNaziv.Text = "Naziv takmicenja  *";
            // 
            // txtNaziv
            // 
            this.txtNaziv.Location = new System.Drawing.Point(24, 34);
            this.txtNaziv.Name = "txtNaziv";
            this.txtNaziv.Size = new System.Drawing.Size(455, 20);
            this.txtNaziv.TabIndex = 3;
            // 
            // lblDatum
            // 
            this.lblDatum.AutoSize = true;
            this.lblDatum.Location = new System.Drawing.Point(21, 121);
            this.lblDatum.Name = "lblDatum";
            this.lblDatum.Size = new System.Drawing.Size(103, 13);
            this.lblDatum.TabIndex = 4;
            this.lblDatum.Text = "Datum odrzavanja  *";
            // 
            // txtDatum
            // 
            this.txtDatum.Location = new System.Drawing.Point(24, 137);
            this.txtDatum.Name = "txtDatum";
            this.txtDatum.Size = new System.Drawing.Size(100, 20);
            this.txtDatum.TabIndex = 5;
            // 
            // lblMesto
            // 
            this.lblMesto.AutoSize = true;
            this.lblMesto.Location = new System.Drawing.Point(21, 175);
            this.lblMesto.Name = "lblMesto";
            this.lblMesto.Size = new System.Drawing.Size(101, 13);
            this.lblMesto.TabIndex = 6;
            this.lblMesto.Text = "Mesto odrzavanja  *";
            // 
            // txtMesto
            // 
            this.txtMesto.Location = new System.Drawing.Point(24, 191);
            this.txtMesto.Name = "txtMesto";
            this.txtMesto.Size = new System.Drawing.Size(100, 20);
            this.txtMesto.TabIndex = 7;
            // 
            // lblGimnastika
            // 
            this.lblGimnastika.AutoSize = true;
            this.lblGimnastika.Location = new System.Drawing.Point(21, 68);
            this.lblGimnastika.Name = "lblGimnastika";
            this.lblGimnastika.Size = new System.Drawing.Size(69, 13);
            this.lblGimnastika.TabIndex = 8;
            this.lblGimnastika.Text = "Gimnastika  *";
            // 
            // cmbGimnastika
            // 
            this.cmbGimnastika.FormattingEnabled = true;
            this.cmbGimnastika.Location = new System.Drawing.Point(24, 84);
            this.cmbGimnastika.Name = "cmbGimnastika";
            this.cmbGimnastika.Size = new System.Drawing.Size(98, 21);
            this.cmbGimnastika.TabIndex = 4;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(24, 292);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(455, 69);
            this.listBox1.TabIndex = 11;
            // 
            // btnIzaberiPrvaDvaKola
            // 
            this.btnIzaberiPrvaDvaKola.Location = new System.Drawing.Point(24, 367);
            this.btnIzaberiPrvaDvaKola.Name = "btnIzaberiPrvaDvaKola";
            this.btnIzaberiPrvaDvaKola.Size = new System.Drawing.Size(130, 23);
            this.btnIzaberiPrvaDvaKola.TabIndex = 12;
            this.btnIzaberiPrvaDvaKola.Text = "Izaberi I kolo i II kolo";
            this.btnIzaberiPrvaDvaKola.UseVisualStyleBackColor = true;
            this.btnIzaberiPrvaDvaKola.Click += new System.EventHandler(this.btnIzaberiPrvaDvaKola_Click);
            // 
            // lblTipTakmicenja
            // 
            this.lblTipTakmicenja.AutoSize = true;
            this.lblTipTakmicenja.Location = new System.Drawing.Point(21, 230);
            this.lblTipTakmicenja.Name = "lblTipTakmicenja";
            this.lblTipTakmicenja.Size = new System.Drawing.Size(76, 13);
            this.lblTipTakmicenja.TabIndex = 14;
            this.lblTipTakmicenja.Text = "Tip takmicenja";
            // 
            // cmbTipTakmicenja
            // 
            this.cmbTipTakmicenja.FormattingEnabled = true;
            this.cmbTipTakmicenja.Location = new System.Drawing.Point(24, 246);
            this.cmbTipTakmicenja.Name = "cmbTipTakmicenja";
            this.cmbTipTakmicenja.Size = new System.Drawing.Size(183, 21);
            this.cmbTipTakmicenja.TabIndex = 15;
            // 
            // ckbKopirajPrethTak
            // 
            this.ckbKopirajPrethTak.AutoSize = true;
            this.ckbKopirajPrethTak.Location = new System.Drawing.Point(548, 23);
            this.ckbKopirajPrethTak.Name = "ckbKopirajPrethTak";
            this.ckbKopirajPrethTak.Size = new System.Drawing.Size(163, 17);
            this.ckbKopirajPrethTak.TabIndex = 16;
            this.ckbKopirajPrethTak.Text = "Kopiraj prethodno takmicenje";
            this.ckbKopirajPrethTak.UseVisualStyleBackColor = true;
            this.ckbKopirajPrethTak.CheckedChanged += new System.EventHandler(this.ckbKopirajPrethTak_CheckedChanged);
            // 
            // txtPrethTak
            // 
            this.txtPrethTak.Location = new System.Drawing.Point(548, 46);
            this.txtPrethTak.Name = "txtPrethTak";
            this.txtPrethTak.Size = new System.Drawing.Size(455, 20);
            this.txtPrethTak.TabIndex = 17;
            // 
            // btnIzaberiPrethTak
            // 
            this.btnIzaberiPrethTak.Location = new System.Drawing.Point(1019, 44);
            this.btnIzaberiPrethTak.Name = "btnIzaberiPrethTak";
            this.btnIzaberiPrethTak.Size = new System.Drawing.Size(75, 23);
            this.btnIzaberiPrethTak.TabIndex = 18;
            this.btnIzaberiPrethTak.Text = "Izaberi";
            this.btnIzaberiPrethTak.UseVisualStyleBackColor = true;
            this.btnIzaberiPrethTak.Click += new System.EventHandler(this.btnIzaberiPrethTak_Click);
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeView1.Location = new System.Drawing.Point(548, 84);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(455, 323);
            this.treeView1.TabIndex = 19;
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(548, 427);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(80, 17);
            this.checkBox1.TabIndex = 20;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(548, 450);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(80, 17);
            this.checkBox2.TabIndex = 21;
            this.checkBox2.Text = "checkBox2";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // TakmicenjeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1113, 518);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.btnIzaberiPrethTak);
            this.Controls.Add(this.txtPrethTak);
            this.Controls.Add(this.ckbKopirajPrethTak);
            this.Controls.Add(this.cmbGimnastika);
            this.Controls.Add(this.btnIzaberiPrvaDvaKola);
            this.Controls.Add(this.cmbTipTakmicenja);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.lblTipTakmicenja);
            this.Controls.Add(this.lblGimnastika);
            this.Controls.Add(this.lblNaziv);
            this.Controls.Add(this.txtNaziv);
            this.Controls.Add(this.lblDatum);
            this.Controls.Add(this.txtDatum);
            this.Controls.Add(this.txtMesto);
            this.Controls.Add(this.lblMesto);
            this.MaximizeBox = true;
            this.Name = "TakmicenjeForm";
            this.Text = "TakmicenjeForm";
            this.Shown += new System.EventHandler(this.TakmicenjeForm_Shown);
            this.Controls.SetChildIndex(this.lblMesto, 0);
            this.Controls.SetChildIndex(this.txtMesto, 0);
            this.Controls.SetChildIndex(this.txtDatum, 0);
            this.Controls.SetChildIndex(this.lblDatum, 0);
            this.Controls.SetChildIndex(this.txtNaziv, 0);
            this.Controls.SetChildIndex(this.lblNaziv, 0);
            this.Controls.SetChildIndex(this.lblGimnastika, 0);
            this.Controls.SetChildIndex(this.lblTipTakmicenja, 0);
            this.Controls.SetChildIndex(this.listBox1, 0);
            this.Controls.SetChildIndex(this.cmbTipTakmicenja, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnIzaberiPrvaDvaKola, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.cmbGimnastika, 0);
            this.Controls.SetChildIndex(this.ckbKopirajPrethTak, 0);
            this.Controls.SetChildIndex(this.txtPrethTak, 0);
            this.Controls.SetChildIndex(this.btnIzaberiPrethTak, 0);
            this.Controls.SetChildIndex(this.treeView1, 0);
            this.Controls.SetChildIndex(this.checkBox1, 0);
            this.Controls.SetChildIndex(this.checkBox2, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblNaziv;
        private System.Windows.Forms.TextBox txtNaziv;
        private System.Windows.Forms.Label lblDatum;
        private System.Windows.Forms.TextBox txtDatum;
        private System.Windows.Forms.Label lblMesto;
        private System.Windows.Forms.TextBox txtMesto;
        private System.Windows.Forms.Label lblGimnastika;
        private System.Windows.Forms.ComboBox cmbGimnastika;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button btnIzaberiPrvaDvaKola;
        private System.Windows.Forms.Label lblTipTakmicenja;
        private System.Windows.Forms.ComboBox cmbTipTakmicenja;
        private System.Windows.Forms.CheckBox ckbKopirajPrethTak;
        private System.Windows.Forms.TextBox txtPrethTak;
        private System.Windows.Forms.Button btnIzaberiPrethTak;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
    }
}